using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using GardeningAPI.Application.Interfaces;
using GardeningAPI.Model;

namespace gardnerAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/posting")]
    [EnableCors("corsapp")]
    [Authorize]
    [Produces("application/json")]
    public class PostingController : ControllerBase
    {
        private readonly IPostingBusinessLogic _logic;
        private readonly IDatabase _db;
        private readonly IItemsService _items;

        public PostingController(IPostingBusinessLogic logic, IDatabase db, IItemsService items)
        {
            _logic = logic;
            _db = db;
            _items = items;
        }

        [HttpPost("orders")]
        [Consumes("application/json")]

        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> PostOrder([FromBody] Document doc, CancellationToken ct)
        {
            var result = await _logic.PostSaleOrderAsync(doc, ct);
            return StatusCode(result.StatusCode, result.Body);
        }

        /// <summary>Create AR Invoice</summary>
        [HttpPost("invoices")]
        [Consumes("application/json")]

        public async Task<IActionResult> PostInvoice([FromBody] Document doc, CancellationToken ct)
        {
            var result = await _logic.PostInvoiceAsync(doc, ct);
            return StatusCode(result.StatusCode, result.Body);
        }

        [HttpPost("creditnotes")]
        [Consumes("application/json")]
        public async Task<IActionResult> PostCredit([FromBody] Document doc, CancellationToken ct)
        {
            var result = await _logic.PostCreditNoteAsync(doc, ct);
            return StatusCode(result.StatusCode, result.Body);
        }


        [HttpPost("payments/incoming")]
        [Consumes("application/json")]
        public async Task<IActionResult> PostIncomingPayment([FromBody] IncomingPayment doc, CancellationToken ct)
        {
            var result = await _logic.PostIncomingPayment(doc, ct);
            return StatusCode(result.StatusCode, result.Body);
        }

        [HttpPost("cart")]
        public async Task<IActionResult> PostCartItems([FromBody] Drafts draft, CancellationToken ct)
        {
            // Validate request
            if (draft == null)
                return BadRequest(new { success = false, error = "Cart payload is required." });

            var result = await _items.PostCartItemsAsync(draft, ct);

            if (!result.Body.success)
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    error = result.Body.error ?? "An error occurred while posting cart items."
                });

            return StatusCode(result.StatusCode, new
            {
                success = true,
                DocEntry = result.Body.DocEntry
            });
        }

        [HttpPatch("UpdateCart")]
        public async Task<IActionResult> PutCartItems([FromQuery] int docEntry, [FromBody] Drafts draft, CancellationToken ct)
        {
            // Validate request
            if (draft == null)
                return BadRequest(new { success = false, error = "Cart payload is required." });

            var isDocExist = await _db.GetSingleCartDetails(docEntry);
            if (isDocExist == null || isDocExist.DocEntry != docEntry)
                return NotFound(new { success = false, error = "No existing cart found to update." });

            var result = await _items.PutCartItemsAsync(docEntry, draft, ct);
            if (!result.Body.success)
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    error = result.Body.error ?? "An error occurred while updating cart items."
                });
            return StatusCode(result.StatusCode, new
            {
                success = true
            });
        }
    }
}
