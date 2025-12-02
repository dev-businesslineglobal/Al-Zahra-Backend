using System.Threading;
using System.Threading.Tasks;
using GardeningAPI.Model;
using gardnerAPIs.Common;

//namespace gardnerAPIs.Application.Interfaces
namespace GardeningAPI.Application.Interfaces
{
    public interface IPostingBusinessLogic
    {
        // posting business logic
        Task<ApiResult> PostBusinessPartner(SignUp bp, CancellationToken ct = default);
        Task<ApiResult> UpdateBusinessPartner(string cardCode, SignUp patch, CancellationToken ct = default);
        Task<ApiResult> PostSaleOrderAsync(Document doc, CancellationToken ct = default);
        Task<ApiResult> PostInvoiceAsync(Document doc, CancellationToken ct = default);
        Task<ApiResult> PostCreditNoteAsync(Document doc, CancellationToken ct = default);
        Task<ApiResult> PostIncomingPayment(IncomingPayment doc, CancellationToken ct = default);

        Task<ApiResult> SaveOTP(OTP otp);

        //Task<ApiResult> SaveOTP(string cardCode,string email, string otp);
        //Task<ApiResult> PostOTPDetails(OTP otp, CancellationToken ct = default);/
    }
}
