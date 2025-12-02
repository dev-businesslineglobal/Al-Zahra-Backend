using System.Runtime.InteropServices;

namespace GardeningAPI.Model
{

 

 

    public class Document
    {

        public required DateTime DocDate { get; set; }
        public required DateTime DocDueDate { get; set; }
        public required string CardCode { get; set; }
        public string? Comments { get; set; }
        public int? Series { get; set; }

        public DateTime TaxDate { get; set; }

        public required List<Documentline> DocumentLines { get; set; }
    }

    public class Documentline
    {
        public string? ItemCode { get; set; }
        public double Quantity { get; set; }
        public double? UnitPrice { get; set; } = null;
        public double? DiscountPercent { get; set; } = null;

        public string? WarehouseCode { get; set; }
        public int? BaseType { get; set; } = -1;
        public int? BaseEntry { get; set; } = null;
        public int? BaseLine { get; set; } = null;
    }






    public class Response
    {
        public int DocEntry { get; set; }
        public string? DocNum { get; set; }
    }
    public class ResponseBP
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Cellular { get; set; }
    }

    public class ResponseOTP
    {
        public string? CardCode { get; set; }
        public string? Email { get; set; }
        public string? OtpCode { get; set; }
    }
    public class ResponseItem
    {
        public string? CardCode { get; set; }

        public int docEntry { get; set; }
    }





    public class Payments
    {
        public int DocNum { get; set; }
        public string? DocType { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public DateTime TaxDate { get; set; }

        public string? AcademicYear { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? DocTotal { get; set; }
        public string? TrsfrAcct { get; set; }
        public double TrsfrSum { get; set; }
        public string? DocCurr { get; set; }
        public string? Comments { get; set; }
        public int TransId { get; set; }
        public int Series { get; set; }
        public string? VatGroup { get; set; }
        public List<PaymentLines> AppliedDocuments { get; set; } = new List<PaymentLines>();
    }

    public class PaymentLines
    {

        public int InvoiceEntry { get; set; }
        public double SumApplied { get; set; }
        public string? ObjType { get; set; }
        public int DocLine { get; set; }
    }


    public class IncomingPayment
    {
        public required string DocType { get; set; } = "rCustomer";
        public required DateTime DocDate { get; set; }
        public required string CardCode { get; set; }
        public string? CashAccount { get; set; }
        public double? CashSum { get; set; }
        public string? TransferAccount { get; set; }
        public double? TransferSum { get; set; }
        public DateTime? TransferDate { get; set; }
        public required Paymentinvoice[] PaymentInvoices { get; set; }
        public CreditCards[] PaymentCreditCards { get; set; } = Array.Empty<CreditCards>();

    }

    public class CreditCards
    {
        public required int CreditCard { get; set; }
        public required string CreditAcct { get; set; }
        public required string CreditCardNumber { get; set; }
        public required DateTime CardValidUntil { get; set; }
        public  required string VoucherNum { get; set; }
        public required double CreditSum { get; set; }
    }




    public class Paymentinvoice
    {
        public required int DocEntry { get; set; }
        public required double SumApplied { get; set; }
    }

}


