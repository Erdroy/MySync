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

        public static void ShowProgress(string message, int initialValue = 0)
        {
            SetProgress(initialValue);

        }

        public static void SetProgress(int value)
        {
            
        }

        public static void HideProgress()
        {
            
        }
    }
}
