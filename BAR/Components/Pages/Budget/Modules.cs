namespace BAR.Components.Pages.Budget.Modules {
    public class CategoryData {
        public string Type {set; get;}
        public decimal Amt {set; get;}

        public CategoryData(){
            Type = string.Empty;
            Amt = 0.0m;
        }
    }
}