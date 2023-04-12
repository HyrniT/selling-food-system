using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = System.Windows.Window;

namespace OnlineSellingSystem.View
{
    /// <summary>
    /// Interaction logic for SignupWindow.xaml
    /// </summary>
    public partial class SignupSustomerWindow : Window
    {
        public SignupSustomerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btn_back(object sender, MouseButtonEventArgs e)
        {
            var screen = new LoginWindow();
            this.Close();
            screen.ShowDialog();
        }

        private void move_to_signin(object sender, MouseButtonEventArgs e)
        {
            var screen = new LoginWindow();
            this.Close();
            screen.ShowDialog();
        }

        private void customerSignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string cusName = fullName.Text;
            string cusEmail = email.Text;
            string cusPhone = phone.Text;

            string cusNoRoad = houseNumber.Text;
            string cusRoad = road.Text;
            string cusWard = ward.Text;
            string cusDistrict = distric.Text;
            string cusCity = city.Text;

            //Connect database
            var _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes;MultipleActiveResultSets=true");
            _connection.Open();

            string sp_insertCustomer = "sp_insertCustomer";
            var command = new SqlCommand(sp_insertCustomer, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@CustomerName",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = cusName
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@CustomerPhone",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = cusPhone
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@CustomerEmail",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = cusEmail
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@CustomerAddressNoR",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = cusNoRoad
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@CustomerAddressRoad",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = cusRoad
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@CustomerAddressWard",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = cusWard
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@CustomerAddressDistrict",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = cusDistrict
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@CustomerAddressCity",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = cusCity
            });

            int isSuccess = command.ExecuteNonQuery();

            if(isSuccess == 1)
            {
                MessageBox.Show($"Success! You can log in with your phone: {cusPhone} and default password: 123456");
            }
            else
            {
                MessageBox.Show("Error");
            }


        }
    }
}
