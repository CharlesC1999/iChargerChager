using System;

namespace backend.ViewModels.Power
{
    public class ReceiveViewModel
    {
        public string id { get; set; }
        public int type { get; set; }
        public DateTime receive_date { get; set; }
        public string receive_number { get; set; }
        public string random_number { get; set; }
        public string seller_id { get; set; }
        public string buyer_id { get; set; }
        public DateTime cancel_date { get; set; }
        public string cancel_reason { get; set; }
        public string npoban_id { get; set; }
        public string carrier_id { get; set; }
        public string qrcode { get; set; }
        public string qrcode2 { get; set; }
        public string barcode { get; set; }
    }
}