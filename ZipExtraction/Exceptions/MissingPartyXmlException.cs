namespace ZipExtraction.Exceptions;

public class MissingPartyXmlException: Exception
{
    public MissingPartyXmlException()
    {
        
    }

    public MissingPartyXmlException(string message): base(message)
    {
        
    }

    public MissingPartyXmlException(string message, Exception inner): base(message, inner)
    {
        
    }
}