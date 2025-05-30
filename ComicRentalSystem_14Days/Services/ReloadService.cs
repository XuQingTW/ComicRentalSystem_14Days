using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ComicRentalSystem_14Days.Services;


namespace ComicRentalSystem_14Days.Services
{
    public class ReloadService : IReloadService
        {
            private CancellationTokenSource? _cts;

            

            public void Start(Func<Task> reloadAction, TimeSpan interval)
            {

                Stop();

                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            await Task.Delay(interval, token);
                            if (token.IsCancellationRequested) break;
                            await reloadAction();
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }, token);
            }

            public void Stop()
            {
                if (_cts != null && !_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                    _cts.Dispose();
                    _cts = null;
                }
            }
    }
}
