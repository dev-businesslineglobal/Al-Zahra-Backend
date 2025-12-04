using BISK_API.Services;
using GardeningAPI.Application.Interfaces;
using GardeningAPI.Model;
using gardnerAPIs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace gardnerAPIs.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    [EnableCors("corsapp")]
    [Authorize]
    public class MastersController : ControllerBase
    {
        private readonly IUserRepository _getData;
        private readonly IItemsService _itemsService;

        public MastersController(IUserRepository getData, IItemsService itemsService)
        {
            _getData = getData;
            _itemsService = itemsService;
        }

        // ======================= Items ========================= //

        [HttpGet("get-items")]
        public async Task<IActionResult> GetItems([FromQuery(Name = "warehouse")] string? whCode = null)
        {
            if (whCode == null)
            {
                whCode = "01"; // default warehouse code
            }
            var result = await _itemsService.GetItemsAsync(whCode);

            if (!result.Success)
            {
                return new JsonResult(new ResponseResult
                {
                    code = 404,
                    message = "Pictures Not found",
                    data = null ?? new { }
                });
            }

            if (string.IsNullOrWhiteSpace(result.FileName))
            {
                return new JsonResult(new ResponseResult
                {
                    code = 500,
                    message = "File generation failed",
                    data = null ?? new { }
                });
            }

            string downloadLink = GenerateDownloadLink(result.FileName, whCode);

            return new JsonResult(new ResponseResult
            {
                code = 200,
                message = "Success",
                data = new { csvDownloadLink = downloadLink }
            });
        }
        // ======================= File Download ========================= //
        [AllowAnonymous]
        [HttpGet("download")]
        public IActionResult DownloadFile([FromQuery] string fileName, [FromQuery] string? whCode = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("fileName is required.");

            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
            string filePath = Path.Combine(basePath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "text/csv", fileName);
        }
        private string GenerateDownloadLink(string fileName, string whCode)
        {
            var http = HttpContext;
            //return $"{http.Request.Scheme}://{http.Request.Host}/api/v1/download?fileName={fileName}";
            return $"{http.Request.Scheme}://{http.Request.Host}/api/v1/download" +
           $"?fileName={fileName}&whCode={whCode}";
        }


        // ======================= Cart ========================= //
        [HttpGet("get/cart")]
        public async Task<IActionResult> GetCart([FromQuery] string cardCode)
        {
            if (string.IsNullOrWhiteSpace(cardCode))
            {
                return BadRequest(new ResponseResult
                {
                    code = 400,
                    message = "CardCode is required.",
                    data = new { } 
                });
            }

            var cart = await _itemsService.GetCartAsync(cardCode);
            //return (cart != null && cart.Count > 0) ? Ok(cart) :NotFound(new object[0]);
            if (cart == null || cart.Count == 0)
            {
                return Ok(new
                {
                    success = false,
                    message = "Cart not found",
                    data = new object[0]
                });
            }

            return Ok(new
            {
                success = true,
                message = "Cart fetched successfully",
                data = cart
            });

        }













        // ======================= File Download (Old) without sending query parameters ========================= //
        //public IActionResult DownloadFile(string fileName)
        //{
        //    string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
        //    string filePath = Path.Combine(basePath, fileName);

        //    if (!System.IO.File.Exists(filePath))
        //        return NotFound("File not found.");

        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

        //    return File(
        //        fileBytes,
        //        "text/csv",
        //        fileName
        //    );
        //}



        //[HttpGet("Students")]
        //public IActionResult GetStudents([FromQuery] string? studentCode = null, [FromQuery] string? academicYear = null)
        //{
        //    var data = _getData.GetStudents(studentCode, academicYear);
        //    return (data != null && data.Count > 0) ? Ok(data) : NotFound(new object[0]);
        //}

        //[HttpGet("StudentInvoices")]
        //public IActionResult GetInvoices(
        //    [FromQuery] string? documentEntry = null,
        //    [FromQuery] string? academicYear = null,
        //    [FromQuery] string? studentCode = null)
        //{
        //    var data = _getData.GetStudentInvoices(documentEntry, academicYear, studentCode);
        //    return (data != null && data.Count > 0) ? Ok(data) : NotFound(new object[0]);
        //}

        //[HttpGet("StudentRefunds")]
        //public IActionResult GetRefunds(
        //    [FromQuery] string? documentEntry = null,
        //    [FromQuery] string? documentStatus = null,
        //    [FromQuery] string? studentCode = null,
        //    [FromQuery] string? invoiceEntry = null,
        //    [FromQuery] string? academicYear = null)
        //{
        //    var data = _getData.GetStudentRefund(documentEntry, documentStatus, studentCode, invoiceEntry, academicYear);
        //    return (data != null && data.Count > 0) ? Ok(data) : NotFound(new object[0]);
        //}

        //[HttpGet("IncomingPayments")]
        //public IActionResult GetIncomingPayments(
        //    [FromQuery] string? invoiceEntry = null,
        //    [FromQuery] string? studentCode = null,
        //    [FromQuery] string? academicYear = null)
        //{
        //    var data = _getData.GetIncomingPayments(invoiceEntry, studentCode, academicYear);
        //    return (data != null && data.Count > 0) ? Ok(data) : NotFound(new object[0]);
        //}

        //[HttpGet("OutgoingPayments")]
        //public IActionResult GetOutgoingPayments(
        //    [FromQuery] string? invoiceEntry = null,
        //    [FromQuery] string? studentCode = null,
        //    [FromQuery] string? academicYear = null)
        //{
        //    var data = _getData.GetOutgoingPayments(invoiceEntry, studentCode, academicYear);
        //    return (data != null && data.Count > 0) ? Ok(data) : NotFound(new object[0]);
        //}
    }
}
