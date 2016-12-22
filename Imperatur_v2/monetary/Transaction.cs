using Imperatur_v2.trade;
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
        private Money _DebitAmount;
        private Money _CreditAmount;
        private TransactionType _TransactionType;
        public Trade _SecuritiesTrade;

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
        public Money DebitAmount
        {
            get { return _DebitAmount; }
        }
        public Money CreditAmount
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

        public Transaction(Money DebitAmount, Money CreditAmount, Guid DebitAccount, Guid CreditAccount, TransactionType TransactionType, Trade SecurtiesTrade)
        {
            _DebitAmount = DebitAmount;
            _CreditAmount = CreditAmount;
            _DebitAccount = DebitAccount;
            _CreditAccount = CreditAccount;
            _TransactionType = TransactionType;
            _SecuritiesTrade = SecurtiesTrade;
            if (!DebitAmount.Amount.Equals(CreditAmount.Amount))
            {
                throw new Exception("Amount is not equal");
            }
        }
        public Money GetGAA()
        {
            NoTransferOnlySecurites();
            return _SecuritiesTrade.GetGAA();
        }
        public Decimal GetQuantity()
        {
            NoTransferOnlySecurites();
            return _SecuritiesTrade.GetQuantity();
        }
        public Money GetCurrentValue()
        {
            NoTransferOnlySecurites();
            return _SecuritiesTrade.GetTradeAmount();
        }
        public Money GetTradeAmount()
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
