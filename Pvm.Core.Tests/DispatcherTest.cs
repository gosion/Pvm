using System;
using System.Collections.Generic;
using System.Linq;
using Pvm.Core.Dispatchers;
using Pvm.Core.Extensions;
using Pvm.Core.Features.Logs;
using Xunit;

namespace Pvm.Core.Tests
{
    public class DispatcherTest
    {
        [Fact]
        public void Test_ParallelDispatcher()
        {
            var process = PauseAndContinute.GenProcess(
                true,
                new ParallelDispatcher(),
                feature =>
                {
                    feature.Writer = new FileWriter(new LogOptions
                    {
                        LogPath = "logs/parallel.log"
                    });
                });
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

            process.Proceed(waitingIds.LastOrDefault(), initData2);

            Assert.Equal(prices[0] + prices[1], process.ProcessContext.Scope.Get<int>("total"));

            initData2["user_input"] = prices[2];

            process.Proceed(waitingIds.LastOrDefault(), initData2);

            Assert.Equal(prices[0] + prices[1] + prices[2], process.ProcessContext.Scope.Get<int>("total"));
        }
    }
}
