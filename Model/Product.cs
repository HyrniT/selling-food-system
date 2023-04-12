using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingSystem.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SqlMoney Price { get; set; }
        public int Quantity { get; set; }
        public SqlInt32 Sold { get; set; }
        public SqlMoney Total { get; set; }
        public string Status { get; set; }
    }
}
