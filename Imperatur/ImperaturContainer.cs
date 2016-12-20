using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur.handler;
using System.IO;
using Imperatur.cache;

namespace Imperatur
{
    public class ImperaturContainer
    {
        private AccountHandler _AccountHandler;
        public string SystemFilePath;
        private const string File_StockTickers = "ticks.txt";
        private const string File_Account = "accounts.json";
        private const string File_Quotes = @"quotes\quotes.json{0}";
        private List<Quote> _Quotes;
        //private Imperatur.cache.Quotes oQ;


        public AccountHandler AccountHandler
        {
            get
            {
                if (_AccountHandler == null)
                {

                    Quotes.SystemDirectory = SystemFilePath;

                    _AccountHandler = new AccountHandler(GetQuotes());
                }
                return _AccountHandler;
            }
        }
        public ImperaturContainer(string FilePathToSystem)
        {
            SystemFilePath = FilePathToSystem;
            
            //create the cache
            //oQ = new cache.Quotes(GetStockTickers(), GetFullPathTo(File_Quotes));

        }

        private string GetFullPathTo(string File)
        {
            return string.Format(@"{0}\{1}", SystemFilePath, File);
        }
        public List<Quote> GetQuotes()
        {
            if (_Quotes == null)
                _Quotes = Quotes.GetQuotes;

            return _Quotes;
        }

        /*

        private List<string> GetStockTickers()
        {
            List<string> Quotes = new List<string>();
            //load the stock quotes
            string line;

            // Read the file and display it line by line.
            using (StreamReader file = new StreamReader(GetFullPathTo(File_StockTickers)))
            {
                int i = 0;
                while ((line = file.ReadLine()) != null)
                {
                    if (i > 0) //skip first line, is columnnames
                    {
                        char[] delimiters = new char[] { '\t' };
                        string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 1)
                        {
                            Quotes.Add(parts[1]);  //the ticket to fetch
                        }
                    }
                    i++;
                }

                file.Close();
            }
            return Quotes;
        }
        */


    }
}
