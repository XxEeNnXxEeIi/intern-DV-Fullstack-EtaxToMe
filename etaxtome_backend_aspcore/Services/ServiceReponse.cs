namespace MyFirestoreApi.Services
{
    public class ServiceResponse<TData, TMeta>
    {
        public TData? Data { get; set; }
        public TMeta? Meta { get; set; } // This will be used for DateRange or other metadata
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
