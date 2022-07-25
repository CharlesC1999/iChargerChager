using System;

namespace backend.Models.Power
{
    public class PageModel
    {
        public int page_count { get; set; }
        public int max_page { get; set; }
        public int now_page { get; set; }
    }
}