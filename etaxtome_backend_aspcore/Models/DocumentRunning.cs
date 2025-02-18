using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFirestoreApi.Models
{
    [FirestoreData]
    public class DocumentRunning
    {
        [FirestoreProperty("documentTypeLabel")]
        [Required(ErrorMessage = "documentTypeLabel is required.")]
        public string? documentTypeLabel { get; set; }

        [FirestoreProperty("documentRunningName")]
        [Required(ErrorMessage = "documentRunningName is required.")]
        public string? documentRunningName { get; set; }

        [FirestoreProperty("runningFormat")]
        [Required(ErrorMessage = "runningFormat is required.")]
        public string? runningFormat { get; set; }

        [FirestoreProperty("runningLength")]
        [Required(ErrorMessage = "runningLength is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "runningLength must be a positive integer.")]
        public int? runningLength { get; set; }

        [FirestoreProperty("corpId")]
        [Required(ErrorMessage = "corpId is required.")]
        public string? corpId { get; set; }

        [FirestoreProperty("documentType")]
        [Required(ErrorMessage = "documentType is required.")]
        public string? documentType { get; set; }

        [FirestoreProperty("prefix")]
        [Required(ErrorMessage = "prefix is required.")]
        public string? prefix { get; set; }

        [FirestoreProperty("suffix")]
        [Required(ErrorMessage = "suffix is required.")]
        public string? suffix { get; set; }

        [FirestoreProperty("createBy")]
        [Required(ErrorMessage = "createBy is required.")]
        public string? createBy { get; set; }

        [FirestoreProperty("created_by")]
        public string? created_by { get; set; }

        [FirestoreProperty("formId")]
        [Range(1, int.MaxValue, ErrorMessage = "formId must be a positive integer.")]
        public int? formId { get; set; }

        [FirestoreProperty("running")]
        public Dictionary<string, int>? running { get; set; } = new Dictionary<string, int>();

        // ============ วันที่ ============ //

        [FirestoreProperty("updateDate")] 
        public DateTime updateDate { get; set; }  

        [FirestoreProperty("createDate")]
        public DateTime createDate { get; set;}
    }
}
