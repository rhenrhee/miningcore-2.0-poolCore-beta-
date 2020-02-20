using System;
using System.Collections.Generic;
using System.Text;

namespace CorePool.Api.Requests
{
    public class AddBalanceRequest
    {
        public string PoolId { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public string Usage { get; set; }
    }
}
