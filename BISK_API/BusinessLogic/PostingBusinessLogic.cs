using System;
using System.Threading;
using System.Threading.Tasks;
using gardnerAPIs.Common;
using GardeningAPI.Application.Interfaces;
using GardeningAPI.Model;
using static System.Net.WebRequestMethods;

namespace GardnerAPI.BusinessLogic
{
    public class PostingBusinessLogic : IPostingBusinessLogic
    {
        private readonly IServiceLayerClient _sl;
        private readonly IDatabase _db;

        public PostingBusinessLogic(IServiceLayerClient serviceLayerClient, IDatabase db)
        {
            _sl = serviceLayerClient ?? throw new ArgumentNullException(nameof(serviceLayerClient));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<ApiResult> PostBusinessPartner(SignUp bp, CancellationToken ct = default)
        {
            try
            {
                if (bp == null) throw new ArgumentNullException(nameof(bp));

                var op = await _sl.AddBP(bp, ct);

                var successData = op.IsSuccess && op.Result != null
                    ? new { StudentCode = op.Result.CardCode, StudentName = op.Result.CardName }
                    : null;

                return ResultMapper.ToApiResult(op, successData!);
            }
            catch (Exception ex)
            {
                return new ApiResult(500, new ApiResponse { success = false, error = ex.Message });
            }
        }

        public async Task<ApiResult> UpdateBusinessPartner(string cardCode, SignUp patch, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cardCode)) throw new ArgumentException("Value is required.", nameof(cardCode));
            if (patch == null) throw new ArgumentNullException(nameof(patch));

            var op = await _sl.PatchBP(cardCode, patch, ct);

            var successData = op.IsSuccess && op.Result != null
                ? new { StudentCode = op.Result.CardCode, StudentName = op.Result.CardName }
                : null;

            return ResultMapper.ToApiResult(op, successData!);
        }


        public async Task<ApiResult> PostSaleOrderAsync(Document doc, CancellationToken ct = default)
        {
            try
            {
                if (doc == null)
                    throw new ArgumentNullException(nameof(doc));

                var op = await _sl.AddSaleOrder(doc, ct);

                var successData = op.IsSuccess && op.Result != null
                    ? new { DocumentEntry = op.Result.DocEntry, DocNumber = op.Result.DocNum }
                    : null;

                return ResultMapper.ToApiResult(op, successData!);
            }
            catch (Exception ex)
            {
                return new ApiResult(500, new ApiResponse
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        public async Task<ApiResult> PostInvoiceAsync(Document doc, CancellationToken ct = default)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            var op = await _sl.AddInvoices(doc, ct);

            var successData = op.IsSuccess && op.Result != null
                ? new { DocumentEntry = op.Result.DocEntry, DocNumber = op.Result.DocNum }
                : null;

            return ResultMapper.ToApiResult(op, successData!);
        }

        public async Task<ApiResult> PostCreditNoteAsync(Document doc, CancellationToken ct = default)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            var op = await _sl.AddCreditNotes(doc, ct);

            var successData = op.IsSuccess && op.Result != null
                ? new { DocumentEntry = op.Result.DocEntry, DocNumber = op.Result.DocNum }
                : null;

            return ResultMapper.ToApiResult(op, successData!);
        }

  
        public async Task<ApiResult> PostIncomingPayment(IncomingPayment doc, CancellationToken ct = default)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            var op = await _sl.AddIncomingPayment(doc, ct);

            var successData = op.IsSuccess && op.Result != null
                ? new { DocumentEntry = op.Result.DocEntry, DocNumber = op.Result.DocNum }
                : null;

            return ResultMapper.ToApiResult(op, successData!);
        }

        public async Task<ApiResult> SaveOTP(OTP payload)
        {
            try
            {
                var result = await _sl.AddOTP(payload);

                return new ApiResult(200, new ApiResponse
                {
                    success = true,
                    data = result.Result
                });
            }
            catch (Exception ex)
            {
                return new ApiResult(400, new ApiResponse
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}
