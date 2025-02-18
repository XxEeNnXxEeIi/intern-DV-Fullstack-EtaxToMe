using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFirestoreApi.Models
{
    [FirestoreData]
    public class DocumentRequestForPost
    {   
        [FirestoreProperty("documentType")]
        [Required(ErrorMessage = "documentType is required.")]
        public string? documentType { get; set; }

        [FirestoreProperty("runningDocId")]
        public string? runningDocId { get; set; }

        [FirestoreProperty("items")]
        [Required(ErrorMessage = "items is required.")]
        public List<Item>? items { get; set; } 

        [FirestoreProperty("paymentMethod")]
        [Required(ErrorMessage = "paymentMethod is required.")]
        public string? paymentMethod { get; set; }

        // ============ รับจาก corpData ============ //

        [FirestoreProperty("corpTaxId")] 
        public string? corpTaxId  { get; set; } 

        [FirestoreProperty("branchCode")] 
        public string? branchCode { get; set; }

        [FirestoreProperty("corpId")]
        public string? corpId { get; set; }

        [FirestoreProperty("createdBy")]
        public string? createdBy { get; set; }

        // ============ รับจากผู้ใช้ ============ //

        [FirestoreProperty("buyerTaxId")]
        [Required(ErrorMessage = "buyerTaxId is required.")]
        public string? buyerTaxId  { get; set; }
        
        [FirestoreProperty("buyerName")]
        [Required(ErrorMessage = "buyerName is required.")]
        public string? buyerName { get; set; }

        [FirestoreProperty("buyerPhone")]
        [Required(ErrorMessage = "buyerPhone is required.")]
        public string? buyerPhone { get; set; }

        [FirestoreProperty("buyerEmail")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? buyerEmail  { get; set; } 

        [FirestoreProperty("buyerAddress")]
        public string? buyerAddress  { get; set; }

        [FirestoreProperty("buyerAddress2")]
        public string? buyerAddress2  { get; set; }

        [FirestoreProperty("buyerPostCode")]
        public string buyerPostCode { get; set; } = string.Empty;

        [FirestoreProperty("requestBy")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string requestBy { get; set; } = string.Empty; // ผู้จัดทำ

        [FirestoreProperty("billNo")]
        public string billNo { get; set; } = string.Empty; // เลขที่อ้างอิงค์

        [FirestoreProperty("files")]
        public List<string> files { get; set; } = new List<string>(); // ไฟล์

        // ============ วันที่ ============ //

        [FirestoreProperty("billDate")]
        public DateTime? billDate { get; set; } = DateTime.UtcNow;

        [FirestoreProperty("createDate")]
        public DateTime? createDate { get; } = DateTime.UtcNow;

        // สำหรับ PUT หรือ Update

        [FirestoreProperty("updateDate")]
        public DateTime? updateDate { get; } = DateTime.UtcNow;

        // ============ เราต้องทำเอง ============ //

        [FirestoreProperty("docCode")]
        public string? docCode { get; set; }

        [FirestoreProperty("newRequestToken")]
        public string? newRequestToken { get; set; }

        [FirestoreProperty("status")]
        public string? status { get; set; }

        [FirestoreProperty("totalAmount")]
        public double? totalAmount { get; set; }

        // ============ กรณีเฉพาะพิเศษ ============ //

        [FirestoreProperty("buyerTaxType")]
        [Required(ErrorMessage = "buyerTaxType is required.")]
        public string? buyerTaxType  { get; set; }


        [FirestoreProperty("buyerBranchCode")]
        public string? buyerBranchCode { get; set; }

        [FirestoreProperty("buyerBranchName")]
        public string? buyerBranchName  { get; set; } 

        /*
        public IEnumerable<ValidationResult> Validate_buyerTaxType(ValidationContext validationContext)
        {
            if (validationContext is null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            if (buyerTaxType == "TXID")
            {
                if (string.IsNullOrWhiteSpace(buyerBranchCode) || !System.Text.RegularExpressions.Regex.IsMatch(buyerBranchCode, @"^\d{5}$"))
                {
                    yield return new ValidationResult("buyerBranchCode is required and must be exactly 5 digits when buyerTaxType is 'TXID'.", new[] { nameof(buyerBranchCode) });
                }

                if (string.IsNullOrWhiteSpace(buyerBranchName) || !System.Text.RegularExpressions.Regex.IsMatch(buyerBranchName, @"^\p{L}+$"))
                {
                    yield return new ValidationResult("buyerBranchName must contain only letters and no spaces.", new[] { nameof(buyerBranchName) });
                }

            }

            else
            
            {
                if (!string.IsNullOrWhiteSpace(buyerBranchCode))
                {
                    yield return new ValidationResult("buyerBranchCode must be null or empty when buyerTaxType is not 'TXID'.", new[] { nameof(buyerBranchCode) });
                }

                if (!string.IsNullOrWhiteSpace(buyerBranchName))
                {
                    yield return new ValidationResult("buyerBranchName must be null or empty when buyerTaxType is not 'TXID'.", new[] { nameof(buyerBranchName) });
                }
            }
        }
        */

        public IEnumerable<ValidationResult> Validate_documentType(ValidationContext validationContext)
        {
            var validDocumentTypes = new HashSet<string>
            {
                "document_invoice_e_document",
                "document_invoice_e_tax",
                "document_receipt_e_document_tax",
                "document_receipt_e_tax",
                "document_credit_note"
            };

            if (documentType != null && !validDocumentTypes.Contains(documentType))
            {
                yield return new ValidationResult("Invalid document type. Allowed values are: document_invoice_e_document, document_invoice_e_tax, document_receipt_e_document_tax, document_receipt_e_tax, document_credit_note.", new[] { nameof(documentType) });
            }
        }
    }

    [FirestoreData(ConverterType = typeof(CustomDocumentRequestForGetConverter))]
    public class DocumentRequestForGet
    {
        [FirestoreProperty("documentType")]
        public string? documentType  { get; set; }

        [FirestoreProperty("runningDocId")]
        public string? runningDocId { get; set; }

        [FirestoreProperty("paymentMethod")]
        public string? paymentMethod { get; set; } 

        // ============ รับจากผู้ใช้ ============ //

        [FirestoreProperty("buyerTaxId")]
        [Required(ErrorMessage = "buyerTaxId is required.")]
        public string? buyerTaxId  { get; set; }

        [FirestoreProperty("buyerTaxType")]
        public string? buyerTaxType  { get; set; }

        [FirestoreProperty("buyerBranchCode")]
        public string? buyerBranchCode { get; set; }

        [FirestoreProperty("buyerBranchName")]
        public string? buyerBranchName  { get; set; } 
        
        [FirestoreProperty("buyerName")]
        [Required(ErrorMessage = "buyerName is required.")]
        public string? buyerName { get; set; }

        [FirestoreProperty("buyerPhone")]
        [Required(ErrorMessage = "buyerPhone is required.")]
        public string? buyerPhone { get; set; }

        [FirestoreProperty("buyerEmail")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? buyerEmail  { get; set; } 

        [FirestoreProperty("buyerAddress")]
        public string? buyerAddress  { get; set; }

        [FirestoreProperty("buyerAddress2")]
        public string? buyerAddress2  { get; set; }

        [FirestoreProperty("buyerPostCode")]
        public string? buyerPostCode { get; set; }

        [FirestoreProperty("requestBy")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? requestBy { get; set; } // ผู้จัดทำ

        [FirestoreProperty("createdBy")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? createdBy { get; set; }

        [FirestoreProperty("billNo")]
        public string? billNo { get; set; } // เลขที่อ้างอิงค์

        [FirestoreProperty("files")]
        public List<string>? files { get; set; } // ไฟล์

        [FirestoreProperty("items")]
        public List<Item>? items { get; set; } 

        [FirestoreProperty("totalAmount")]
        public double? totalAmount { get; set; }

        [FirestoreProperty("status")]
        public string? status { get; set; }

        // ============ วันที่ ============ //

        // อันนี้คือถูกต้องแล้ว
        
        [FirestoreProperty("billDate")]
        public DateTime? billDate { get; set; } = DateTime.MinValue.ToUniversalTime();

        [FirestoreProperty("createDate")]
        public DateTime? createDate { get; set; } = DateTime.MinValue.ToUniversalTime();

        [FirestoreProperty("updateDate")]
        public DateTime? updateDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    }

    public class CustomDocumentRequestForGetConverter : IFirestoreConverter<DocumentRequestForGet>
    {
        public object ToFirestore(DocumentRequestForGet value)
        {
            return new
            {
                value.documentType,
                value.runningDocId,
                value.paymentMethod,
                value.buyerTaxId,
                value.buyerTaxType,
                value.buyerBranchCode,
                value.buyerBranchName,
                value.buyerName,
                value.buyerPhone,
                value.buyerEmail,
                value.buyerAddress,
                value.buyerAddress2,
                value.buyerPostCode,
                value.requestBy,
                value.createdBy,
                value.billNo,
                value.files,
                value.items,
                value.totalAmount,
                value.status,
                billDate = value.billDate?.ToUniversalTime() ?? DateTime.MinValue.ToUniversalTime(),
                createDate = value.createDate?.ToUniversalTime() ?? DateTime.MinValue.ToUniversalTime(),
                updateDate = value.updateDate?.ToUniversalTime() ?? DateTime.MinValue.ToUniversalTime()
            };
        }

        public DocumentRequestForGet FromFirestore(object value)
        {
            if (value is IDictionary<string, object> map)
            {
                // Original files list
                var originalFiles = map.ContainsKey("files") && map["files"] is List<object> filesListx
                    ? filesListx
                    : null;

                // Deserialize files
                var files = originalFiles != null
                    ? JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(originalFiles))
                    : new List<string>();

                // Log original and deserialized files
                //Console.WriteLine("Original Files: " + (originalFiles != null ? JsonConvert.SerializeObject(originalFiles) : "null"));
                //Console.WriteLine("Deserialized Files: " + JsonConvert.SerializeObject(files));

                // Original items list
                var originalItems = map.ContainsKey("items") && map["items"] is List<object> itemListx
                    ? itemListx
                    : null;

                // Deserialize items
                var items = originalItems != null
                    ? JsonConvert.DeserializeObject<List<Item>>(JsonConvert.SerializeObject(originalItems))
                    : new List<Item>();

                // Log original and deserialized items
                //Console.WriteLine("Original Items: " + (originalItems != null ? JsonConvert.SerializeObject(originalItems) : "null"));
                //Console.WriteLine("Deserialized Items: " + JsonConvert.SerializeObject(items));

                return new DocumentRequestForGet
                {
                    documentType = map.ContainsKey("documentType") ? map["documentType"] as string : null,
                    runningDocId = map.ContainsKey("runningDocId") ? map["runningDocId"] as string : null,
                    paymentMethod = map.ContainsKey("paymentMethod") ? map["paymentMethod"] as string : null,
                    buyerTaxId = map.ContainsKey("buyerTaxId") ? map["buyerTaxId"] as string : null,
                    buyerTaxType = map.ContainsKey("buyerTaxType") ? map["buyerTaxType"] as string : null,
                    buyerBranchCode = map.ContainsKey("buyerBranchCode") ? map["buyerBranchCode"] as string : null,
                    buyerBranchName = map.ContainsKey("buyerBranchName") ? map["buyerBranchName"] as string : null,
                    buyerName = map.ContainsKey("buyerName") ? map["buyerName"] as string : null,
                    buyerPhone = map.ContainsKey("buyerPhone") ? map["buyerPhone"] as string : null,
                    buyerEmail = map.ContainsKey("buyerEmail") ? map["buyerEmail"] as string : null,
                    buyerAddress = map.ContainsKey("buyerAddress") ? map["buyerAddress"] as string : null,
                    buyerAddress2 = map.ContainsKey("buyerAddress2") ? map["buyerAddress2"] as string : null,
                    buyerPostCode = map.ContainsKey("buyerPostCode") ? map["buyerPostCode"] as string : null,
                    requestBy = map.ContainsKey("requestBy") ? map["requestBy"] as string : null,
                    createdBy = map.ContainsKey("createdBy") ? map["createdBy"] as string : null,
                    billNo = map.ContainsKey("billNo") ? map["billNo"] as string : null,
                    files = map.ContainsKey("files") && map["files"] is List<object> filesList 
                        ? JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(filesList)) 
                        : new List<string>(),

                    items = map.ContainsKey("items") && map["items"] is List<object> itemList 
                        ? JsonConvert.DeserializeObject<List<Item>>(JsonConvert.SerializeObject(itemList)) 
                        : new List<Item>(),
                    totalAmount = map.ContainsKey("totalAmount") ? Convert.ToDouble(map["totalAmount"]) : (double?)null,
                    status = map.ContainsKey("status") ? map["status"] as string : null,
                    billDate = ParseDate(map, "billDate"),
                    createDate = ParseDate(map, "createDate"),
                    updateDate = ParseDate(map, "updateDate")
                };
            }

            throw new ArgumentException($"Unexpected data: {value.GetType()}");
        }

        private DateTime ParseDate(IDictionary<string, object> map, string key)
        {
            return map.ContainsKey(key) && map[key] is Timestamp timestamp 
                ? timestamp.ToDateTime().ToUniversalTime() 
                : DateTime.MinValue.ToUniversalTime();
        }
    }

    public class DocumentRequest_Schema
    {
        public static DocumentRequestForPost ExampleDocumentRequest => new DocumentRequestForPost
        {
            documentType = "document_invoice_e_document",
            paymentMethod = "CASH",
            buyerTaxType = "TXID",
            buyerTaxId = "1234567891011",
            buyerBranchCode = "00000",
            buyerName = "ชื่อนามสกุลทดสอบ",
            buyerEmail = "buyerEmail_อีเมลผู้รับ@hotmail.com",
            buyerAddress = "447/55 ถนนราชปรารภทดสอบ",
            buyerAddress2 = "574/27 หมู่บ้านแอนเนกซ์ทดสอบ",
            buyerBranchName = "สำนักงานใหญ่ทดสอบ",
            buyerPostCode = "10400",
            buyerPhone = "029936113",
            requestBy = "requestBy@hotmail.com",
            billNo = "1110987654321",
            newRequestToken = "newRequestToken",
            totalAmount = 2469,
            files = new List<string> { "string" },
            items = new List<Item>
            {
                new Item
                {
                    productAmount = 2,
                    productId = "0001",
                    productName = "ทดสอบ",
                    productPrice = 1234.5
                }
            },

            //billDate = DateTime.UtcNow, // billDate ผู้ใช้ต้องใส่เอง ตาม "รูปภาพใบเอกสาร" ที่อัพโหลดขึ้นไป
            //created_at = DateTime.UtcNow,
            //createDate = DateTime.UtcNow, // ห้ามแก้ไขภายหลัง
            //updated_at = DateTime.UtcNow,
            //updateDate = DateTime.UtcNow,
        };
    }
}