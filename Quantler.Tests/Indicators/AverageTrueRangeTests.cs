﻿#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using Quantler.Data.Bars;
using Quantler.Indicators;
using System;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class AverageTrueRangeTests
    {
        #region Public Methods

        [Fact]
        public void ComparesAgainstExternalData()
        {
            var atr = new AverageTrueRange(14, MovingAverageType.Simple);
            TestHelper.TestIndicator(atr, "spy_atr.txt", "Average True Range 14");
        }

        [Fact]
        public void ResetsProperly()
        {
            var atr = new AverageTrueRange(14, MovingAverageType.Simple);
            atr.Update(new TradeBar
            {
                Occured = DateTime.Today,
                Open = 1m,
                High = 3m,
                Low = .5m,
                Close = 2.75m,
                Volume = 1234567890,
                TimeZone = TimeZone.Utc
            });

            atr.Reset();

            TestHelper.AssertIndicatorIsInDefaultState(atr);
            TestHelper.AssertIndicatorIsInDefaultState(atr.TrueRange);
        }

        [Fact]
        public void TrueRangePropertyIsReadyAfterOneSample()
        {
            var atr = new AverageTrueRange(14, MovingAverageType.Simple);
            Assert.False(atr.TrueRange.IsReady);

            atr.Update(new TradeBar
            {
                Occured = DateTime.Today,
                Open = 1m,
                High = 3m,
                Low = .5m,
                Close = 2.75m,
                Volume = 1234567890,
                TimeZone = TimeZone.Utc
            });

            Assert.True(atr.TrueRange.IsReady);
        }

        #endregion Public Methods
    }
}