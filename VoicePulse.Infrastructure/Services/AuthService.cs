using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using VoicePulse.Application.Common.Consts;
using VoicePulse.Application.Common.Errors;
using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Authentication;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;
using VoicePulse.Infrastructure.Helpers;
using VoicePulse.Infrastructure.Persistence;

namespace VoicePulse.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtProvider jwtProvider,
      SignInManager<ApplicationUser> signInManager,
      ILogger<AuthService> logger,
      IEmailSender emailSender,
      IHttpContextAccessor httpContextAccessor,
      ApplicationDbContext context) : IAuthService
{
    private readonly UserManager<ApplicationUser> _usermanager = userManager;
    private readonly IJwtProvider _jwtprovider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signinmanager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailsender = emailSender;
    private readonly IHttpContextAccessor _httpcontextaccessor = httpContextAccessor;
    private readonly ApplicationDbContext _context = context;
    private readonly int _refreshTokenEpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //chech user?
        var user = await _usermanager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        //chech password and confirm email
        var result = await _signinmanager.PasswordSignInAsync(user, password, false, true);

        if(result.Succeeded)
        {

            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);

            //generate JWT token
            var (token, expiresIn) = _jwtprovider.GenerateToken(user, userRoles, userPermissions);

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

        var error = result.IsNotAllowed
            ? UserErrors.EmailNotConfirmed
            : result.IsLockedOut
            ? UserErrors.LockedUser
            : UserErrors.InvalidCredentials;

        return Result.Failure<AuthResponse>(error);

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

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        //chech token?
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        //remove old token
        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);

        //generate JWT NewToken
        var (newToken, expiresIn) = _jwtprovider.GenerateToken(user, userRoles, userPermissions);

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
        {
            await _usermanager.AddToRoleAsync(user, DefaultRoles.Member);
            return Result.Success();
        }

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

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await _usermanager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var code = await _usermanager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset code: {code}", code);

        await SendResetPasswordEmail(user, code);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _usermanager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _usermanager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_usermanager.ErrorDescriber.InvalidToken());
        }

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
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

        BackgroundJob.Enqueue(() => _emailsender.SendEmailAsync(user.Email!, "✅  Voice Pulse: Email Confirmation", emailBody));
    }

    private async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpcontextaccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FristName },
                { "{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
            }
        );

        BackgroundJob.Enqueue(() => _emailsender.SendEmailAsync(user.Email!, "✅ Voice Pulse: Change Password", emailBody));

        await Task.CompletedTask;
    }

    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _usermanager.GetRolesAsync(user);

        var userPermissions = await (from r in _context.Roles
                                     join p in _context.RoleClaims
                                     on r.Id equals p.RoleId
                                     where userRoles.Contains(r.Name!)
                                     select p.ClaimValue!)
                                     .Distinct()
                                     .ToListAsync(cancellationToken);

        return (userRoles, userPermissions);
    }

}

