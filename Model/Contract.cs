using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingSystem.Model
{
    public class Contract
    {
        public int ContractId { get; set; }

        public int ContractPartnerId { get; set; }
        public string ContractPartnerName { get; set; } 

        public string ContractDate { get; set; }
        public string ContractStartDay { get; set; }
        public string ContractEndDay { get; set; }

        public string ContractStatus { get; set; }
    }
}
