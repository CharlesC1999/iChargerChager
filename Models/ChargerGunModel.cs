using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Models.Power
{
    public class ChargerGunModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public int reserve_fee { get; set; }
        public int status { get; set; }
    }
}