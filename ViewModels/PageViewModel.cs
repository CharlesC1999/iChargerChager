using System;

namespace backend.ViewModels.Power
{
    public class PageViewModel
    {
        public int page_count { get; set; }
        public int max_page { get; set; }
        public int now_page { get; set; }
    }
}