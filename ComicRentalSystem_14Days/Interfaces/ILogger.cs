using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        void Log(string message, Exception ex); // 技術點4: 過載 (Log 方法的過載)
        void LogError(string message, Exception? ex = null);
        void LogWarning(string message);
        void LogInformation(string message); // 新增
        void LogDebug(string message);       // 新增
    }
}
