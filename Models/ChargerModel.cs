using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Models.Power
{
    public class ChargerModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string chargersupplier_name { get; set; }
        public string chargerlocation_name { get; set; }
        public string chargerlocation_address { get; set; }
        public string chargerlocation_geom { get; set; }
        public string chargergun_type { get; set; }
        public string chargergun_type_power { get; set; }
        public int chargergun_fee { get; set; }
        public string chargergun_name { get; set; }
    }
}