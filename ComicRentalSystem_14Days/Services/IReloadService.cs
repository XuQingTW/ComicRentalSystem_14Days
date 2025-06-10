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
        Task Start(Func<Task> reloadAction, TimeSpan interval, CancellationToken cancellationToken);

        Task StopAsync();
    }
}
