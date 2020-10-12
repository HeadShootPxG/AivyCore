using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AivyDofus.Extension
{
    public static class AsyncExtension
    {
        public static async Task ExecuteAsync(Action action, Action endAction, Action<Exception> onError)
        {
            CancellationTokenSource _source = new CancellationTokenSource();

            await Task.Run(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch(Exception e)
                {
                    _source.Cancel();
                    onError(e);
                }
            }, _source.Token).ContinueWith(task =>
            {
                if (endAction is null) return;

                if (!_source.IsCancellationRequested)
                {
                    try
                    {
                        endAction.Invoke();
                    }
                    catch(Exception e)
                    {
                        onError(e);
                    }
                }
            });
        }
    }
}
