using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Models.Power
{
    public class ChargerInfoModel
    {
        public string id { get; set; }
        public string charger_id { get; set; }
        public string chargergun_id { get; set; }
        public int charge_time { get; set; }
        public int charge_current { get; set; }
        public int charge_voltage { get; set; }
        public int charge_kw { get; set; }
        public int current_kw { get; set; }
        public string soc { get; set; }
        public DateTime time { get; set; }
    }
}