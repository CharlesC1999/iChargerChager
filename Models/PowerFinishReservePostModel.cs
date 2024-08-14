using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Power
{
    public class PowerFinishReservePostModel
    {
        [Required(ErrorMessage = " [交易代號] 為必填欄位 ")]
        [DisplayName("交易代號")]
        public int OrderId { get; set; }
    }
}