﻿#region License Header

/*
* QUANTLER.COM - Quant Fund Development Platform
* Quantler Core Trading Engine. Copyright 2018 Quantler B.V.
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
*/

#endregion License Header

using System.Composition;
using Quantler.Securities;

namespace Quantler.Data.Market.Filter
{
    /// <summary>
    /// Only allow ticks which are quotes
    /// </summary>
    [Export(typeof(DataFilter))]
    public class QuotesOnlyTickFilter : DataFilter
    {
        #region Public Methods

        /// <summary>
        /// Regular checks for accepting tick data
        /// </summary>
        /// <param name="security"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Accept(Security security, DataPoint data) =>
            //Only accept full quotes
            !(data is Tick) || ((Tick) data).IsValid && ((Tick) data).IsFullQuote;

        #endregion Public Methods
    }
}