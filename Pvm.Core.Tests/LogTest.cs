using System;
using System.Collections.Generic;
using Pvm.Core.Extensions;
using Pvm.Core.Features.Logs;
using Xunit;

namespace Pvm.Core.Tests
{
    public class LogTest
    {
        [Fact]
        public void Test_LogsToFile()
        {
            this.common(true, feature =>
            {
                feature.Writer = new FileWriter();
            });
        }

        [Fact]
        public void Test_LogsToConsole()
        {
            this.common(true, _ => {});
        }

        private void common(bool enableLog = false, Action<LogFeature> action = null)
        {
            var process = PauseAndContinute.GenProcess(enableLog, null, action);
            var prices = new int[] { 26, 32, 15 };
            var initData1 = new Dictionary<string, object>
            {
                { "price1", prices[0] },
                { "price2", prices[1] },
            };
            process.Start(initData1);
            var waitingIds = process.ProcessContext.Scope.Get<IList<Guid>>("waiting_ids");

            Assert.Equal(prices[0] + prices[1], process.ProcessContext.Scope.Get<int>("total"));
            Assert.Equal(1, waitingIds.Count);

            var initData2 = new Dictionary<string, object>
            {
                { "user_input", 0 }
            };

            process.Proceed(waitingIds[0], initData2);

            Assert.Equal(prices[0] + prices[1], process.ProcessContext.Scope.Get<int>("total"));

            initData2["user_input"] = prices[2];

            process.Proceed(waitingIds[0], initData2);

            Assert.Equal(prices[0] + prices[1] + prices[2], process.ProcessContext.Scope.Get<int>("total"));
        }
    }
}
