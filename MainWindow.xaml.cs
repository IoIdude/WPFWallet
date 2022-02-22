using NBitcoin;
using System;
using System.Windows;
using HBitcoin.KeyManagement;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;

namespace Crypto
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string walletFilePath = @"Wallets\";

        SqlCommand clientInsert;
        SqlCommand bankAccountInsert;
        SqlCommand bankAccountSelect;
        SqlCommand bankAccountDelete;

        SqlDataReader dataReader;

        public MainWindow()
        {
            InitializeComponent();

            SqlConnection sqlConnection = new SqlConnection("Data Source=LOLDUDEBB63;Initial Catalog=Crypto;Integrated Security=True");
            sqlConnection.Open();

            bankAccountDelete = new SqlCommand("DELETE FROM BankAccount where IdBankAccount=", sqlConnection);
            clientInsert = new SqlCommand("Insert into [Client] (Surname, Name, MiddleName, PassportSeries, " +
                "PassportNumber, Login, Password, BankAccount) values (@Surname, @Name, @MiddleName, @PassportSeries, " +
                "@PassportNumber, @Login, @Password, @BankAccount)", sqlConnection);
            bankAccountInsert = new SqlCommand("Insert into BankAccount ([Key], Address) values (@Key, @Address)", sqlConnection);

            bankAccountSelect = new SqlCommand("Select @@IDENTITY", sqlConnection);
        }

        private void ClientReg(int IdBankAccount)
        {
            try
            {
                clientInsert.Parameters.AddWithValue("@Surname", Surname.Text);
                clientInsert.Parameters.AddWithValue("@Name", Name.Text);
                clientInsert.Parameters.AddWithValue("@MiddleName", MiddleName.Text);
                clientInsert.Parameters.AddWithValue("@PassportSeries", Convert.ToInt32(PassportSeries.Text));
                clientInsert.Parameters.AddWithValue("@PassportNumber", Convert.ToInt32(PassportNumber.Text));
                clientInsert.Parameters.AddWithValue("@Login", Login.Text);
                clientInsert.Parameters.AddWithValue("@Password", Password.Text);
                clientInsert.Parameters.AddWithValue("@BankAccount", IdBankAccount);

                clientInsert.ExecuteNonQuery();
            }
            catch
            {
                File.Delete(walletFilePath + Login.Text + ".json");
                bankAccountDelete.CommandText += IdBankAccount;
                bankAccountDelete.ExecuteNonQuery();
                MessageBox.Show("Enter correct data");
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Network network = Network.TestNet;
            string password = Password.Text;

            bool failure = true;
            while (failure)
            {
                try
                {
                    Mnemonic mnemonic;

                    Safe safe = Safe.Create(out mnemonic, password, walletFilePath + Login.Text + ".json", network);

                    MessageBox.Show(($"Address: {safe.GetAddress(0)} -> Private key: {safe.FindPrivateKey(safe.GetAddress(0))}"));

                    bankAccountInsert.Parameters.AddWithValue("@Key", safe.GetAddress(0).ToString());
                    bankAccountInsert.Parameters.AddWithValue("@Address", safe.FindPrivateKey(safe.GetAddress(0)).ToString());

                    bankAccountInsert.ExecuteNonQuery();
                    dataReader = bankAccountSelect.ExecuteReader();

                    int IdBankAccount = 0;

                    while (dataReader.Read())
                    {
                        IdBankAccount = Convert.ToInt32(dataReader[0].ToString());
                    }

                    dataReader.Close();
                    ClientReg(IdBankAccount);

                    failure = false;
                }
                catch
                {
                    MessageBox.Show("Wallet already exists");
                    failure = false;
                    dataReader.Close();
                }
            }
        }

        private void Authorizate_Click(object sender, RoutedEventArgs e)
        {
            Auth auth = new Auth();
            this.Close();
            auth.Show();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://t.me/WalletHelperBot");
        }
    }
}