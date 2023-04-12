using OnlineSellingSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static Person Person { get; set; }

        string selectedType;

        public LoginWindow()
        {
            InitializeComponent();
        }

        public string phoneNumber { get; set; }
        public string password { get; set; }

        private void Login_Window_Loaded(object sender, RoutedEventArgs e)
        {
            selectedType = StartWindow.selectedType;

            if(selectedType == "Admin" || selectedType == "Employee")
            {
                moveToSignup.Visibility = Visibility.Collapsed;
            }
        }

        private void btn_back(object sender, MouseButtonEventArgs e)
        {
            var screen = new StartWindow();
            this.Close();
            screen.ShowDialog();
        }

        private void move_to_signup(object sender, MouseButtonEventArgs e)
        {
            if(selectedType == "Customer")
            {
                var screen = new SignupSustomerWindow();
                this.Close();
                screen.ShowDialog();
            }
            else if(selectedType == "Partner")
            {
                var screen = new SignupPartnerWindow();
                this.Close();
                screen.ShowDialog();
            }
            else if(selectedType == "Driver")
            {
                var screen = new SignupDriverWindow();
                this.Close();
                screen.ShowDialog();
            }
        }

        private bool loginHandle(string table, string phoneNumber, string password, string sql, string sqlQueryGetInfor)
        {
            bool result = false;

            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            var command = new SqlCommand(sql, _connection);

            int userCount = 0;
            userCount = (int)command.ExecuteScalar();
            if (userCount > 0 && password == "123456")
            {   result = true;
                setInfor(table,_connection, sqlQueryGetInfor, command);
            }
            
            return result;
        }

        private void setInfor(string table, SqlConnection _connection, string sqlQueryGetInfor, SqlCommand command)
        {
            string type = table;
            if(type == "Admin" || type == "Employee")
            {
                type = "Staff";
            }

            command = new SqlCommand(sqlQueryGetInfor, _connection);
            var reader = command.ExecuteReader();

            int id = 0;
            string fullName = "";
            string email = "";
            string phone = "";

            while (reader.Read())
            {
                id = reader.GetInt32(reader.GetOrdinal($"{type}Id"));
                fullName = reader.GetString(reader.GetOrdinal($"{type}Name"));
                email = reader.GetString(reader.GetOrdinal($"{type}Email"));
                phone = reader.GetString(reader.GetOrdinal($"{type}Phone"));
            }
            Person = new Person { Type = type, Id = id, Fullname = fullName, Email = email, PhoneNumber = phone };
        }

        private void login(object sender, RoutedEventArgs e)
        {
            selectedType = StartWindow.selectedType;

            phoneNumber = loginPhoneNumber.Text;
            password = loginPassword.Password;

            if ( selectedType == "Partner")
            {
                //Check email, phone number
                string sql = $"SELECT COUNT(*) FROM Partner WHERE PartnerPhone={phoneNumber}";
                string sqlQueryGetInfor = $"SELECT* FROM Partner WHERE PartnerPhone={phoneNumber}";
                bool isLoginSuccess = false;
                isLoginSuccess = loginHandle(selectedType, phoneNumber, password, sql, sqlQueryGetInfor);

                if (isLoginSuccess)
                {
                    var screen = new MainWindowPartner();
                    this.Close();
                    screen.ShowDialog();
                }
                else
                {
                    incorrectLogin.Visibility = Visibility.Visible;
                }
            }
            else if(selectedType == "Employee")
            {
                //Check email, phone number
                string sql = $"SELECT COUNT(*) FROM Staff WHERE StaffPhone={phoneNumber} AND StaffAdmin = '0'";
                string sqlQueryGetInfor = $"SELECT* FROM Staff WHERE StaffPhone={phoneNumber} AND StaffAdmin = '0'";
                bool isLoginSuccess = false;
                isLoginSuccess = loginHandle(selectedType, phoneNumber, password,sql, sqlQueryGetInfor);

                if (isLoginSuccess)
                {
                    var screen = new MainWindowEmployee();
                    this.Close();
                    screen.ShowDialog();
                }
                else
                {
                    incorrectLogin.Visibility = Visibility.Visible;
                }
            }
            else if( selectedType == "Driver")
            {
                //Check email, phone number
                bool isLoginSuccess = false;
                string sql = $"SELECT COUNT(*) FROM Driver WHERE DriverPhone={phoneNumber}";
                string sqlQueryGetInfor = $"SELECT* FROM Driver WHERE DriverPhone={phoneNumber}";
                isLoginSuccess = loginHandle(selectedType, phoneNumber, password, sql, sqlQueryGetInfor);

                if (isLoginSuccess)
                {
                    var screen = new MainWindowDriver();
                    this.Close();
                    screen.ShowDialog();
                }
                else
                {
                    incorrectLogin.Visibility = Visibility.Visible;
                }

            }
            else if(selectedType == "Admin")
            {
                //Check email, phone number
                string sql = $"SELECT COUNT(*) FROM Staff WHERE StaffPhone={phoneNumber} AND StaffAdmin = '1'";
                string sqlQueryGetInfor = $"SELECT* FROM Staff WHERE StaffPhone={phoneNumber} AND StaffAdmin = '1'";
                bool isLoginSuccess = false;
                isLoginSuccess = loginHandle(selectedType, phoneNumber, password, sql, sqlQueryGetInfor);

                if (isLoginSuccess)
                {
                    var screen = new MainWindowAdmin();
                    this.Close();
                    screen.ShowDialog();
                }
                else
                {
                    incorrectLogin.Visibility = Visibility.Visible;
                }

            }
            else if(selectedType == "Customer")
            {
                //Check email, phone number
                string sql = $"SELECT COUNT(*) FROM Customer WHERE CustomerPhone={phoneNumber}";
                string sqlQueryGetInfor = $"SELECT* FROM Customer WHERE CustomerPhone={phoneNumber}";
                bool isLoginSuccess = false;
                isLoginSuccess = loginHandle(selectedType, phoneNumber, password, sql, sqlQueryGetInfor);

                if (isLoginSuccess)
                {
                    var screen = new MainWindowCustomer();
                    this.Close();
                    screen.ShowDialog();
                }
                else
                {
                    incorrectLogin.Visibility = Visibility.Visible;
                }

            }
        }
    }
}
