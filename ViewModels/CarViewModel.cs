using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.ViewModels.Power
{
    public class CarViewModel
    {
        public string id { get; set; }
        public string plate { get; set; }
        public string vehiclestyle_id { get; set; }
        public CarVehicleStyleViewModel vehiclestyle { get; set; }
    }
}