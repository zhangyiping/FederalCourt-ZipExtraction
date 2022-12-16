using NSubstitute;
using ZipExtraction.Services;
using ZipExtraction.Validators;
using NUnit.Framework;
using TestStack.BDDfy;
using ZipExtraction.Exceptions;

namespace ZipExtraction.Test;

[TestFixture]
public class ZipExtractServiceTest
{
    private ZipExtractService _subject;

    private IZipContentValidator _zipContentValidator;
    private IXmlValidator _xmlValidator;
    private IEmailService _emailService;

    [SetUp]
    public void Setup()
    {
        _zipContentValidator = Substitute.For<IZipContentValidator>();
        _xmlValidator = Substitute.For<IXmlValidator>();
        _emailService = Substitute.For<IEmailService>();
        
        _subject = new ZipExtractService(_zipContentValidator, _xmlValidator, _emailService);
    }

    [Test]
    public void ItShouldThrowInvalidFileTypeExceptionIfInvalidFileTypeIsInZip()
    {
        this.Given(x => GivenInvalidFileTypeIsInZip())
            .When(x => WhenExtractZip())
            .Then(x => ThenEmailShouldBeSentWithFailureReason("Invalid file type is included in zip folder"))
            .BDDfy();
    }

    private void GivenInvalidFileTypeIsInZip()
    {
        _zipContentValidator.ValidateZipContent(Arg.Any<string>())
            .Returns(x => throw new InvalidFileTypeException("Invalid file type is included"));
    }

    private async Task WhenExtractZip()
    {
        await _subject.ExtractZip("file path to zip"); 
    }

    private void ThenEmailShouldBeSentWithFailureReason(string expectedFailureReason)
    {
        _emailService.Received().SendEmail(expectedFailureReason);
    }
}