using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur_v2.shared;
using Ninject;
using Newtonsoft.Json;

namespace Imperatur_v2.monetary
{
    public class Money : IMoney
    {
        private ICurrency m_oCurrencyCode;
        private Decimal m_oAmount;
        //private int v;
        //private Currency currency;

        #region Constructor
        /*
    public Money(decimal Amount, ICurrency Currency)
    {
        this.m_oAmount = Amount;
        this.m_oCurrencyCode = Currency;
    }*/
        //[Inject]
        [JsonConstructor]
        public Money(decimal m_oAmount, ICurrency m_oCurrencyCode)
        {
            this.m_oAmount = m_oAmount;
            this.m_oCurrencyCode = m_oCurrencyCode;
        }
        #endregion

        public IMoney Add(IMoney Add)
        {
            if (m_oCurrencyCode != Add.CurrencyCode)
                throw new Exception("Can't add two money objects with different currency");

            if (Add.Amount().Equals(0))
                return this;

            return GetMoney(m_oAmount + Add.Amount(), m_oCurrencyCode);
        }

        public IMoney Add(decimal Add)
        {
            return this.Add(GetMoney(Add, m_oCurrencyCode));
        }

        public IMoney Divide(IMoney Divider)
        {
            if (this.m_oCurrencyCode != Divider.CurrencyCode)
                throw new Exception("Can't divide two money objects with different currency");
            if (Divider.Amount() == 0)
                throw new Exception("Can't divide by zero");

            return GetMoney(m_oAmount / Divider.Amount(), m_oCurrencyCode);
        }

        public IMoney Divide(decimal Divider)
        {

            return this.Divide(GetMoney(Divider, m_oCurrencyCode));
        }

        public IMoney Multiply(decimal Multiplier)
        {
            return GetMoney(this.m_oAmount * Multiplier, m_oCurrencyCode);
        }

        public IMoney Subtract(IMoney Subtract)
        {
            if (!m_oCurrencyCode.Equals(Subtract.CurrencyCode))
                throw new Exception("Can't add two money objects with different currency");
            if (Subtract.Amount().Equals(0))
                return this;

            return GetMoney(m_oAmount - Subtract.Amount(), m_oCurrencyCode);
        }

        public IMoney Subtract(decimal Subtract)
        {
            return this.Subtract(GetMoney(Subtract, m_oCurrencyCode));
        }

        private IMoney GetMoney(decimal Amount, ICurrency Currency)
        {
            return ImperaturGlobal.GetMoney(Amount, Currency);
        }
        public IMoney SwitchSign
        {
            get
            {
                return GetMoney(-this.m_oAmount, m_oCurrencyCode);
            }
        }

        public override string ToString()
        {
            return ToString(false, true);
        }

        public string ToString(bool WithSign, bool WithCurrencyCode)
        {
            //no need add a minussign!
            if (WithSign)
                return string.Format("{0}{1} {2}", m_oAmount == 0 ? "" : m_oAmount > 0 ? "+" : "", Math.Round(m_oAmount, 2, MidpointRounding.AwayFromZero).ToString("#,0.00", System.Globalization.CultureInfo.GetCultureInfo("sv-SE")), WithCurrencyCode ? m_oCurrencyCode.GetCurrencyString().ToUpper().Trim() : "");
            else
                return string.Format("{0} {1}", Math.Round(m_oAmount, 2, MidpointRounding.AwayFromZero).ToString("#,0.00", System.Globalization.CultureInfo.GetCultureInfo("sv-SE")), WithCurrencyCode ? m_oCurrencyCode.GetCurrencyString().ToUpper().Trim() : "");

        }
        public ICurrency CurrencyCode
        {
            get
            {
                return m_oCurrencyCode;
            }
        }

        public decimal Amount()
        {
            return this.m_oAmount;
        }


/*
        public Money(int v, Currency currency)
        {
            this.v = v;
            this.currency = currency;
        }*/
        /*
public Money()
{

}
public IMoney GetMoney(decimal Amount, string CurrencyCode)
{
   return ImperaturGlobal.Kernel.Get<IMoney>(
                         new Ninject.Parameters.ConstructorArgument("Amount", Amount),
                         new Ninject.Parameters.ConstructorArgument("Currency",
                           ImperaturGlobal.Kernel.Get<ICurrency>(
                           new Ninject.Parameters.ConstructorArgument("CurrencyCode", CurrencyCode))
                           ));
}*/

    }
}
