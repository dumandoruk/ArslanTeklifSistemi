namespace ArslanTeklifSistemi.Models
{
    public class ProductDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public string ImageUrl { get; set; } = string.Empty;
        public string DocUrl { get; set; } = string.Empty;
        public double DiscountPercent { get; set; }
    }
}
