namespace GardeningAPI.Model
{

    public class Drafts
    {
        public string? U_Cart { get; set; } = "Y";
        public required DateTime DocDate { get; set; }
        public required DateTime DocDueDate { get; set; }
        public required string CardCode { get; set; }
        public int? Series { get; set; }
        public DateTime TaxDate { get; set; }
        public string DocObjectCode { get; set; }= "17";
        public Draftsline[] DocumentLines { get; set; }
    }

    public class Draftsline
    {
        public string ItemCode { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
    }

}
