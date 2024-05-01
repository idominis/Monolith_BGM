using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGM.Common
{
    public interface IStatusUpdateService
    {
        event Action<string> StatusUpdated;
        void RaiseStatusUpdated(string message);

        void RaiseStatusUpdated(string message, Exception ex);
    }
}
