// MySync © 2016-2017 Damian 'Erdroy' Korczowski

namespace MySync.Core
{
    /// <summary>
    /// The client UI helper class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class ClientUI
    {
        /// <summary>
        /// Shows message overaly.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isError">Is this message an error?</param>
        public static void ShowMessage(string message, bool isError = false)
        {
            Javascript.Run("showMessage('" + message + "', " + isError.ToString().ToLower() + ");");
        }

        /// <summary>
        /// Shows progress window.
        /// </summary>
        /// <param name="message">The progress message.</param>
        public static void ShowProgress(string message)
        {
            SetProgress(message);
            Javascript.Run("showProgress();");
        }

        /// <summary>
        /// Sets progress window message.
        /// </summary>
        /// <param name="message">The progress message.</param>
        public static void SetProgress(string message)
        {
            Javascript.Run("setProgressMesssage('" + message + "');");
        }

        /// <summary>
        /// Closes progress window.
        /// </summary>
        public static void HideProgress()
        {
            Javascript.Run("hideProgress();");
        }
    }
}

