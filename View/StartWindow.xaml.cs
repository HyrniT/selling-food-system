using System;
using System.Collections.Generic;
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

namespace OnlineSellingSystem.View
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public static string selectedType;

        public StartWindow()
        {
            InitializeComponent();
        }

        private void btn_admin_click(object sender, RoutedEventArgs e)
        {
            selectedType = Admin.Name.ToString();

            var screen = new LoginWindow();
            this.Close();
            screen.ShowDialog();
        }

        private void btn_employee_click(object sender, RoutedEventArgs e)
        {
            selectedType = Employee.Name.ToString();

            var screen = new LoginWindow();
            this.Close();
            screen.ShowDialog();
        }

        private void btn_partner_click(object sender, RoutedEventArgs e)
        {
            selectedType = Partner.Name.ToString();

            var screen = new LoginWindow();
            this.Close();
            screen.ShowDialog();
        }

        private void btn_delivery_click(object sender, RoutedEventArgs e)
        {
            selectedType = Driver.Name.ToString();

            var screen = new LoginWindow();
            this.Close();
            screen.ShowDialog();
        }

        private void btn_customer_click(object sender, RoutedEventArgs e)
        {
            selectedType = Customer.Name.ToString();

            var screen = new LoginWindow();
            this.Close();
            screen.ShowDialog();
        }
    }
}
