using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backend.Models.Power
{
    public class OrderGetModel
    {
        [BindRequired]
        [DisplayName("頁面")]
        public int Page { get; set; }
        [BindRequired]
        [DisplayName("頁面筆數")]
        public int PageCount { get; set; }
        [BindRequired]
        [DisplayName("排序")]
        public bool Order { get; set; }
    }
}