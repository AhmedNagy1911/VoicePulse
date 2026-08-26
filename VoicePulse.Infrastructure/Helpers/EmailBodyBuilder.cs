namespace VoicePulse.Infrastructure.Helpers;

public static class EmailBodyBuilder
{
    public static string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
    {
        var templatePath = Path.Combine(
           AppContext.BaseDirectory,
           "Templates",
           $"{template}.html");

        //var streamReader = new StreamReader(templatePath);
        //var body = streamReader.ReadToEnd();
        //streamReader.Close();
        var body = File.ReadAllText(templatePath);

        foreach (var item in templateModel)
            body = body.Replace(item.Key, item.Value);

        return body;
    }
}