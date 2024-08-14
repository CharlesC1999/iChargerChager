using System;
using System.Collections.Generic;

namespace backend.ViewModels.Power
{
    public class OrderListViewModel
    {
        public List<OrderViewModel> list { get; set; }
        public PageViewModel pageinfo { get; set; }
    }
}