using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System;
using System.Data;

namespace cAlgo
{
    [Indicator(AccessRights = AccessRights.None)]
    public class MyCandlestickPatterns : Indicator
    {
        #region Parameters

        [Parameter("Doji", DefaultValue = "D")]
        public string Doji_s { get; set; }
        [Parameter("Hammer", DefaultValue = "H")]
        public string Hammer_s { get; set; }

        [Parameter("Bullish Harami", DefaultValue = "BH")]
        public string BullishHarami_s { get; set; }

        [Parameter("Bearish Harami", DefaultValue = "BH")]
        public string BearishHarami_s { get; set; }

        [Parameter("Dark Cloud Cover", DefaultValue = "DCC")]
        public string DarkCloudCover_s { get; set; }

        [Parameter("Piercing", DefaultValue = "P")]
        public string Piercing_s { get; set; }

        [Parameter("Bullish Engulfing", DefaultValue = "BE")]
        public string BullishEngulfing_s { get; set; }

        [Parameter("Bearish Engulfing", DefaultValue = "BE")]
        public string BearishEngulfing_s { get; set; }

        [Parameter("Shooting Star", DefaultValue = "SS")]
        public string ShootingStar_s { get; set; }

        [Parameter("Evening Star", DefaultValue = "ES")]
        public string EveningStar_s { get; set; }

        [Parameter("Morning Star", DefaultValue = "MS")]
        public string MorningStar_s { get; set; }

        [Parameter("PinBar", DefaultValue = "PB")]
        public string PinBar_s { get; set; }

        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        #endregion

        private double offset;
        private RelativeStrengthIndex _rsi;
        private ExponentialMovingAverage ema;

        protected override void Initialize()
        {
            offset = Symbol.PipSize * 5;
            _rsi = Indicators.RelativeStrengthIndex(Source, 14);
            ema = Indicators.ExponentialMovingAverage(Source, 21);
        }

        public override void Calculate(int index)
        {
            if (index <= 0)
                return;

            CandlestickPatternType pattern = CandlestickPatternType.Unknown;

            double bodySize = Math.Abs(Source[index] - Source[index - 1]);

            if (IsDoji(index, bodySize))
            {
                pattern = CandlestickPatternType.Doji;
                DrawSymbols(index, CandlestickPatternType.Doji);
            }
            else if (IsHammer(index))
            {
                pattern = CandlestickPatternType.Hammer;
                DrawSymbols(index, CandlestickPatternType.Hammer);
            }
            else if (IsBullishHarami(index))
            {
                pattern = CandlestickPatternType.BullishHarami;
                DrawSymbols(index, CandlestickPatternType.BullishHarami);
            }
            else if (IsBearishHarami(index))
            {
                pattern = CandlestickPatternType.BearishHarami;
                DrawSymbols(index, CandlestickPatternType.BearishHarami);
            }
            else if (IsDarkCloudCover(index))
            {
                pattern = CandlestickPatternType.DarkCloudCover;
                DrawSymbols(index, CandlestickPatternType.DarkCloudCover);
            }
            else if (IsPiercingPattern(index))
            {
                pattern = CandlestickPatternType.PiercingPattern;
                DrawSymbols(index, CandlestickPatternType.PiercingPattern);
            }
            else if (IsBullishEngulfing(index))
            {
                pattern = CandlestickPatternType.BullishEngulfing;
                DrawSymbols(index, CandlestickPatternType.BullishEngulfing);
            }
            else if (IsBearishEngulfing(index))
            {
                pattern = CandlestickPatternType.BearishEngulfing;
                DrawSymbols(index, CandlestickPatternType.BearishEngulfing);
            }
            else if (IsShootingStar(index))
            {
                pattern = CandlestickPatternType.ShootingStar;
                DrawSymbols(index, CandlestickPatternType.ShootingStar);
            }
            else if (IsEveningStar(index))
            {
                pattern = CandlestickPatternType.EveningStar;
                DrawSymbols(index, CandlestickPatternType.EveningStar);
            }
            else if (IsMorningStar(index))
            {
                pattern = CandlestickPatternType.MorningStar;
                DrawSymbols(index, CandlestickPatternType.MorningStar);
            }
            else if (IsPinBar(index))
            {
                pattern = CandlestickPatternType.PinBar;
                DrawSymbols(index, CandlestickPatternType.PinBar);
            }

            if (pattern != CandlestickPatternType.Unknown)
                Print("Pattern identificato: {0}", pattern.ToString());
        }

        private bool IsMorningStar(int index)
        {
            var previousCandle = Bars[index - 1];
            var currentCandle = Bars[index];

            return false;
        }

        private bool IsEveningStar(int index)
        {
            var previousCandle = Bars[index - 1];
            var currentCandle = Bars[index];

            return false;
        }

        private bool IsPinBarOld(int index)
        {
            double open = Bars.OpenPrices[index];
            double high = Bars.HighPrices[index];
            double low = Bars.LowPrices[index];
            double close = Bars.ClosePrices[index];

            double prevOpen = Bars.OpenPrices[index - 1];
            double prevHigh = Bars.HighPrices[index - 1];
            double prevLow = Bars.LowPrices[index - 1];
            double prevClose = Bars.ClosePrices[index - 1];

            bool isBullishPinBar = false;
            bool isBearishPinBar = false;

            double bodyLength = Math.Abs(open - close);
            double upperWickLength = Math.Abs(high - Math.Max(open, close));
            double lowerWickLength = Math.Abs(low - Math.Min(open, close));

            if (bodyLength >= (2.0 * Math.Min(upperWickLength, lowerWickLength)))
            {
                if (open < prevLow && close > prevHigh)
                    isBullishPinBar = true;
                else if (open > prevHigh && close < prevLow)
                    isBearishPinBar = true;
            }

            if (isBullishPinBar)
                Chart.DrawText("bullishPinBar" + index, "Bullish Pin Bar", index, low - Symbol.PipSize, Color.Green);
            else if (isBearishPinBar)
                Chart.DrawText("bearishPinBar" + index, "Bearish Pin Bar", index, high + Symbol.PipSize, Color.Red);

            return isBullishPinBar || isBearishPinBar;
        }

        private bool IsPinBarOld2(int index)
        {
            double currentHigh = Bars.HighPrices[index];
            double currentLow = Bars.LowPrices[index];
            double currentOpen = Bars.OpenPrices[index];
            double currentClose = Bars.ClosePrices[index];
            double previousHigh = Bars.HighPrices[index - 1];
            double previousLow = Bars.LowPrices[index - 1];

            // Calculate the size of the body and tail
            double bodySize = Math.Abs(currentClose - currentOpen);
            double tailSize = Math.Min(Math.Abs(currentLow - previousLow), Math.Abs(currentHigh - previousHigh));

            return (bodySize / tailSize >= 50) && (currentHigh > previousHigh && currentLow > previousLow) || (currentLow < previousLow && currentHigh < previousHigh) && tailSize > bodySize;
        }

        private bool IsPinBar(int index)
        {           
            double previousHigh = Bars.HighPrices[index - 1];
            double previousLow = Bars.LowPrices[index - 1];
            double currentHigh = Bars.HighPrices[index];
            double currentLow = Bars.LowPrices[index];
            double currentOpen = Bars.OpenPrices[index];
            double currentClose = Bars.ClosePrices[index];

            bool isBearish = currentOpen > currentClose;
            double bodySize = Math.Abs(currentOpen - currentClose);
            double upperShadow = currentHigh - Math.Max(currentOpen, currentClose);
            double lowerShadow = Math.Min(currentOpen, currentClose) - currentLow;

            bool isTouchingOrBreakingKeyLevel = upperShadow >= bodySize && currentHigh >= previousHigh || lowerShadow >= bodySize && currentLow <= previousLow;
            bool isTouchingOrBreakingEma = upperShadow >= bodySize && currentHigh >= ema.Result.Last(index) || lowerShadow >= bodySize && currentLow <= ema.Result.Last(index);
            bool isBodyInsidePreviousRange = currentHigh <= previousHigh && currentLow >= previousLow;
            bool isCurrentHighGreaterThanPreviousHigh = currentHigh > previousHigh;

            return isBearish && isTouchingOrBreakingKeyLevel && isTouchingOrBreakingEma && isBodyInsidePreviousRange && isCurrentHighGreaterThanPreviousHigh;
        }

        private bool IsShootingStar(int index)
        {
            var currentCandle = Bars[index];

            // Ombra superiore della candela corrente
            double currentUpperShadow = currentCandle.High - Math.Max(currentCandle.Open, currentCandle.Close);

            // Ombra inferiore della candela corrente
            double currentLowerShadow = Math.Min(currentCandle.Open, currentCandle.Close) - currentCandle.Low;

            // Lunghezza totale della candela corrente
            double currentCandleLength = Math.Abs(currentCandle.Open - currentCandle.Close);

            // Se la candela corrente ha un'ombra superiore almeno due volte più grande del corpo e un'ombra inferiore molto piccola
            return _rsi.Result.LastValue > 50 && currentUpperShadow >= 2 * currentCandleLength && currentLowerShadow <= 0.1 * currentCandleLength;

            ////Altro modo:
            //double upperShadow = currentCandle.High - currentCandle.Close;
            //double lowerShadow = currentCandle.Open - currentCandle.Low;

            //// Calcolo della dimensione del corpo della candela
            //double bodySize = currentCandle.Close - currentCandle.Open;

            //// Calcolo della soglia di accettazione per la dimensione del corpo
            //double bodyThreshold = Symbol.TickSize * 5;

            //// Controllo se la candela soddisfa le condizioni per essere una Shooting Star
            //return _rsi.Result.LastValue > 50 && upperShadow >= 2 * bodySize && upperShadow > lowerShadow && bodySize <= bodyThreshold;
        }

        private bool IsBearishEngulfing(int index)
        {
            var previousCandle = Bars[index - 1];
            var currentCandle = Bars[index];

            var isBullishPrev = IsBullish(previousCandle);
            var isBearishCurr = IsBearish(currentCandle);

            return isBearishCurr && isBullishPrev &&
                 _rsi.Result.LastValue > 50 && IsCandleWithLargeBody(previousCandle) &&
                 currentCandle.Open > previousCandle.Close && previousCandle.Open > currentCandle.Close &&
                 currentCandle.High > previousCandle.High && currentCandle.Low < previousCandle.Low;
        }

        private bool IsBullishEngulfing(int index)
        {
            var previousCandle = Bars[index - 1];
            var currentCandle = Bars[index];

            var isBullishCurr = IsBullish(currentCandle);
            var isBearishPrev = IsBearish(previousCandle);

            return isBullishCurr && isBearishPrev &&
                _rsi.Result.LastValue < 50 && IsCandleWithLargeBody(previousCandle) &&
                currentCandle.Open < previousCandle.Close && previousCandle.Open < currentCandle.Close &&
                currentCandle.High > previousCandle.High && currentCandle.Low < previousCandle.Low;
            
        }

        private bool IsPiercingPattern(int index)
        {
            var previousCandle = Bars[index - 1];
            var currentCandle = Bars[index];

            var isBullishCurr = IsBullish(currentCandle);
            var isBearishPrev = IsBearish(previousCandle);
            var body = previousCandle.Open - previousCandle.Close;
            var mid = previousCandle.Close + body / 2;

            return isBullishCurr && isBearishPrev &&
                    _rsi.Result.LastValue < 50 &&
                   currentCandle.Open < previousCandle.Low && 
                   currentCandle.Close >= mid && 
                   currentCandle.High < previousCandle.Open;
        }

        private bool IsDarkCloudCover(int index)
        {
            var previousCandle = Bars[index - 1];
            var currentCandle = Bars[index];

            //Primo metodo:
            //// Calcolare l'intervallo di prezzo medio della precedente candela
            //double previousCandleRange = (previousCandle.High - previousCandle.Low);
            //// Calcolare il punto medio del corpo della precedente candela
            //double previousCandleMidpoint = previousCandle.Low + (previousCandleRange / 2);
            //return IsBearish(currentCandle) && IsBullish(previousCandle) &&
            //       currentCandle.Open > previousCandleMidpoint &&
            //       currentCandle.Close < previousCandle.Open &&
            //       currentCandle.Close > previousCandleMidpoint &&
            //       currentCandle.Open < previousCandle.Close &&
            //       currentCandle.Open - currentCandle.Close < previousCandle.Open - previousCandle.Close;

            ////Altro modo:
            //var isBearishCurr = IsBearish(currentCandle);
            //var isBullishPrev = IsBullish(previousCandle);
            //var isBetweenPrev = currentCandle.Open < previousCandle.Close && currentCandle.Close > previousCandle.Open;
            //var isRetracement = currentCandle.Open > previousCandle.Close;
            //var isCloseBelowPrevMidpoint = currentCandle.Close < ((previousCandle.Close + previousCandle.Open) / 2);
            //return isBearishCurr && isBullishPrev && isBetweenPrev && isRetracement && isCloseBelowPrevMidpoint;

            //Altro modo:
            //var prevCandleMidPoint = (previousCandle.High + previousCandle.Low) / 2;
            //var currentCandleGapUp = currentCandle.Open > previousCandle.High;
            //var currentCandleBearish = IsBearish(currentCandle);
            //var prevCandleBullish = IsBullish(previousCandle);
            //var currentCandleClosesBelowMidPoint = currentCandle.Close < prevCandleMidPoint;
            //return _rsi.Result.LastValue > 50 && prevCandleBullish && currentCandleGapUp && currentCandleBearish && currentCandleClosesBelowMidPoint;

            //Altro Modo:
            var isBullishPrev = IsBullish(previousCandle);
            var isBearishCurr = IsBearish(currentCandle);
            var bearishRange = currentCandle.Open - currentCandle.Close;
            var bullishRange = previousCandle.Close - previousCandle.Open;
            var midpoint = previousCandle.Open + bullishRange / 2;

            return isBullishPrev && isBearishCurr &&
                _rsi.Result.LastValue > 50 &&
                previousCandle.Open < currentCandle.Low &&
                currentCandle.Open > previousCandle.High &&
                currentCandle.Close <= midpoint - (bearishRange * 60 / 100);
        }

        private bool IsBearishHarami(int index)
        {
            var previousCandle = Bars[index - 1];
            var currentCandle = Bars[index];

            return IsBullish(previousCandle) && IsBearish(currentCandle) &&
                currentCandle.Low >= previousCandle.Low && currentCandle.High <= previousCandle.High &&
                currentCandle.Close < previousCandle.Open && currentCandle.Open > previousCandle.Close &&
                currentCandle.Open < previousCandle.Open && currentCandle.Close > previousCandle.Close;
        }

        private bool IsBullishHarami(int index)
        {
            var previousCandle = Bars[index - 1];
            var currentCandle = Bars[index];

            return IsBullish(currentCandle) && IsBearish(previousCandle) && 
                currentCandle.Low >= previousCandle.Low && currentCandle.High <= previousCandle.High &&
                currentCandle.Close < previousCandle.Open && currentCandle.Open > previousCandle.Close && 
                currentCandle.Open < previousCandle.Open && currentCandle.Close > previousCandle.Close;
        }

        private bool IsHammer(int index)
        {
            var currentCandle = Bars[index];

            //bool isBullish = IsBullish(currentCandle);
            double body = Math.Abs(currentCandle.Open - currentCandle.Close);
            double shadowTop = Math.Abs(currentCandle.High - Math.Max(currentCandle.Open, currentCandle.Close));
            double shadowBottom = Math.Abs(currentCandle.Low - Math.Min(currentCandle.Open, currentCandle.Close));

            return shadowTop >= body * 2 && shadowBottom <= body * 0.5;
        }

        private bool IsDoji(int index, double bodySize)
        {
            double totalSize = Math.Abs(Source[index] - Source[index - 1]) + Math.Abs(Source[index - 1] - Source[index - 2]);
            return (bodySize < totalSize * 0.05) && (bodySize > 0);
        }

        private bool IsBullish(Bar candle)
        {
            return candle.Close > candle.Open;
        }

        private bool IsBearish(Bar candle)
        {
            return candle.Close < candle.Open;
        }

        public bool IsCandleWithLargeBody(Bar candle)
        {
            double body = Math.Abs(candle.Open - candle.Close);
            double upperShadow = candle.High - Math.Max(candle.Open, candle.Close);
            double lowerShadow = Math.Min(candle.Open, candle.Close) - candle.Low;

            return body > upperShadow && body > lowerShadow;
        }

        private void DrawText(int index, CandlestickPatternType _Type)
        {
            var high = Source[index];
            var low = Source[index];

            int x = index;

            double h_y = high + offset;
            double h_d_y = h_y + offset * 2.5;
            double h_t_y = h_d_y + offset;

            double l_y = low - offset * 2.5;
            double l_d_y = l_y - offset;


            if (TimeFrame == TimeFrame.Minute)
            {
                h_y = high + offset / 2;
                h_d_y = h_y + offset / 2;
                h_t_y = h_d_y + offset;

                l_y = low - offset / 2;
                l_d_y = l_y - offset / 2;
            }



            if (TimeFrame == TimeFrame.Minute15)
            {
                h_y = high + offset / 1.5;
                h_d_y = h_y + offset;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 1.5;
                l_d_y = l_y - offset * 1.1;
            }


            if (TimeFrame == TimeFrame.Hour)
            {
                h_y = high + offset;
                h_d_y = h_y + offset * 3;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 3;
                l_d_y = l_y - offset * 3;
            }

            if (TimeFrame == TimeFrame.Hour4)
            {
                h_y = high + offset;
                h_d_y = h_y + offset * 5;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 5;
                l_d_y = l_y - offset * 5;
            }

            if (TimeFrame == TimeFrame.Daily)
            {
                h_y = high + offset * 3;
                h_d_y = h_y + offset * 12;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 12;
                l_d_y = l_y - offset * 12;
            }

            if (TimeFrame == TimeFrame.Weekly)
            {
                h_y = high + offset * 6;
                h_d_y = h_y + offset * 24;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 24;
                l_d_y = l_y - offset * 24;
            }

            if (TimeFrame == TimeFrame.Monthly)
            {
                h_y = high + offset * 24;
                h_d_y = h_y + offset * 64;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 64;
                l_d_y = l_y - offset * 64;
            }

            string f_ObjName;
            string s_ObjName;

            switch (_Type)
            {
                case CandlestickPatternType.Doji:

                    f_ObjName = string.Format("Doji {0}", index);
                    s_ObjName = string.Format("Doji | {0}", index);

                    Chart.DrawText(f_ObjName, "Doji", index, h_d_y, Color.White);
                    Chart.DrawText(s_ObjName, "\n|", index, h_y, Color.White);

                    break;

                case CandlestickPatternType.Hammer:

                    f_ObjName = string.Format("Hammer {0}", index);
                    s_ObjName = string.Format("Hammer | {0}", index);

                    Chart.DrawText(f_ObjName, "Hammer", index, h_y, Color.White);
                    Chart.DrawText(s_ObjName, "\n|", index, h_y, Color.White);

                    break;
                case CandlestickPatternType.BullishHarami:

                    f_ObjName = string.Format("Bullish Harami {0}", index);
                    s_ObjName = string.Format("Bullish Harami | {0}", index);

                    Chart.DrawText(f_ObjName, "Bullish Harami", index, h_d_y, Color.White);
                    Chart.DrawText(s_ObjName, "\n|", index, h_y, Color.White);

                    break;

            }
        }

        private void DrawSymbols(int index, CandlestickPatternType _Type)
        {
            var high = Source[index];
            var low = Source[index];

            double h_y = high + offset;
            double h_d_y = h_y + offset * 2.5;
            double h_t_y = h_d_y + offset;

            double l_y = low - offset * 2.5;
            double l_d_y = l_y - offset;

            if (TimeFrame == TimeFrame.Minute)
            {
                h_y = high + offset / 2;
                h_d_y = h_y + offset / 2;
                h_t_y = h_d_y + offset;

                l_y = low - offset / 2;
                l_d_y = l_y - offset / 2;
            }

            if (TimeFrame == TimeFrame.Minute15)
            {
                h_y = high + offset / 1.5;
                h_d_y = h_y + offset;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 1.5;
                l_d_y = l_y - offset * 1.1;
            }

            if (TimeFrame == TimeFrame.Hour)
            {
                h_y = high + offset;
                h_d_y = h_y + offset * 3;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 3;
                l_d_y = l_y - offset * 3;
            }

            if (TimeFrame == TimeFrame.Hour4)
            {
                h_y = high + offset;
                h_d_y = h_y + offset * 5;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 5;
                l_d_y = l_y - offset * 5;
            }

            if (TimeFrame == TimeFrame.Daily)
            {
                h_y = high + offset * 3;
                h_d_y = h_y + offset * 12;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 12;
                l_d_y = l_y - offset * 12;
            }

            if (TimeFrame == TimeFrame.Weekly)
            {
                h_y = high + offset * 6;
                h_d_y = h_y + offset * 24;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 24;
                l_d_y = l_y - offset * 24;
            }

            if (TimeFrame == TimeFrame.Monthly)
            {
                h_y = high + offset * 24;
                h_d_y = h_y + offset * 64;
                h_t_y = h_d_y + offset;

                l_y = low - offset * 64;
                l_d_y = l_y - offset * 64;
            }

            string f_ObjName;

            switch (_Type)
            {
                case CandlestickPatternType.Doji:

                    f_ObjName = string.Format("Doji {0}", index);
                    Chart.DrawText(f_ObjName, Doji_s, index, h_d_y, Color.White);
                    break;

                case CandlestickPatternType.Hammer:

                    f_ObjName = string.Format("Hammer {0}", index);
                    Chart.DrawText(f_ObjName, Hammer_s, index, h_d_y, Color.Aqua);
                    break;

                case CandlestickPatternType.BullishHarami:

                    f_ObjName = string.Format("Bullish Harami {0}", index);
                    Chart.DrawText(f_ObjName, BullishHarami_s, index, h_d_y, Color.Green);
                    break;

                case CandlestickPatternType.BearishHarami:

                    f_ObjName = string.Format("Bearish Harami {0}", index);
                    Chart.DrawText(f_ObjName, BearishHarami_s, index, h_d_y, Color.Red);
                    break;

                case CandlestickPatternType.DarkCloudCover:
                    f_ObjName = string.Format("Dark Cloud Cover {0}", index);
                    Chart.DrawText(f_ObjName, DarkCloudCover_s, index, h_d_y, Color.Blue);
                    break;

                case CandlestickPatternType.PiercingPattern:
                    f_ObjName = string.Format("Piercing {0}", index);
                    Chart.DrawText(f_ObjName, Piercing_s, index, h_d_y, Color.Brown);
                    break;

                case CandlestickPatternType.BullishEngulfing:
                    f_ObjName = string.Format("Bullish Engulfing {0}", index);
                    Chart.DrawText(f_ObjName, BullishEngulfing_s, index, h_d_y, Color.Coral);
                    break;

                case CandlestickPatternType.BearishEngulfing:
                    f_ObjName = string.Format("Bearish Engulfing {0}", index);
                    Chart.DrawText(f_ObjName, BearishEngulfing_s, index, h_d_y, Color.HotPink);
                    break;

                case CandlestickPatternType.ShootingStar:
                    f_ObjName = string.Format("Shooting Star {0}", index);
                    Chart.DrawText(f_ObjName, ShootingStar_s, index, h_d_y, Color.Yellow);
                    break;

                case CandlestickPatternType.EveningStar:
                    f_ObjName = string.Format("Evening Star {0}", index);
                    Chart.DrawText(f_ObjName, EveningStar_s, index, h_d_y, Color.Violet);
                    break;

                case CandlestickPatternType.MorningStar:
                    f_ObjName = string.Format("Morning Star {0}", index);
                    Chart.DrawText(f_ObjName, MorningStar_s, index, h_d_y, Color.GreenYellow);
                    break;

                case CandlestickPatternType.PinBar:
                    f_ObjName = string.Format("PinBar {0}", index);
                    Chart.DrawText(f_ObjName, PinBar_s, index, h_d_y, Color.DarkMagenta);
                    break;

                default:
                    break;
            }
        }

        public enum CandlestickPatternType
        {
            Doji,
            Hammer,
            BullishHarami,
            BearishHarami,
            DarkCloudCover,
            PiercingPattern,
            BullishEngulfing,
            BearishEngulfing,
            ShootingStar,
            EveningStar,
            MorningStar,
            PinBar,
            Unknown
        }
    }
}