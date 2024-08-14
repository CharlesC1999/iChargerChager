using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Models.Power
{
    public class ChargerPostModel
    {
        public string station_id { get; set; }
        public string charger_id { get; set; }
        public int trans_no { get; set; }
    }
}