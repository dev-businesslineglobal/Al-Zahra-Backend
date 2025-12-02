using GardeningAPI.Model;
using gardnerAPIs.Common;

namespace GardeningAPI.Application.Interfaces
{
    public interface IItemsService
    {
        //Task<ResponseResult> GetItemsAsync();
        Task<(bool Success, string? FileName)> GetItemsAsync(string whsCode, CancellationToken cancellationToken = default);
        Task<ApiResult> PostCartItemsAsync(Drafts draft, CancellationToken ct);
        Task<ApiResponse> GetCartAsync(string CardCode, CancellationToken cancellationToken = default);
        //Task<(bool Success, object Data)> GetItemsAsync(string whsCode);
    }
}
