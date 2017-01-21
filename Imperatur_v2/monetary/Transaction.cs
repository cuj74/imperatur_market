using Imperatur_v2.trade;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.monetary
{

    public class Transaction : ITransactionInterface
    {
        private Guid _DebitAccount; //reference to the accounts
        private Guid _CreditAccount;
        private IMoney _DebitAmount;
        private IMoney _CreditAmount;
        private TransactionType _TransactionType;
        public ITradeInterface _SecuritiesTrade;
        private DateTime _TransactionDate;
        private string _ProcessCode;

        public Guid DebitAccount
        {
            get { return _DebitAccount; }
        }
        public Guid CreditAccount
        {
            get { return _CreditAccount; }
        }

        public TransactionType TransactionType
        {
            get { return _TransactionType; }
        }
        public IMoney DebitAmount
        {
            get { return _DebitAmount; }
        }
        public IMoney CreditAmount
        {
            get { return _CreditAmount; }
        }

        Guid ITransactionInterface.DebitAccount
        {
            get
            {
                return _DebitAccount;
            }
        }

        Guid ITransactionInterface.CreditAccount
        {
            get
            {
                return _CreditAccount;
            }
        }

        TransactionType ITransactionInterface.TransactionType
        {
            get
            {
                return _TransactionType;
            }
        }

        public ITradeInterface SecuritiesTrade
        {
            get
            {
                return _SecuritiesTrade;
            }
        }

        public DateTime TransactionDate
        {
            get
            {
                return _TransactionDate;
            }
        }

        public string ProcessCode
        {
            get
            {
                return _ProcessCode;
            }

        }

        [JsonConstructor]
        public Transaction(IMoney _DebitAmount, IMoney _CreditAmount, Guid _DebitAccount, Guid _CreditAccount, TransactionType _TransactionType, ITradeInterface _SecurtiesTrade,  DateTime _TransactionDate, string _ProcessCode = "Manual")
        {
            this._DebitAmount = _DebitAmount;
            this._CreditAmount = _CreditAmount;
            this._DebitAccount = _DebitAccount;
            this._CreditAccount = _CreditAccount;
            this._TransactionType = _TransactionType;
            this._SecuritiesTrade = _SecurtiesTrade;
            this._TransactionDate = _TransactionDate;
            this._ProcessCode = _ProcessCode;
            if (!_DebitAmount.Amount.Equals(_CreditAmount.Amount))
            {
                throw new Exception("Amount is not equal");
            }
        }

        public Transaction(IMoney _DebitAmount, IMoney _CreditAmount, Guid _DebitAccount, Guid _CreditAccount, TransactionType _TransactionType, ITradeInterface _SecurtiesTrade, string _ProcessCode = "Manual")
        {
            this._DebitAmount = _DebitAmount;
            this._CreditAmount = _CreditAmount;
            this._DebitAccount = _DebitAccount;
            this._CreditAccount = _CreditAccount;
            this._TransactionType = _TransactionType;
            this._SecuritiesTrade = _SecurtiesTrade;
            this._TransactionDate = DateTime.Now;
            this._ProcessCode = _ProcessCode;
            if (!_DebitAmount.Amount.Equals(_CreditAmount.Amount))
            {
                throw new Exception("Amount is not equal");
            }
        }

        public IMoney GetGAA()
        {
            NoTransferOnlySecurites();
            return _SecuritiesTrade.GetGAA();
        }
        public Decimal GetQuantity()
        {
            NoTransferOnlySecurites();
            return _SecuritiesTrade.GetQuantity();
        }
        public IMoney GetCurrentValue()
        {
            NoTransferOnlySecurites();
            return _SecuritiesTrade.GetTradeAmount();
        }
        public IMoney GetTradeAmount()
        {
            NoTransferOnlySecurites();
            return _SecuritiesTrade.GetTradeAmount();
        }

        private void NoTransferOnlySecurites()
        {
            if (_SecuritiesTrade == null || _TransactionType.Equals(TransactionType.Transfer))
                throw new Exception("No available on transfer");
        }

    }

    public enum TransactionType
    {
        Buy,
        Sell,
        Transfer,
        Withdrawal
    }

}
