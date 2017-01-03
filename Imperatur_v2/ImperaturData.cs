﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2
{
    public class ImperaturData
    {
        //TODO: add exhange
        public string SystemDirectory;
        public string AcccountDirectory;
        public string AccountFile;
        public string QuoteDirectory;
        public string QuoteFile;
        public string CustomerDirectory;
        public string CustomerFile;
        public string SystemCurrency;
        public string ULR_Quotes;
        public string QuoteRefreshTime;
        public bool IsAutomaticMaintained = false;

    }

    public static class ImperaturDataStandard
    {
        public static string SystemDirectory;
        public static string AcccountDirectory = "account";
        public static string AccountFile = "account.json";
        public static string QuoteDirectory = "quote";
        public static string QuoteFile = "qoute.json";
        public static string CustomerDirectory = "customer";
        public static string CustomerFile = "customer.json";
        public static string SystemCurrency = "SEK";
        public static string ULR_Quotes = "http://finance.google.com/finance/info?client=ig&q={exhange}%3A";
        public static string QuoteRefreshTime = "15";
    }

    public class Exchange
    {
        public string Region;
        public string ExhangeCode;
        public string Name;

    }
    public enum QuoteSupplier
    {
       Google
    }
}
