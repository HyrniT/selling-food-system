using OnlineSellingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
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
using MessageBox = System.Windows.Forms.MessageBox;

namespace OnlineSellingSystem.View
{
    /// <summary>
    /// Interaction logic for MainWindowDriver.xaml
    /// </summary>
    public partial class MainWindowDriver : Window, INotifyPropertyChanged
    {
        public MainWindowDriver()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        SqlConnection _connection = null;

        ObservableCollection<Order> _subItemsOrder = new ObservableCollection<Order>();
        ObservableCollection<Order> _subItemsOrderDelivered = new ObservableCollection<Order>();

        public int RowsPerPage { get; set; } = 30;

        public int TotalPagesOrder { get; set; } = 0;
        public int CurrentPageOrder { get; set; } = 1;

        public int TotalItemsOrder { get; set; } = 0;

        public int TotalPagesOrderDelivered { get; set; } = 0;
        public int CurrentPageOrderDelivered { get; set; } = 1;

        public int TotalItemsOrderDelivered { get; set; } = 0;

        //Function
        private int NumberOfItems(string sqlQueryTotalPerson)
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            //Count number of person: admin/employee/customer/driver/partner
            var command = new SqlCommand(sqlQueryTotalPerson, _connection);
            int count = (int)command.ExecuteScalar();

            return count;
        }

        private void SelectSubItemsOrder()
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPageOrder - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectListProduct =
                $"""
                    SELECT [Order].OrderId AS OrderId, [Order].OrderFinalPrice AS Total, Branch.BranchName AS BranchName, Customer.CustomerName AS CustomerName, Customer.CustomerPhone AS CustomerPhone
                    FROM Branch INNER JOIN [Order] ON Branch.BranchId = [Order].BranchId AND [Order].StatusId = 4
                			      INNER JOIN Customer ON [Order].CustomerId = Customer.CustomerId
                    ORDER BY OrderId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY
                """;

            var command = new SqlCommand(sqlSelectListProduct, _connection);
            var reader = command.ExecuteReader();

            _subItemsOrder.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("OrderId"));
                string branchName = reader.GetString(reader.GetOrdinal("BranchName"));
                string branchPhone = reader.GetString(reader.GetOrdinal("BranchPhone"));
                string customerName = reader.GetString(reader.GetOrdinal("CustomerName"));
                string customerPhone = reader.GetString(reader.GetOrdinal("CustomerPhone"));
                SqlMoney total = reader.GetSqlMoney(reader.GetOrdinal("Total"));

                var _order = new Order { Id=id, BranchName=branchName, BranchPhone=branchPhone, CustomerName=customerName, CustomerPhone=customerPhone, Total=total};
                _subItemsOrder.Add(_order);
            }

            contentSelectOrdersDataGrid.ItemsSource = _subItemsOrder;

        }

        private void SelectSubItemsOrderDelivered()
        {
            //Driver Id
            int driverId = LoginWindow.Person.Id;

            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPageOrderDelivered - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectListProduct =
                $"""
                    SELECT [Order].OrderId AS OrderId,[Order].OrderFinalPrice AS Price, [Branch].BranchName AS BranchName, [Customer].CustomerName AS CustomerName, [Customer].CustomerPhone AS CustomerPhone, [Order].StatusId AS StatusId
                    FROM [Order] INNER JOIN [Branch] ON [Order].BranchId = [Branch].BranchId AND [Order].DriverId = {driverId} AND ([Order].StatusId = 1 OR [Order].StatusId = 6)
                			 INNER JOIN [Customer] ON [Order].CustomerId = [Customer].CustomerId
                    ORDER BY OrderId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY
                """;

            var command = new SqlCommand(sqlSelectListProduct, _connection);
            var reader = command.ExecuteReader();

            _subItemsOrderDelivered.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("OrderId"));
                string branchName = reader.GetString(reader.GetOrdinal("BranchName"));
                SqlMoney price = reader.GetSqlMoney(reader.GetOrdinal("Price"));
                string customerName = reader.GetString(reader.GetOrdinal("CustomerName"));
                string customerPhone = reader.GetString(reader.GetOrdinal("CustomerPhone"));

                int statusid = reader.GetInt32(reader.GetOrdinal("StatusId"));

                string status = "";
                if (statusid == 1)
                    status = "Thành công";
                else if (statusid == 2)
                    status = "Thất bại";
                else if (statusid == 3)
                    status = "Bị hủy";
                else if (statusid == 4)
                    status = "Chờ tài xế";
                else if (statusid == 5)
                    status = "Đang chuẩn bị món";
                else if (statusid == 6)
                    status = "Đang giao";


                var _order = new Order {Id=id, BranchName=branchName, CustomerName=customerName, CustomerPhone=customerPhone, Total=price, Status=status };
                _subItemsOrderDelivered.Add(_order);
            }

            contentDeliveredOrdersDataGrid.ItemsSource = _subItemsOrderDelivered;

        }

        private void MainWindowDriverLoaded(object sender, RoutedEventArgs e)
        {
            btnSelectOrders.IsChecked = true;
            contentSelectOrders.Visibility = Visibility.Visible;

            driverName.Text = LoginWindow.Person.Fullname;

            btnSelectChecked(sender, e);
        }

        //Menu
        private void btnSelectChecked(object sender, RoutedEventArgs e)
        {
            contentRevenue.Visibility = Visibility.Collapsed;

            btnSelectOrders.IsChecked = true;
            contentSelectOrders.Visibility = Visibility.Visible;

            //Data handle orders list
            CurrentPageOrder = 1;

            string sqlQueryTotalOrder =
                """
                    SELECT COUNT(*)
                    FROM [Branch] INNER JOIN [Order] ON [Branch].BranchId = [Order].BranchId AND [Order].StatusId = 4
                    INNER JOIN [Customer] ON [Order].CustomerId = [Customer].CustomerId
                """;
            TotalItemsOrder = NumberOfItems(sqlQueryTotalOrder);

            int isDivisibleOrder = TotalItemsOrder % RowsPerPage;
            if (isDivisibleOrder != 0)
            {
                TotalPagesOrder = TotalItemsOrder / RowsPerPage + 1;
            }
            else
            {
                TotalPagesOrder = TotalItemsOrder / RowsPerPage;
            }

            ordersListCurrentPage.Text = CurrentPageOrder.ToString();
            ordersListTotalPage.Text = TotalPagesOrder.ToString();
            SelectSubItemsOrder();
        }

        private void btnRevenueChecked(object sender, RoutedEventArgs e)
        {
            //Diver id
            int driverId = LoginWindow.Person.Id;

            contentSelectOrders.Visibility=Visibility.Collapsed;

            btnRevenue.IsChecked = true;
            contentRevenue.Visibility = Visibility.Visible;

            //Data handle orders list
            CurrentPageOrderDelivered = 1;

            string sqlQueryTotalOrderDelivered =
                $"""
                    SELECT COUNT(*)
                    FROM [Order] INNER JOIN [Branch] ON [Order].BranchId = [Branch].BranchId AND [Order].DriverId = {driverId} AND ([Order].StatusId = 1 OR [Order].StatusId = 6)
                			 INNER JOIN [Customer] ON [Order].CustomerId = [Customer].CustomerId
                """;
            TotalItemsOrderDelivered = NumberOfItems(sqlQueryTotalOrderDelivered);

            int isDivisibleOrderDelivered = TotalItemsOrderDelivered % RowsPerPage;
            if (isDivisibleOrderDelivered != 0)
            {
                TotalPagesOrderDelivered = TotalItemsOrderDelivered / RowsPerPage + 1;
            }
            else
            {
                TotalPagesOrderDelivered = TotalItemsOrderDelivered / RowsPerPage;
            }

            deliveredCurrentPage.Text = CurrentPageOrderDelivered.ToString();
            deliveredTotalPage.Text = TotalPagesOrderDelivered.ToString();
            SelectSubItemsOrderDelivered();
        }

        private void logoutButton(object sender, MouseButtonEventArgs e)
        {
            var screen = new StartWindow();
            this.Close();
            screen.Show();
        }

//Select Orders================================================================================
        //Page Navigation
        private void contentSelectOrdersPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageOrder <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItemsOrder.Clear();
                CurrentPageOrder--;
                ordersListCurrentPage.Text = CurrentPageOrder.ToString();
                SelectSubItemsOrder();
            }
        }
        private void contentSelectOrdersNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageOrder >= TotalPagesOrder)
            {
                //Do nothing
            }
            else
            {
                _subItemsOrder.Clear();
                CurrentPageOrder++;
                ordersListCurrentPage.Text = CurrentPageOrder.ToString();
                SelectSubItemsOrder();
            }
        }

        private void selectOrder_Click(object sender, RoutedEventArgs e)
        {
            if(selectOrderID.Text == "")
            {
                //Do nothing
            }
            else
            {
                //Driver id
                int driverId = LoginWindow.Person.Id;

                //Order id
                int orderId = int.Parse(selectOrderID.Text);

                //Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                string sp_selectOrder = "sp_UpdateOrder_Driver";
                var command = new SqlCommand(sp_selectOrder, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@DriverId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = driverId
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@OrderId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = orderId,
                });

                int isSuccess = command.ExecuteNonQuery();
                if (isSuccess == 1)
                {
                    MessageBox.Show("Selected");
                    selectOrderID.Text = "";
                }
                else
                {
                    MessageBox.Show("Can not select");
                    selectOrderID.Text = "";
                }

            }

        }


//Revenue=====================================================================================
        private void contentRevenuePreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageOrderDelivered <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItemsOrderDelivered.Clear();
                CurrentPageOrderDelivered--;
                deliveredCurrentPage.Text = CurrentPageOrderDelivered.ToString();
                SelectSubItemsOrderDelivered();
            }
        }

        private void contentRevenueNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageOrderDelivered >= TotalPagesOrderDelivered)
            {
                //Do nothing
            }
            else
            {
                _subItemsOrderDelivered.Clear();
                CurrentPageOrderDelivered++;
                deliveredCurrentPage.Text = CurrentPageOrderDelivered.ToString();
                SelectSubItemsOrderDelivered();
            }
        }

        //Staticstics
        private void driverSeeStatisticsDay_Click(object sender, RoutedEventArgs e)
        {
            //DriverId
            int driverId = LoginWindow.Person.Id;

            //Date Picker
            int day = driverStatisticsDayDatePicker.SelectedDate.Value.Day;
            int month = driverStatisticsDayDatePicker.SelectedDate.Value.Month;
            int year = driverStatisticsDayDatePicker.SelectedDate.Value.Year;

            //Connection to database
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            //Amount of orders
            string sp_amountOfOrdersDay = "sp_CalculateSumOrdersInDay_Driver";
            var command = new SqlCommand(sp_amountOfOrdersDay, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = driverId
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

            var amountOfOrdersDay = command.Parameters.Add("@Return", SqlDbType.Int);
            amountOfOrdersDay.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultPerDay = amountOfOrdersDay.Value;
            tbl_amountOfOrdersDay.Text = resultPerDay.ToString();

            //Revenue Day
            string sp_revenueDay = "sp_CalculateDriverTotalRevenueInDay_Driver";
            command = new SqlCommand(sp_revenueDay, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = driverId
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

            var revenueDay = command.Parameters.Add("@Return", SqlDbType.Int);
            revenueDay.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultRevenueDay = revenueDay.Value;
            tbl_revenueDay.Text = resultRevenueDay.ToString();

        }

        private void driverSeeStatisticsMonth_Click(object sender, RoutedEventArgs e)
        {
            // DriverId
            int driverId = LoginWindow.Person.Id;

            //Date Picker
            int day = driverStatisticsMonthDatePicker.SelectedDate.Value.Day;
            int month = driverStatisticsMonthDatePicker.SelectedDate.Value.Month;
            int year = driverStatisticsMonthDatePicker.SelectedDate.Value.Year;

            //Connection to database
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            //Amount of orders
            string sp_amountOfOrdersMonth = "sp_CalculateSumOrdersInMonth_Driver";
            var command = new SqlCommand(sp_amountOfOrdersMonth, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = driverId
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

            var amountOfOrdersMonth = command.Parameters.Add("@Return", SqlDbType.Int);
            amountOfOrdersMonth.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultOrderPerMonth = amountOfOrdersMonth.Value;
            tbl_amountOfOrdersMonth.Text = resultOrderPerMonth.ToString();

            //Revenue
            string sp_revenueMonth = "sp_CalculateDriverTotalRevenueInMonth_Driver";
            command = new SqlCommand(sp_revenueMonth, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@DriverId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = driverId
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

            var revenueMonth = command.Parameters.Add("@Return", SqlDbType.Int);
            revenueMonth.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultRevenueMonth = revenueMonth.Value;
            tbl_revenueMonth.Text = resultRevenueMonth.ToString();


        }
    }
}
