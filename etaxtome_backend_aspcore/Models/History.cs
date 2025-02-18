using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFirestoreApi.Models
{
    [FirestoreData]
    public class History
    {
        [FirestoreProperty]
        public string? reason { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "document_request data is required.")]
        public DocumentRequestForPost? data { get; set; }

        [FirestoreProperty]
        public string? actionBy { get; set; }

        [FirestoreProperty]
        public string? action { get; set; }

        [FirestoreProperty]
        public Timestamp timestamp { get; set; } = Timestamp.GetCurrentTimestamp();
    }
}
