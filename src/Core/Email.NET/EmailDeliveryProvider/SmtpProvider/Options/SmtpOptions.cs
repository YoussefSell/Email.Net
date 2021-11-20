namespace Email.NET.EDP.Smtp
{
    using Exceptions;
    using Utilities;
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
        /// the name of the Target
        /// </summary>
        public string TargetName { get; set; }

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
        public SmtpDeliveryMethod DeliveryMethod { get; set; } = SmtpDeliveryMethod.Network;

        /// <summary>
        /// Gets or sets the delivery format used by System.Net.Mail.SmtpClient to send e-mail.
        /// </summary>
        /// <returns>Returns System.Net.Mail.SmtpDeliveryFormat. The delivery format used by System.Net.Mail.SmtpClient.</returns>
        public SmtpDeliveryFormat DeliveryFormat { get; set; } = SmtpDeliveryFormat.SevenBit;

        /// <summary>
        /// Gets or sets a System.Boolean value that controls whether the System.Net.CredentialCache.DefaultCredentials are sent with requests.
        /// </summary>
        /// <returns>true if the default credentials are used; otherwise false. The default value is false.</returns>
        /// <exception cref="System.InvalidOperationException">You cannot change the value of this property when an e-mail is being sent.</exception>
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Gets or sets the credentials used to authenticate the sender.
        /// </summary>
        /// <returns>
        /// An System.Net.ICredentialsByHost that represents the credentials to use for authentication;
        /// or null if no credentials have been specified.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">You cannot change the value of this property when an email is being sent.</exception>
        public ICredentialsByHost Credentials { get; set; }

        /// <summary>
        /// validate if the smtp options are all set correctly
        /// </summary>
        public void Validate()
        {
            if (DeliveryMethod == SmtpDeliveryMethod.Network)
            {
                if (!Host.IsValid())
                    throw new RequiredOptionValueNotSpecifiedException<SmtpOptions>(
                        $"{nameof(Host)}", "the given SmtpOptions.Host value is null or empty.");

                if (Port <= 0)
                    throw new RequiredOptionValueNotSpecifiedException<SmtpOptions>(
                        $"{nameof(Port)}", "the given SmtpOptions.Port value less then or equals to Zero.");
            }

            if (DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
            {
                if (string.IsNullOrEmpty(PickupDirectoryLocation))
                    throw new RequiredOptionValueNotSpecifiedException<SmtpOptions>(
                        $"{nameof(Host)}", "you must supply a SmtpOptions.PickupDirectoryLocation in order to deliver with SmtpDeliveryMethod.SpecifiedPickupDirectory.");
            }
        }
    }
}
