using Serilog;

namespace BGM.Common
{
    public class ErrorHandlerService
    {
        public void LogError(Exception ex, string message, string fileName = null)
        {
            var errorMessage = fileName == null ? $"{message}\nError: {ex.Message}" : $"{message} from file: {fileName}\nError: {ex.Message}";
            Log.Error(ex, "{Message} {FileName}", message, fileName);
        }
    }

}
