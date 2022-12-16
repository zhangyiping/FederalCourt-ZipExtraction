namespace ZipExtraction.Services;

public interface IEmailService
{
    Task SendEmail(string message);
}