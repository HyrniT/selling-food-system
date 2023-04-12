using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingSystem.Model
{
    public class Order
    {
        public int Id { get; set; }

        public SqlMoney Total { get; set; }
        public SqlDateTime Date { get; set; }

        public string Status { get; set; }

        public string BranchName { get; set; }
        public string BranchPhone { get; set; }

        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }

        public string DriverName { get; set; }

    }
}
