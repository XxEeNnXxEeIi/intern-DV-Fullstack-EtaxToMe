using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;
using System;

namespace MyFirestoreApi.Models
{
    //[DateRangeValidation(31)] // .ใช้ไม่ได้
    public class DateRangeForQuery
    {
        [DataType(DataType.Date)]
        [Display(Name = "StartDate (YYYY-MM-DD)")]
        public DateTime startDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date (YYYY-MM-DD)")]
        public DateTime endDate { get; set; }

        [Range(1, 20, ErrorMessage = "Limit must be between 1 and 20.")]
        [Display(Name = "Limit List")]
        public int limitList { get; set; } = 20;

        [Range(0, int.MaxValue, ErrorMessage = "Offset must be greater than or equal to 0.")]
        [Display(Name = "Offset")]
        public int offSet { get; set; }

        public DateRangeForQuery()
        {
            if (startDate == default)
            {
                startDate = DateTime.UtcNow.AddDays(-31); 
            }

            if (endDate == default)
            {
                endDate = DateTime.UtcNow; 
            }
        }
    }

    public class DateRangeForReponse
    {
        public int nextOffset { get; set; }
        public int total { get; set; }
    }

    /*
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        private int _maxDaysDifference;

        public DateRangeValidationAttribute(int maxDaysDifference)
        {
            _maxDaysDifference = maxDaysDifference;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Ensure the value is of the expected type
            if (value is not DateRangeForQuery dateRange)
            {
                return new ValidationResult("Invalid type");
            }

            // Check if startDate is after endDate
            if (dateRange.startDate.Date > dateRange.endDate.Date)
            {
                int totalDays = (dateRange.endDate.Date - dateRange.startDate.Date).Days; // Calculate total days
                return new ValidationResult($"Start date cannot be later than end date. Total days: {totalDays}");
            }

            // Check if the date range exceeds the maximum allowed difference
            if ((dateRange.endDate.Date - dateRange.startDate.Date).TotalDays > _maxDaysDifference - 1)
            {
                int totalDays = (dateRange.endDate.Date - dateRange.startDate.Date).Days; // Calculate total days
                return new ValidationResult($"The date range cannot exceed {_maxDaysDifference} days. Total days: {totalDays}");
            }

            // Return success if all checks pass
            return ValidationResult.Success;
        }
    }
    */
}
