using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chartboost.Events;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Editor
{
    public class TaskTests
    {
        private const string DummyTaskException = "Dummy Task Exception";

        [UnityTest]
        public IEnumerator TestContinueWithMainThreadCompletion()
        {
            var task = DummyTask(false).ContinueWithOnMainThread(t => Debug.Log("Task complete"));

            Debug.Log("Awaiting task completion....");
            yield return new WaitUntil(() => task.IsCompleted);
            
            Assert.False(task.Status == TaskStatus.Faulted);
            Assert.True(task.Status == TaskStatus.RanToCompletion);
        }
        
        [UnityTest]
        public IEnumerator TestContinueWithMainThreadException()
        {
            var task = DummyTask(true).ContinueWithOnMainThread(t => Debug.Log("Task complete"));
            
            Debug.Log("Awaiting task completion....");
            yield return new WaitUntil(() => task.IsCompleted);
            
            LogAssert.Expect(LogType.Exception, new Regex($"Exception: {DummyTaskException}*"));
            
            Assert.True(task.Status == TaskStatus.Faulted);
            Assert.False(task.Status == TaskStatus.RanToCompletion);
        }
        
        private async Task DummyTask(bool throwException)
        {
            await Task.Delay(1000);

            if (throwException)
            {
                throw new Exception(DummyTaskException);
            }
        }
    }
}
