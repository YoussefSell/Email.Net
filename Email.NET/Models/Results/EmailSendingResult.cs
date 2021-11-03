namespace Email.NET
{
    using System;

    /// <summary>
    /// the sending result of the email
    /// </summary>
    public class EmailSendingResult
    {
        internal static EmailSendingResult Success()
        {
            throw new NotImplementedException();
        }

        internal static EmailSendingResult Failed(Exception ex)
        {
            throw new NotImplementedException();
        }

        internal static EmailSendingResult SendingPaused()
        {
            throw new NotImplementedException();
        }
    }
}
