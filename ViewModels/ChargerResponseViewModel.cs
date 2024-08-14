using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.ViewModels.Power
{
    public class ChargerResponseViewModel
    {
        public string rescode { get; set; }
        public string resmsg { get; set; }
        public string resdata { get; set; }
    }
}