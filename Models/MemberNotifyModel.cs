using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Power
{
    public class MemberNotifyModel
    {
        public string account { get; set; }
        public string token { get; set; }
    }
}