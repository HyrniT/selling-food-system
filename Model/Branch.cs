using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingSystem.Model
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public int BranchPartnerId { get; set; }
        public string BranchPartnerName { get; set; }

        public int BranchStar { get; set; }
    }
}
