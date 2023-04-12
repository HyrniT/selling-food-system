using Microsoft.VisualBasic;
using OnlineSellingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace OnlineSellingSystem.View
{
    /// <summary>
    /// Interaction logic for MainWindowAdmin.xaml
    /// </summary>
    public partial class MainWindowAdmin : Window, INotifyCollectionChanged
    {
        public MainWindowAdmin()
        {
            InitializeComponent();
        }
        SqlConnection _connection = null;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        ObservableCollection<Person> _subItems = new ObservableCollection<Person>();

        public int RowsPerPage { get; set; } = 20;
        public int TotalPages { get; set; } = 0;
        public int CurrentPage { get; set; } = 1;

        public int TotalItems { get; set; } = 0;

        private int NumberOfPersons(string sqlQueryTotalPerson)
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            //Count number of person: admin/employee/customer/driver/partner
            int count = 0;
            var command = new SqlCommand(sqlQueryTotalPerson, _connection);
            count = (int)command.ExecuteScalar();

            return count;
        }

        private void SelectList20PersonsForAdmin()
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPage - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectList20Persons = $"SELECT* FROM Staff WHERE StaffAdmin = '1' ORDER BY StaffId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY";
            var command = new SqlCommand(sqlSelectList20Persons, _connection);
            var reader = command.ExecuteReader();

            _subItems.Clear();
            while(reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("StaffId"));
                string fullName = reader.GetString(reader.GetOrdinal("StaffName"));
                string email = reader.GetString(reader.GetOrdinal("StaffEmail"));
                string phone = reader.GetString(reader.GetOrdinal("StaffPhone"));
                var _person = new Person { Type = "Admin", Id = id, Fullname = fullName, Email = email, PhoneNumber = phone };
                _subItems.Add(_person);
            }

            contentAdminAdminDataGrid.ItemsSource = _subItems;
        }
        private void SelectList20PersonsForEmployee()
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPage - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectList20Persons = $"SELECT* FROM Staff WHERE StaffAdmin = '0' ORDER BY StaffId OFFSET {offset} ROWS FETCH NEXT {RowsPerPage} ROWS ONLY";
            var command = new SqlCommand(sqlSelectList20Persons, _connection);
            var reader = command.ExecuteReader();

            _subItems.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("StaffId"));
                string fullName = reader.GetString(reader.GetOrdinal("StaffName"));
                string email = reader.GetString(reader.GetOrdinal("StaffEmail"));
                string phone = reader.GetString(reader.GetOrdinal("StaffPhone"));
                var _person = new Person { Type = "Employee", Id = id, Fullname = fullName, Email = email, PhoneNumber = phone };
                _subItems.Add(_person);
            }

            contentEmployeeDataGrid.ItemsSource = _subItems;
        }
        private void SelectList20Persons(string table)
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPage - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectList20Persons = $"SELECT* FROM {table} ORDER BY {table}Id OFFSET {offset} ROWS FETCH NEXT {RowsPerPage} ROWS ONLY";
            var command = new SqlCommand(sqlSelectList20Persons, _connection);
            var reader = command.ExecuteReader();

            _subItems.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal($"{table}Id"));
                string fullName = reader.GetString(reader.GetOrdinal($"{table}Name"));
                string email = reader.GetString(reader.GetOrdinal($"{table}Email"));
                string phone = reader.GetString(reader.GetOrdinal($"{table}Phone"));
                var _person = new Person { Type = table, Id = id, Fullname = fullName, Email = email, PhoneNumber = phone };
                _subItems.Add(_person);
            }

            if(table == "Customer")
            {
                contentCustomerDataGrid.ItemsSource = _subItems;
            }
            else if(table == "Driver")
            {
                contentDriverDataGrid.ItemsSource = _subItems;
            }
            else if(table == "Partner")
            {
                contentPartnerDataGrid.ItemsSource = _subItems;
            }
        }

        private void MainWindowAdminLoaded(object sender, RoutedEventArgs e)
        {
            adminName.Text = LoginWindow.Person.Fullname;

            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            btnAdminManagementChecked(sender, e);
        }
        //Menu
        private void btnAdminManagementChecked(object sender, RoutedEventArgs e)
        {
            //Switch Content
            contentEmployee.Visibility = Visibility.Collapsed;
            contentCustomer.Visibility = Visibility.Collapsed;
            contentDriver.Visibility = Visibility.Collapsed;
            contentPartner.Visibility = Visibility.Collapsed;


            btnAdminManagement.IsChecked = true;
            contentAdmin.Visibility = Visibility.Visible;

            //Data handle
            //Paging
            CurrentPage = 1;

            string sqlQueryTotalAdmin = "SELECT COUNT(*) FROM Staff WHERE StaffAdmin = '1'";
            TotalItems = NumberOfPersons(sqlQueryTotalAdmin);

            int isDivisible = TotalItems % RowsPerPage;
            if (isDivisible != 0)
            {
                TotalPages = TotalItems / RowsPerPage + 1;
            }
            else
            {
                TotalPages = TotalItems / RowsPerPage;
            }

            adminListCurrentPage.Text = CurrentPage.ToString();
            adminListTotalPage.Text = TotalPages.ToString();

            //Data Grid
            SelectList20PersonsForAdmin();
        }

        private void btnEmployeeManagementChecked(object sender, RoutedEventArgs e)
        {
            contentAdmin.Visibility = Visibility.Collapsed;
            contentCustomer.Visibility = Visibility.Collapsed;
            contentDriver.Visibility = Visibility.Collapsed;
            contentPartner.Visibility = Visibility.Collapsed;


            btnEmployeeManagement.IsChecked = true;
            contentEmployee.Visibility = Visibility.Visible;

            //Data handle
            //Paging
            CurrentPage = 1;

            string sqlQueryTotalAdmin = "SELECT COUNT(*) FROM Staff WHERE StaffAdmin = '0' ";
            TotalItems = NumberOfPersons(sqlQueryTotalAdmin);

            int isDivisible = TotalItems % RowsPerPage;
            if (isDivisible != 0)
            {
                TotalPages = TotalItems / RowsPerPage + 1;
            }
            else
            {
                TotalPages = TotalItems / RowsPerPage;
            }

            employeeListCurrentPage.Text = CurrentPage.ToString();
            employeeListTotalPage.Text = TotalPages.ToString();

            //Data Grid
            SelectList20PersonsForEmployee();
        }

        private void btnCustomerManagementChecked(object sender, RoutedEventArgs e)
        {
            contentAdmin.Visibility = Visibility.Collapsed;
            contentEmployee.Visibility = Visibility.Collapsed;
            contentDriver.Visibility = Visibility.Collapsed;
            contentPartner.Visibility = Visibility.Collapsed;


            btnCustomerManagement.IsChecked = true;
            contentCustomer.Visibility = Visibility.Visible;

            //Data handle
            //Paging
            CurrentPage = 1;

            string sqlQueryTotalCustomers = "SELECT COUNT(*) FROM Customer";
            TotalItems = NumberOfPersons(sqlQueryTotalCustomers);

            int isDivisible = TotalItems % RowsPerPage;
            if (isDivisible != 0)
            {
                TotalPages = TotalItems / RowsPerPage + 1;
            }
            else
            {
                TotalPages = TotalItems / RowsPerPage;
            }

            customerListCurrentPage.Text = CurrentPage.ToString();
            customerListTotalPage.Text = TotalPages.ToString();

            //Data Grid
            SelectList20Persons("Customer");
        }

        private void btnDriverManagementChecked(object sender, RoutedEventArgs e)
        {
            contentEmployee.Visibility = Visibility.Collapsed;
            contentCustomer.Visibility = Visibility.Collapsed;
            contentAdmin.Visibility = Visibility.Collapsed;
            contentPartner.Visibility = Visibility.Collapsed;


            btnDriverManagement.IsChecked = true;
            contentDriver.Visibility = Visibility.Visible;

            //Data handle
            //Paging
            CurrentPage = 1;

            string sqlQueryTotalAdmin = "SELECT COUNT(*) FROM Driver";
            TotalItems = NumberOfPersons(sqlQueryTotalAdmin);

            int isDivisible = TotalItems % RowsPerPage;
            if (isDivisible != 0)
            {
                TotalPages = TotalItems / RowsPerPage + 1;
            }
            else
            {
                TotalPages = TotalItems / RowsPerPage;
            }

            driverListCurrentPage.Text = CurrentPage.ToString();
            driverListTotalPage.Text = TotalPages.ToString();

            //Data Grid
            SelectList20Persons("Driver");
        }

        private void btnPartnerManagementChecked(object sender, RoutedEventArgs e)
        {
            contentEmployee.Visibility = Visibility.Collapsed;
            contentCustomer.Visibility = Visibility.Collapsed;
            contentDriver.Visibility = Visibility.Collapsed;
            contentAdmin.Visibility = Visibility.Collapsed;


            btnPartnerManagement.IsChecked = true;
            contentPartner.Visibility = Visibility.Visible;

            //Data handle
            //Paging
            CurrentPage = 1;

            string sqlQueryTotalAdmin = "SELECT COUNT(*) FROM Partner";
            TotalItems = NumberOfPersons(sqlQueryTotalAdmin);

            int isDivisible = TotalItems % RowsPerPage;
            if (isDivisible != 0)
            {
                TotalPages = TotalItems / RowsPerPage + 1;
            }
            else
            {
                TotalPages = TotalItems / RowsPerPage;
            }

            partnerListCurrentPage.Text = CurrentPage.ToString();
            partnerListTotalPage.Text = TotalPages.ToString();

            //Data Grid
            SelectList20Persons("Partner");
        }

        private void logoutButton(object sender, MouseButtonEventArgs e)
        {
            var screen = new StartWindow();
            this.Close();
            screen.Show();
        }
//=======================================Admin Management========================================================
        //Admin List Page navigation
        private void contentaAdminPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentPage <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage--;
                adminListCurrentPage.Text = CurrentPage.ToString();
                SelectList20PersonsForAdmin();
            }
        }

        private void contentaAdminNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == TotalPages)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage++;
                adminListCurrentPage.Text = CurrentPage.ToString();
                SelectList20PersonsForAdmin();
            }
        }
        //Options
        private void contentAdminAddButton_Click(object sender, RoutedEventArgs e)
        {
            contentAdminOptionsRemove.Visibility = Visibility.Collapsed;
            contentAdminOptionsUpdate.Visibility = Visibility.Collapsed;

            contentAdminOptionsAdd.Visibility = Visibility.Visible;
        }

        private void contentAdminRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            contentAdminOptionsUpdate.Visibility = Visibility.Collapsed;
            contentAdminOptionsAdd.Visibility = Visibility.Collapsed;

            contentAdminOptionsRemove.Visibility = Visibility.Visible;
        }

        private void contentAdminUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            contentAdminOptionsAdd.Visibility = Visibility.Collapsed;
            contentAdminOptionsRemove.Visibility = Visibility.Collapsed;

            contentAdminOptionsUpdate.Visibility = Visibility.Visible;
        }

        private Random _random = new Random();
        private void contentAdminAddDoneButton_Click(object sender, RoutedEventArgs e)
        {
            string name = addAdminName.Text.ToString();
            string phone = addAdminPhone.Text.ToString();
            string email = addAdminEmail.Text.ToString();
            string citizenId = addAdminCitizenID.Text.ToString();

            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string sql = "INSERT INTO[dbo].[Staff]([StaffName], [StaffPhone], [StaffEmail], [StaffCitizenId], [StaffAdmin])" +
                         $"VALUES(N'{name}', '{phone}', '{email}', '{citizenId}', '1')";


            var command = new SqlCommand(sql, _connection);
            int count = command.ExecuteNonQuery();

            bool success = count == 1;
            if (success)
            {
                btnAdminManagementChecked(sender, e);
                addAdminName.Text = "";
                addAdminPhone.Text = "";
                addAdminEmail.Text = "";
                addAdminCitizenID.Text = "";
            }

        }

        private void contentAdminRemoveDoneButton_Click(object sender, RoutedEventArgs e)
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string idToRemove = removeAdminID.Text.ToString();
            string sql = $"DELETE FROM Staff WHERE StaffId = '{idToRemove}'";

            var command = new SqlCommand(sql, _connection);
            int count = command.ExecuteNonQuery();

            bool success = count == 1;
            if (success)
            {
                btnAdminManagementChecked(sender, e);
                removeAdminID.Text = "";
            }

        }

        private void updateAdminButton_Click(object sender, RoutedEventArgs e)
        {
            string id = updateAdminID.Text;

            // Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string sql = $"SELECT* FROM Staff WHERE StaffID = '{id}'";
            var command = new SqlCommand(sql, _connection);

            var reader = command.ExecuteReader();
            while(reader.Read())
            {
                placeholderUpdateAdminName.Text = reader.GetString(reader.GetOrdinal("StaffName"));
                placeholderUpdateAdminEmail.Text = reader.GetString(reader.GetOrdinal("StaffEmail"));
                placeholderUpdateAdminPhone.Text = reader.GetString(reader.GetOrdinal("StaffPhone"));
                placeholderUpdateAdminCitizenID.Text = reader.GetString(reader.GetOrdinal("StaffCitizenId"));
            }

        }

        private void contentAdminUpdateDoneButton_Click(object sender, RoutedEventArgs e)
        {
            string newName = placeholderUpdateAdminName.Text;
            string newPhone = placeholderUpdateAdminPhone.Text;
            string newCitizenID = placeholderUpdateAdminCitizenID.Text;
            string newEmail = placeholderUpdateAdminEmail.Text;


            if(updateAdminName.Text == "" && updateAdminPhone.Text == "" &&
                updateAdminCitizenID.Text == "" && updateAdminEmail.Text == "")
            {
                //Do nothing
            }
            else
            {
                if (updateAdminName.Text != "")
                    newName = updateAdminName.Text;
                if (updateAdminPhone.Text != "")
                    newPhone = updateAdminPhone.Text;
                if(updateAdminCitizenID.Text != "")
                    newCitizenID=updateAdminCitizenID.Text;
                if(updateAdminEmail.Text != "")
                    newEmail = updateAdminEmail.Text;

                // Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                string id = updateAdminID.Text;
                string sql = $"UPDATE Staff SET StaffName = N'{newName}', StaffPhone = '{newPhone}', StaffCitizenId = '{newCitizenID}', StaffEmail = '{newEmail}' WHERE StaffId = '{id}'";

                var command = new SqlCommand(sql, _connection);
                int count = command.ExecuteNonQuery();

                bool success = count == 1;
                if (success)
                {
                    btnAdminManagementChecked(sender, e);
                }

            }

            updateAdminName.Text = "";
            updateAdminPhone.Text = "";
            updateAdminCitizenID.Text = "";
            updateAdminEmail.Text = "";
            updateAdminButton_Click(sender, e);
        }


//=======================================Employee Management========================================================
        //Data Grid Page Navigation
        private void contentaEmployeePreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage--;
                employeeListCurrentPage.Text = CurrentPage.ToString();
                SelectList20PersonsForEmployee();
            }
        }

        private void contentaEmployeeNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == TotalPages)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage++;
                employeeListCurrentPage.Text = CurrentPage.ToString();
                SelectList20PersonsForEmployee();
            }
        }
        //Options
        private void contentEmployeeAddButton_Click(object sender, RoutedEventArgs e)
        {
            contentEmployeeOptionsRemove.Visibility = Visibility.Collapsed;
            contentEmployeeOptionsUpdate.Visibility = Visibility.Collapsed;

            contentEmployeeOptionsAdd.Visibility = Visibility.Visible;
        }

        private void contentEmployeeRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            contentEmployeeOptionsAdd.Visibility = Visibility.Collapsed;
            contentEmployeeOptionsUpdate.Visibility = Visibility.Collapsed;

            contentEmployeeOptionsRemove.Visibility = Visibility.Visible;
        }

        private void contentEmployeeUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            contentEmployeeOptionsRemove.Visibility = Visibility.Collapsed;
            contentEmployeeOptionsAdd.Visibility = Visibility.Collapsed;

            contentEmployeeOptionsUpdate.Visibility = Visibility.Visible;
        }
        //Content
        private void contentEmployeeAddDoneButton_Click(object sender, RoutedEventArgs e)
        {
            string name = addEmployeeName.Text.ToString();
            string phone = addEmployeePhone.Text.ToString();
            string email = addEmployeeEmail.Text.ToString();
            string citizenId = addEmployeeCitizenID.Text.ToString();

            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string sql = "INSERT INTO[dbo].[Staff]([StaffName], [StaffPhone], [StaffEmail], [StaffCitizenId], [StaffAdmin])" +
                         $"VALUES(N'{name}', '{phone}', '{email}', '{citizenId}', '0')";


            var command = new SqlCommand(sql, _connection);
            int count = command.ExecuteNonQuery();

            bool success = count == 1;
            if (success)
            {
                btnEmployeeManagementChecked(sender, e);

                addEmployeeName.Text = "";
                addEmployeePhone.Text = "";
                addEmployeeEmail.Text = "";
                addEmployeeCitizenID.Text = "";
            }
        }

        private void contentEmployeeRemoveDoneButton_Click(object sender, RoutedEventArgs e)
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string idToRemove = removeEmployeeID.Text.ToString();
            string sql = $"DELETE FROM Staff WHERE StaffId = '{idToRemove}'";

            var command = new SqlCommand(sql, _connection);
            int count = command.ExecuteNonQuery();

            bool success = count == 1;
            if (success)
            {
                btnEmployeeManagementChecked(sender, e);
                removeEmployeeID.Text = "";
            }
        }

        private void updateEmployeeID_Click(object sender, RoutedEventArgs e)
        {
            string id = updateEmployeeID.Text;

            // Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string sql = $"SELECT* FROM Staff WHERE StaffID = '{id}'";
            var command = new SqlCommand(sql, _connection);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                placeholderUpdateEmployeeName.Text = reader.GetString(reader.GetOrdinal("StaffName"));
                placeholderUpdateEmployeeEmail.Text = reader.GetString(reader.GetOrdinal("StaffEmail"));
                placeholderUpdateEmployeePhone.Text = reader.GetString(reader.GetOrdinal("StaffPhone"));
                placeholderUpdateEmployeeCitizenId.Text = reader.GetString(reader.GetOrdinal("StaffCitizenId"));
            }
        }

        private void contentEmployeeUpdateDoneButton_Click(object sender, RoutedEventArgs e)
        {
            string newName = placeholderUpdateEmployeeName.Text;
            string newPhone = placeholderUpdateEmployeePhone.Text;
            string newCitizenID = placeholderUpdateEmployeeCitizenId.Text;
            string newEmail = placeholderUpdateEmployeeEmail.Text;


            if (updateEmployeeName.Text == "" && updateEmployeePhone.Text == "" &&
                updateEmployeeCitizenID.Text == "" && updateEmployeeEmail.Text == "")
            {
                //Do nothing
            }
            else
            {
                if (updateEmployeeName.Text != "")
                    newName = updateEmployeeName.Text;
                if (updateEmployeePhone.Text != "")
                    newPhone = updateEmployeePhone.Text;
                if (updateEmployeeCitizenID.Text != "")
                    newCitizenID = updateEmployeeCitizenID.Text;
                if (updateEmployeeEmail.Text != "")
                    newEmail = updateEmployeeEmail.Text;

                // Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                string id = updateEmployeeID.Text;
                string sql = $"UPDATE Staff SET StaffName = N'{newName}', StaffPhone = '{newPhone}', StaffCitizenId = '{newCitizenID}', StaffEmail = '{newEmail}' WHERE StaffId = '{id}'";

                var command = new SqlCommand(sql, _connection);
                int count = command.ExecuteNonQuery();

                bool success = count == 1;
                if (success)
                {
                    btnEmployeeManagementChecked(sender, e);
                }

            }

            updateEmployeeName.Text = "";
            updateEmployeePhone.Text = "";
            updateEmployeeCitizenID.Text = "";
            updateEmployeeEmail.Text = "";
            updateEmployeeID_Click(sender, e);

        }
//Customer Management=====================================================================================
        private void contentaCustomerPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage--;
                customerListCurrentPage.Text = CurrentPage.ToString();
                SelectList20Persons("Customer");
            }
        }

        private void contentCustomerNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == TotalPages)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage++;
                customerListCurrentPage.Text = CurrentPage.ToString();
                SelectList20Persons("Customer");
            }
        }

        private void contentCustomerSearID_Click(object sender, RoutedEventArgs e)
        {
            string id = updateCustomerID.Text;

            // Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string sql = $"SELECT* FROM Customer WHERE CustomerID = '{id}'";
            var command = new SqlCommand(sql, _connection);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                placeholderUpdateCustomerName.Text = reader.GetString(reader.GetOrdinal("CustomerName"));
                placeholderUpdateCustomerEmail.Text = reader.GetString(reader.GetOrdinal("CustomerEmail"));
                placeholderUpdateCustomerPhone.Text = reader.GetString(reader.GetOrdinal("CustomerPhone"));
                placeholderUpdateCustomerNoR.Text = reader.GetString(reader.GetOrdinal("CustomerAddressNoR"));
                placeholderUpdateCustomerRoad.Text = reader.GetString(reader.GetOrdinal("CustomerAddressRoad"));
                placeholderUpdateCustomerWard.Text = reader.GetString(reader.GetOrdinal("CustomerAddressWard"));
                placeholderUpdateCustomerAddressDistrict.Text = reader.GetString(reader.GetOrdinal("CustomerAddressDistrict"));
                placeholderUpdateCustomerCity.Text = reader.GetString(reader.GetOrdinal("CustomerAddressCity"));
            }
        }

        private void contentCustomerUpdateDoneButton_Click(object sender, RoutedEventArgs e)
        {
            string newName = placeholderUpdateCustomerName.Text;
            string newPhone = placeholderUpdateCustomerPhone.Text;
            string newEmail = placeholderUpdateCustomerEmail.Text;
            string newNoR = placeholderUpdateCustomerNoR.Text;
            string newRoad = placeholderUpdateCustomerRoad.Text;
            string newWard = placeholderUpdateCustomerRoad.Text;
            string newAddressDistrict = placeholderUpdateCustomerAddressDistrict.Text;
            string newCity = placeholderUpdateCustomerCity.Text;

            if (updateCustomerName.Text == "" && updateCustomerPhone.Text == "" && updateCustomerEmail.Text == "" &&
                updateCustomerNoR.Text == "" && updateCustomerRoad.Text == "" && updateCustomerWard.Text == "" &&
                updateCustomerAddressDistrict.Text == "" && updateCustomerCity.Text == "")
            {
                //Do nothing
            }
            else
            {
                if (updateCustomerName.Text != "")
                    newName = updateCustomerName.Text;
                if (updateCustomerPhone.Text != "")
                    newPhone = updateCustomerPhone.Text;
                if (updateCustomerEmail.Text != "")
                    newEmail = updateCustomerEmail.Text;
                if(updateCustomerNoR.Text != "")
                    newNoR = updateCustomerNoR.Text;
                if(updateCustomerRoad.Text != "")
                    newRoad = updateCustomerRoad.Text;
                if(updateCustomerWard.Text != "")
                    newWard = updateCustomerWard.Text;
                if (updateCustomerAddressDistrict.Text != "")
                    newAddressDistrict = updateCustomerAddressDistrict.Text;
                if (updateCustomerCity.Text != "")
                    newCity = updateCustomerCity.Text;

                // Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                string id = updateCustomerID.Text;
                string sql = $"UPDATE Customer SET CustomerName = N'{newName}', CustomerPhone = '{newPhone}', CustomerAddressDistrict = '{newAddressDistrict}', CustomerEmail = '{newEmail}', CustomerAddressNoR = '{newNoR}', CustomerAddressRoad = '{newRoad}', CustomerAddressWard='{newWard}', CustomerAddressCity = '{newCity}'" +
                    $"WHERE CustomerId = '{id}'";

                var command = new SqlCommand(sql, _connection);
                int count = command.ExecuteNonQuery();

                bool success = count == 1;
                if (success)
                {
                    btnCustomerManagementChecked(sender, e);
                }
            }

            updateCustomerName.Text = "";
            updateCustomerPhone.Text = "";
            updateCustomerEmail.Text = "";
            updateCustomerNoR.Text = "";
            updateCustomerRoad.Text = "";
            updateCustomerWard.Text = "";
            updateCustomerAddressDistrict.Text = "";
            updateCustomerCity.Text = "";
            contentCustomerSearID_Click(sender, e);
        }
//Driver Management=====================================================================================
        private void contentaDriverPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage--;
                driverListCurrentPage.Text = CurrentPage.ToString();
                SelectList20Persons("Driver");
            }
        }

        private void contentDriverNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == TotalPages)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage++;
                driverListCurrentPage.Text = CurrentPage.ToString();
                SelectList20Persons("Driver");
            }
        }

        private void contentDriverSearhID_Click(object sender, RoutedEventArgs e)
        {
            string id = updateDriverID.Text;

            // Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string sql = $"SELECT* FROM Driver WHERE DriverId = '{id}'";
            var command = new SqlCommand(sql, _connection);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                placeholderUpdateDriverName.Text = reader.GetString(reader.GetOrdinal("DriverName"));
                placeholderUpdateDriverEmail.Text = reader.GetString(reader.GetOrdinal("DriverEmail"));
                placeholderUpdateDriverPhone.Text = reader.GetString(reader.GetOrdinal("DriverPhone"));
                placeholderUpdateDriverNoR.Text = reader.GetString(reader.GetOrdinal("DriverAddressNoR"));
                placeholderUpdateDriverRoad.Text = reader.GetString(reader.GetOrdinal("DriverAddressRoad"));
                placeholderUpdateDriverWard.Text = reader.GetString(reader.GetOrdinal("DriverAddressWard"));
                placeholderUpdateDriverAddressDistrict.Text = reader.GetString(reader.GetOrdinal("DriverAddressDistrict"));
                placeholderUpdateDriverCity.Text = reader.GetString(reader.GetOrdinal("DriverAddressCity"));
                placeholderUpdateDriverLicensePlate.Text = reader.GetString(reader.GetOrdinal("DriverLicensePlates"));
            }
        }

        private void contentDiverUpdateDoneButton_Click(object sender, RoutedEventArgs e)
        {
            string newName = placeholderUpdateDriverName.Text;
            string newEmail = placeholderUpdateDriverEmail.Text;
            string newPhone = placeholderUpdateDriverPhone.Text;
            string newNoR = placeholderUpdateDriverNoR.Text;
            string newRoad = placeholderUpdateDriverRoad.Text;
            string newWard = placeholderUpdateDriverWard.Text;
            string newDistrict = placeholderUpdateDriverAddressDistrict.Text;
            string newCity = placeholderUpdateDriverCity.Text;
            string newLicense = placeholderUpdateDriverLicensePlate.Text;

            if (updateDriverName.Text == "" && updateDriverPhone.Text == "" && updateDriverEmail.Text == "" &&
                updateDriverNoR.Text == "" && updateDriverRoad.Text == "" && updateDriverWard.Text == "" && updateDriverAddressDistrict.Text == "" && updateDriverCity.Text == "" &&
                updateDriverLicensePlate.Text == "")
            {
                //Do nothing
            }
            else
            {
                if (updateDriverName.Text != "")
                    newName = updateDriverName.Text;
                if(updateDriverPhone.Text != "")
                    newPhone = updateDriverPhone.Text;
                if(updateDriverEmail.Text != "")
                    newEmail = updateDriverEmail.Text;

                if (updateDriverNoR.Text != "")
                    newNoR = updateDriverNoR.Text;
                if (updateDriverRoad.Text != "")
                    newRoad = updateDriverRoad.Text;
                if (updateDriverWard.Text != "")
                    newWard = updateDriverWard.Text;
                if (updateDriverAddressDistrict.Text != "")
                    newDistrict = updateDriverAddressDistrict.Text;
                if (updateDriverCity.Text != "")
                    newCity = updateDriverCity.Text;

                if (updateDriverLicensePlate.Text != "")
                    newLicense = updateDriverLicensePlate.Text;

                // Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                string id = updateDriverID.Text;
                string sql = $"UPDATE Driver SET DriverName = N'{newName}', DriverPhone = '{newPhone}', DriverAddressDistrict = '{newDistrict}', DriverEmail = '{newEmail}', DriverAddressNoR = '{newNoR}', DriverAddressRoad = '{newRoad}', DriverAddressWard='{newWard}', DriverAddressCity = '{newCity}', DriverLicensePlates='{newLicense}'" +
                    $"WHERE DriverId = '{id}'";

                var command = new SqlCommand(sql, _connection);
                int count = command.ExecuteNonQuery();

                bool success = count == 1;
                if (success)
                {
                    btnDriverManagementChecked(sender, e);
                }

            }

            updateDriverName.Text = "";
            updateDriverPhone.Text = "";
            updateDriverEmail.Text = "";
            updateDriverNoR.Text = "";
            updateDriverRoad.Text = "";
            updateDriverWard.Text = "";
            updateDriverAddressDistrict.Text = "";
            updateDriverCity.Text = "";
            updateDriverLicensePlate.Text = "";
            contentDriverSearhID_Click(sender, e);
        }
//Partner Management=====================================================================================
        private void contentaPartnerPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage--;
                partnerListCurrentPage.Text = CurrentPage.ToString();
                SelectList20Persons("Partner");
            }
        }


        private void contentPartnerNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == TotalPages)
            {
                //Do nothing
            }
            else
            {
                _subItems.Clear();
                CurrentPage++;
                driverListCurrentPage.Text = CurrentPage.ToString();
                SelectList20Persons("Driver");
            }
        }

        private void contentPartnerSearID_Click(object sender, RoutedEventArgs e)
        {
            string id = updatePartnerID.Text;

            // Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            string sql = $"SELECT* FROM Partner WHERE PartnerId = '{id}'";
            var command = new SqlCommand(sql, _connection);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                placeholderUpdatePartnerName.Text = reader.GetString(reader.GetOrdinal("PartnerName"));
                placeholderUpdatePartnerEmail.Text = reader.GetString(reader.GetOrdinal("PartnerEmail"));
                placeholderUpdatePartnerPhone.Text = reader.GetString(reader.GetOrdinal("PartnerPhone"));
                placeholderUpdatePartnerNoR.Text = reader.GetOrdinal("PartnerAddressNoR").ToString();
                placeholderUpdatePartnerRoad.Text = reader.GetString(reader.GetOrdinal("PartnerAddressRoad"));
                placeholderUpdatePartnerWard.Text = reader.GetString(reader.GetOrdinal("PartnerAddressWard"));
                placeholderUpdatePartnerAddressDistrict.Text = reader.GetString(reader.GetOrdinal("PartnerAddressDistrict"));
                placeholderUpdatePartnerCity.Text = reader.GetString(reader.GetOrdinal("PartnerAddressCity"));
            }
        }

        private void contentPartnerUpdateDoneButton_Click(object sender, RoutedEventArgs e)
        {
            string newName = placeholderUpdatePartnerName.Text;
            string newPhone = placeholderUpdatePartnerPhone.Text;
            string newEmail = placeholderUpdatePartnerEmail.Text;
            string newNoR = placeholderUpdatePartnerNoR.Text;
            string newRoad = placeholderUpdatePartnerRoad.Text;
            string newWard = placeholderUpdatePartnerRoad.Text;
            string newAddressDistrict = placeholderUpdatePartnerAddressDistrict.Text;
            string newCity = placeholderUpdatePartnerCity.Text;

            if (updatePartnerName.Text == "" && updatePartnerPhone.Text == "" && updatePartnerEmail.Text == "" &&
                updatePartnerNoR.Text == "" && updatePartnerRoad.Text == "" && updatePartnerWard.Text == "" &&
                updatePartnerAddressDistrict.Text == "" && updatePartnerCity.Text == "")
            {
                //Do nothing
            }
            else
            {
                if (updatePartnerName.Text != "")
                    newName = updatePartnerName.Text;
                if (updatePartnerPhone.Text != "")
                    newPhone = updatePartnerPhone.Text;
                if (updatePartnerEmail.Text != "")
                    newEmail = updatePartnerEmail.Text;
                if (updatePartnerNoR.Text != "")
                    newNoR = updatePartnerNoR.Text;
                if (updatePartnerRoad.Text != "")
                    newRoad = updatePartnerRoad.Text;
                if (updatePartnerWard.Text != "")
                    newWard = updatePartnerWard.Text;
                if (updatePartnerAddressDistrict.Text != "")
                    newAddressDistrict = updatePartnerAddressDistrict.Text;
                if (updatePartnerCity.Text != "")
                    newCity = updatePartnerCity.Text;

                // Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                string id = updatePartnerID.Text;
                string sql = $"UPDATE Partner SET PartnerName = N'{newName}', PartnerPhone = '{newPhone}', PartnerAddressDistrict = '{newAddressDistrict}', PartnerEmail = '{newEmail}', PartnerAddressNoR = '{newNoR}', PartnerAddressRoad = '{newRoad}', PartnerAddressWard='{newWard}', PartnerAddressCity = '{newCity}'" +
                    $"WHERE PartnerId = '{id}'";

                var command = new SqlCommand(sql, _connection);
                int count = command.ExecuteNonQuery();

                bool success = count == 1;
                if (success)
                {
                    btnPartnerManagementChecked(sender, e);
                }
            }

            updatePartnerName.Text = "";
            updatePartnerPhone.Text = "";
            updatePartnerEmail.Text = "";
            updatePartnerNoR.Text = "";
            updatePartnerRoad.Text = "";
            updatePartnerWard.Text = "";
            updatePartnerAddressDistrict.Text = "";
            updatePartnerCity.Text = "";
            contentPartnerSearID_Click(sender, e);
        }
    }
}
