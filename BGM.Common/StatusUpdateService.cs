using System;

namespace BGM.Common
{
    public class StatusUpdateService : IStatusUpdateService
    {
        public event Action<string>? StatusUpdated;

        public void RaiseStatusUpdated(string message)
        {
            StatusUpdated?.Invoke(message);
        }

        public void RaiseStatusUpdated(string message, Exception ex)
        {
            // Create a combined message with the exception details
            string fullMessage = $"{message} - Error: {ex.Message}";
            StatusUpdated?.Invoke(fullMessage);
        }
    }
}
