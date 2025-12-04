//using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace GardeningAPI.Model
{

    public class Drafts
    {
        [JsonIgnore]
        public string? U_Cart { get; set; } = "Y";
        public required DateTime DocDate { get; set; }
        public required DateTime DocDueDate { get; set; }
        public required string CardCode { get; set; }

        [JsonIgnore]
        public int? Series { get; set; }
        public DateTime TaxDate { get; set; }

        [JsonIgnore]
        public string DocObjectCode { get; set; }= "17";
        public string? U_PayMethod { get; set; }
        public Draftsline[]? DocumentLines { get; set; }
    }

    public class Draftsline
    {
        required
        public string ItemCode { get; set; }
        required
        public string Quantity { get; set; }
        public string? UnitPrice { get; set; }
    }

}
