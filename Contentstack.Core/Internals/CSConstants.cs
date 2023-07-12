using System;
namespace Contentstack.Core.Internals
{
    public static class CSConstants
    {
        #region Internal Constants
        internal const long ContentBufferSize = 1024 * 1024 * 1024;
        internal readonly static TimeSpan Timeout = TimeSpan.FromSeconds(30);
        internal readonly static TimeSpan Delay = TimeSpan.FromMilliseconds(300);
        internal const string Slash = "/";
        internal const char SlashChar = '/';
        #endregion

        #region Internal Message
        internal const string YouAreLoggedIn = "You are already logged in.";
        internal const string YouAreNotLoggedIn = "You are need to login.";

        internal const string MissingUID = "Uid should not be empty.";
        internal const string MissingAPIKey = "API Key should not be empty.";
        internal const string APIKey = "API Key should be empty.";

        internal const string RemoveUserEmailError = "Please enter email id to remove from org.";
        internal const string OrgShareUIDMissing = "Please enter share uid to resend invitation.";
        #endregion
    }
}

