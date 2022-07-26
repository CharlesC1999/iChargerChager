using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.ViewModels.Power
{
    public class OrderViewModel
    {
        public int id { get; set; }
        public string account { get; set; }
        public string car_id { get; set; }
        public CarViewModel car { get; set; }
        public string charger_id { get; set; }
        public ChargerViewModel charger { get; set; }
        public int status { get; set; }
        public string createid { get; set; }
        public DateTime createat { get; set; }
        public string updateid { get; set; }
        public DateTime updateat { get; set; }
    }
}