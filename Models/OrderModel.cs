using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Models.Power
{
    public class OrderModel
    {
        public string id { get; set; }
        public string account { get; set; }
        public string car_id { get; set; }
        public string charger_id { get; set; }
        public string chargergun_id { get; set; }
        public int status { get; set; }
        public int transaction_id { get; set; }
        public string createid { get; set; }
        public DateTime createat { get; set; }
        public string updateid { get; set; }
        public DateTime updateat { get; set; }
        public string car_plate { get; set; }
        public string car_vehiclestyle_id { get; set; }
        public string car_vehiclestyle_brand { get; set; }
        public string car_vehiclestyle_model { get; set; }
        public string chargersupplier_id { get; set; }
        public string chargersupplier_name { get; set; }
        public string chargerlocation_id { get; set; }
        public string chargerlocation_name { get; set; }
        public string chargerlocation_address { get; set; }
        public string chargerlocation_geom { get; set; }
        public string charger_name { get; set; }
        public string charger_address { get; set; }
        public string chargergun_type { get; set; }
        public string chargergun_type_power { get; set; }
        public int chargergun_fee { get; set; }
        public string chargergun_name { get; set; }
        public string chargergun_address { get; set; }
    }
}