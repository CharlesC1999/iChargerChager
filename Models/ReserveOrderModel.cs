using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Models.Power
{
    public class ReserveOrderModel
    {
        public int id { get; set; }
        public string account { get; set; }
        public string car_id { get; set; }
        public string receive_id { get; set; }
        public string pay_id { get; set; }
        public string charger_id { get; set; }
        public string chargergun_id { get; set; }
        public int price { get; set; }
        public int status { get; set; }
        public int pay_status { get; set; }
        public DateTime reserve_start { get; set; }
        public DateTime reserve_end { get; set; }
    }
}