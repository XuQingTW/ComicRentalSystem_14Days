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
        void Start(Func<Task> reloadAction, TimeSpan interval);

        void Stop();
    }
}
