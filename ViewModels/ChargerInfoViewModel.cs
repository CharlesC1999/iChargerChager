using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.ViewModels.Power
{
    public class ChargerInfoViewModel
    {
        public string id { get; set; }
        public string charger_id { get; set; }
        public string chargergun_id { get; set; }
        public int charge_time { get; set; }
        public float charge_current { get; set; }
        public float charge_voltage { get; set; }
        public float charge_kw { get; set; }
        public float current_kw { get; set; }
        public string soc { get; set; }
        public int trans_no { get; set; }
        public DateTime time { get; set; }
    }
}