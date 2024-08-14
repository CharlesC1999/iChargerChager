using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.ViewModels.Power
{
    public class ChargerStatusViewModel
    {
        public string charger_id { get; set; }
        public string chargergun_id { get; set; }
        public int trans_no { get; set; }
        public string vendor_error_code { get; set; }
        public int status { get; set; }
        public string time { get; set; }
    }
}