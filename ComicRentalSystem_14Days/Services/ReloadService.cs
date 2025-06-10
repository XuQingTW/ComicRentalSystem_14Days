using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces;


namespace ComicRentalSystem_14Days.Services
{
    public class ReloadService : IReloadService
        {
            private readonly ILogger _logger;
            private CancellationTokenSource? _cts;
            private Task? _runningTask;


            public ReloadService(ILogger logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public Task Start(Func<Task> reloadAction, TimeSpan interval, CancellationToken cancellationToken)
            {

                StopAsync().GetAwaiter().GetResult();

                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var token = _cts.Token;

                _runningTask = Task.Run(async () =>
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
                            _logger.LogError("Reload loop encountered an error", ex);
                        }
                    }
                }, token);
                return _runningTask;
            }

            public async Task StopAsync()
            {
                if (_cts != null && !_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                    try
                    {
                        if (_runningTask != null)
                        {
                            await _runningTask;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error while stopping reload loop", ex);
                    }
                    _cts.Dispose();
                    _cts = null;
                    _runningTask = null;
                }
            }
    }
}
