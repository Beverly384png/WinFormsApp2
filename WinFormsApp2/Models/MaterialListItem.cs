namespace WinFormsApp2.Models
{
    public class MaterialListItem
    {
        public int Id { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal MinQty { get; set; }
        public decimal StockQty { get; set; }
        public decimal Price { get; set; }
        public string MeasureUnit { get; set; } = string.Empty;
        public decimal RequiredQty { get; set; }
    }
}