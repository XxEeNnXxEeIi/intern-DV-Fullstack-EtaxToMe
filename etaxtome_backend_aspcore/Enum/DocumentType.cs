using System.ComponentModel;

namespace MyFirestoreApi.Enums
{
    public enum DocumentType
    {
        [Description("ใบแจ้งหนี้")]
        document_invoice_e_document,
        
        [Description("ใบแจ้งหนี้/ใบกำกับภาษี e-tax")]
        document_invoice_e_tax,
        
        [Description("ใบเสร็จรับเงิน/ใบกำกับภาษี")]
        document_receipt_e_document,
        
        [Description("ใบเสร็จรับเงิน/ใบกำกับภาษี")]
        document_receipt_e_document_tax,
        
        [Description("ใบเสร็จรับเงิน/ใบกำกับภาษี e-tax")]
        document_receipt_e_tax,
        
        [Description("ใบลดหนี้")]
        document_credit_note,

        [Description("คำขอเอกสาร")]
        document_request,
    }
}