using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        SqlCommand clientSelect;
        SqlCommand employeeSelect;
        SqlDataReader dataReader;

        public Auth()
        {
            InitializeComponent();

            SqlConnection sqlConnection = new SqlConnection("Data Source=LOLDUDEBB63;Initial Catalog=Crypto;Integrated Security=True");
            sqlConnection.Open();

            clientSelect = new SqlCommand("Select * from Client", sqlConnection);
            employeeSelect = new SqlCommand("Select * from Employee", sqlConnection);
        }

        private void Authorizate_Click(object sender, RoutedEventArgs e)
        {
            bool cheker = true;

            if (cheker)
            {
                dataReader = clientSelect.ExecuteReader();

                while (dataReader.Read())
                {
                    if (dataReader["Login"].ToString() == Login.Text && dataReader["Password"].ToString() == Password.Text)
                    {
                        Profile profile = new Profile(Convert.ToInt32(dataReader["IdClient"].ToString()));
                        dataReader.Close();
                        this.Close();
                        profile.Show();
                        return;
                    }
                }
                dataReader.Close();
            }

            if (cheker)
            {
                dataReader = employeeSelect.ExecuteReader();

                while (dataReader.Read())
                {
                    if (dataReader["Login"].ToString() == Login.Text && dataReader["Password"].ToString() == Password.Text)
                    {
                        if (dataReader["StaffId"].ToString() == "3")
                        {
                            dataReader.Close();
                            Safety safety = new Safety();
                            this.Close();
                            safety.Show();
                            return;
                        }
                        else if (dataReader["StaffId"].ToString() == "2")
                        {
                            dataReader.Close();
                            Purchaser purchaser = new Purchaser();
                            this.Close();
                            purchaser.Show();
                            return;
                        }
                        else if (dataReader["StaffId"].ToString() == "1")
                        {
                            dataReader.Close();
                            Engineering engineering = new Engineering();
                            this.Close();
                            engineering.Show();
                            return;
                        }
                    }
                }

                MessageBox.Show("Wrong password or login");
                dataReader.Close();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }
    }
}