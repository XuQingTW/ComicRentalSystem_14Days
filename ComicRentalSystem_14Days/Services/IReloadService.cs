using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ComicRentalSystem_14Days.Services
{
    public interface IReloadService
    {
        /// <summary>
        /// 啟動週期性執行 reloadAction，間隔 interval。
        /// </summary>
        void Start(Func<Task> reloadAction, TimeSpan interval);

        /// <summary>
        /// 停止週期性執行。
        /// </summary>
        void Stop();
    }
}
