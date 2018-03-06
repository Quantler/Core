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
using System;

namespace Quantler.Indicators
{
    /// <summary>
    /// The Fisher transform is a mathematical process which is used to convert any data set to a modified
    /// data set whose Probabilty Distrbution Function is approximately Gaussian.  Once the Fisher transform
    /// is computed, the transformed data can then be analyzed in terms of it's deviation from the mean.
    ///
    /// The equation is y = .5 * ln [ 1 + x / 1 - x ] where
    /// x is the input
    /// y is the output
    /// ln is the natural logarithm
    ///
    /// The Fisher transform has much sharper turning points than other indicators such as MACD
    ///
    /// For more info, read chapter 1 of Cybernetic Analysis for Stocks and Futures by John F. Ehlers
    ///
    /// We are implementing the latest version of this indicator found at Fig. 4 of
    /// http://www.mesasoftware.com/papers/UsingTheFisherTransform.pdf
    ///
    /// </summary>
    public class FisherTransform : BarIndicator
    {
        #region Private Fields

        private readonly Maximum _medianMax;
        private readonly Minimum _medianMin;
        private double _alpha;
        private double _previous;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the FisherTransform class with the default name and period
        /// </summary>
        /// <param name="period">The period of the WMA</param>
        public FisherTransform(int period)
            : this("FISH_" + period, period)
        {
        }

        /// <summary>
        /// A Fisher Transform of Prices
        /// </summary>
        /// <param name="name">string - the name of the indicator</param>
        /// <param name="period">The number of periods for the indicator</param>
        public FisherTransform(string name, int period)
            : base(name)
        {
            _alpha = .33;

            // Initialize the local variables
            _medianMax = new Maximum("MedianMax", period);
            _medianMin = new Minimum("MedianMin", period);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady =>
            _medianMax.IsReady && _medianMax.IsReady;

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Computes the next value in the transform.
        /// value1 is a function used to normalize price withing the last _period day range.
        /// value1 is centered on its midpoint and then doubled so that value1 will swing between -1 and +1.
        /// value1 is also smoothed with an exponential moving average whose alpha is 0.33.
        ///
        /// Since the smoothing may allow value1 to exceed the _period day price range, limits are introduced to
        /// preclude the transform from blowing up by having an input larger than unity.
        /// </summary>
        /// <param name="input">IndicatorDataPoint - the time and value of the next price</param>
        /// <returns></returns>
        protected override decimal ComputeNextValue(DataPointBar input)
        {
            var x = 0.0;
            var y = 0.0;
            var price = (input.Low + input.High) / 2m;
            _medianMin.Update(input.Occured, input.TimeZone, price);
            _medianMax.Update(input.Occured, input.TimeZone, price);

            if (!IsReady) return 0;

            var minL = _medianMin.Current.Price;
            var maxH = _medianMax.Current.Price;

            if (minL != maxH)
            {
                x = _alpha * 2 * ((double)((price - minL) / (maxH - minL)) - .5) + (1 - _alpha) * _previous;
                y = FisherTransformFunction(x);
            }
            _previous = x;

            return Convert.ToDecimal(y) + .5m * Current.Price;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// The Fisher transform is a mathematical process which is used to convert any data set to a modified
        /// data set whose Probabilty Distrbution Function is approximately Gaussian.  Once the Fisher transform
        /// is computed, the transformed data can then be analyzed in terms of it's deviation from the mean.
        ///
        /// The equation is y = .5 * ln [ 1 + x / 1 - x ] where
        /// x is the input
        /// y is the output
        /// ln is the natural logarithm
        ///
        /// The Fisher transform has much sharper turning points than other indicators such as MACD
        ///
        /// For more info, read chapter 1 of Cybernetic Analysis for Stocks and Futures by John F. Ehlers
        /// </summary>
        /// <param name="x">Input</param>
        /// <returns>Output</returns>
        private double FisherTransformFunction(double x)
        {
            if (x > .99)
            {
                x = .999;
            }
            if (x < -.99)
            {
                x = -.999;
            }

            return .5 * Math.Log((1.0 + x) / (1.0 - x));
        }

        #endregion Private Methods
    }
}