using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Chartboost.Utilities
{
    public class AwaitableAndroidJavaProxy<TResult> : AndroidJavaProxy
    {
        public TaskAwaiter<TResult> GetAwaiter() 
        {
            if (_completionSource != null)
                return _completionSource.Task.GetAwaiter();
                
            _completionSource = new TaskCompletionSource<TResult>();
            
            if (isComplete)
                _setResult();
            else
                DidComplete += result => _setResult();

            return _completionSource.Task.GetAwaiter();
        }

        protected bool isComplete = false;

        protected AwaitableAndroidJavaProxy(string nativeInterface) : base(nativeInterface) { }
        protected AwaitableAndroidJavaProxy(AndroidJavaClass nativeInterface) : base(nativeInterface) { }

        protected void _complete(TResult result)
        {
            if (isComplete)
                return;

            _result = result;
            var toComplete = DidComplete;
            DidComplete = null;
            isComplete = true;
            toComplete?.Invoke(_result);
        }

        protected void _fail(string error, bool except = false)
        {
            if (except)
            {
                var exception = new Exception(error);
                Debug.LogException(exception);
                _completionSource.TrySetException(exception);
            }
            else
            {
                Debug.LogError(error);
                _completionSource.TrySetCanceled();
            }
        }

        private void _setResult()
        {
            try
            {
                _completionSource.TrySetResult(_result);
            }
            catch (ObjectDisposedException e)
            {
                Debug.Log(e.Message);
            }
        }

        private TaskCompletionSource<TResult> _completionSource;
        private event Action<TResult> DidComplete;
        private TResult _result;
    }
}
