using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.trade;
using Imperatur_v2.securites;

namespace Imperatur_v2.handler
{
    public interface ITradeHandlerInterface
    {
        List<Quote> GetQuotes();
        Quote GetQuote(string Symbol);
    }
}
