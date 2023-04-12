using OnlineSellingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for SignupPartnerWindow.xaml
    /// </summary>
    public partial class SignupPartnerWindow : Window, INotifyPropertyChanged
    {
        public SignupPartnerWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void SignUpWindowLoaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<ProductType> productTypes = new ObservableCollection<ProductType>();

            productTypes.Add(new ProductType() { Id = 1, Name = "Cơm" });
            productTypes.Add(new ProductType() { Id = 2, Name = "Bún" });
            productTypes.Add(new ProductType() { Id = 3, Name = "Bánh mì" });
            productTypes.Add(new ProductType() { Id = 4, Name = "Thức ăn nhanh" });
            productTypes.Add(new ProductType() { Id = 5, Name = "Món nướng" });
            productTypes.Add(new ProductType() { Id = 6, Name = "Món chiên" });
            productTypes.Add(new ProductType() { Id = 7, Name = "Món chay" });
            productTypes.Add(new ProductType() { Id = 8, Name = "Trà sữa" });
            productTypes.Add(new ProductType() { Id = 9, Name = "Cafe" });
            productTypes.Add(new ProductType() { Id = 10, Name = "Sinh tố" });
            productTypes.Add(new ProductType() { Id = 11, Name = "Đá xay" });
            productTypes.Add(new ProductType() { Id = 12, Name = "Nước ép hoa quả" });
            productTypes.Add(new ProductType() { Id = 13, Name = "Kem" });
            productTypes.Add(new ProductType() { Id = 14, Name = "Yogurt" });

            productTypeComboBox.ItemsSource = productTypes;

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

        private void SignUpPartnerButton_Click(object sender, RoutedEventArgs e)
        {
            string partnerName = fullName.Text;
            string partnerEmail = email.Text;
            string partnerPhone = phone.Text;

            string partnerTaxCode = taxCode.Text;

            string partnerNoRoad = houseNumber.Text;
            string partnerRoad = road.Text;
            string partnerWard = ward.Text;
            string partnerDistrict = distric.Text;
            string partnerCity = city.Text;

            string partnerBranchName = branchName.Text;
            int partnerProductType = productTypeComboBox.SelectedIndex + 1;
            string partnerNumberOfBranch = numberOfBranch.Text;

            //Connect database
            var _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes;MultipleActiveResultSets=true");
            _connection.Open();

            string sp_insertPartner = "sp_insertPartner";
            var command = new SqlCommand(sp_insertPartner, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerName",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = partnerName
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerPhone",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = partnerPhone
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerEmail",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = partnerEmail
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerTaxCode",
                SqlDbType = System.Data.SqlDbType.Char,
                Value = partnerTaxCode
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerAddressNoR",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = partnerNoRoad
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerAddressRoad",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = partnerRoad
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerAddressWard",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = partnerWard
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerAddressDistrict",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = partnerDistrict
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerAddressCity",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = partnerCity
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchName",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = partnerBranchName
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductType",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = partnerProductType
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerNoBranch",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = partnerNumberOfBranch
            });

            int isSuccess = command.ExecuteNonQuery();

            if (isSuccess != 0)
            {
                MessageBox.Show($"Success! You can log in with your phone: {partnerPhone} and default password: 123456");
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        
    }
}
