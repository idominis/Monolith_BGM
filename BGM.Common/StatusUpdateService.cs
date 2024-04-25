using System;

namespace BGM.Common
{
    public class StatusUpdateService : IStatusUpdateService
    {
        public event Action<string> StatusUpdated;

        public void RaiseStatusUpdated(string message)
        {
            StatusUpdated?.Invoke(message);
        }
    }
}
