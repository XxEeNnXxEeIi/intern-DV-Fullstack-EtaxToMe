using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using MyFirestoreApi.Models;

public class DocumentRequestSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Handle schema for DocumentRequest
        if (context.Type == typeof(DocumentRequestForPost))
        {
            var exampleRequest = DocumentRequest_Schema.ExampleDocumentRequest;

            //Console.WriteLine("DocumentRequestSchemaFilter applied");

            var filesArray = new OpenApiArray();
            if (exampleRequest.files != null)
            {
                foreach (var file in exampleRequest.files)
                {
                    filesArray.Add(new OpenApiString(file));
                }
            }

            var itemsArray = new OpenApiArray();
            if (exampleRequest.items != null)
            {
                foreach (var item in exampleRequest.items)
                {
                    var itemObject = new OpenApiObject
                    {
                        ["productAmount"] = new OpenApiInteger(item.productAmount ?? 0), // Default to 0 if null
                        ["productId"] = new OpenApiString(item.productId ?? string.Empty), // Default to empty string if null
                        ["productName"] = new OpenApiString(item.productName ?? string.Empty), // Default to empty string if null
                        ["productPrice"] = new OpenApiDouble(item.productPrice ?? 0.0) // Default to 0.0 if null
                    };
                    itemsArray.Add(itemObject);
                }
            }

            schema.Example = new OpenApiObject
            {
                ["documentType"] = exampleRequest.documentType != null ? new OpenApiString(exampleRequest.documentType) : new OpenApiString(string.Empty),
                ["paymentMethod"] = exampleRequest.paymentMethod != null ? new OpenApiString(exampleRequest.paymentMethod) : new OpenApiString(string.Empty),
                ["buyerTaxType"] = exampleRequest.buyerTaxType != null ? new OpenApiString(exampleRequest.buyerTaxType) : new OpenApiString(string.Empty),
                ["buyerTaxId"] = exampleRequest.buyerTaxId != null ? new OpenApiString(exampleRequest.buyerTaxId) : new OpenApiString(string.Empty),
                ["buyerBranchCode"] = exampleRequest.buyerBranchCode != null ? new OpenApiString(exampleRequest.buyerBranchCode) : new OpenApiString(string.Empty),
                ["buyerName"] = exampleRequest.buyerName != null ? new OpenApiString(exampleRequest.buyerName) : new OpenApiString(string.Empty),
                ["buyerEmail"] = exampleRequest.buyerEmail != null ? new OpenApiString(exampleRequest.buyerEmail) : new OpenApiString(string.Empty),
                ["buyerAddress"] = exampleRequest.buyerAddress != null ? new OpenApiString(exampleRequest.buyerAddress) : new OpenApiString(string.Empty),
                ["buyerAddress2"] = exampleRequest.buyerAddress2 != null ? new OpenApiString(exampleRequest.buyerAddress2) : new OpenApiString(string.Empty),
                ["buyerBranchName"] = exampleRequest.buyerBranchName != null ? new OpenApiString(exampleRequest.buyerBranchName) : new OpenApiString(string.Empty),
                ["buyerPostCode"] = exampleRequest.buyerPostCode != null ? new OpenApiString(exampleRequest.buyerPostCode) : new OpenApiString(string.Empty),
                ["buyerPhone"] = exampleRequest.buyerPhone != null ? new OpenApiString(exampleRequest.buyerPhone) : new OpenApiString(string.Empty),
                ["requestBy"] = exampleRequest.requestBy != null ? new OpenApiString(exampleRequest.requestBy) : new OpenApiString(string.Empty),
                ["createdBy"] = exampleRequest.createdBy != null ? new OpenApiString(exampleRequest.createdBy) : new OpenApiString(string.Empty),
                ["billNo"] = exampleRequest.billNo != null ? new OpenApiString(exampleRequest.billNo) : new OpenApiString(string.Empty),
                ["newRequestToken"] = exampleRequest.newRequestToken != null ? new OpenApiString(exampleRequest.newRequestToken) : new OpenApiString(string.Empty),
                ["totalAmount"] = exampleRequest.totalAmount.HasValue ? new OpenApiDouble((double)exampleRequest.totalAmount.Value) : new OpenApiDouble(0.0),
                ["files"] = filesArray,
                ["items"] = itemsArray,
                
                ["billDate"] = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                ["created_at"] = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                ["createDate"] = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                ["updated_at"] = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                ["updateDate"] = new OpenApiString(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
            };
        }
    }
}
