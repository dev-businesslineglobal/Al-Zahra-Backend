namespace GardeningAPI.Model
{
  

    public class Cart
    {
        public string? U_Cart { get; set; }
        public required DateTime DocDate { get; set; }
        public required DateTime DocDueDate { get; set; }
        public required string CardCode { get; set; }
        public int? Series { get; set; }
        public DateTime TaxDate { get; set; }
        public required List<CartLines> DocumentLines { get; set; }
    }

    public class CartLines
    {
        public string? ItemCode { get; set; }
        public string? Dscription { get; set; }
        public double Quantity { get; set; }
        public double? UnitPrice { get; set; } = null;
   
    }
}
