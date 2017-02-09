using Imperatur_Market_Core.account;
using Imperatur_Market_Core.database;
using Imperatur_Market_Core.securities;
using Imperatur_Market_Core.system;
using Imperatur_Market_Core.trade;
using Imperatur_Market_Core.user;
using Imperatur_Market_Core.monetary;

namespace Imperatur_Market_Core
{
    public interface IImperaturMarket
    {
        IAccountHandler AccountHandler { get; }
        IDatabaseHandler DatabaseHandler { get; }
        ISecurityHandler SecurityHandler { get; }
        ISystemHandler SystemHandler { get; }
        ITradeHandler TradeHandler { get; }
        IUserHandler UserHandler { get; }
        ILogicalTransactionHandler LogicalTransactionHandler { get; }
    }
}