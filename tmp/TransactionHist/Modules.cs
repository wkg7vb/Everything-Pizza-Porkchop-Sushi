using System.ComponentModel.DataAnnotations;

namespace BAR.Components.Pages.TransactionHist
{
    public class TSingle
    {
        public string TimeStamp { get; set; }
        public string Label { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }

        public TSingle()
        {
            TimeStamp = DateTimeOffset.Now.ToString("yyyyMMddHHmmssffff");
            Label = string.Empty;
            Category = string.Empty;
            Amount = 0.0m;
        }
    }
}
