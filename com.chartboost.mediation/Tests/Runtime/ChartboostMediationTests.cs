using Chartboost.Logging;
using Chartboost.Mediation;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime
{
    public class ChartboostMediationTests
    {
        [Test]
        public void LoggingLevel()
        {
            var initial = ChartboostMediation.LogLevel;
            
            ChartboostMediation.LogLevel = LogLevel.Disabled;
            Assert.AreEqual(LogLevel.Disabled, ChartboostMediation.LogLevel);
            
            ChartboostMediation.LogLevel = LogLevel.Error;
            Assert.AreEqual(LogLevel.Error, ChartboostMediation.LogLevel);
            
            ChartboostMediation.LogLevel = LogLevel.Warning;
            Assert.AreEqual(LogLevel.Warning, ChartboostMediation.LogLevel);
            
            ChartboostMediation.LogLevel = LogLevel.Info;
            Assert.AreEqual(LogLevel.Info, ChartboostMediation.LogLevel);
    
            ChartboostMediation.LogLevel = LogLevel.Info;
            Assert.AreEqual(LogLevel.Info, ChartboostMediation.LogLevel);
            
            ChartboostMediation.LogLevel = LogLevel.Debug;
            Assert.AreEqual(LogLevel.Debug, ChartboostMediation.LogLevel);
            
            ChartboostMediation.LogLevel = LogLevel.Verbose;
            Assert.AreEqual(LogLevel.Verbose, ChartboostMediation.LogLevel);
            
            ChartboostMediation.LogLevel = initial;
            Assert.AreEqual(initial, ChartboostMediation.LogLevel);
        }
    }
}
