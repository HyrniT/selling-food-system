using OnlineSellingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using MessageBox = System.Windows.Forms.MessageBox;
using Window = System.Windows.Window;

namespace OnlineSellingSystem.View
{
    /// <summary>
    /// Interaction logic for MainWindowPartner.xaml
    /// </summary>
    public partial class MainWindowPartner : Window, INotifyPropertyChanged
    {
        public MainWindowPartner()
        {
            InitializeComponent();

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        //Connect Database
        SqlConnection _connection = null;

        private void ConnectDatabase()
        {
            _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes;MultipleActiveResultSets=true");
            _connection.Open();
        }

        //Function
        private int GetBranchIdFromDatabase(int partnerId)
        {
            int branchId = 0;

            string sql =
                $"""
                        SELECT [Branch].BranchId
                	    FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                				   INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                        WHERE [Partner].PartnerId = {partnerId}
                """;
            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                branchId = reader.GetInt32(0);
            }

            _connection.Close();

            return branchId;
        }

        private string GetBranchNameFromDatabase(int partnerId)
        {
            string branchName = string.Empty;

            ConnectDatabase();

            string sql =
                $"""
                        SELECT [Branch].BranchName
                        FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                			   INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                        WHERE [Partner].PartnerId = {partnerId}
                """;


            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();
            while(reader.Read())
            {
                branchName = reader.GetString(0);
            }

            //string sp_getBranchName = "sp_BranchName_Partner";
            //var command = new SqlCommand(sp_getBranchName, _connection);
            //command.CommandType = System.Data.CommandType.StoredProcedure;
            //command.Parameters.Add(new SqlParameter()
            //{
            //    ParameterName = "@PartnerId",
            //    SqlDbType = System.Data.SqlDbType.Int,
            //    Value = partnerId,
            //});

            //var returnBranchName = command.Parameters.Add("@BranchName", SqlDbType.NChar);
            //returnBranchName.Direction = ParameterDirection.ReturnValue;

            //command.ExecuteNonQuery();

            //var result = returnBranchName.Value;
            //branchName = result.ToString();

            return branchName;
        }

        private int RegisteredTime(int partnerId)
        {
            int registeredTime = 0;

            ConnectDatabase();

            string sql =
                $"""
                    SELECT CONVERT(INT, DATEDIFF(DAY, [Partner].PartnerRegisterTime, GETDATE()))
                    FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                    WHERE [Partner].PartnerId = {partnerId}
                """;
            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();
            while( reader.Read())
            {
                registeredTime = reader.GetInt32(0);
            }

            return registeredTime;
        }

        private int GetIntStatus(int partnerId)
        {
            int status = -1;

            ConnectDatabase();

            string sql =
                $"""
                        SELECT [Branch].StatusId
                        FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                			   INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                		WHERE [Partner].PartnerId = {partnerId};
                """;

            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                status = reader.GetInt32(0);
            }

            return status;
        }

        private string GetOpenTime(int partnerId)
        {
            string openTime = "";

            ConnectDatabase();

            string sql =
                $"""
                        SELECT CONVERT(VARCHAR(8), [Branch].BranchOpenTime, 108)
                        FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                				INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                        WHERE [Partner].PartnerId = {partnerId}
                """;
            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                openTime = reader.GetString(0);
            }

            return openTime;
        }

        private string GetCloseTime(int partnerId)
        {
            string closeTime = "";

            ConnectDatabase();

            string sql =
                $"""
                        SELECT CONVERT(VARCHAR(8), [Branch].BranchCloseTime, 108)
                        FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = [Contract].PartnerId
                				INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                        WHERE [Partner].PartnerId = {partnerId}
                """;
            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                closeTime = reader.GetString(0);
            }

            return closeTime;
        }

        private void UpdateBranchStatus(int branchId, int statusId)
        {
            ConnectDatabase();
            string sp_updateStatus = "sp_UpdateStatus_Branch";
            var command = new SqlCommand(sp_updateStatus, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId,
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@StatusId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = statusId,
            });

            command.ExecuteNonQuery();
        }

        private int NumberOfItems(string sqlQueryTotalItems)
        {
            //Connect Database
            ConnectDatabase();

            //Count number of person: admin/employee/customer/driver/partner
            var command = new SqlCommand(sqlQueryTotalItems, _connection);
            int count = (int)command.ExecuteScalar();

            return count;
        }

        private void SubItemsProduct()
        {
            int partnerId = LoginWindow.Person.Id;

            //Connect Database
            ConnectDatabase();

            int offset = (CurrentPageProduct - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelect =
                $"""
                    SELECT [Product].ProductId AS ProductId, [Product].ProductName AS ProductName, [Product].ProductPrice AS ProductPrice, [Product].ProductDescription AS ProductDescription, [Product].StatusId AS StatusId
                    FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = {partnerId} AND [Partner].PartnerId = [Contract].PartnerId
                				INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                				INNER JOIN [BranchProductType] ON [Branch].BranchId = [BranchProductType].BranchId
                				INNER JOIN [Product] ON [BranchProductType].BranchProductTypeId = [Product].BranchProductTypeId
                    ORDER BY ProductId OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY
                """;
            var command = new SqlCommand(sqlSelect, _connection);
            var reader = command.ExecuteReader();

            _subItemsProduct.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                string name = reader.GetString(reader.GetOrdinal("ProductName"));
                SqlMoney price = reader.GetSqlMoney(reader.GetOrdinal("ProductPrice"));
                string desc = reader.GetString(reader.GetOrdinal("ProductDescription"));
                int status = reader.GetInt32(reader.GetOrdinal("StatusId"));

                string str_status = "";
                if (status == -1)
                    str_status = "Lỗi";
                else if (status == 3)
                    str_status = "Hết hàng";
                else if (status == 4)
                    str_status = "Ngưng phục vụ";
                else if (status == 6)
                    str_status = "Còn hàng";

                var _product = new Product {Id=id, Name=name, Price=price, Description=desc, Status = str_status};
                _subItemsProduct.Add(_product);
            }

            contentMenuDataGridProduct.ItemsSource = _subItemsProduct;
        }

        private void SubItemsApprovedOrder()
        {
            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);

            //Connect Database
            ConnectDatabase();

            int offset = (CurrentPageApprovedOrder - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelect =
                $"""
                    SELECT [Order].OrderId AS OrderId, [Order].OrderDate AS OrderDate, [Order].OrderFinalPrice AS Price, [Customer].CustomerName AS CustomerName, [Driver].DriverName AS DriverName, [Order].StatusId AS StatusId
                    FROM [Order] INNER JOIN [Customer] ON [Order].CustomerId = [Customer].CustomerId AND [Order].BranchId = {branchId} AND [Order].StatusId <> -1 AND [Order].StatusId <> 0
                    INNER JOIN [Driver] ON [Driver].DriverId = [Order].DriverId
                """;
            var command = new SqlCommand(sqlSelect, _connection);
            var reader = command.ExecuteReader();

            _subItemsApprovedOrder.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("OrderId"));
                SqlDateTime date = reader.GetDateTime(reader.GetOrdinal("OrderDate"));
                SqlMoney total = reader.GetSqlMoney(reader.GetOrdinal("Price"));
                string customerName = reader.GetString(reader.GetOrdinal("CustomerName"));
                string driverName = reader.GetString(reader.GetOrdinal("DriverName"));
                int status = reader.GetInt32(reader.GetOrdinal("StatusId"));

                string str_status = "";
                if (status == -1)
                    str_status = "Lỗi";
                else if (status == 0)
                    str_status = "Chờ tiếp nhận";
                else if (status == 1)
                    str_status = "Thành công";
                else if (status == 2)
                    str_status = "Thất bại";
                else if (status == 3)
                    str_status = "Bị hủy";
                else if (status == 4)
                    str_status = "Chờ tài xế";
                else if (status == 5)
                    str_status = "Đang chuẩn bị món";
                else if (status == 6)
                    str_status = "Đang giao";

                var _order = new Order() { Id = id, Date = date, CustomerName = customerName, DriverName = driverName, Total = total, Status = str_status };
                _subItemsApprovedOrder.Add(_order);                
            }

            contentOrdersListView.ItemsSource = _subItemsApprovedOrder;
        }

        private void SubItemsUnapprovedOrder()
        {
            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);

            //Connect Database
            ConnectDatabase();

            int offset = (CurrentPageApprovedOrder - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelect =
                $"""
                    SELECT [Order].OrderId AS OrderId, [Order].OrderDate AS OrderDate, [Order].OrderFinalPrice AS Price, [Customer].CustomerName AS CustomerName, [Customer].CustomerPhone AS CustomerPhone
                    FROM [Order] INNER JOIN [Customer] ON [Order].CustomerId = Customer.CustomerId AND [Order].BranchId = {branchId} AND [Order].StatusId = 0
                """;
            var command = new SqlCommand(sqlSelect, _connection);
            var reader = command.ExecuteReader();

            _subItemsUnapprovedOrder.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("OrderId"));
                SqlDateTime date = reader.GetDateTime(reader.GetOrdinal("OrderDate"));
                SqlMoney total = reader.GetSqlMoney(reader.GetOrdinal("Price"));
                string customerName = reader.GetString(reader.GetOrdinal("CustomerName"));
                string customerPhone = reader.GetString(reader.GetOrdinal("CustomerPhone"));

                var _order = new Order() { Id = id, Date = date, CustomerName = customerName,CustomerPhone =customerName, Total = total };
                _subItemsUnapprovedOrder.Add(_order);
            }

            contentUnapprovedOrdersListView.ItemsSource = _subItemsUnapprovedOrder;
        }

        //Window Loaded
        private void MainWindowPartnerLoaded(object sender, RoutedEventArgs e)
        {
            contentMenus.Visibility = Visibility.Collapsed;
            contentOrders.Visibility = Visibility.Collapsed;
            contentStatistics.Visibility = Visibility.Collapsed;
            contentWallet.Visibility = Visibility.Collapsed;

            btnShopManagement.IsChecked = true;
            contentShopManagement.Visibility = Visibility.Visible;

            shopOwnerName.Text = LoginWindow.Person.Fullname;

            btnShopManagementChecked(sender, e);


        }
        private void logoutButton(object sender, MouseButtonEventArgs e)
        {
            //log out
            var screen = new StartWindow();
            this.Close();
            screen.Show();
        }

        //Switch Content
        private void btnShopManagementChecked(object sender, RoutedEventArgs e)
        {
            contentMenus.Visibility = Visibility.Collapsed;
            contentOrders.Visibility = Visibility.Collapsed;
            contentStatistics.Visibility = Visibility.Collapsed;
            contentWallet.Visibility = Visibility.Collapsed;

            btnShopManagement.IsChecked = true;
            contentShopManagement.Visibility = Visibility.Visible;

            //PartnerId
            int partnerId = LoginWindow.Person.Id;

            //Shop name
            string branchName = GetBranchNameFromDatabase(partnerId);
            shopName.Text = branchName;
            
            //Can Edit Name?
            int registeredTime = RegisteredTime(partnerId);
            if(registeredTime <= 30)
            {
                contentShopNameEdit.Visibility = Visibility.Visible;
            }
            else
            {
                contentShopNameEdit.Visibility = Visibility.Collapsed;
            }

            //Shop status
            int statusId = GetIntStatus(partnerId);
            if (statusId == 6)
                contentShopStatusNormal.IsChecked = true;
            else if(statusId == 3)
                contentShopStatusStop.IsChecked = true;
            else if(statusId == 4)
                contentShopStatusStopOrder.IsChecked = true;

            //Opentime
            string opentTime = GetOpenTime(partnerId);
            placeholderOpenTime.Text = opentTime;

            //CloseTime
            string closeTime = GetCloseTime(partnerId);
            placeholderCloseTime.Text = closeTime;
        }

        //Menu Loaded
        ObservableCollection<Product> _subItemsProduct = new ObservableCollection<Product>();
        ObservableCollection<Product> _ItemsProductOfBranch = new ObservableCollection<Product>();
        ObservableCollection<Order> _subItemsApprovedOrder = new ObservableCollection<Order>();
        ObservableCollection<Order> _subItemsUnapprovedOrder = new ObservableCollection<Order>();

        public int RowsPerPage { get; set; } = 30;

        public int TotalPagesProduct { get; set; } = 0;
        public int CurrentPageProduct { get; set; } = 1;
        public int TotalItemsProduct { get; set; } = 0;

        public int TotalPagesApprovedOrder { get; set; } = 0;
        public int CurrentPageApprovedOrder { get; set; } = 1;
        public int TotalItemsApprovedOrder { get; set; } = 0;
        
        public int TotalPagesUnapprovedOrder { get; set; } = 0;
        public int CurrentPageUnapprovedOrder { get; set; } = 1;
        public int TotalItemsUnapprovedOrder { get; set; } = 1;

        private void btnMenuChecked(object sender, RoutedEventArgs e)
        {
            contentShopManagement.Visibility = Visibility.Collapsed;
            contentOrders.Visibility = Visibility.Collapsed;
            contentStatistics.Visibility = Visibility.Collapsed;
            contentWallet.Visibility = Visibility.Collapsed;

            btnMenu.IsChecked = true;
            contentMenus.Visibility = Visibility.Visible;

            int partnerId = LoginWindow.Person.Id;

            string sqlTotalProduct =
                $"""
                    SELECT COUNT(*)
                    FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = {partnerId} AND [Partner].PartnerId = [Contract].PartnerId
                				INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                				INNER JOIN [BranchProductType] ON [Branch].BranchId = [BranchProductType].BranchId
                				INNER JOIN [Product] ON [BranchProductType].BranchProductTypeId = [Product].BranchProductTypeId
                """;
            TotalItemsProduct = NumberOfItems(sqlTotalProduct);

            int isDivisibleProduct = TotalItemsProduct % RowsPerPage;
            if (isDivisibleProduct != 0)
            {
                TotalPagesProduct = TotalItemsProduct / RowsPerPage + 1;
            }
            else
            {
                TotalPagesProduct = TotalItemsProduct / RowsPerPage;
            }

            productCurrentPage.Text = CurrentPageProduct.ToString();
            productTotalPage.Text = TotalPagesProduct.ToString();

            SubItemsProduct();
        }

        private void btnOrdersChecked(object sender, RoutedEventArgs e)
        {
            contentShopManagement.Visibility = Visibility.Collapsed;
            contentMenus.Visibility = Visibility.Collapsed;
            contentStatistics.Visibility = Visibility.Collapsed;
            contentWallet.Visibility = Visibility.Collapsed;

            btnOrders.IsChecked = true;
            contentOrders.Visibility = Visibility.Visible;

            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);

            //Connect Database
            ConnectDatabase();

            string sqlTotalApprovedOrder =
                $"""
                    SELECT COUNT(*)
                    FROM [Order] INNER JOIN [Customer] ON [Order].CustomerId = [Customer].CustomerId AND [Order].BranchId = {branchId} AND [Order].StatusId <> -1 AND [Order].StatusId <> 0
                    INNER JOIN [Driver] ON [Driver].DriverId = [Order].DriverId
                """;
            TotalItemsApprovedOrder = NumberOfItems(sqlTotalApprovedOrder);

            int isDivisibleApprovedOrder = TotalItemsApprovedOrder % RowsPerPage;
            if (isDivisibleApprovedOrder != 0)
            {
                TotalPagesApprovedOrder = TotalItemsApprovedOrder / RowsPerPage + 1;
            }
            else
            {
                TotalPagesApprovedOrder = TotalItemsApprovedOrder / RowsPerPage;
            }

            approvedOrdersCurrentPage.Text = CurrentPageApprovedOrder.ToString();
            approvedOrdersTotalPage.Text = TotalPagesApprovedOrder.ToString();

            SubItemsApprovedOrder();

            //List unapproved order
            string sqlTotalUnapprovedOrder =
                $"""
                    SELECT COUNT(*)
                    FROM [Order] INNER JOIN [Customer] ON [Order].CustomerId = Customer.CustomerId AND [Order].BranchId = {branchId} AND [Order].StatusId = 0
                """;
            TotalItemsUnapprovedOrder = NumberOfItems(sqlTotalUnapprovedOrder);

            int isDivisibleUnapprovedOrder = TotalItemsUnapprovedOrder % RowsPerPage;
            if (isDivisibleUnapprovedOrder != 0)
            {
                TotalPagesUnapprovedOrder = TotalItemsUnapprovedOrder / RowsPerPage + 1;
            }
            else
            {
                TotalPagesUnapprovedOrder = TotalItemsUnapprovedOrder / RowsPerPage;
            }

            unapprovedOrdersListCurrentPage.Text = CurrentPageUnapprovedOrder.ToString();
            unapprovedOrdersListTotalPage.Text = TotalPagesUnapprovedOrder.ToString();

            SubItemsUnapprovedOrder();
        }

        private void btnStatisticsChecked(object sender, RoutedEventArgs e)
        {
            contentShopManagement.Visibility = Visibility.Collapsed;
            contentMenus.Visibility = Visibility.Collapsed;
            contentOrders.Visibility = Visibility.Collapsed;
            contentWallet.Visibility = Visibility.Collapsed;

            btnStatistics.IsChecked = true;
            contentStatistics.Visibility = Visibility.Visible;

            int partnerID = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerID);

            //Connect Datebase
            ConnectDatabase();

            string sql =
                $"""
                    SELECT [Product].ProductId AS ProductId, [Product].ProductName AS ProductName, [Product].ProductPrice AS ProductPrice, [Product].ProductDescription AS ProductDescription, [Product].StatusId AS StatusId, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                    FROM [Branch] INNER JOIN [BranchProductType] ON [Branch].BranchId = {branchId} AND [Branch].BranchId = [BranchProductType].BranchId
                				INNER JOIN [Product] ON [BranchProductType].BranchProductTypeId = [Product].BranchProductTypeId
                				INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                    GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice, [Product].ProductDescription, [Product].StatusId
                """;

            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                string name = reader.GetString(reader.GetOrdinal("ProductName"));
                string desc = reader.GetString(reader.GetOrdinal("ProductDescription"));
                SqlMoney price = reader.GetSqlMoney(reader.GetOrdinal("ProductPrice"));
                int statusId = reader.GetInt32(reader.GetOrdinal("StatusId"));
                int sold = reader.GetInt32(reader.GetOrdinal("ProductSold"));

                string str_status = "";
                if (statusId == 3)
                    str_status = "Hết hàng";
                else if (statusId == 4)
                    str_status = "Ngưng phục vụ";
                else if (statusId == 6)
                    str_status = "Còn hàng";

                var _product = new Product { Id = id, Name = name, Description = desc, Price = price, Sold = sold, Status = str_status };
                _ItemsProductOfBranch.Add(_product);
            }

            topSalesDataGridProduct.ItemsSource = _ItemsProductOfBranch;
        }

        private void btnWalletChecked(object sender, RoutedEventArgs e)
        {
            contentShopManagement.Visibility = Visibility.Collapsed;
            contentMenus.Visibility = Visibility.Collapsed;
            contentStatistics.Visibility = Visibility.Collapsed;
            contentOrders.Visibility = Visibility.Collapsed;

            btnWallet.IsChecked = true;
            contentWallet.Visibility = Visibility.Visible;

            int partnerId = LoginWindow.Person.Id;

            //Connect database
            ConnectDatabase();
            string sp_getDeposite = "sp_getDepositeOfPartner";
            var command = new SqlCommand(sp_getDeposite, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = partnerId
            });

            var returnDeposite = command.Parameters.Add("@result", SqlDbType.Money);
            returnDeposite.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultDeposite = returnDeposite.Value;
            deposite.Text = resultDeposite.ToString();
        }

        private void contentShopnameEdit(object sender, MouseButtonEventArgs e)
        {
            contentEditShopName.Visibility = Visibility.Visible;
        }

//==================================Content Shop Management Begin====================================
    

        //update shop name
        private void contentShopEditShopNameOkButton(object sender, RoutedEventArgs e)
        {
            int partnerId = LoginWindow.Person.Id;
            string newName = newShopName.Text;

            ConnectDatabase();
            string sp_updateBranchName = "sp_UpdateBranchName_Branch";
            var command = new SqlCommand(sp_updateBranchName, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = partnerId,
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchName",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = newName,
            });

            int isSuccess = command.ExecuteNonQuery();
            if(isSuccess == 1)
            {
                MessageBox.Show("Updated Branch Name");
                contentEditShopName.Visibility = Visibility.Collapsed;
                shopName.Text = GetBranchNameFromDatabase(partnerId);
            }
            else
            {
                MessageBox.Show("Can not Edit Branch Name");
                contentEditShopName.Visibility = Visibility.Collapsed;
            }

        }

        //update shop status
        private void contentShopStatusNormalClick(object sender, RoutedEventArgs e)
        {
            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);
            int statusId = 6;

            UpdateBranchStatus(branchId, statusId);
        }

        //update shop status
        private void contentShopStatusStopOrderClick(object sender, RoutedEventArgs e)
        {
            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);
            int statusId = 4;

            UpdateBranchStatus(branchId, statusId);
        }

        //update shop status
        private void contentShopStatusStopClick(object sender, RoutedEventArgs e)
        {
            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);
            int statusId = 3;

            UpdateBranchStatus(branchId, statusId);
        }

        private void updateOpenTime(object sender, RoutedEventArgs e)
        {
            if(openTime.Text == "")
            {
                //Do nothing
            }
            else
            {
                string newOpenTime = openTime.Text;
                int partnerId = LoginWindow.Person.Id;
                int branchId = GetBranchIdFromDatabase(partnerId);

                ConnectDatabase();
                string sp_updateOpenTime = "sp_UpdateBranchOpenTime_Branch";
                var command = new SqlCommand(sp_updateOpenTime, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@BranchId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = branchId,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@BranchOpenTime",
                    SqlDbType = System.Data.SqlDbType.Time,
                    Value = newOpenTime,
                });

                int isSuccess = command.ExecuteNonQuery();
                if (isSuccess == 1)
                {
                    MessageBox.Show("Updated Open Time");
                    openTime.Text = "";
                    placeholderOpenTime.Text = GetOpenTime(partnerId);
                }
                else
                {
                    MessageBox.Show("Error");
                    openTime.Text = "";
                    placeholderOpenTime.Text = GetOpenTime(partnerId);
                }
            }
        }

        private void updateCloseTime(object sender, RoutedEventArgs e)
        {
            if(closeTime.Text == "")
            {
                //Do nothing
            }
            else
            {
                string newCloseTime = closeTime.Text;
                int partnerId = LoginWindow.Person.Id;
                int branchId = GetBranchIdFromDatabase(partnerId);

                ConnectDatabase();
                string sp_updateCloseTime = "sp_UpdateBranchCloseTime_Branch";
                var command = new SqlCommand(sp_updateCloseTime, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@BranchId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = branchId,
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@BranchCloseTime",
                    SqlDbType = System.Data.SqlDbType.Time,
                    Value = newCloseTime,
                });

                int isSuccess = command.ExecuteNonQuery();
                if (isSuccess == 1)
                {
                    MessageBox.Show("Updated Close Time");
                    closeTime.Text = "";
                    placeholderCloseTime.Text = GetCloseTime(partnerId);
                }
                else
                {
                    MessageBox.Show("Error");
                    closeTime.Text = "";
                    placeholderCloseTime.Text = GetOpenTime(partnerId);
                }
            }
        }

        private void showRenewContract(object sender, MouseButtonEventArgs e)
        {
            renewContractView.Visibility = Visibility.Visible;
        }


        private void renewContractButton(object sender, RoutedEventArgs e)
        {

        }


//================================Content Menu Begin=========================================
        private void addProductButton_Click(object sender, RoutedEventArgs e)
        {
            contentAddProduct.Visibility = Visibility.Visible;
            contentUpdateProduct.Visibility = Visibility.Collapsed;
            contentRemoveProduct.Visibility = Visibility.Collapsed;

            ObservableCollection<ProductType> _productTypes = new ObservableCollection<ProductType>();
            _productTypes.Add(new ProductType() { Id = 1, Name = "Cơm" });
            _productTypes.Add(new ProductType() { Id = 2, Name = "Bún" });
            _productTypes.Add(new ProductType() { Id = 3, Name = "Bánh mì" });
            _productTypes.Add(new ProductType() { Id = 4, Name = "Thức ăn nhanh" });
            _productTypes.Add(new ProductType() { Id = 5, Name = "Món nướng" });
            _productTypes.Add(new ProductType() { Id = 6, Name = "Món chiên" });
            _productTypes.Add(new ProductType() { Id = 7, Name = "Món chay" });
            _productTypes.Add(new ProductType() { Id = 8, Name = "Trà sữa" });
            _productTypes.Add(new ProductType() { Id = 9, Name = "Cafe" });
            _productTypes.Add(new ProductType() { Id = 10, Name = "Sinh tố" });
            _productTypes.Add(new ProductType() { Id = 11, Name = "Đá xay" });
            _productTypes.Add(new ProductType() { Id = 12, Name = "Nước ép hoa quả" });
            _productTypes.Add(new ProductType() { Id = 13, Name = "Kem" });
            _productTypes.Add(new ProductType() { Id = 14, Name = "Yogurt" });

            comboBoxProductType.ItemsSource = _productTypes;

        }

        private void removeProductButton_Click(object sender, RoutedEventArgs e)
        {
            contentAddProduct.Visibility = Visibility.Collapsed;
            contentUpdateProduct.Visibility = Visibility.Collapsed;
            contentRemoveProduct.Visibility = Visibility.Visible;
        }

        private void updateProductButton_Click(object sender, RoutedEventArgs e)
        {
            contentAddProduct.Visibility = Visibility.Collapsed;
            contentUpdateProduct.Visibility = Visibility.Visible;
            contentRemoveProduct.Visibility = Visibility.Collapsed;
        }

        private void addProductDoneButton_Click(object sender, RoutedEventArgs e)
        {
            //Branch Id
            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);

            //Parameters
            int id = 1;
            id = comboBoxProductType.SelectedIndex + 1;
            string name = addProductName.Text;
            string desc = addProductDescription.Text;
            string price = addProductPrice.Text;

            //Connect DB
            ConnectDatabase();

            //Proc Add
            string sp_AddProduct = "sp_InsertProduct_Branch";
            var command = new SqlCommand(sp_AddProduct, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductTypeId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = id
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductName",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = name
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductDescription",
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Value = desc
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductPrice",
                SqlDbType = System.Data.SqlDbType.Money,
                Value = price
            });

            int isSuccess = command.ExecuteNonQuery();
            if (isSuccess == 1)
            {
                MessageBox.Show("Added A Product");

                addProductDescription.Text = "";
                addProductName.Text = "";
                addProductPrice.Text = "";

                SubItemsProduct();
            }
            else
            {
                MessageBox.Show("Error");

                addProductDescription.Text = "";
                addProductName.Text = "";
                addProductPrice.Text = "";
            }


        }

        private void updateProductSearchIdButton_Click(object sender, RoutedEventArgs e)
        {
            string productId = updateProductID.Text;

            //Connect Database
            ConnectDatabase();

            string sql =
                $"""
                    select ProductName, ProductDescription, ProductPrice, Status.StatusId AS Status
                    from Product, Status
                    where Product.ProductId = {productId} and Status.StatusId = Product.StatusId
                """;

            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            int status = 0;
            while (reader.Read())
            {
                placeholderUpdateProductName.Text = reader.GetString(reader.GetOrdinal("ProductName"));
                placeholderUpdateProductDesc.Text = reader.GetString(reader.GetOrdinal("ProductDescription"));
                placeholderUpdateProductPrice.Text = reader.GetSqlMoney(reader.GetOrdinal("ProductPrice")).ToString();
                status = reader.GetInt32(reader.GetOrdinal("Status"));
            }

            if (status == 3)
                productStatus.SelectedIndex = 0;
            else if (status == 4)
                productStatus.SelectedIndex = 1;
            else if (status == 6)
                productStatus.SelectedIndex = 2;
            
        }

        private void removeProductDoneButton_Click(object sender, RoutedEventArgs e)
        {
            string productId = removeProductId.Text;

            //Connect DB
            ConnectDatabase();

            //Proc Add
            string sp_RemoveProduct = "sp_DeleteProduct";

            var command = new SqlCommand(sp_RemoveProduct, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = productId
            });
            int isSuccess = command.ExecuteNonQuery();
            if (isSuccess == 1)
            {
                MessageBox.Show("Removed");

                removeProductId.Text = "";
                SubItemsProduct();
            }
            else
            {
                MessageBox.Show("Error");
                removeProductId.Text = "";
            }


        }

        private void contentProductUpdateDoneButton_Click(object sender, RoutedEventArgs e)
        {
            //Parameters
            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);

            string productId = updateProductID.Text;

            string name = placeholderUpdateProductName.Text;
            string desc = placeholderUpdateProductDesc.Text;
            string price = placeholderUpdateProductPrice.Text;

            int statusId = productStatus.SelectedIndex;
            if (statusId == 0)
                statusId = 3;
            else if (statusId == 1)
                statusId = 4;
            else if (statusId == 2)
                statusId = 6;


            if(updateProductName.Text != "")
                name = updateProductName.Text;
            if(updateProductDesc.Text != "")
                desc = updateProductDesc.Text;
            if(updateProductPrice.Text != "")
                price = updateProductPrice.Text;


            //Connect database
            ConnectDatabase();

            //Proc UPdate
            string sp_UpdateProduct = "sp_UpdateProduct_Branch";
            var command = new SqlCommand(sp_UpdateProduct, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = productId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductName",
                SqlDbType = System.Data.SqlDbType.NChar,
                Value = name
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductPrice",
                SqlDbType = System.Data.SqlDbType.Money,
                Value = price
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@ProductDescription",
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Value = desc
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@StatusId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = statusId
            });


            int isSuccess = command.ExecuteNonQuery();
            if (isSuccess == 1)
            {
                MessageBox.Show("Updated");

                updateProductSearchIdButton_Click(sender, e);

                updateProductName.Text = "";
                updateProductDesc.Text = "";
                updateProductPrice.Text = "";

                SubItemsProduct();
            }
            else
            {
                MessageBox.Show("Error");
                updateProductID.Text = "";
                updateProductName.Text = "";
                updateProductDesc.Text = "";
                updateProductPrice.Text = "";
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentPageProduct <= 1)
            {
                //DO nothing
            }
            else
            {
                _subItemsProduct.Clear();
                CurrentPageProduct--;
                productCurrentPage.Text = CurrentPageProduct.ToString();
                SubItemsProduct();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentPageProduct >= TotalPagesProduct)
            {
                //Do nothing
            }
            else
            {
                _subItemsProduct.Clear();
                CurrentPageProduct++;
                productCurrentPage.Text = CurrentPageProduct.ToString();
                SubItemsProduct();
            }
        }

//================================Content Orders Begin=========================================

        private void contentUnapprovedOrdersListPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageApprovedOrder <= 1)
            {
                //DO nothing
            }
            else
            {
                _subItemsApprovedOrder.Clear();
                CurrentPageApprovedOrder--;
                approvedOrdersCurrentPage.Text = CurrentPageApprovedOrder.ToString();
                SubItemsApprovedOrder();
            }
        }

        private void contentUnapprovedOrdersListNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageApprovedOrder >= TotalPagesApprovedOrder)
            {
                //DO nothing
            }
            else
            {
                _subItemsApprovedOrder.Clear();
                CurrentPageApprovedOrder++;
                approvedOrdersCurrentPage.Text = CurrentPageApprovedOrder.ToString();
                SubItemsApprovedOrder();
            }
        }

        private void contentApprovedOrdersListPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageUnapprovedOrder <= 1)
            {
                //DO nothing
            }
            else
            {
                _subItemsUnapprovedOrder.Clear();
                CurrentPageUnapprovedOrder--;
                unapprovedOrdersListCurrentPage.Text = CurrentPageUnapprovedOrder.ToString();
                SubItemsUnapprovedOrder();
            }
        }

        private void contentApprovedOrdersListNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageUnapprovedOrder >= TotalPagesUnapprovedOrder)
            {
                //DO nothing
            }
            else
            {
                _subItemsUnapprovedOrder.Clear();
                CurrentPageUnapprovedOrder++;
                unapprovedOrdersListTotalPage.Text = CurrentPageUnapprovedOrder.ToString();
                SubItemsUnapprovedOrder();
            }
        }

        private void acceptOrder_Click(object sender, RoutedEventArgs e)
        {
            int partnerId = LoginWindow.Person.Id;
            int brachId = GetBranchIdFromDatabase(partnerId);

            if(acceptOrderId.Text != "")
            {
                int orderId = int.Parse(acceptOrderId.Text);

                //Connect database
                ConnectDatabase();

                string sp_acceptOrder = "sp_AcceptOrder_Branch";
                var command = new SqlCommand(sp_acceptOrder, _connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@BranchId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = brachId
                });
                command.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@OrderId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Value = orderId
                });

                int isSuccess = command.ExecuteNonQuery();
                if (isSuccess == 1)
                {
                    MessageBox.Show("Accepted");
                    acceptOrderId.Text = "";
                    SubItemsApprovedOrder();
                    SubItemsUnapprovedOrder();
                }
                else
                {
                    MessageBox.Show("Error");
                    acceptOrderId.Text = "";
                }

            }
            else
            {
                //Do nothing
            }
        }


//================================Content Statistics Begin=========================================

        private bool isLeapYear(int y)
        {
            return ((y % 4 == 0 && y % 100 != 0) || y % 400 == 0);
        }

        private int DaysInMonth(int m, int y)
        {
            int days = 31;

            if (m == 2 || m == 4 || m == 6 || m == 9 || m==11)
                days = 30;

            if(m == 2)
            {
                if (isLeapYear(y))
                    days = 29;
                else
                    days= 28;
            }
            
            return days;
        }

        private List<int> DateAfterSevenDay(int day, int month, int year)
        {
            List<int> result = new List<int>();

            int dayInmonth = DaysInMonth(month, year);

            int newDay = day + 7;
            int newMonth = month;
            int newYear = year;

            if (newDay > dayInmonth)
            {
                newDay -= dayInmonth;
                newMonth++;

                if (newMonth == 1)
                    newYear++;
            }

            result.Add(newDay);
            result.Add(newMonth);
            result.Add(newYear);

            return result;
        }

        private void partnerSeeStaticstics_Click(object sender, RoutedEventArgs e)
        {
            int partnerId = LoginWindow.Person.Id;
            int branchId = GetBranchIdFromDatabase(partnerId);

            int day = staticsticsDatePicker.SelectedDate.Value.Day;
            int month = staticsticsDatePicker.SelectedDate.Value.Month;
            int year = staticsticsDatePicker.SelectedDate.Value.Year;

            //Connect Database
            ConnectDatabase();

            //Revenue Day
            string sp_RevenueDay = "sp_CalculateBranchTotalRevenueInDay_Branch";
            var command = new SqlCommand(sp_RevenueDay, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Day",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = day
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Month",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = month
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Year",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = year
            });

            var returnReveneDay = command.Parameters.Add("@Total", SqlDbType.Money);
            returnReveneDay.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultRevenueDay = returnReveneDay.Value;
            tbl_revenueDay.Text = resultRevenueDay.ToString();


            //Revenue Week
            List<int>  NewDate = new List<int>();
            NewDate = DateAfterSevenDay(day, month, year);

            int newDay = NewDate[0];
            int newMonth = NewDate[1];
            int newYear = NewDate[2];

            string sp_RevenueWeek = "sp_CalculateBranchTotalRevenueInPeriod_Branch";
            command = new SqlCommand(sp_RevenueWeek, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@StartDay",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = day
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@StartMonth",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = month
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@StartYear",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = year
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@EndDay",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = newDay
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@EndMonth",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = newMonth
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@EndYear",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = newYear
            });

            var returnReveneWeek = command.Parameters.Add("@Total", SqlDbType.Money);
            returnReveneWeek.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultRevenueWeek = returnReveneWeek.Value;
            tbl_revenueWeek.Text = resultRevenueWeek.ToString();

            //Revenue Month
            string sp_RevenueMonth = "sp_RevenueOfBranchInMonth_Partner";
            command = new SqlCommand(sp_RevenueMonth, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Month",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = month
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Year",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = year
            });

            var returnReveneMonth = command.Parameters.Add("@result", SqlDbType.Money);
            returnReveneMonth.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultRevenueMonth = returnReveneMonth.Value;
            tbl_revenueMonth.Text = resultRevenueMonth.ToString();


            //Orders Day
            string sp_OrderDay = "sp_SalesOfBranchInDay_Partner";
            command = new SqlCommand(sp_OrderDay, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Day",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = day
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Month",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = month
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Year",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = year
            });

            var returnOrderDay = command.Parameters.Add("@result", SqlDbType.Money);
            returnOrderDay.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultOrderDay = returnOrderDay.Value;
            tbl_orderDay.Text = resultOrderDay.ToString();

            //Order Week
            string sp_OrderWeek = "sp_CalculateSumOrdersInPeriod_Branch";
            command = new SqlCommand(sp_OrderWeek, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@StartDay",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = day
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@StartMonth",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = month
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@StartYear",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = year
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@EndDay",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = newDay
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@EndMonth",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = newMonth
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@EndYear",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = newYear
            });

            var returnOrderWeek = command.Parameters.Add("@SumOrders", SqlDbType.Int);
            returnOrderWeek.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultOrderWeek = returnOrderWeek.Value;
            tbl_orderWeek.Text = resultOrderWeek.ToString();

            //Orders Month
            string sp_OrderMonth = "sp_SalesOfBranchInMonth_Partner";
            command = new SqlCommand(sp_OrderMonth, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@BranchId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = branchId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Month",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = month
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Year",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = year
            });

            var returnOrderMonth = command.Parameters.Add("@SalesOfBranch", SqlDbType.Money);
            returnOrderMonth.Direction = ParameterDirection.ReturnValue;

            command.ExecuteNonQuery();
            var resultOrderMonth = returnOrderMonth.Value;
            tbl_orderMonth.Text = resultOrderMonth.ToString();

        }


//================================Content Wallet Begin=========================================
        private void contentWalletOpenWallet_Click(object sender, RoutedEventArgs e)
        {
            contentWalletRegisterWallet.Visibility = Visibility.Visible;
        }

        private void contentWalletConfirmOpenWallet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void contentWalletTopup_Click(object sender, RoutedEventArgs e)
        {
            contentWalletTopupOpen.Visibility = Visibility.Visible;
        }

        private void contentWalletTopupMoneyOk_Click(object sender, RoutedEventArgs e)
        {
            int partnerId = LoginWindow.Person.Id;
            string money = moneyTopup.Text;

            ConnectDatabase();
            string sp_topUp = "sp_addMoneyToWallet_Wallet";
            var command = new SqlCommand(sp_topUp, _connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@PartnerId",
                SqlDbType = System.Data.SqlDbType.Int,
                Value = partnerId
            });
            command.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Money",
                SqlDbType = System.Data.SqlDbType.Money,
                Value = money
            });

            int isSuccess = command.ExecuteNonQuery();
            if(isSuccess == 1)
            {
                MessageBox.Show("Success");
                moneyTopup.Text = "";
                btnWalletChecked(sender, e);
            }
            else
            {
                MessageBox.Show("Error");
                moneyTopup.Text = "";
            }

        }

        










        //================================Content Wallet End=========================================
    }
}
