using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.trade;
using Imperatur_v2.securites;
using Imperatur_v2.trade.analysis;
using Imperatur_v2.monetary;

namespace Imperatur_v2.handler
{
    public interface ITradeHandlerInterface
    {
        ITradeInterface GetTrade(string Symbol, decimal Quantity);
        ITradeInterface GetTrade(string Symbol, decimal Quantity, DateTime TradeDateTime, IMoney Revenue);

        ISecurityAnalysis GetSecurityAnalysis(string Symbol);
        ISecurityAnalysis GetSecurityAnalysis(Instrument Instrument);

        // ISecurityAnalysis SecurityAnalysis { get; }
        List<Quote> GetQuotes();
        Quote GetQuote(string Symbol);
        bool ForceUpdate();
        void CacheQuotes();
        event ImperaturMarket.QuoteUpdateHandler QuoteUpdateEvent;
    }
}
