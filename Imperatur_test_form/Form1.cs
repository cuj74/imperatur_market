using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur;
using System.IO;

namespace Imperatur_test_form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.textBox_systemdir.Text = @"C:\Users\urbajoha\Documents\imperatur";
        }

        private void button1_Click(object sender, EventArgs e)
        {
 
           /*


            
            List<string> Quotes = new List<string>();
            //load the stock quotes
            string line;

            // Read the file and display it line by line.
            using (StreamReader file = new StreamReader(@"C:\Users\urbajoha\Documents\ticks.txt"))
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
            ImperaturContainer Ic = new ImperaturContainer(Quotes);

            List<Guid> HouseAndBankList = new List<Guid>();
            HouseAndBankList.AddRange(Ic.BankAccounts.Select(b => b.Identifier));
            HouseAndBankList.AddRange(Ic.HouseAccounts.Select(b => b.Identifier));

            Guid ToTestIdentifier = new Guid("462e209e-d331-403b-aab3-ea4142b35ddc");
            List<Imperatur.monetary.Money> oNext = Ic.AccessAccount().Single(a => a.Identifier.Equals(ToTestIdentifier)).GetAvailableFunds(HouseAndBankList);

            Imperatur.monetary.Transaction oT2 = new Imperatur.monetary.Transaction(
    new Imperatur.monetary.Money
    {
        Amount = 20,
        CurrencyCode = "SEK"
    },
    new Imperatur.monetary.Money
    {
        Amount = 20,
        CurrencyCode = "SEK"
    },
    ToTestIdentifier,//debitaccount
    (Guid)Ic.BankAccounts.Select(b => b.Identifier).First(),
    Imperatur.monetary.TransactionType.Withdrawal,
    null
    );
            Ic.AddTransactionToAccount(oT2.DebitAccount, oT2); //uttag
            */
            /*
            //create house account
            List<Imperatur.account.Account> oAccounts = new List<Imperatur.account.Account>();
            oAccounts.Add(new Imperatur.account.Account("House", Imperatur.account.AccountType.House));
            oAccounts.Add(new Imperatur.account.Account("Bank1", Imperatur.account.AccountType.Bank));
            oAccounts.Add(new Imperatur.account.Account("Bank2", Imperatur.account.AccountType.Bank));
            Ic.CreateAccount(oAccounts);

            oAccounts.Clear();
            for (int i = 0; i < 10; i++)
            {
                oAccounts.Add(
                    new Imperatur.account.Account(string.Format("test{0}", i), Imperatur.account.AccountType.Customer)
                    );
            }
            Ic.CreateAccount(oAccounts);

            Guid ToTestIdenfifier = Ic.AccessAccount().Where(a => a.AccountType.Equals(Imperatur.account.AccountType.Customer)).First().Identifier;

            List<Guid> HouseAndBankList = new List<Guid>();
            HouseAndBankList.AddRange(Ic.BankAccounts.Select(b=>b.Identifier));
            HouseAndBankList.AddRange(Ic.HouseAccounts.Select(b => b.Identifier));

            Imperatur.monetary.Transaction oT = new Imperatur.monetary.Transaction(
                new Imperatur.monetary.Money
                {
                    Amount = 0,
                    CurrencyCode = "SEK"
                },
                new Imperatur.monetary.Money
                {
                    Amount = 150,
                    CurrencyCode = "SEK"
                },
                (Guid)Ic.BankAccounts.Select(b=>b.Identifier).First(), //for transfer the debit account is not available
                ToTestIdenfifier,//creditaccount
                Imperatur.monetary.TransactionType.Transfer,
                null
                );

            Ic.AddTransactionToAccount(ToTestIdenfifier, oT);
            Ic.AddTransactionToAccount(ToTestIdenfifier, oT);
            
            Imperatur.monetary.Transaction oT2 = new Imperatur.monetary.Transaction(
    new Imperatur.monetary.Money
    {
        Amount = 200,
        CurrencyCode = "SEK"
    },
    new Imperatur.monetary.Money
    {
        Amount = 200,
        CurrencyCode = "SEK"
    },
    ToTestIdenfifier,//debitaccount
    (Guid)Ic.BankAccounts.Select(b => b.Identifier).First(), 
    Imperatur.monetary.TransactionType.Withdrawal,
    null
    );
            Ic.AddTransactionToAccount(ToTestIdenfifier, oT2); //uttag

            List<Imperatur.monetary.Money>  oNext = Ic.AccessAccount().Single(a => a.Identifier.Equals(ToTestIdenfifier)).GetAvailableFunds(HouseAndBankList);

            
            Ic.SaveAccounts();
            */
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox_systemdir.Text))
            {
                Form_imperaturclient oFC = new Form_imperaturclient(textBox_systemdir.Text);
                oFC.Visible = true;
                oFC.Show();
                //ImperaturContainer Ic = new ImperaturContainer(textBox_systemdir.Text);
                this.Hide();
            }
            else
                MessageBox.Show(string.Format("Can't find directory {0}", textBox_systemdir.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("sdfsdf");
        }
    }
}
