using System;
using SmartView2.Core;

namespace SmartTVRemoteControl.Common
{
    public interface IDispatcher : IBaseDispatcher
    {
        void Invoke(Action callback, System.Windows.Threading.DispatcherPriority priority);
        TResult Invoke<TResult>(Func<TResult> callback);
        TResult Invoke<TResult>(Func<TResult> callback, System.Windows.Threading.DispatcherPriority priority);
        System.Windows.Threading.DispatcherOperation InvokeAsync(Action callback);
        System.Windows.Threading.DispatcherOperation InvokeAsync(Action callback, System.Windows.Threading.DispatcherPriority priority);
        System.Windows.Threading.DispatcherOperation<TResult> InvokeAsync<TResult>(Func<TResult> callback);
    }
}
