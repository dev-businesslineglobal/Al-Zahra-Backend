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
        public ItemsService(IDatabase db, HelperService helper, IServiceLayerClient sl)
        {
            _db = db;
            _helper = helper;
            _sl = sl;
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

        public async Task<ApiResult> PostCartItemsAsync(Drafts draft, CancellationToken ct)
        {
            // Validate input
            if (draft == null)
            {
                return new ApiResult(400, new ApiResponse
                {
                    success = false,
                    error = "Cart payload is required."
                });
            }

            // Get cart details for the card code
            var isCart = await _db.GetCartDetails(draft.CardCode ?? string.Empty);

            // If user already has cart → update (PUT)
            if (isCart != null && isCart.U_Cart == "Y")
            {
                var putResponse = await _sl.PutCart(draft.CardCode, draft, ct);

                // Validate PUT result
                if (putResponse == null || !putResponse.IsSuccess)
                {
                    return new ApiResult(400, new ApiResponse
                    {
                        success = false,
                        error = "Failed to update cart."
                    });
                }

                return new ApiResult(200, new ApiResponse
                {
                    success = true,
                    data = putResponse.Result
                });
            }

            // Otherwise → create new cart (POST)
            var cartResult = await _sl.AddCart(draft, ct);

            // Validate POST result
            if (cartResult == null)
            {
                return new ApiResult(400, new ApiResponse
                {
                    success = false,
                    error = "Service returned null response."
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
                data = cartResult.Result
            });
        }



        public async Task<ApiResponse> GetCartAsync(string CardCode, CancellationToken cancellationToken = default)
        {
            var cart = await _db.GetCartDetails(CardCode);

            if (cart == null)
            {
                return new ApiResponse
                {
                    success = false,
                    error = "No cart found for the given CardCode."
                };
            }


            return new ApiResponse
            {
                success = true,
                data = cart
            };
        }







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
