using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyFirestoreApi.Models
{
    [FirestoreData]
    public class Item
    {
        [FirestoreProperty]
        [Required(ErrorMessage = "productAmount is required.")]
        public int? productAmount { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "productId is required.")]
        public string? productId { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "productName is required.")]
        public string? productName { get; set; }

        [FirestoreProperty]
        [Required(ErrorMessage = "productPrice is required.")]
        public double? productPrice { get; set; }

        [FirestoreProperty]
        public double productTotal 
        { 
            get 
            {
                if (productPrice.HasValue && productAmount.HasValue)
                {
                    return productPrice.Value * productAmount.Value;
                }
                return 0;
            } 
        }
    }
}
