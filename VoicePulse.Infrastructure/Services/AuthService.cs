using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using VoicePulse.Application.Common.Errors;
using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Authentication;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;
using VoicePulse.Infrastructure.Helpers;

namespace VoicePulse.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider,
      SignInManager<ApplicationUser> signInManager,
      ILogger<AuthService> logger,
      IEmailSender emailSender,
      IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly UserManager<ApplicationUser> _usermanager = userManager;
    private readonly IJwtProvider _jwtprovider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signinmanager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailsender = emailSender;
    private readonly IHttpContextAccessor _httpcontextaccessor = httpContextAccessor;
    private readonly int _refreshTokenEpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //chech user?
        var user = await _usermanager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        //chech password and confirm email
        var result = await _signinmanager.PasswordSignInAsync(user, password, false, false);

        if(result.Succeeded)
        {
            //generate JWT token
            var (token, expiresIn) = _jwtprovider.GenerateToken(user);

            // Add RefreshToken
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenEpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _usermanager.UpdateAsync(user);

            //Return New AuthResponse() 
            var response = new AuthResponse(user.Id, user.Email, user.UserName!, user.FristName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }

        return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
        
    }


    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        //chech userid? 
        var userId =_jwtprovider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        ////chech user?
        var user =await _usermanager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        //chech token?
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        //remove old token
        userRefreshToken.RevokedOn = DateTime.UtcNow;

        //generate JWT NewToken
        var (newToken, expiresIn) = _jwtprovider.GenerateToken(user);

        // Add NewRefreshToken
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenEpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _usermanager.UpdateAsync(user);

        //Return New AuthResponse() 
        var response = new AuthResponse(user.Id, user.Email, user.UserName! , user.FristName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        //chech userid? 
        var userId = _jwtprovider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        ////chech user?
        var user = await _usermanager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        //chech token?
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken);


        //remove old token
        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _usermanager.UpdateAsync(user);

        return Result.Success();

    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExists = await _usermanager.Users.AnyAsync(x => x.Email == request.Email , cancellationToken);

        if (emailIsExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await _usermanager.CreateAsync(user, request.Password);


        if (result.Succeeded)
        {
            // Generate Verification Code
            var code = await _usermanager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {code}", code);

            // send email
            await SendConfirmationEmail(user, code);

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _usermanager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _usermanager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        if (await _usermanager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await _usermanager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation code: {code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Success();
    }

    private async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        var origin = _httpcontextaccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FristName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            }
        );

        await _emailsender.SendEmailAsync(user.Email!, "✅  Voice Pulse: Email Confirmation", emailBody);
    }
}

