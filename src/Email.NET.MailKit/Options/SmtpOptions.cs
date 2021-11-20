namespace Email.NET.EDP.MailKit
{
    using Email.NET.Exceptions;
using global::MailKit.Security;
    using System.Net;
    using System.Net.Mail;

    /// <summary>
    /// the SMTP Options
    /// </summary>
    public class SmtpOptions
    {
        /// <summary>
        /// Gets or sets a value that specifies the amount of time after which a synchronous Overload:System.Net.Mail.SmtpClient.Send call times out.
        /// </summary>
        /// <returns>
        /// An System.Int32 that specifies the time-out value in milliseconds. The default value is 100,000 (100 seconds).
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value specified for a set operation was less than zero.</exception>
        /// <exception cref="System.InvalidOperationException">You cannot change the value of this property when an email is being sent.</exception>
        public int Timeout { get; set; } = 100000;

        /// <summary>
        /// Gets or sets the port used for SMTP transactions.
        /// </summary>
        /// <returns>An System.Int32 that contains the port number on the SMTP host. The default value is 25.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The value specified for a set operation was less than zero.</exception>
        /// <exception cref="System.InvalidOperationException">You cannot change the value of this property when an email is being sent.</exception>
        public int Port { get; set; } = 25;

        /// <summary>
        /// Gets or sets the folder where applications save mail messages to be processed by the local SMTP server.
        /// </summary>
        /// <returns>A System.String that specifies the pickup directory for mail messages.</returns>
        public string PickupDirectoryLocation { get; set; }

        /// <summary>
        /// Gets or sets the name or IP address of the host used for SMTP transactions.
        /// </summary>
        /// <returns>A System.String that contains the name or IP address of the computer to use for SMTP transactions.</returns>
        /// <exception cref="System.ArgumentNullException">The value specified for a set operation is null.</exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.InvalidOperationException">You cannot change the value of this property when an email is being sent.</exception>
        public string Host { get; set; }

        /// <summary>
        /// Specify whether the System.Net.Mail.SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection.
        /// </summary>
        /// <returns>true if the System.Net.Mail.SmtpClient uses SSL; otherwise, false. The default is false.</returns>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Specifies how outgoing email messages will be handled.
        /// </summary>
        /// <returns>An System.Net.Mail.SmtpDeliveryMethod that indicates how email messages are delivered. The default is <see cref="SmtpDeliveryMethod.Network"/></returns>
        public MailKitDeliveryMethod DeliveryMethod { get; set; } = MailKitDeliveryMethod.Network;

        /// <summary>
        /// Get or set the secure socket options.
        /// </summary>
        public SecureSocketOptions SecureSocketOptions { get; set; }

        /// <summary>
        /// Get or set the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Get or set the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// validate if the smtp options are all set correctly
        /// </summary>
        public void Validate()
        {
            if (DeliveryMethod == MailKitDeliveryMethod.Network)
            {
                if (string.IsNullOrEmpty(Host))
                    throw new RequiredOptionValueNotSpecifiedException<SmtpOptions>(
                        $"{nameof(Host)}", "the given SmtpOptions.Host value is null or empty.");

                if (Port <= 0)
                    throw new RequiredOptionValueNotSpecifiedException<SmtpOptions>(
                        $"{nameof(Port)}", "the given SmtpOptions.Port value less then or equals to Zero.");
            }

            if (DeliveryMethod == MailKitDeliveryMethod.SpecifiedPickupDirectory)
            {
                if (string.IsNullOrEmpty(PickupDirectoryLocation))
                    throw new RequiredOptionValueNotSpecifiedException<SmtpOptions>(
                        $"{nameof(Host)}", "you must supply a SmtpOptions.PickupDirectoryLocation in order to deliver with MailKitDeliveryMethod.SpecifiedPickupDirectory.");
            }
        }
    }
}
