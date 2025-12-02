using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GardeningAPI.Model
{
    // ===================== SignUp Models ================= //
    #region SignUp Models
    public class SignUpRequest
    {
        [JsonIgnore]
        public int? reqSeries { get; set; } = 1;
        [Required]
        [MaxLength(200, ErrorMessage = "CardName cannot exceed 200 characters.")]
        public string? CardName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [MaxLength(100, ErrorMessage = "EmailAddress cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? EmailAddress { get; set; }
        public string? Cellular { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? U_Password { get; set; }
        public string? Language { get; set; }
    }

    public class SignUp
    {
        public int? Series { get; set; } = 1;

        [Required]
        [MaxLength(200, ErrorMessage = "CardName cannot exceed 200 characters.")]
        public string? CardName { get; set; }
        public string? CardType { get; set; } = "C";

        [Required(ErrorMessage = "Email address is required.")]
        [MaxLength(100, ErrorMessage = "EmailAddress cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? EmailAddress { get; set; }
        public string? Cellular { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? U_Password { get; set; }
        public int LanguageCode { get; set; }
        public string? U_Verified { get; set; }
    }

    public class SignUpResponse
    {
        public string? CardCode { get; set; }
        public string? userName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Mobile { get; set; }
        [JsonIgnore]
        public ConfigurationData? configuration { get; set; }
        public string? Language { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class APIResponse
    {
        public string? message;
        public int code;
        public object? data;
    }

    public class ConfigurationData
    {
        public int customerSeries { get; set; }
        public int customerGroup { get; set; }
        public int saleOrderSeries { get; set; }
        public int deliverySeries { get; set; }
        public int invoiceSeries { get; set; }
        public int memoSeries { get; set; }
        public int downPaymentSeries { get; set; }
        public int incomingSeries { get; set; }
        public string? whsCode { get; set; }

    }

    #endregion


    // ===================== Items Models ================= //
    #region Items Master Data


    public class ItemsMasterData
    {
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public double Price { get; set; }
        public double OnHand { get; set; }
        public string? SaleUoM { get; set; }
        public string? PictureInfo { get; set; }

        public string? PictureBase64 { get; set; }
    }






    //public class ItemsMasterData
    //{
    //    public string? ItemCode { get; set; }
    //    public string? ItemName { get; set; }
    //    public double Price { get; set; }
    //    public double OnHand { get; set; }
    //    public string? SaleUoM { get; set; }

    //    private string? _pictureInfo;

    //    public string? PictureInfo
    //    {
    //        get => _pictureInfo;
    //        set
    //        {
    //            _pictureInfo = value;
    //            if (!string.IsNullOrEmpty(value) && File.Exists(value))
    //            {
    //                PictureBase64 = Convert.ToBase64String(File.ReadAllBytes(value));
    //            }
    //            else
    //            {
    //                PictureBase64 = null;
    //            }
    //        }
    //    }

    //    public string? PictureBase64 { get; private set; }
    //}

    #endregion


}
