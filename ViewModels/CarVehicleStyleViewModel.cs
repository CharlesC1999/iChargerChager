using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.ViewModels.Power
{
    public class CarVehicleStyleViewModel
    {
        public string id { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
    }
}