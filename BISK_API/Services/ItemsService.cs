using GardeningAPI.Application.Interfaces;
using GardeningAPI.Helper;
using gardnerAPIs.Common;
using GardnerAPI.Model;
using SAPbobsCOM;
using GardeningAPI.Model;

namespace GardeningAPI.Services
{
    public class ItemsService : IItemsService
    {
        private readonly IDatabase _db;
        private readonly HelperService _helper;
        private readonly IServiceLayerClient _sl;
        private readonly IConfig _conf;
        public ItemsService(IDatabase db, HelperService helper, IServiceLayerClient sl, IConfig conf)
        {
            _db = db;
            _helper = helper;
            _sl = sl;
            _conf = conf;
        }

        // ======================= Fetching Items ========================= //
        public async Task<(bool Success, string? FileName)> GetItemsAsync(string whsCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(whsCode))
                return (false, null);

            var items = await _db.GetItems(whsCode);
            if (items is not { Count: > 0 })
                return (false, null);
            var itemsToSave = items
                        .Where(i =>
                            !string.IsNullOrWhiteSpace(i.SaleUoM) &&
                            !i.SaleUoM.Equals("N", StringComparison.OrdinalIgnoreCase) && // SaleUoM not "N"
                            !i.ItemCode.Equals("SP-00471", StringComparison.OrdinalIgnoreCase) // ignore specific item
                        )
                        .Take(10)
                        .Select(i =>
                        {
                            i.PictureBase64 = null; // remove Base64 for CSV
                            return i;
                        })
                        .ToList();


            if (itemsToSave.Count == 0)
                return (false, null);

            string? fileResponse = _helper.GenerateCsvFile(itemsToSave, "items");
            if (string.IsNullOrWhiteSpace(fileResponse))
                return (false, null);

            string fileName = fileResponse.Replace("fileName=", "");

            return (true, fileName);
        }


        // ======================= Cart Services ========================= //
        public async Task<ApiResult> PutCartItemsAsync(int docEntry, Drafts draft, CancellationToken ct)
        {
            var config = await _conf.GetConfiguration();
            draft.Series = config?.saleOrderSeries;
            var putResponse = await _sl.PutCart(docEntry, draft, ct);

            // Validate PUT result
            if (putResponse == null || !putResponse.IsSuccess)
            {
                return new ApiResult(400, new ApiResponse
                {
                    success = false,
                    error = putResponse?.ErrorMessages.First().error.message.value.ToString() ?? "Error in Patching Cart"
                });
            }

            return new ApiResult(200, new ApiResponse
            {
                success = true,
                data = putResponse.Result
            });
            //return new ApiResult(400, new ApiResponse
            //{
            //    success = false,
            //    error = "No existing cart found to update."
            //});
        }



        public async Task<ApiResult> PostCartItemsAsync(Drafts draft, CancellationToken ct)
        {
            // Validate input
            if (draft == null)
            {
                return new ApiResult(400, new ApiResponse
                {
                    success = false,
                    error = "Cart payload is required.",
                });
            }
            var config = await _conf.GetConfiguration();
            draft.Series = config?.saleOrderSeries;
            var cartResult = await _sl.AddCart(draft, ct);

            // Validate POST result
            if (cartResult == null)
            {
                return new ApiResult(400, new ApiResponse
                {
                    success = false,
                    error = "Service returned null response."
                    //error = cartResult.ErrorMessages.
                });
            }

            if (!cartResult.IsSuccess)
            {
                return new ApiResult(400, new ApiResponse
                {
                    success = false,
                    error = "Service returned error."
                });
            }

            if (cartResult.Result == null)
            {
                return new ApiResult(400, new ApiResponse
                {
                    success = false,
                    error = "Service returned empty cart data."
                });
            }

            // Success
            return new ApiResult(200, new ApiResponse
            {
                success = true,
                DocEntry = cartResult.Result.docEntry
            });
        }



        public async Task<List<Cart>> GetCartAsync(string CardCode, CancellationToken cancellationToken = default) => await _db.GetCartDetails(CardCode) ?? new List<Cart>();
        
      
        







        //public async Task<(bool Success, string? FileName)> GetItemsAsync(string whsCode, CancellationToken cancellationToken = default)
        //{
        //    if (string.IsNullOrWhiteSpace(whsCode))
        //        return (false, null);

        //    var items = await _db.GetItems(whsCode);
        //    if (items is not { Count: > 0 })
        //        return (false, null);

        //    var itemsWithPictures = items
        //        .Where(i => !string.IsNullOrWhiteSpace(i.PictureBase64))
        //        .ToList();

        //    if (itemsWithPictures.Count == 0)
        //        return (false, null);
        //    string? fileResponse = _helper.GenerateCsvFile(itemsWithPictures, "items");

        //    if (string.IsNullOrWhiteSpace(fileResponse))
        //        return (false, null);

        //    string fileName = fileResponse.Replace("fileName=", "");

        //    return (true, fileName);
        //}



    }
}
