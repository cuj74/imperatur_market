using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.securites;
using Imperatur_v2.monetary;

namespace Imperatur_v2.trade.recommendation
{
    public enum TradingForecastMethod
    {
        Elliott,
        Bollinger,
        Crossover,
        LongShortCrossover,
        TripleCrossover,
        Undefined
    }

    public class TradingRecommendation : ITradingRecommendation
    {
        private Instrument m_oInstrument;
        private IMoney m_oBuyPrice;
        private IMoney m_oSellPrice;
        private DateTime m_oPredictedBuyDate;
        private DateTime m_oPredictedSellDate;
        private TradingForecastMethod m_oTradingForecastMethod;

        public TradingRecommendation()
        {
            m_oTradingForecastMethod = TradingForecastMethod.Undefined;
        }
        public TradingRecommendation(Instrument instrument, IMoney buyPrice, IMoney sellPrice, DateTime predictedBuyDate, DateTime predictedSellDate, TradingForecastMethod tradingForeCastMethod)
        {
            m_oTradingForecastMethod = tradingForeCastMethod;
            m_oInstrument = instrument;
            m_oBuyPrice = buyPrice;
            m_oSellPrice = sellPrice;
            m_oPredictedBuyDate = predictedBuyDate;
            m_oPredictedSellDate = predictedSellDate;
        }


        public Instrument Instrument
        {
            get
            {
                return m_oInstrument;
            }
        }

        public IMoney BuyPrice
        {
            get
            {
                return m_oBuyPrice;
            }
        }

        public IMoney SellPrice
        {
            get
            {
                return m_oSellPrice;
            }

        }

        public DateTime PredictedBuyDate
        {
            get
            {
                return m_oPredictedBuyDate;
            }


        }

        public DateTime PredictedSellDate
        {
            get
            {
                return m_oPredictedSellDate;
            }

        }

        public TradingForecastMethod TradingForecastMethod
        {
            get
            {
                return m_oTradingForecastMethod;
            }
        }
    }
}
