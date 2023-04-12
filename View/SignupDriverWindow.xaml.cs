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
using System.Windows.Shapes;

namespace OnlineSellingSystem.View
{
    /// <summary>
    /// Interaction logic for SignupDriverWindow.xaml
    /// </summary>
    public partial class SignupDriverWindow : Window
    {
        public SignupDriverWindow()
        {
            InitializeComponent();
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

        private void DriverSignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string driverName = fullName.Text;
            string driverEmail = email.Text;
            string driverPhone = phone.Text;

            string cityzen = cityzenId.Text;

            string driverNoRoad = houseNumber.Text;
            string driverRoad = road.Text;
            string driverWard = ward.Text;
            string driverDistrict = distric.Text;
            string driverCity = city.Text;

            string license = licensePlate.Text;

            string shipZoneDistrict = shipDistrict.Text;
            string shipZoneCity = shipCity.Text;

            string driverBankBranch = bankBranchName.Text;
            string driverBankNumber = bankNumber.Text;
            string driverBankFullname = bankFullname.Text;

            //Connect database
            var _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes;MultipleActiveResultSets=true");
            _connection.Open();

            string sp_insertDriver = "sp_insertDriver";
            var command = new SqlCommand(sp_insertDriver, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverName",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = driverName
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverPhone",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = driverPhone
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverEmail",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = driverEmail
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverCitizenId",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = cityzen
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverAddressNoR",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = driverNoRoad
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverAddressRoad",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = driverRoad
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverAddressWard",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = driverWard
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverAddressDistrict",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = driverDistrict
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverAddressCity",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = driverCity
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverLicensePlates",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = license
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ShippingZoneDistrict",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = shipZoneDistrict
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ShippingZoneCity",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = shipZoneCity
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BankBranch",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = driverBankBranch
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BankNumber",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = driverBankNumber
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@FullName",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = driverBankFullname
            });

            int isSuccess = command.ExecuteNonQuery();

            if (isSuccess != 0)
            {
                MessageBox.Show($"Success! You can log in with your phone: {driverPhone} and default password: 123456");
            }
            else
            {
                MessageBox.Show("Error");
            }
        }
    }
}
