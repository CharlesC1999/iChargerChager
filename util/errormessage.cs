using System;
namespace backend.util
{
    public class errormessage
    {
        public static string DateCompare(string startDate, string endDate){
            string Result = string.Empty;
            DateTime start = Convert.ToDateTime(startDate);
            DateTime end = Convert.ToDateTime(endDate);
            if(DateTime.Compare(start, end) > 0){
                Result = " 結束時間不能大於開始時間 ";
            }
            return Result;
        }
    }
}