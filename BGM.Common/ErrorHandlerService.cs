using Serilog;

namespace BGM.Common
{
    public class ErrorHandlerService
    {
        /// <summary>
        /// Logs an error with a specified message and optional file context.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">A custom message describing the error.</param>
        /// <param name="fileName">An optional file name where the error occurred.</param>
        /// <param name="errorCategory">Optional category of the error (e.g., "Validation", "Connection").</param>
        public void LogError(Exception ex, string message, string fileName = null, string errorCategory = "General")
        {
            var errorMessage = fileName == null
                ? $"{message}\nCategory: {errorCategory}\nError: {ex.Message}"
                : $"{message} from file: {fileName}\nCategory: {errorCategory}\nError: {ex.Message}";

            Log.Error(ex, "{Message} | File: {FileName} | Category: {Category}", message, fileName, errorCategory);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">A custom warning message.</param>
        public void LogWarning(string message)
        {
            Log.Warning(message);
        }

        /// <summary>
        /// Logs a critical error, sending alerts if necessary.
        /// </summary>
        /// <param name="ex">The critical exception to log.</param>
        /// <param name="message">A custom message describing the critical error.</param>
        public void LogCriticalError(Exception ex, string message)
        {
            Log.Fatal(ex, "{Message}", message);
            // Optionally, send email or push notifications to administrators
        }

        /// <summary>
        /// Log an informational message that isn't an error.
        /// </summary>
        /// <param name="message">An informational message.</param>
        public void LogInfo(string message)
        {
            Log.Information(message);
        }
    }
}
