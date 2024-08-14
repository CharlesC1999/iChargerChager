using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.ViewModels.Power
{
    public class PayViewModel
    {
        public string id { get; set; }
        public string account { get; set; }
        public string number { get; set; }
    }
}