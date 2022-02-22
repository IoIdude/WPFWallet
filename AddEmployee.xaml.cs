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
using System.Windows.Shapes;

namespace Crypto
{
    /// <summary>
    /// Логика взаимодействия для AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        SqlCommand employeeInsert;
        SqlCommand staffSelect;

        public AddEmployee()
        {
            InitializeComponent();

            SqlConnection sqlConnection = new SqlConnection("Data Source=LOLDUDEBB63;Initial Catalog=Crypto;Integrated Security=True");
            sqlConnection.Open();

            employeeInsert = new SqlCommand("Insert into Employee (Surname, Name, MiddleName, PassportSeries, " +
                "PassportNumber, Login, Password, StaffId) values (@Surname, @Name, @MiddleName, @PassportSeries, " +
                "@PassportNumber, @Login, @Password, @StaffId)", sqlConnection);

            staffSelect = new SqlCommand("Select * from Staff", sqlConnection);

            DataTable tbl1 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(staffSelect);

            da.Fill(tbl1);
            Staff.ItemsSource = tbl1.DefaultView;
            Staff.DisplayMemberPath = "Name"; // столбец для отображения
            Staff.SelectedValuePath = "idStaff"; //столбец с id
            Staff.SelectedIndex = -1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView idStaff = (DataRowView)Staff.SelectedItem;

                employeeInsert.Parameters.AddWithValue("@Surname", Surname.Text);
                employeeInsert.Parameters.AddWithValue("@Name", Name.Text);
                employeeInsert.Parameters.AddWithValue("@MiddleName", MiddleName.Text);
                employeeInsert.Parameters.AddWithValue("@PassportSeries", Convert.ToInt32(PassportSeries.Text));
                employeeInsert.Parameters.AddWithValue("@PassportNumber", Convert.ToInt32(PassportNumber.Text));
                employeeInsert.Parameters.AddWithValue("@Login", Login.Text);
                employeeInsert.Parameters.AddWithValue("@Password", Password.Text);
                employeeInsert.Parameters.AddWithValue("@StaffId", Convert.ToInt32(idStaff["idStaff"]));

                employeeInsert.ExecuteNonQuery();

                this.Close();
            }
            catch
            {
                MessageBox.Show("Enter correct data");
            }
        }
    }
}