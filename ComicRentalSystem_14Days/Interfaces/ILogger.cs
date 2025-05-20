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
        void LogError(string message, Exception? ex = null); // 新增一個專門記錄錯誤的過載
    }
}
