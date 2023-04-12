using OnlineSellingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace OnlineSellingSystem.View
{
    /// <summary>
    /// Interaction logic for MainWindowEmployee.xaml
    /// </summary>
    public partial class MainWindowEmployee : Window, INotifyCollectionChanged
    {
        public MainWindowEmployee()
        {
            InitializeComponent();
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        SqlConnection _connection = null;

        //Partner, Branch management
        ObservableCollection<Person> _subItemsPartner = new ObservableCollection<Person>();
        ObservableCollection<Branch> _subItemsLowestRating = new ObservableCollection<Branch>();

        public int RowsPerPage { get; set; } = 30;

        public int TotalPagesPartner { get; set; } = 0;
        public int CurrentPagePartner { get; set; } = 1;
        public int TotalItemsPartner { get; set; } = 0;

        public int TotalPagesLowestRating { get; set; } = 0;
        public int CurrentPageLowestRating { get; set; } = 1;
        public int TotalItemsLowestRating { get; set; } = 0;

        //Contract management
        ObservableCollection<Contract> _subItemsContract = new ObservableCollection<Contract>();
        ObservableCollection<Contract> _subItemsContractExpire = new ObservableCollection<Contract>();
        ObservableCollection<Contract> _subItemsContractUnapproved = new ObservableCollection<Contract>();

        public int TotalPagesContract { get; set; } = 0;
        public int CurrentPageContract { get; set; } = 1;
        public int TotalItemsContract { get; set; } = 0;

        public int TotalPagesContractExpire { get; set; } = 0;
        public int CurrentPageContractExpire { get; set; } = 1;
        public int TotalItemsContractExpire { get; set; } = 0;

        public int TotalPagesContractUnapproved { get; set; } = 0;
        public int CurrentPageContractUnapproved { get; set; } = 1;
        public int TotalItemsContractUnapproved { get; set; } = 0;

        //Function
        private int NumberOfPersons(string sqlQueryTotalPerson)
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open(); 

            //Count number of person: admin/employee/customer/driver/partner
            var command = new SqlCommand(sqlQueryTotalPerson, _connection);
            int count = (int)command.ExecuteScalar();

            return count;
        }

        private void SelectSubItemsPartner()
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPagePartner - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectList20Persons = $"SELECT* FROM Partner ORDER BY PartnerId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY";
            var command = new SqlCommand(sqlSelectList20Persons, _connection);
            var reader = command.ExecuteReader();

            _subItemsPartner.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("PartnerId"));
                string fullName = reader.GetString(reader.GetOrdinal("PartnerName"));
                string email = reader.GetString(reader.GetOrdinal("PartnerEmail"));
                string phone = reader.GetString(reader.GetOrdinal("PartnerPhone"));
                var _person = new Person { Type = "Partner", Id = id, Fullname = fullName, Email = email, PhoneNumber = phone };
                _subItemsPartner.Add(_person);
            }

            contentPartnerManagementPartnerListView.ItemsSource = _subItemsPartner;
            
        }

        private void SelectSubItemsLowestRating()
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPageLowestRating - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectListLowestRating =
                $"""
                SELECT [Branch].BranchId, [Branch].BranchName, [Partner].PartnerId, [Partner].PartnerName, [dbo].[f_CalculateMeanStar_Branch] ([Branch].BranchId) AS RatingStar
                FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                			   INNER JOIN [Branch] ON [Branch].ContractId = [Contract].ContractId
                WHERE [dbo].[f_CalculateMeanStar_Branch] ([Branch].BranchId) < 3
                ORDER BY [Branch].BranchId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY
                
                """;
            var command = new SqlCommand(sqlSelectListLowestRating, _connection);
            var reader = command.ExecuteReader();

            _subItemsLowestRating.Clear();
            while (reader.Read())
            {
                int branchId = reader.GetInt32(reader.GetOrdinal("BranchId"));
                string branchName = reader.GetString(reader.GetOrdinal("BranchName"));

                int branchPartnerId = reader.GetInt32(reader.GetOrdinal("PartnerId"));
                string branchPartnerName = reader.GetString(reader.GetOrdinal("PartnerName"));

                int branchStar = reader.GetInt32(reader.GetOrdinal("RatingStar"));

                var _branch = new Branch { BranchId = branchId, BranchName = branchName, BranchPartnerId = branchPartnerId, BranchPartnerName = branchPartnerName, BranchStar = branchStar};
                _subItemsLowestRating.Add(_branch);
            }

            contentPartnerManagementPartnerLowestRatingListView.ItemsSource = _subItemsLowestRating;

        }
        
        private void SelectSubItemsContract()
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPageContract - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectSubItems =
                $"""
                    SELECT [Contract].ContractId, [Partner].PartnerId, [Partner].PartnerName, [Contract].StatusId
                    FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                    ORDER BY ContractId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY
                """;

            var command = new SqlCommand(sqlSelectSubItems, _connection);
            var reader = command.ExecuteReader();

            _subItemsContract.Clear();
            while (reader.Read())
            {

                int contractId = reader.GetInt32(reader.GetOrdinal("ContractId"));
                int contractPartnerId = reader.GetInt32(reader.GetOrdinal("PartnerId"));
                string contractPartnerName = reader.GetString(reader.GetOrdinal("PartnerName"));

                int contractStatus = reader.GetInt32(reader.GetOrdinal("StatusId"));

                string string_contractStatus = "";
                if (contractStatus == -1)
                    string_contractStatus = "Lỗi/Bị khóa";
                else if (contractStatus == 2)
                    string_contractStatus = "Sắp hết hạn";
                else if (contractStatus == 3)
                    string_contractStatus = "Hết hạn";
                else if (contractStatus == 4)
                    string_contractStatus = "Chờ duyệt";
                else if (contractStatus == 6)
                    string_contractStatus = "Còn hạn";

                var _contract = new Contract {ContractId = contractId, ContractPartnerId = contractPartnerId, ContractPartnerName = contractPartnerName, ContractStatus = string_contractStatus};
                _subItemsContract.Add(_contract);
            }

            contentContractManagementContractListView.ItemsSource = _subItemsContract;
        }

        private void SelectSubItemsContractExpire()
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPageContractExpire - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            //Cần cập nhật lại sql
            string sqlSelectSubItems =
                $"""
                    SELECT [Contract].ContractId, [Partner].PartnerId,[Partner].PartnerName, [Contract].ContractStartDay, [Contract].ContractEndDay
                    FROM [Contract] INNER JOIN [Partner] ON [Contract].PartnerId = [Partner].PartnerId
                    WHERE DATEDIFF(D, [Contract].ContractEndDay, GETDATE()) < 60 OR [Contract].StatusId = 2
                    ORDER BY ContractId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY
                """;

            var command = new SqlCommand(sqlSelectSubItems, _connection);
            var reader = command.ExecuteReader();

            _subItemsContractExpire.Clear();
            while (reader.Read())
            {
                int contractId = reader.GetInt32(reader.GetOrdinal("ContractId"));
                int contractPartnerId = reader.GetInt32(reader.GetOrdinal("PartnerId"));
                string contractPartnerName = reader.GetString(reader.GetOrdinal("PartnerName"));
                string contractStartDay = reader.GetDateTime(reader.GetOrdinal("ContractStartDay")).ToString("dd/mm/yyyy");
                string contractSEndDay = reader.GetDateTime(reader.GetOrdinal("ContractEndDay")).ToString("dd/mm/yyyy");

                var _contract = new Contract { ContractId = contractId, ContractPartnerId = contractPartnerId, ContractPartnerName = contractPartnerName, ContractStartDay = contractStartDay, ContractEndDay = contractSEndDay };
                _subItemsContractExpire.Add(_contract);
            }

            contentContractManagementContractDueToExpireListView.ItemsSource = _subItemsContractExpire;
        }

        private void SelectSubItemsUnapprovedContract()
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPageContractUnapproved - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            //Cần cập nhật lại sql
            string sqlSelectSubItems =
                $"""
                    SELECT [Contract].ContractId, [Partner].PartnerId, [Partner].PartnerName, [Partner].PartnerRegisterTime
                    FROM [Contract] INNER JOIN [Partner] ON [Contract].PartnerId = [Partner].PartnerId
                    WHERE [Contract].StatusId = 4
                    ORDER BY ContractId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY
                """;

            var command = new SqlCommand(sqlSelectSubItems, _connection);
            var reader = command.ExecuteReader();

            _subItemsContractUnapproved.Clear();
            while (reader.Read())
            {
                int contractId = reader.GetInt32(reader.GetOrdinal("ContractId"));
                int contractPartnerId = reader.GetInt32(reader.GetOrdinal("PartnerId"));
                string contractPartnerName = reader.GetString(reader.GetOrdinal("PartnerName"));
                string contractDate = reader.GetDateTime(reader.GetOrdinal("PartnerRegisterTime")).ToString("dd/mm/yyyy");

                var _contract = new Contract { ContractId = contractId, ContractPartnerId = contractPartnerId, ContractPartnerName = contractPartnerName, ContractDate = contractDate };
                _subItemsContractUnapproved.Add(_contract);
            }

            contentContractManagementUnapprovedContractListView.ItemsSource = _subItemsContractUnapproved;
        }

        private void MainWindowEmployeeLoaded(object sender, RoutedEventArgs e)
        {
           
            btnPartnerManagement.IsChecked = true;
            contentPartnerManagement.Visibility = Visibility.Visible;
            btnPartnerManagementChecked(sender, e);

            employeeName.Text = LoginWindow.Person.Fullname;
        }

//=====================Menu Navigation======================================================

        private void btnPartnerManagementChecked(object sender, RoutedEventArgs e)
        {
            contentContractManagement.Visibility = Visibility.Collapsed;

            btnPartnerManagement.IsChecked = true;
            contentPartnerManagement.Visibility = Visibility.Visible;

            //Data handle partner list
            CurrentPagePartner = 1;

            string sqlQueryTotalPartner = "SELECT COUNT(*) FROM Partner";
            TotalItemsPartner = NumberOfPersons(sqlQueryTotalPartner);

            int isDivisiblePartner = TotalItemsPartner % RowsPerPage;
            if (isDivisiblePartner != 0)
            {
                TotalPagesPartner = TotalItemsPartner / RowsPerPage + 1;
            }
            else
            {
                TotalPagesPartner = TotalItemsPartner / RowsPerPage;
            }

            partnerListCurrentPage.Text = CurrentPagePartner.ToString();
            partnerListTotalPage.Text = TotalPagesPartner.ToString();

            //Data Grid
            SelectSubItemsPartner();


            //Data handle branch lowest rating list
            CurrentPageLowestRating = 1;

            string sqlQueryTotalBranch =
                """
                SELECT COUNT(*)
                FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                			   INNER JOIN [Branch] ON [Branch].ContractId = [Contract].ContractId
                WHERE [dbo].[f_CalculateMeanStar_Branch] ([Branch].BranchId) < 3
                """;
            TotalItemsLowestRating = NumberOfPersons(sqlQueryTotalBranch);

            int isDivisibleLowestRating = TotalItemsLowestRating % RowsPerPage;
            if (isDivisibleLowestRating != 0)
            {
                TotalPagesLowestRating = TotalItemsLowestRating / RowsPerPage + 1;
            }
            else
            {
                TotalPagesLowestRating = TotalItemsLowestRating / RowsPerPage;
            }

            lowestRatingListCurrentPage.Text = CurrentPageLowestRating.ToString();
            lowestRatingListTotalPage.Text = TotalPagesLowestRating.ToString();

            //Data Grid
            SelectSubItemsLowestRating();
        }

        private void btnContractManagementChecked(object sender, RoutedEventArgs e)
        {
            contentPartnerManagement.Visibility=Visibility.Collapsed;

            btnContractManagement.IsChecked=true;
            contentContractManagement.Visibility=Visibility.Visible;

            //Contract List
            CurrentPageContract = 1;
            string sqlContractList =
                """
                    SELECT COUNT(*)
                    FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                """;
            TotalItemsContract = NumberOfPersons(sqlContractList);

            int isDivisibleContractList = TotalItemsContract % RowsPerPage;
            if (isDivisibleContractList != 0)
            {
                TotalPagesContract = TotalItemsContract / RowsPerPage + 1;
            }
            else
            {
                TotalPagesContract = TotalItemsContract / RowsPerPage;
            }

            contractListCurrentPage.Text = CurrentPageContract.ToString();
            contractListTotalPage.Text = TotalPagesContract.ToString();
            SelectSubItemsContract();

            //Contract Expire List
            CurrentPageContractExpire = 1;
            string sqlContractExpireList =
                """
                    SELECT COUNT(*)
                    FROM [Contract] INNER JOIN [Partner] ON [Contract].PartnerId = [Partner].PartnerId
                    WHERE DATEDIFF(D, [Contract].ContractEndDay, GETDATE()) < 60 OR [Contract].StatusId = 2
                """;
            TotalItemsContractExpire = NumberOfPersons(sqlContractExpireList);

            int isDivisibleContractExpireList = TotalItemsContractExpire % RowsPerPage;
            if (isDivisibleContractExpireList != 0)
            {
                TotalPagesContractExpire = TotalItemsContractExpire / RowsPerPage + 1;
            }
            else
            {
                TotalPagesContractExpire = TotalItemsContractExpire / RowsPerPage;
            }

            contractExpireListCurrentPage.Text = CurrentPageContractExpire.ToString();
            contractExpireListTotalPage.Text = TotalPagesContractExpire.ToString();
            SelectSubItemsContractExpire();

            //Contract Unapproved List
            CurrentPageContractUnapproved = 1;
            string sqlContractUnapprovedList =
                """
                    SELECT COUNT(*)
                    FROM [Contract] INNER JOIN [Partner] ON [Contract].PartnerId = [Partner].PartnerId
                    WHERE [Contract].StatusId = 4
                """;
            TotalItemsContractUnapproved = NumberOfPersons(sqlContractUnapprovedList);

            int isDivisibleContractUnapprovedList = TotalItemsContractUnapproved % RowsPerPage;
            if (isDivisibleContractUnapprovedList != 0)
            {
                TotalPagesContractUnapproved = TotalItemsContractUnapproved / RowsPerPage + 1;
            }
            else
            {
                TotalPagesContractUnapproved = TotalItemsContractUnapproved / RowsPerPage;
            }

            unapprovedContractListCurrentPage.Text = CurrentPageContractUnapproved.ToString();
            unapprovedContractListTotalPage.Text = TotalPagesContractUnapproved.ToString();
            SelectSubItemsUnapprovedContract();

        }

        private void logoutButton(object sender, MouseButtonEventArgs e)
        {
            //log out
            var screen = new StartWindow();
            this.Close();
            screen.Show();
        }

//Shop management================================================================================

        private void contentaEmployeePartnerListPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPagePartner <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItemsPartner.Clear();
                CurrentPagePartner--;
                partnerListCurrentPage.Text = CurrentPagePartner.ToString();
                SelectSubItemsPartner();
            }
        }

        private void contentaEmployeePartnerListNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPagePartner == TotalPagesPartner)
            {
                //Do nothing
            }
            else
            {
                _subItemsPartner.Clear();
                CurrentPagePartner++;
                partnerListCurrentPage.Text = CurrentPagePartner.ToString();
                SelectSubItemsPartner();
            }
        }

        private void contentaEmployeeLowestRatingPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageLowestRating <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItemsLowestRating.Clear();
                CurrentPageLowestRating--;
                lowestRatingListCurrentPage.Text = CurrentPageLowestRating.ToString();
                SelectSubItemsLowestRating();
            }
        }

        private void contentaEmployeeLowestRatingNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageLowestRating == TotalPagesLowestRating)
            {
                //Do nothing
            }
            else
            {
                _subItemsLowestRating.Clear();
                CurrentPageLowestRating++;
                lowestRatingListCurrentPage.Text = CurrentPageLowestRating.ToString();
                SelectSubItemsLowestRating();
            }
        }


        private void contentPartnerManagementSeeStatistics_Click(object sender, RoutedEventArgs e)
        {
            //Get Partner ID
            string partnerID = inputPartnerID.Text;

            //Get date
            int day = partnerStatisticsDatePicker.SelectedDate.Value.Day;
            int month = partnerStatisticsDatePicker.SelectedDate.Value.Month;
            int year = partnerStatisticsDatePicker.SelectedDate.Value.Year;

            if(partnerID != "")
            {
                //Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                //Customers Per Day
                string spCustomersPerDay = "sp_CalculateSumCustomersInDay_Partner";
                var command = new SqlCommand(spCustomersPerDay, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PartnerId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = partnerID,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Day",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = day,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Month",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = month,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterCustomersPerDay = command.Parameters.Add("@Return", SqlDbType.Int);
                returnParameterCustomersPerDay.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultPerDay = returnParameterCustomersPerDay.Value;
                customerPerDay.Text = resultPerDay.ToString();

                //Number of customers/month
                string spCustomersPerMonth = "sp_CalculateSumCustomersInMonth_Partner";
                command = new SqlCommand(spCustomersPerMonth, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PartnerId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = partnerID,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Month",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = month,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterCustomersPerMonth = command.Parameters.Add("@Return", SqlDbType.Int);
                returnParameterCustomersPerMonth.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultPerMonth = returnParameterCustomersPerMonth.Value;
                customerPerMonth.Text = resultPerMonth.ToString();

                //Number of customers/year
                string spCustomersPerYear = "sp_CalculateSumCustomersInYear_Partner";
                command = new SqlCommand(spCustomersPerYear, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PartnerId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = partnerID,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterCustomersPerYear = command.Parameters.Add("@Return", SqlDbType.Int);
                returnParameterCustomersPerYear.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultPerYear = returnParameterCustomersPerYear.Value;
                customersPerYear.Text = resultPerYear.ToString();

                //Revenue
                //Revenue Day
                string sp_revenueDay = "sp_CalculatePartnerTotalRevenueInDay_Partner";
                command = new SqlCommand(sp_revenueDay, _connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                                    {
                                        ParameterName = "@PartnerId",
                                        SqlDbType = SqlDbType.Int,
                                        Value = partnerID,
                                    });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Day",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = day,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Month",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = month,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterRevenueDay = command.Parameters.Add("@Return", SqlDbType.Money);
                returnParameterRevenueDay.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultRevenueDay = returnParameterRevenueDay.Value;
                revenueOfPartnerDay.Text = resultRevenueDay.ToString();

                //Revenue Month
                string sp_revenueMonth = "sp_CalculatePartnerTotalRevenueInMonth_Partner";
                command = new SqlCommand(sp_revenueMonth, _connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PartnerId",
                    SqlDbType = SqlDbType.Int,
                    Value = partnerID,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Month",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = month,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterRevenueMonth = command.Parameters.Add("@Return", SqlDbType.Money);
                returnParameterRevenueMonth.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultRevenueMonth = returnParameterRevenueMonth.Value;
                revenueOfPartnerMonth.Text = resultRevenueMonth.ToString();

                //Revenue Year
                string sp_revenueYear = "sp_CalculatePartnerTotalRevenueInYear_Partner";
                command = new SqlCommand(sp_revenueYear, _connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PartnerId",
                    SqlDbType = SqlDbType.Int,
                    Value = partnerID,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterRevenueYear = command.Parameters.Add("@Return", SqlDbType.Money);
                returnParameterRevenueYear.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultRevenueYear = returnParameterRevenueYear.Value;
                revenueOfPartnerYear.Text = resultRevenueYear.ToString();

                //Commmission
                //Commission Day
                var sp_commissionDay = "sp_CalculatePartnerCommissionInDay_Partner";
                command = new SqlCommand(sp_commissionDay, _connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PartnerId",
                    SqlDbType = SqlDbType.Int,
                    Value = partnerID,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Day",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = day,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Month",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = month,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterCommissionDay = command.Parameters.Add("@Return", SqlDbType.Money);
                returnParameterCommissionDay.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultCommissionDay = returnParameterCommissionDay.Value;
                commissionFromPartnerDay.Text = resultCommissionDay.ToString();

                //Commission Month
                string sp_commissionMonth = "sp_CalculatePartnerCommissionInMonth_Partner";
                command = new SqlCommand(sp_commissionMonth, _connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PartnerId",
                    SqlDbType = SqlDbType.Int,
                    Value = partnerID,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Month",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = month,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterCommissionMonth = command.Parameters.Add("@Return", SqlDbType.Money);
                returnParameterCommissionMonth.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultCommissionMonth = returnParameterCommissionMonth.Value;
                commissionFromPartnerMonth.Text = resultCommissionMonth.ToString();

                //Commission Year
                string sp_commissionYear = "sp_CalculatePartnerCommissionInYear_Partner";
                command = new SqlCommand(sp_commissionYear, _connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@PartnerId",
                    SqlDbType = SqlDbType.Int,
                    Value = partnerID,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Year",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = year,
                });

                var returnParameterCommissionYear = command.Parameters.Add("@Return", SqlDbType.Money);
                returnParameterCommissionYear.Direction = ParameterDirection.ReturnValue;

                command.ExecuteNonQuery();
                var resultCommissionYear = returnParameterCommissionYear.Value;
                commissionFromPartnerYear.Text = resultCommissionYear.ToString();
            }
            else
            {
                //Do nothing
            }


        }


//Contract management==============================================================================================

        //Send notification to partner who have contract is due to expire
        private void sendNotiToPartnerExpireContract_Click(object sender, RoutedEventArgs e)
        {
            string partnerID = sendNotiContractID.Text;

            if(partnerID != "")
            {
                string sql = $"UPDATE Partner SET Partner.StatusId = 5 WHERE PartnerId = {partnerID}";

                //Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                var command = new SqlCommand(sql, _connection);
                int sent = command.ExecuteNonQuery();
                if (sent == 1)
                {
                    MessageBox.Show($"Sent A Notification To Partner With ID: {partnerID}");
                    sendNotiContractID.Text = "";


                }
                else
                {
                    //Do something
                }
            }
            else
            {
                //Do nothing
            }
        }
        

        //Pagingnation Contract list
        private void contractListPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageContract <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItemsContract.Clear();
                CurrentPageContract--;
                contractListCurrentPage.Text = CurrentPageContract.ToString();
                SelectSubItemsContract();
            }
        }

        private void contractListNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageContract == TotalPagesContract)
            {
                //Do nothing
            }
            else
            {
                _subItemsContract.Clear();
                CurrentPageContract++;
                contractListCurrentPage.Text = CurrentPageContract.ToString();
                SelectSubItemsContract();
            }
        }

        //Pagingnation contract expire list
        private void contractExpireListPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageContractExpire <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItemsContractExpire.Clear();
                CurrentPageContractExpire--;
                contractExpireListCurrentPage.Text = CurrentPageContractExpire.ToString();
                SelectSubItemsContractExpire();
            }
        }

        private void contractExpireListNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageContractExpire == TotalPagesContractExpire)
            {
                //Do nothing
            }
            else
            {
                _subItemsContractExpire.Clear();
                CurrentPageContractExpire++;
                contractExpireListCurrentPage.Text = CurrentPageContractExpire.ToString();
                SelectSubItemsContractExpire();
            }
        }

        //Pagingnation unpproved contract list
        private void unapprovedContractListPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageContractUnapproved <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItemsContractUnapproved.Clear();
                CurrentPageContractUnapproved--;
                unapprovedContractListCurrentPage.Text = CurrentPageContractUnapproved.ToString();
                SelectSubItemsUnapprovedContract();
            }
        }

        private void unapprovedContractListNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageContractUnapproved == TotalPagesContractUnapproved)
            {
                //Do nothing
            }
            else
            {
                _subItemsContractUnapproved.Clear();
                CurrentPageContractUnapproved++;
                unapprovedContractListCurrentPage.Text = CurrentPageContractUnapproved.ToString();
                SelectSubItemsUnapprovedContract();
            }
        }

        //Options: Approve or remove unapproved contracts
        private void unapprovedContractApproveButton_Click(object sender, RoutedEventArgs e)
        {
            string contractID = removeUnapprovedContract.Text;

            if (contractID != "")
            {
                string sql = $"UPDATE Contract SET Contract.StatusId = 6 WHERE ContractId = {contractID}";

                //Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                var command = new SqlCommand(sql, _connection);
                int sent = command.ExecuteNonQuery();
                if (sent == 1)
                {
                    MessageBox.Show($"Approved Contract With ID: {contractID}");
                    removeUnapprovedContract.Text = "";
                }
                else
                {
                    //Do something
                }
            }
            else
            {
                //Do nothing
            }
        }

        private void unapprovedContractRemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
