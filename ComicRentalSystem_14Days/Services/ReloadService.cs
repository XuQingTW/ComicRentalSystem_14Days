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
                // 若已啟動，先停止舊的
                Stop();

                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                // 背景執行非同步迴圈
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
                            // 正常取消
                        }
                        catch (Exception ex)
                        {
                            // TODO: 日誌記錄 ex
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
