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
        public TradingRecommendation(Instrument Instrument, IMoney BuyPrice, IMoney SellPrice, DateTime PredictedBuyDate, DateTime PredictedSellDate, TradingForecastMethod TradingForeCastMehtod)
        {
            m_oTradingForecastMethod = TradingForecastMethod;
            m_oInstrument = Instrument;
            m_oBuyPrice = BuyPrice;
            m_oSellPrice = SellPrice;
            m_oPredictedBuyDate = PredictedBuyDate;
            m_oPredictedSellDate = PredictedSellDate;
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
