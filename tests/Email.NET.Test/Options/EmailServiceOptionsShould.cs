namespace Email.NET.Test
{
    using Email.NET.Exceptions;
    using Xunit;

    public class EmailServiceOptionsShould
    {
        [Fact]
        public void ThrowIfFRequiredValueIsNotSpecified()
        {
            // arrange
            var options = new EmailServiceOptions() { DefaultEmailDeliveryProvider = null };

            // assert
            Assert.Throws<RequiredOptionValueNotSpecifiedException<EmailServiceOptions>>(() =>
            {
                // act
                options.Validate();
            });
        }
    }
}
