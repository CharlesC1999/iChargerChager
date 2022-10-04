using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Power
{
    public class PowerPostModel
    {
        [Required(ErrorMessage = " [車輛編號] 為必填欄位 ")]
        [DisplayName("車輛編號")]
        [StringLengthAttribute(100, ErrorMessage = " [車輛編號] 不能超過100字元 ")]
        public string CarId { get; set; }

        [Required(ErrorMessage = " [卡片編號] 為必填欄位 ")]
        [DisplayName("卡片編號")]
        [StringLengthAttribute(100, ErrorMessage = " [卡片編號] 不能超過100字元 ")]
        public string PayId { get; set; }

        [Required(ErrorMessage = " [發票編號] 為必填欄位 ")]
        [DisplayName("發票編號")]
        [StringLengthAttribute(100, ErrorMessage = " [發票編號] 不能超過100字元 ")]
        public string ReceiveId { get; set; }

        [Required(ErrorMessage = " [充電槍代碼] 為必填欄位 ")]
        [DisplayName("充電槍代碼")]
        [StringLengthAttribute(100, ErrorMessage = " [充電槍代碼] 不能超過100字元 ")]
        public string Key { get; set; }
    }
}