namespace ZipExtraction.Services;

public class EmailService: IEmailService
{
    public async Task SendEmail(string message)
    {
        // TODO either do an API call to email server, or publish a email event
        for (int i = 0; i < 2; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}