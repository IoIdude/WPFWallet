using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
using System.Windows.Shapes;

namespace Crypto
{
    /// <summary>
    /// Логика взаимодействия для Safety.xaml
    /// </summary>
    public partial class Safety : Window
    {
        SqlCommand clientSelect;
        SqlCommand clientDelete;
        SqlCommand bankAccountSelect;
        SqlCommand bankAccountDelete;
        SqlCommand employeeSelect;
        SqlCommand employeeDelete;
        SqlDataAdapter employeeAdp;
        DataTable employeeDt;
        SqlDataAdapter clientAdp;
        DataTable clientDt;

        public Safety()
        {
            InitializeComponent();

            SqlConnection sqlConnection = new SqlConnection("Data Source=LOLDUDEBB63;Initial Catalog=Crypto;Integrated Security=True");
            sqlConnection.Open();

            //clientSelect = new SqlCommand("Select * from Client", sqlConnection);
            clientSelect = new SqlCommand("SELECT dbo.BankAccount.[Key], dbo.BankAccount.Address, dbo.Client.Surname, dbo.Client.Name, dbo.Client.MiddleName, dbo.Client.PassportSeries, dbo.Client.PassportNumber, dbo.Client.Login, dbo.Client.Password, " +
                "dbo.Client.IdClient, dbo.BankAccount.IdBankAccount " +
                "FROM dbo.BankAccount CROSS JOIN dbo.Client", sqlConnection);
            bankAccountSelect = new SqlCommand("Select * from BankAccount", sqlConnection);
            //employeeSelect = new SqlCommand("Select * from Employee", sqlConnection);
            employeeSelect = new SqlCommand("SELECT dbo.Employee.IdEmployee, dbo.Employee.Surname, dbo.Employee.Name, dbo.Employee.MiddleName, dbo.Employee.PassportSeries, dbo.Employee.PassportNumber, dbo.Employee.Login, dbo.Employee.Password, " +
                "dbo.Staff.IdStaff, dbo.Staff.Name AS Expr1 FROM dbo.Employee INNER JOIN dbo.Staff ON dbo.Employee.StaffId = dbo.Staff.IdStaff", sqlConnection);
            employeeDelete = new SqlCommand("DELETE FROM Employee where IdEmployee=", sqlConnection);
            clientDelete = new SqlCommand("DELETE FROM Client where IdClient=", sqlConnection);
            bankAccountDelete = new SqlCommand("DELETE FROM BankAccount where IdBankAccount=", sqlConnection);

            clientAdp = new SqlDataAdapter(clientSelect);
            clientDt = new DataTable("Client"); // В скобках указываем название таблицы
            clientAdp.Fill(clientDt);
            Clients.ItemsSource = clientDt.DefaultView; // Сам вывод 

            employeeAdp = new SqlDataAdapter(employeeSelect);
            employeeDt = new DataTable("Employee"); // В скобках указываем название таблицы
            employeeAdp.Fill(employeeDt);
            Employee.ItemsSource = employeeDt.DefaultView; // Сам вывод 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Auth auth = new Auth();
            this.Close();
            auth.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView id = (DataRowView)Employee.SelectedItem;

                employeeDelete.CommandText += id["IdEmployee"];
                employeeDelete.ExecuteNonQuery();

                employeeAdp.Dispose();
                employeeAdp = new SqlDataAdapter(employeeSelect);
                employeeDt = new DataTable("Employee");
                employeeAdp.Fill(employeeDt);
                Employee.ItemsSource = employeeDt.DefaultView;
            }
            catch
            {
                MessageBox.Show("Select item");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) // добавление сотрудника
        {
            AddEmployee auth = new AddEmployee();
            auth.Owner = this;
            auth.ShowDialog();

            employeeAdp.Dispose();
            employeeAdp = new SqlDataAdapter(employeeSelect);
            employeeDt = new DataTable("Employee");
            employeeAdp.Fill(employeeDt);
            Employee.ItemsSource = employeeDt.DefaultView;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView id = (DataRowView)Clients.SelectedItem;
                const string walletFilePath = @"Wallets\";

                clientDelete.CommandText += id["IdClient"];
                clientDelete.ExecuteNonQuery();

                bankAccountDelete.CommandText += id["BankAccount"];
                bankAccountDelete.ExecuteNonQuery();

                File.Delete(walletFilePath + id["Login"] + ".json");

                clientAdp.Dispose();
                clientAdp = new SqlDataAdapter(clientSelect);
                clientDt = new DataTable("Client");
                clientAdp.Fill(clientDt);
                Clients.ItemsSource = clientDt.DefaultView;
            }
            catch
            {
                MessageBox.Show("Select item");
            }
        }
    }
}