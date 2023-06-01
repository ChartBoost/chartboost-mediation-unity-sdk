using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Chartboost.Utilities
{
    /// <summary>For referencing <see cref="ILater{TResult}"/> generically</summary>
    public interface ILater { }

    /// <summary>Read-only interface for a standard <see cref="Later{TResult}"/></summary>
    public interface ILater<TResult> : ILater
    {
        event Action<TResult> OnComplete;
        
        TaskAwaiter<TResult> GetAwaiter();
    }

    /// <summary>Basic Later for passing a single type to callbacks and awaiters</summary>
    public class Later<TResult> : BaseLater<TResult>
    {
        public void Complete(TResult result) => _complete(result);
    }

    /// <summary>Separated implementation so the derivations can offer different methods for completion</summary>
    public abstract class BaseLater<TResult> : ILater<TResult>
    {
        public event Action<TResult> OnComplete
        {
            remove => DidComplete -= value;
            add
            {
                if (_isComplete)
                    DidComplete += value;
                else
                    value?.Invoke(_result);
            }
        }

        public TaskAwaiter<TResult> GetAwaiter()
        {
            if (_completionSource != null)
                return _completionSource.Task.GetAwaiter();

            _completionSource = new TaskCompletionSource<TResult>();

            if (_isComplete)
                _completionSource.TrySetResult(_result);
            else
                DidComplete += result => _completionSource.TrySetResult(result);

            return _completionSource.Task.GetAwaiter();
        }

        protected void _complete(TResult result)
        {
            if (_isComplete)
                return;

            _result = result;

            var toComplete = DidComplete;
            DidComplete = null;

            _isComplete = true;
            toComplete?.Invoke(_result);
        }

        private TaskCompletionSource<TResult> _completionSource;
        private event Action<TResult> DidComplete;
        private bool _isComplete;
        private TResult _result;
    }
}
