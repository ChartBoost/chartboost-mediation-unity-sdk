using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Chartboost.Utilities
{
    internal class AwaitableAndroidJavaProxy<TResult> : AndroidJavaProxy
    {
        public TaskAwaiter<TResult> GetAwaiter() 
        {
            if (_taskCompletionSource != null)
                return _taskCompletionSource.Task.GetAwaiter();
                
            _taskCompletionSource = new TaskCompletionSource<TResult>();
            
            if (_isComplete)
                _setResult();
            else
                DidComplete += result => _setResult();

            return _taskCompletionSource.Task.GetAwaiter();
        }
        
        protected AwaitableAndroidJavaProxy(string nativeInterface) : base(nativeInterface) { }

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
        
        private void _setResult()
        {
            try
            {
                _taskCompletionSource.TrySetResult(_result);
            }
            catch (ObjectDisposedException e)
            {
                Debug.Log(e.Message);
            }
        }

        private TaskCompletionSource<TResult> _taskCompletionSource;
        private event Action<TResult> DidComplete;
        private TResult _result;
        private bool _isComplete;
    }
}
