using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations; // Import for validation attributes
using System;
using System.Collections.Generic;

namespace MyFirestoreApi.Models
{
    [FirestoreData]
    public class EtaxBill
    {
        // ============ รับจาก document_request ============ //

        [FirestoreProperty("runningDocId")]
        [Required(ErrorMessage = "runningDocId is required.")]
        public string? runningDocId { get; set; }

        [FirestoreProperty("documentType")]
        [Required(ErrorMessage = "documentType is required.")]
        public string? documentType { get; set; }

        [FirestoreProperty("docCode")]
        [Required(ErrorMessage = "docCode is required.")]
        public string? docCode { get; set; }

        [FirestoreProperty("paymentMethod")]
        [Required(ErrorMessage = "paymentMethod is required.")]
        public string? paymentMethod { get; set; }

        [FirestoreProperty("corpId")]
        [Required(ErrorMessage = "corpId is required.")]
        public string? corpId { get; set; }

        [FirestoreProperty("corpTaxId")]
        [Required(ErrorMessage = "corpTaxId is required.")]
        public string? corpTaxId { get; set; }

        [FirestoreProperty("buyerEmail")]
        public string? buyerEmail { get; set; }

        [FirestoreProperty("branchCode")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "branchCode must be exactly 5 digits.")]
        public string? branchCode { get; set; }

        [FirestoreProperty("totalAmount")]
        [Required(ErrorMessage = "totalAmount is required.")]
        public double? totalAmount { get; set; }

        [FirestoreProperty("createdBy")]
        [Required(ErrorMessage = "createdBy is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? createdBy { get; set; }

        [FirestoreProperty("status")]
        [Required(ErrorMessage = "status is required.")]
        public string? status { get; set; }

        [FirestoreProperty("items")]
        [Required(ErrorMessage = "items is required.")]
        public List<Item>? items { get; set; }

        [FirestoreProperty("files")]
        public List<string>? files { get; set; }

        // ============ วันที่ ============ //

        [FirestoreProperty("billDate")]
        // 2 อันนี้หล่ะที่ Error เพราะเราใส่ required ก่อน และ ไปกำหนดทีหลัง
        //[Required(ErrorMessage = "billDate is required.")] 
        //[DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime billDate { get; set; }

        [FirestoreProperty("createDate")]
        public DateTime createDate { get; set; }

        // ============ เราต้องทำเอง ============ //

        [FirestoreProperty("thaiBathText")]
        public string? thaiBathText { get; set; }

        [FirestoreProperty("englishBathText")]
        public string? englishBathText { get; set; }
    }
}
