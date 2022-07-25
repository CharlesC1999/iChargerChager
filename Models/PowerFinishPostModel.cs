using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Power
{
    public class PowerFinishPostModel
    {
        [Required(ErrorMessage = " [充電樁編號] 為必填欄位 ")]
        [DisplayName("充電樁編號")]
        [StringLengthAttribute(100, ErrorMessage = " [充電樁編號] 不能超過100字元 ")]
        public string ChargerId { get; set; }

        [Required(ErrorMessage = " [充電槍編號] 為必填欄位 ")]
        [DisplayName("充電槍編號")]
        [StringLengthAttribute(100, ErrorMessage = " [充電槍編號] 不能超過100字元 ")]
        public string ChargerGunId { get; set; }
        
        [Required(ErrorMessage = " [交易代號] 為必填欄位 ")]
        [DisplayName("交易代號")]
        public int TransNo { get; set; }
    }
}