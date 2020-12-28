using System;
using System.Collections.Generic;
using System.Text;

namespace PayrollCore.Entities
{
    public class Claim
    {
        public int ClaimID { get; set; }
        public Claim_Type ClaimType { get; set; }
        public float ClaimableAmount { get; set; }
        public Rate ApplicableRate { get; set; }
        public DateTime ClaimDate { get; set; }
        public int ActivityID { get; set; }
    }
}
