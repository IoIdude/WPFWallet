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
    /// Логика взаимодействия для Purchaser.xaml
    /// </summary>
    public partial class Purchaser : Window
    {
        SqlCommand productSelect;
        SqlCommand statusSelect;
        SqlCommand orderInsert;
        SqlCommand orderUpdate;
        SqlCommand orderSelect;
        SqlDataAdapter orderAdp;
        DataTable orderDt;

        public Purchaser()
        {
            InitializeComponent();

            SqlConnection sqlConnection = new SqlConnection("Data Source=LOLDUDEBB63;Initial Catalog=Crypto;Integrated Security=True");
            sqlConnection.Open();

            productSelect = new SqlCommand("Select * from Product", sqlConnection);
            statusSelect = new SqlCommand("Select * from Status", sqlConnection);
            //orderSelect = new SqlCommand("Select * from [Order]", sqlConnection);
            orderSelect = new SqlCommand("SELECT dbo.[Order].IdOrder, dbo.Product.IdProduct, dbo.Product.Name, dbo.Product.SupplierId, dbo.Status.IdStatus, dbo.Status.Name AS Expr1 " +
                "FROM dbo.[Order] INNER JOIN dbo.Status ON dbo.[Order].StatusId = dbo.Status.IdStatus INNER JOIN " +
                "dbo.Product ON dbo.[Order].ProductId = dbo.Product.IdProduct", sqlConnection);
            orderInsert = new SqlCommand("Insert into [Order] (ProductId, StatusId) values (@ProductId, @StatusId)", sqlConnection);
            orderUpdate = new SqlCommand("UPDATE [Order] SET StatusId = @StatusId WHERE IdOrder=", sqlConnection);

            DataTable tbl1 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter(productSelect);

            da1.Fill(tbl1);
            Product.ItemsSource = tbl1.DefaultView;
            Product.DisplayMemberPath = "Name"; // столбец для отображения
            Product.SelectedValuePath = "IdProduct"; // столбец с id
            Product.SelectedIndex = -1;

            DataTable tbl2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(statusSelect);

            da2.Fill(tbl2);
            Status.ItemsSource = tbl2.DefaultView;
            Status.DisplayMemberPath = "Name"; // столбец для отображения
            Status.SelectedValuePath = "IdStatus"; // столбец с id
            Status.SelectedIndex = -1;

            orderAdp = new SqlDataAdapter(orderSelect);
            orderDt = new DataTable("Client"); // В скобках указываем название таблицы
            orderAdp.Fill(orderDt);
            Order.ItemsSource = orderDt.DefaultView; // Сам вывод 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView idProduct = (DataRowView)Product.SelectedItem;
                DataRowView idStatus = (DataRowView)Status.SelectedItem;

                orderInsert.Parameters.AddWithValue("@ProductId", Convert.ToInt32(idProduct["IdProduct"]));
                orderInsert.Parameters.AddWithValue("@StatusId", Convert.ToInt32(idStatus["IdStatus"]));

                orderInsert.ExecuteNonQuery();

                orderAdp.Dispose();
                orderAdp = new SqlDataAdapter(orderSelect);
                orderDt = new DataTable("Order");
                orderAdp.Fill(orderDt);
                Order.ItemsSource = orderDt.DefaultView;
            }
            catch
            {
                MessageBox.Show("fields cannot be empty");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView id = (DataRowView)Order.SelectedItem;
                DataRowView idStatus = (DataRowView)Status.SelectedItem;

                orderUpdate.CommandText += id["IdOrder"];
                orderUpdate.Parameters.AddWithValue("@StatusId", Convert.ToInt32(idStatus["IdStatus"]));
                orderUpdate.ExecuteNonQuery();

                orderAdp.Dispose();
                orderAdp = new SqlDataAdapter(orderSelect);
                orderDt = new DataTable("Order");
                orderAdp.Fill(orderDt);
                Order.ItemsSource = orderDt.DefaultView;
            }
            catch
            {
                MessageBox.Show("Select item");
            }
        }
    }
}