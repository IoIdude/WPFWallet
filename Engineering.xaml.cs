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
    /// Логика взаимодействия для Engineering.xaml
    /// </summary>
    public partial class Engineering : Window
    {
        SqlCommand orderSelect;
        SqlCommand stagesSelect;
        SqlCommand releseInsert;
        SqlCommand releseUpdate;
        SqlCommand releseSelect;
        SqlDataAdapter releseAdp;
        DataTable releseDt;

        public Engineering()
        {
            InitializeComponent();

            SqlConnection sqlConnection = new SqlConnection("Data Source=LOLDUDEBB63;Initial Catalog=Crypto;Integrated Security=True");
            sqlConnection.Open();

            orderSelect = new SqlCommand("Select * from [Order]", sqlConnection);
            stagesSelect = new SqlCommand("Select * from Stages", sqlConnection);
            //releseSelect = new SqlCommand("Select * from Release", sqlConnection);
            releseSelect = new SqlCommand("SELECT dbo.Release.IdRelease, dbo.Stages.IdStages, dbo.Stages.Name, dbo.Product.IdProduct, dbo.Product.Name AS Expr1 " +
                "FROM dbo.Release INNER JOIN dbo.[Order] ON dbo.Release.IdOrder = dbo.[Order].IdOrder INNER JOIN " +
                "dbo.Stages ON dbo.Release.IdStages = dbo.Stages.IdStages INNER JOIN dbo.Product ON dbo.[Order].ProductId = dbo.Product.IdProduct", sqlConnection);
            releseUpdate = new SqlCommand("UPDATE Release SET IdStages = @IdStages WHERE IdRelease=", sqlConnection);
            releseInsert = new SqlCommand("Insert into Release (IdOrder, IdStages) values (@IdOrder, @IdStages)", sqlConnection);

            DataTable tbl1 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter(orderSelect);

            da1.Fill(tbl1);
            Product.ItemsSource = tbl1.DefaultView;
            Product.DisplayMemberPath = "ProductId"; // столбец для отображения
            Product.SelectedValuePath = "IdOrder"; // столбец с id
            Product.SelectedIndex = -1;

            DataTable tbl2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(stagesSelect);

            da2.Fill(tbl2);
            Stages.ItemsSource = tbl2.DefaultView;
            Stages.DisplayMemberPath = "Name"; // столбец для отображения
            Stages.SelectedValuePath = "IdStages"; // столбец с id
            Stages.SelectedIndex = -1;

            releseAdp = new SqlDataAdapter(releseSelect);
            releseDt = new DataTable("Release"); // В скобках указываем название таблицы
            releseAdp.Fill(releseDt);
            Relese.ItemsSource = releseDt.DefaultView; // Сам вывод 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView idProduct = (DataRowView)Product.SelectedItem;
                DataRowView idStages = (DataRowView)Stages.SelectedItem;

                releseInsert.Parameters.AddWithValue("@IdOrder", Convert.ToInt32(idProduct["IdOrder"]));
                releseInsert.Parameters.AddWithValue("@IdStages", Convert.ToInt32(idStages["IdStages"]));

                releseInsert.ExecuteNonQuery();

                releseAdp.Dispose();
                releseAdp = new SqlDataAdapter(releseSelect);
                releseDt = new DataTable("Release");
                releseAdp.Fill(releseDt);
                Relese.ItemsSource = releseDt.DefaultView;
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
                DataRowView id = (DataRowView)Relese.SelectedItem;
                DataRowView idStages = (DataRowView)Stages.SelectedItem;

                releseUpdate.CommandText += id["IdRelease"];
                releseUpdate.Parameters.AddWithValue("@IdStages", Convert.ToInt32(idStages["IdStages"]));
                releseUpdate.ExecuteNonQuery();

                releseAdp.Dispose();
                releseAdp = new SqlDataAdapter(releseSelect);
                releseDt = new DataTable("Release");
                releseAdp.Fill(releseDt);
                Relese.ItemsSource = releseDt.DefaultView;
            }
            catch
            {
                MessageBox.Show("Select item");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Auth auth = new Auth();
            this.Close();
            auth.Show();
        }
    }
}