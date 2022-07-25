using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Power
{
    public class ChargerQRCodeModel
    {
        public string id { get; set; }
        public string charger_id { get; set; }
        public string chargergun_id { get; set; }
        public string key { get; set; }
        public DateTime time { get; set; }
    }
}