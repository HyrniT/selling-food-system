using OnlineSellingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
    /// Interaction logic for MainWindowCustomer.xaml
    /// </summary>
    public partial class MainWindowCustomer : Window, INotifyCollectionChanged
    {
        public MainWindowCustomer()
        {
            InitializeComponent();
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        SqlConnection _connection = null;

        ObservableCollection<Person> _subItemsPartner = new ObservableCollection<Person>();
        ObservableCollection<Product> _subItemsProduct = new ObservableCollection<Product>();

        public int RowsPerPage { get; set; } = 30;

        public int TotalPagesPartner { get; set; } = 0;
        public int CurrentPagePartner { get; set; } = 1;
        public int TotalItemsPartner { get; set; } = 0;

        public int TotalPagesProduct { get; set; } = 0;
        public int CurrentPageProduct { get; set; } = 1;
        public int TotalItemsProduct { get; set; } = 0;

        ObservableCollection<Order> _subItemsOrder = new ObservableCollection<Order>();

        public int TotalPagesOrder { get; set; } = 0;
        public int CurrentPageOrder { get; set; } = 1;
        public int TotalItemsOrder { get; set; } = 0;

        //Product show mode
        public bool IsAllProductShow = true;
        public bool IsTopSaleProductShow = false;
        public bool IsPriceLowToHighShow = false;
        public bool IsPriceHighToLowShow = false;
        public bool IsProducOfPartnerShow = false;

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

            contentOrdersPartner.ItemsSource = _subItemsPartner;

        }

        private void SetTotalPagesProduct()
        {
            string sqlQueryTotalProduct =
                """
                    SELECT COUNT(*) FROM Product
                """;
            TotalItemsProduct = NumberOfItems(sqlQueryTotalProduct);

            int isDivisibleProduct = TotalItemsProduct % RowsPerPage;
            if (isDivisibleProduct != 0)
            {
                TotalPagesProduct = TotalItemsProduct / RowsPerPage + 1;
            }
            else
            {
                TotalPagesProduct = TotalItemsProduct / RowsPerPage;
            }
        }

        private void SelectSubItemsProduct(string sql, string orderbyAtributes)
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPageProduct - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            string sqlSelectListProduct = sql + " ORDER BY " + orderbyAtributes + $" OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY";
            var command = new SqlCommand(sqlSelectListProduct, _connection);
            var reader = command.ExecuteReader();

            _subItemsProduct.Clear();
            while (reader.Read())
            {
                int productId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                string productName = reader.GetString(reader.GetOrdinal("ProductName"));
                SqlInt32 productSold = reader.GetInt32(reader.GetOrdinal("ProductSold"));
                SqlMoney productPrice = reader.GetSqlMoney(reader.GetOrdinal("ProductPrice"));

                Product _product = new Product { Id = productId, Name = productName, Sold = productSold, Price = productPrice }; 
                _subItemsProduct.Add(_product);
            }

            contentOrdersProductDataGrid.ItemsSource = _subItemsProduct;

        }

        private void SelectSubItemsOrder(int customerId)
        {
            //Connect Database
            SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
            _connection.Open();

            int offset = (CurrentPageOrder - 1) * RowsPerPage;
            int fetch = RowsPerPage;

            SqlCommand command = _connection.CreateCommand();
            command.CommandText = "sp_ShowListAllOrders_Customer";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(
                    new SqlParameter
                    {
                        ParameterName = "@CustomerId",
                        SqlDbType = SqlDbType.Int,
                        Value = customerId
                    }
                );

            var reader = command.ExecuteReader();

            _subItemsOrder.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("OrderId"));
                SqlDateTime dateTime = reader.GetSqlDateTime(reader.GetOrdinal("OrderDate"));
                SqlMoney total = reader.GetSqlMoney(reader.GetOrdinal("OrderFinalPrice"));
                int status = reader.GetInt32(reader.GetOrdinal("StatusId"));

                string str_status = "Lỗi";
                if (status == 1)
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

                var _order = new Order { Id = id, Date = dateTime, Total = total, Status = str_status};
                _subItemsOrder.Add(_order);
            }

            contentTrackingYourOrdersDataGrid.ItemsSource = _subItemsOrder;
        }

        private void MainWindowCustomerLoaded(object sender, RoutedEventArgs e)
        {
            btnOrders.IsChecked = true;
            contentOrders.Visibility = Visibility.Visible;

            customerName.Text = LoginWindow.Person.Fullname;
            btnOrdersChecked(sender, e);
        }

        //Menu Navigation
        private void btnOrdersChecked(object sender, RoutedEventArgs e)
        {
             contentTracking.Visibility = Visibility.Collapsed;

            btnOrders.IsChecked = true;
            contentOrders.Visibility = Visibility.Visible;


            //Data handle partner list
            CurrentPagePartner = 1;

            string sqlQueryTotalPartner = "SELECT COUNT(*) FROM Partner";
            TotalItemsPartner = NumberOfItems(sqlQueryTotalPartner);

            int isDivisiblePartner = TotalItemsPartner % RowsPerPage;
            if (isDivisiblePartner != 0)
            {
                TotalPagesPartner = TotalItemsPartner / RowsPerPage + 1;
            }
            else
            {
                TotalPagesPartner = TotalItemsPartner / RowsPerPage;
            }

            contentOrderPartnerListCurrentPage.Text = CurrentPagePartner.ToString();
            contentOrderPartnerListTotalPage.Text = TotalPagesPartner.ToString();
            SelectSubItemsPartner();

            //Data handle Product list
            CurrentPagePartner = 1;

            SetTotalPagesProduct();

            contentOrdersProductCurrentPage.Text = CurrentPageProduct.ToString();
            contentOrdersProductTotalPage.Text = TotalPagesProduct.ToString();

            string allProductQuery =
                """
                    SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                    FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                    GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                """;
            string orderby = "ProductId";
            SelectSubItemsProduct(allProductQuery, orderby);

        }

        private void btnOrdersTrackingChecked(object sender, RoutedEventArgs e)
        {
            contentOrders.Visibility = Visibility.Collapsed;

            btnOrdersTracking.IsChecked = true;
            contentTracking.Visibility = Visibility.Visible;

            CurrentPageOrder = 1;
            orderCurrentPage.Text = CurrentPageOrder.ToString();

            int customerId = LoginWindow.Person.Id;

            string sqlQueryTotalOrder =
                $"""
                    SELECT COUNT([Order].OrderId)
                    FROM [Order]
                    WHERE [Order].CustomerId = {customerId}
                """;
            TotalItemsOrder = NumberOfItems(sqlQueryTotalOrder);

            int isDivisiblePartner = TotalItemsOrder % RowsPerPage;
            if (isDivisiblePartner != 0)
            {
                TotalPagesOrder = TotalItemsOrder / RowsPerPage + 1;
            }
            else
            {
                TotalPagesOrder = TotalItemsOrder / RowsPerPage;
            }

            orderCurrentPage.Text = CurrentPageOrder.ToString();
            orderTotalPage.Text = TotalPagesOrder.ToString();

            SelectSubItemsOrder(customerId);
        }

        private void logoutButton(object sender, MouseButtonEventArgs e)
        {
            var screen = new StartWindow();
            this.Close();
            screen.Show();
        }


        //Content Orders
        private void contentOrdersProductTopSalesButton_Click(object sender, RoutedEventArgs e)
        {
            IsAllProductShow = false;
            IsTopSaleProductShow = true;
            IsPriceLowToHighShow = false;
            IsPriceHighToLowShow = false;
            IsProducOfPartnerShow = false;

            //Data handle Product list
            CurrentPageProduct = 1;
            contentOrdersProductCurrentPage.Text = CurrentPageProduct.ToString();

            SetTotalPagesProduct();
            contentOrdersProductTotalPage.Text = TotalPagesProduct.ToString();

            string allProductQuery =
                """
                    SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                    FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                    GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                """;
            string orderby = "ProductSold DESC, ProductId";
            SelectSubItemsProduct(allProductQuery, orderby);
        }

        private void contentOrdersProductPriceLowToHighButton_Click(object sender, RoutedEventArgs e)
        {
            IsAllProductShow = false;
            IsTopSaleProductShow = false;
            IsPriceLowToHighShow = true;
            IsPriceHighToLowShow = false;
            IsProducOfPartnerShow = false;

            //Data handle Product list
            CurrentPageProduct = 1;
            contentOrdersProductCurrentPage.Text = CurrentPageProduct.ToString();

            SetTotalPagesProduct();
            contentOrdersProductTotalPage.Text = TotalPagesProduct.ToString();

            string allProductQuery =
                """
                    SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                    FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                    GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                """;
            string orderby = "ProductPrice ASC, ProductId";
            SelectSubItemsProduct(allProductQuery, orderby);
        }

        private void contentOrdersProductPriceHighToLowButton_Click(object sender, RoutedEventArgs e)
        {
            IsAllProductShow = false;
            IsTopSaleProductShow = false;
            IsPriceLowToHighShow = false;
            IsPriceHighToLowShow = true;
            IsProducOfPartnerShow = false;

            //Data handle Product list
            CurrentPageProduct = 1;
            contentOrdersProductCurrentPage.Text = CurrentPageProduct.ToString();

            SetTotalPagesProduct();
            contentOrdersProductTotalPage.Text = TotalPagesProduct.ToString();

            string allProductQuery =
                """
                    SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                    FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                    GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                """;
            string orderby = "ProductPrice DESC, ProductId";
            SelectSubItemsProduct(allProductQuery, orderby);
        }

        private void contentOrdersPartnerButton_Click(object sender, RoutedEventArgs e)
        {
            contentOrdersProductInputPartnerID.Visibility = Visibility.Visible;
        }

        private void contentOrdersProductInputPartnerID_Click(object sender, RoutedEventArgs e)
        {
            IsAllProductShow = false;
            IsTopSaleProductShow = false;
            IsPriceLowToHighShow = false;
            IsPriceHighToLowShow = false;
            IsProducOfPartnerShow = true;

            string partnerId = partnerID.Text;

            //Data handle Product list
            CurrentPageProduct = 1;
            contentOrdersProductCurrentPage.Text = CurrentPageProduct.ToString();

            string sqlQueryTotalProduct =
                $"""
                    SELECT COUNT([Partner].PartnerId)
                    FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = {partnerId} AND [Partner].PartnerId = [Contract].PartnerId
                				    INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                				    INNER JOIN [BranchProductType] ON [Branch].BranchId = [BranchProductType].BranchId
                				    INNER JOIN [Product] ON [BranchProductType].BranchProductTypeId = [Product].BranchProductTypeId
                				    INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                    GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice 
                """;
            TotalItemsProduct = NumberOfItems(sqlQueryTotalProduct);

            int isDivisibleProduct = TotalItemsProduct % RowsPerPage;
            if (isDivisibleProduct != 0)
            {
                TotalPagesProduct = TotalItemsProduct / RowsPerPage + 1;
            }
            else
            {
                TotalPagesProduct = TotalItemsProduct / RowsPerPage;
            }
            contentOrdersProductTotalPage.Text = TotalPagesProduct.ToString();

            string allProductOfPartner =
                $"""
                   SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                    FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = {partnerId} AND [Partner].PartnerId = [Contract].PartnerId
                				    INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                				    INNER JOIN [BranchProductType] ON [Branch].BranchId = [BranchProductType].BranchId
                				    INNER JOIN [Product] ON [BranchProductType].BranchProductTypeId = [Product].BranchProductTypeId
                				    INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                    GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice 
                """;
            string orderby = "ProductSold DESC, ProductId";
            SelectSubItemsProduct(allProductOfPartner, orderby);
        }
        //Partner List Paging
        private void contentOrdersPartnerListPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPagePartner <= 1)
            {
                //Do nothing
            }
            else
            {
                _subItemsPartner.Clear();
                CurrentPagePartner--;
                contentOrderPartnerListCurrentPage.Text = CurrentPagePartner.ToString();
                SelectSubItemsPartner();
            }
        }

        private void contentOrdersPartnerListNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPagePartner == TotalPagesPartner)
            {
                //Do nothing
            }
            else
            {
                _subItemsPartner.Clear();
                CurrentPagePartner++;
                contentOrderPartnerListCurrentPage.Text = CurrentPagePartner.ToString();
                SelectSubItemsPartner();
            }
        }

        //Product List
        private void contentOrdersProductPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentPageProduct <= 1)
            {
                //Do nothing
            }
            else
            {
                //TODO
                _subItemsProduct.Clear();
                CurrentPageProduct--;
                contentOrdersProductCurrentPage.Text = CurrentPageProduct.ToString();

                if (IsAllProductShow == true)
                {
                    //TODO
                    string allProductQuery =
                        """
                            SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                            FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                            GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                        """;
                    string orderby = "ProductId";
                    SelectSubItemsProduct(allProductQuery, orderby);

                }
                else if(IsTopSaleProductShow == true)
                {
                    //TODO
                    string allProductQuery =
                        """
                            SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                            FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                            GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                        """;
                    string orderby = "ProductSold DESC, ProductId";
                    SelectSubItemsProduct(allProductQuery, orderby);
                }
                else if(IsPriceHighToLowShow == true)
                {
                    //TODO
                    string allProductQuery =
                        """
                            SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                            FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                            GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                        """;
                    string orderby = "ProductPrice DESC, ProductId";
                    SelectSubItemsProduct(allProductQuery, orderby);
                }
                else if(IsPriceLowToHighShow == true)
                {
                    //TODO
                    string allProductQuery =
                        """
                            SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                            FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                            GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                        """;
                    string orderby = "ProductPrice ASC, ProductId";
                    SelectSubItemsProduct(allProductQuery, orderby);
                }
                else if(IsProducOfPartnerShow == true)
                {
                    //TODO
                    string partnerId = partnerID.Text;
                    string allProductOfPartner =
                        $"""
                           SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                                   FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = {partnerId} AND [Partner].PartnerId = [Contract].PartnerId
                                   INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                                   INNER JOIN [BranchProductType] ON [Branch].BranchId = [BranchProductType].BranchId
                                   INNER JOIN [Product] ON [BranchProductType].BranchProductTypeId = [Product].BranchProductTypeId
                                   INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                           GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice 
                        """;
                    string orderby = "ProductSold DESC, ProductId";
                    SelectSubItemsProduct(allProductOfPartner, orderby);
                }
            }
        }

        private void contentOrdersProductNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPageProduct  >= TotalPagesProduct)
            {
                //Do nothing
            }
            else
            {
                //TODO
                _subItemsProduct.Clear();
                CurrentPageProduct++;
                contentOrdersProductCurrentPage.Text = CurrentPageProduct.ToString();

                if (IsAllProductShow == true)
                {
                    //TODO
                    string allProductQuery =
                        """
                            SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                            FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                            GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                        """;
                    string orderby = "ProductId";
                    SelectSubItemsProduct(allProductQuery, orderby);

                }
                else if (IsTopSaleProductShow == true)
                {
                    //TODO
                    string allProductQuery =
                        """
                            SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                            FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                            GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                        """;
                    string orderby = "ProductSold DESC, ProductId";
                    SelectSubItemsProduct(allProductQuery, orderby);
                }
                else if (IsPriceHighToLowShow == true)
                {
                    //TODO
                    string allProductQuery =
                        """
                            SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                            FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                            GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                        """;
                    string orderby = "ProductPrice DESC, ProductId";
                    SelectSubItemsProduct(allProductQuery, orderby);
                }
                else if (IsPriceLowToHighShow == true)
                {
                    //TODO
                    string allProductQuery =
                        """
                            SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                            FROM [Product] INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                            GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice
                        """;
                    string orderby = "ProductPrice ASC, ProductId";
                    SelectSubItemsProduct(allProductQuery, orderby);
                }
                else if (IsProducOfPartnerShow == true)
                {
                    //TODO
                    string partnerId = partnerID.Text;
                    string allProductOfPartner =
                        $"""
                           SELECT [Product].ProductId, [Product].ProductName, [Product].ProductPrice, SUM([OrderProduct].OrderProductQuantity) AS ProductSold
                                   FROM [Partner] INNER JOIN [Contract] ON [Partner].PartnerId = {partnerId} AND [Partner].PartnerId = [Contract].PartnerId
                                   INNER JOIN [Branch] ON [Contract].ContractId = [Branch].ContractId
                                   INNER JOIN [BranchProductType] ON [Branch].BranchId = [BranchProductType].BranchId
                                   INNER JOIN [Product] ON [BranchProductType].BranchProductTypeId = [Product].BranchProductTypeId
                                   INNER JOIN [OrderProduct] ON [Product].ProductId = [OrderProduct].ProductId
                           GROUP BY [Product].ProductId, [Product].ProductName, [Product].ProductPrice 
                        """;
                    string orderby = "ProductSold DESC, ProductId";
                    SelectSubItemsProduct(allProductOfPartner, orderby);
                }
            }
        }

        //Select Product
        public SqlMoney TotalOrder { get; set; } = 0;
        public SqlMoney ShipCostOrder { get; set; } = 25000;
        public SqlMoney FinalTotalOrder { get; set; } = 0; 

        ObservableCollection<Product> _subItemsSelectedProduct = new ObservableCollection<Product>();
        private void contentOrdersSelectProductSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if(productID.Text != "" && productQuantity.Text != "")
            {
                int productId = int.Parse(productID.Text);
                int quantity = int.Parse(productQuantity.Text);

                string sql =
                $"""
                    SELECT ProductId, ProductName, ProductPrice
                    FROM Product WHERE ProductId = {productId}
                """;

                //Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                var command = new SqlCommand(sql, _connection);
                var reader = command.ExecuteReader();

                string name = "";
                SqlMoney price = 0;

                while (reader.Read())
                {
                    name = reader.GetString(reader.GetOrdinal("ProductName"));
                    price = reader.GetSqlMoney(reader.GetOrdinal("ProductPrice"));
                }

                SqlMoney total = price * quantity;
                

                var _productSeleted = new Product { Id = productId, Name = name, Quantity = quantity, Price = price, Total = total };
                _subItemsSelectedProduct.Add(_productSeleted);

                contentOrdersSelectedProductDataGrid.ItemsSource = _subItemsSelectedProduct;

                productID.Text = "";
                productQuantity.Text = "";

                TotalOrder += total;
                FinalTotalOrder = TotalOrder + ShipCostOrder;

                orderTotalOrder.Text = TotalOrder.ToString();
                orderShipCost.Text = ShipCostOrder.ToString();
                orderFinalTotalOrder.Text = FinalTotalOrder.ToString();
            }
            else
            {
                //Do nothing
            }
            
        }


        //Order Button
        private void contentOrdersPaymentOrder_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        //Content Tracking=============================================================================
        //Your Orders
        private void contentTrackingYourOrdersPreviousButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void contentTrackingYourOrdersNextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        ObservableCollection<Product> _subItemsProductOrderId = new ObservableCollection<Product>();
        private void searchProductOfOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ratingOrderID.Text != "")
            {
                int orderId = int.Parse(ratingOrderID.Text);
                productOfOrderId.Text = orderId.ToString();

                //Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                SqlCommand command = _connection.CreateCommand();
                command.CommandText = "sp_ShowListAllProductOfOrder";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(
                        new SqlParameter
                        {
                            ParameterName = "@OrderId",
                            SqlDbType = SqlDbType.Int,
                            Value = orderId
                        }
                    );

                var reader = command.ExecuteReader();

                _subItemsProductOrderId.Clear();
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                    string name = reader.GetString(reader.GetOrdinal("ProductName"));
                    SqlMoney total = reader.GetSqlMoney(reader.GetOrdinal("Total"));
                    int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));

                    var _product = new Product { Id = id, Name = name, Total = total, Quantity = quantity };
                    _subItemsProductOrderId.Add(_product);
                }

                productOfOrder.ItemsSource = _subItemsProductOrderId;
            }
            else
            {
                //Do nothing
            }
        }

        private void contentTrackingRating_Click(object sender, RoutedEventArgs e)
        {
            if(ratingProductID.Text == "")
            {
                //Do nothing
            }
            else
            {
                int customerId = LoginWindow.Person.Id;
                int productId = int.Parse(ratingProductID.Text);

                int ratingStar = 5;
                if (contentTrackingRatingScoreOne.IsChecked == true)
                    ratingStar = 1;
                else if (contentTrackingRatingScoreTwo.IsChecked == true)
                    ratingStar = 2;
                else if (contentTrackingRatingScoreThree.IsChecked == true)
                    ratingStar = 3;
                else if (contentTrackingRatingScoreFour.IsChecked == true)
                    ratingStar = 4;

                string comment = ratingOrderComment.Text;

                //Connect Database
                SqlConnection _connection = new SqlConnection("server=.; database=OnlineSellingDatabase;Trusted_Connection=yes");
                _connection.Open();

                string sql =
                    $"""
                    INSERT Rating (CustomerID,RatingStar,RatingComment,RatingTime,ProductID)
                    VALUES ({customerId}, {ratingStar},N'{comment}', getdate(), {productId})
                """;

                var command = new SqlCommand(sql, _connection);
                int isSuccess = command.ExecuteNonQuery();
                if (isSuccess == 1)
                {
                    MessageBox.Show("Sent Rating & Comment");
                    ratingProductID.Text = "";
                    ratingOrderComment.Text = "";
                }
                else
                {
                    MessageBox.Show("Can not send");
                    ratingProductID.Text = "";
                    ratingOrderComment.Text = "";
                }
            }

        }

        
    }
}
