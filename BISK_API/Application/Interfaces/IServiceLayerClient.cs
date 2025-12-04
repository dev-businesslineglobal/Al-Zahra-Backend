using System.Threading;
using System.Threading.Tasks;
using GardeningAPI.Model;
using gardnerAPIs.Common;

//namespace gardnerAPIs.Application.Interfaces
namespace GardeningAPI.Application.Interfaces
{
    public interface IServiceLayerClient
    {
        // Auto-managed technical session for all operations below
        // functions to call service layer
        Task<OperationResult<Response>> AddSaleOrder(Document doc, CancellationToken ct = default);
        Task<OperationResult<Response>> AddDelivery(Document doc, CancellationToken ct = default);
        Task<OperationResult<Response>> AddInvoices(Document doc, CancellationToken ct = default);
        Task<OperationResult<Response>> AddCreditNotes(Document doc, CancellationToken ct = default);
        Task<OperationResult<Response>> AddIncomingPayment(IncomingPayment payment, CancellationToken ct = default);
        Task<OperationResult<ResponseBP>> AddBP(SignUp bp, CancellationToken ct = default);
        Task<OperationResult<ResponseOTP>> AddOTP(OTP otpBP, CancellationToken ct = default);

        Task<OperationResult<ResponseBP>> PatchBP(string cardCode, SignUp patch, CancellationToken ct = default);
        //Task<OperationResult<OTP>> SaveOtpToUDOAsync(OTP otp, CancellationToken ct = default);

        Task<OperationResult<ResponseBP>> PatchBPU(string cardCode, object patch, CancellationToken ct = default);
        //Task<OperationResult<OTP>> PatchOtpToUDOAsync(OTP isExistEmail, CancellationToken ct = default);

        Task<OperationResult<OTP>> PatchOtp(string code, object patch, CancellationToken ct = default);
        Task<OperationResult<ResponseItem>> AddCart(Drafts draft, CancellationToken ct = default);
        Task<OperationResult<ResponseItem>> PutCart(int docEntry, Drafts put, CancellationToken ct = default);
        //Task<OperationResult<ResponseBP>> SaveOtpAsync(OTP otp, CancellationToken ct = default);
        //Task<OperationResult<ResponseBP>> SaveOtpAsync(string cardCode, string email, string otp, CancellationToken ct = default);
    }
}
