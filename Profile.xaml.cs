using HBitcoin.KeyManagement;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Crypto
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        const string walletFilePath = @"Wallets\";
        SqlCommand clientSelect;
        SqlCommand bankAccountSelect;
        SqlDataReader dataReader;
        int UserId { get; set; }
        UserInfo userInfo = new UserInfo();

        public Profile(UserInfo user)
        {
            userInfo.login = user.login;
            userInfo.password = user.password;
            userInfo.key = user.key;
            userInfo.address = user.address;
        }

        public Profile(int id)
        {
            InitializeComponent();

            SqlConnection sqlConnection = new SqlConnection("Data Source=LOLDUDEBB63;Initial Catalog=Crypto;Integrated Security=True");
            sqlConnection.Open();

            clientSelect = new SqlCommand("Select * from Client", sqlConnection);
            bankAccountSelect = new SqlCommand("Select * from BankAccount", sqlConnection);

            UserId = id;

            dataReader = clientSelect.ExecuteReader();
            int idBankAccount = 0;

            while (dataReader.Read())
            {
                if (Convert.ToInt32(dataReader["IdClient"].ToString()) == UserId)
                {
                    idBankAccount = Convert.ToInt32(dataReader["BankAccount"].ToString());
                    userInfo.password = Convert.ToInt32(dataReader["Password"].ToString()); ;
                    userInfo.login = dataReader["Login"].ToString();

                    dataReader.Close();
                    break;
                }
            }

            dataReader = bankAccountSelect.ExecuteReader();

            while (dataReader.Read())
            {
                if (Convert.ToInt32(dataReader["IdBankAccount"].ToString()) == idBankAccount)
                {
                    Address.Text = dataReader["Address"].ToString();
                    Key.Text = dataReader["Key"].ToString();

                    dataReader.Close();
                    break;
                }
            }

            Safe wallet = Safe.Load(userInfo.password.ToString(), walletFilePath + userInfo.login + ".json");
            QBitNinjaClient client = new QBitNinjaClient(Network.TestNet);
            decimal totalBalance = 0;

            var balance = client.GetBalance(BitcoinAddress.Create(wallet.GetAddress(0).ToString(), Network.TestNet), true).Result;
            foreach (var entry in balance.Operations)
            {
                foreach (var coin in entry.ReceivedCoins)
                {
                    Money amount = (Money)coin.Amount;
                    decimal currentAmount = amount.ToDecimal(MoneyUnit.BTC);
                    totalBalance += currentAmount;
                }
            }

            Sum.Text = totalBalance.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Auth auth = new Auth();
            this.Close();
            auth.Show();
        }
    }
}