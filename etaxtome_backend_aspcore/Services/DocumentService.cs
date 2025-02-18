using Google.Cloud.Firestore;
using MyFirestoreApi.Models;
using MyFirestoreApi.Enums;
using DateTime = System.DateTime;
using Enum = System.Enum;
using System.Reflection;
using System;

namespace MyFirestoreApi.Services
{
    public class DocumentService
    {
        private FirestoreDb _firestoreDb;
        private CorpService _corpService = new CorpService();
        private FireStoreService _fireStoreService = new FireStoreService();
        public DocumentService()
        {
            _firestoreDb = _fireStoreService.GetFirestoreDb();
        }
        public async Task<bool> AddDocumentRequestAsync(DocumentRequestForPost documentRequest, string actionMethod) 
        {
            // ------------------------------ เอา corpId ไปดึง corpData มาเก็บไว้ก่อน ------------------------------ //    
            try
            {
                // 1. ดึง corpData มาเก็บไว้ก่อน

                var corpCollectionId = string.Empty;

                if (documentRequest.corpId != null)
                {
                    corpCollectionId = documentRequest.corpId;
                }

                var corpData = await _corpService.GetCorpDataByCorpIdAsync(corpCollectionId);

                // 2 ใส่ค่าที่ได้มาจาก corpData บาง field ที่จำเป็น

                documentRequest.branchCode = corpData.ContainsKey("branchCode") ? corpData["branchCode"] as string : "00000";
                documentRequest.corpTaxId = corpData.ContainsKey("taxId") ? corpData["taxId"] as string : null;
                documentRequest.totalAmount = (float?)(documentRequest.items?.Sum(item => item.productTotal) ?? 0);

                // 3. กำหนด default ของ document_running

                string defaultStatus = "New";
                string defaultPrefix = "DOC";
                string defaultSuffix = "DOC";

                var defaultDocumentRunningName = Enum.TryParse<DocumentType>(documentRequest.documentType, out var documentTypeParseForDocumentRunningName)
                    ? documentTypeParseForDocumentRunningName.GetDescription()
                    : string.Empty;

                string defaultYearMonth = "yyyyMM";
                string defaultYearMonthForEnum = "YYYYMM";
                string defaultYearMonthToDate = DateTime.Now.ToString(defaultYearMonth); //202409
                int defaultRunningLength = 4;

                // 4. เอา documentRequest มาสร้างใหม่ และ ใส่ค่าเพิ่มลงไป

                var newDocumentRequest = documentRequest;

                // 5. กำหนด collection ที่จะเข้าถึง

                var documentRequestCllection = _firestoreDb.Collection("document_request");

                // สุ่ม collection Id สำหรับ document_request 
                var documentRequestRef = documentRequestCllection.Document();

                // ค้นหาใน document_running ที่ documentType และ corpId ตรงกัน
                var docRunningCollection = _firestoreDb.Collection("document_running");
                var query = docRunningCollection
                    .WhereEqualTo("documentType", documentRequest.documentType)
                    .WhereEqualTo("corpId", documentRequest.corpId);

                // จำ collection ที่ matching มา
                var querySnapshot = await query.GetSnapshotAsync();

                // ตัวแปรที่ต้องใช้
                
                string docCodeNewNew = string.Empty;
                string runningDocId = string.Empty;

                // ============ ให้หาใน document_running ก่อนโดย documentType, corpId ตรงกัน ============ //

                // A.ถ้าเจอที่ตรงกันใน document_running
                if (querySnapshot.Documents.Count > 0)
                {
                    // เลือกที่เจอเป็นอันแรก อาจจะมีหลายอันแต่ควรมีได้แค่ 1 อัน แต่เผื่อไว้
                    var docRunning = querySnapshot.Documents[0];

                    // runningDocId คือ collection Id ของอันนี้
                    runningDocId = querySnapshot.First().Id;

                    // docCode ให้หาใน running 
                    var running = docRunning.GetValue<Dictionary<string, int>>("running");
                    if (running != null)
                    {
                        // Aa. ถ้าหาเจอ
                        if (running.TryGetValue(defaultYearMonthToDate, out int runningValue)) // นำ yyyyMM ในปัจจุบันไปหาใน Key ของ running ที่เป็น Dict
                        {
                            Console.WriteLine($"Found matching entry for {defaultYearMonthToDate}: {runningValue}");

                            // 1. อัพเดทใน running นั้นของ document_running
                            running[defaultYearMonthToDate] = runningValue + 1;
                            await docRunning.Reference.UpdateAsync("running", running);
                            Console.WriteLine($"Updating running in DocumentRunning ID: {runningDocId}, Data: {running.ToString}");

                            // 2. นำ prefix + running.yyyyMM + (runningLength + running.Int) ไปใส่ใน docCode ใหม่
                            if (running.TryGetValue(defaultYearMonthToDate, out int runningValueAfterUpdate)) // ดึง yyyyMM ที่อัพเดทแล้วใน running นั้นออกมา
                            {
                                int runningLength = docRunning.GetValue<int>("runningLength");
                                int runningNewValue = running[defaultYearMonthToDate];
                                string runningNewValueAdd0 = runningNewValue.ToString("D" + runningLength);
                                Console.WriteLine($"runningAdd0: {runningNewValueAdd0}");
                                docCodeNewNew = docRunning.GetValue<string>("prefix") + defaultYearMonthToDate + runningNewValueAdd0; // สร้าง docCode ใหม่ที่อัพเดทตามใน running
                            }
                        }
                        // Ab. ถ้าหาไม่เจอ
                        else
                        {
                            Console.WriteLine($"No matching entry found for {defaultYearMonthToDate}");

                            // 1. ให้ update running โดย running.yymm ปัจจุบัน: 1
                            running[defaultYearMonthToDate] = 1;
                            await docRunning.Reference.UpdateAsync("running", running);
                            Console.WriteLine($"Add running in DocumentRunning ID: {docRunningCollection.Id}, Data: {running}");

                            // 2. สร้าง docCode ใหม่โดย prefix, runningLength, runningFormat  ใส่ใน docCode

                            // ดึงค่า prefix และ runningLength ออกมา
                            string prefix = docRunning.GetValue<string>("prefix");
                            int runningLength = docRunning.GetValue<int>("runningLength");
                            int incrementedValue = docRunning.GetValue<int>("running") + 1;
                            string formattedValue = incrementedValue.ToString("D" + runningLength);

                            docCodeNewNew = $"{prefix}{defaultYearMonthToDate}{formattedValue}";
                        }
                    }
                    else
                    {
                        Console.WriteLine("The 'running' field is null or does not exist.");
                    }
                    Console.WriteLine($"Matching DocumentRunning ID: {runningDocId}");
                }

                // B. ถ้าไม่เจอที่ตรงกันใน document_running
                else
                {
                    // 1. POST ใน document_running 
                    var documentRunningCollection = _firestoreDb.Collection("document_running");
                    var documentRunningRef = documentRunningCollection.Document();

                    // แก้ไข 5/9/2567 กัปตันให้ใช้ document_request เป็น documentType แทน
                    //var documentType = newDocumentRequest.documentType; 
                    var documentType = "document_request";

                    var documentTypeDescription = string.Empty; // เก็บภาษาไทยของ documentType
                    var runningLengthDescription = string.Empty; // เก็บภาษาไทยของ runningFormat

                    // ใช้สำหรับ ดึง Descriptstion Enum ของ documentType
                    if (Enum.TryParse<DocumentType>(documentType, out var documentTypeParse))
                    {
                        var description = documentTypeParse.GetDescription();
                        documentTypeDescription = description;
                    }

                    // ใช้สำหรับ ดึง Descriptstion Enum ของ runningFormat
                    if (Enum.TryParse<RunningType>(defaultYearMonthForEnum, out var runningLengthParse))
                    {
                        var description = runningLengthParse.GetDescription();
                        runningLengthDescription = description;
                    }

                    // สร้าง model ใหม่ และ ใส่ข้อมูลที่จำเป็นลงไป
                    var newDocumentRunning = new DocumentRunning
                    {
                        documentType = documentType, // ภาษาอังกฤษของ documentType
                        documentTypeLabel = documentTypeDescription, // ภาษาไทยของ documentType
                        documentRunningName = documentTypeDescription,
                        runningFormat = defaultYearMonthForEnum,
                        runningLength = defaultRunningLength,
                        corpId = corpCollectionId,
                        prefix = defaultPrefix,
                        suffix = defaultSuffix,
                        createBy = corpData.ContainsKey("corpEmail") ? corpData["corpEmail"] as string : "createBy_ButFailToReadFromCorpData@hotmail.com", // ผิดเพราะใน document_request เราต้องให้ user พิมพ์เอง
                        running = new Dictionary<string, int> { [defaultYearMonthToDate] = 1 }
                        //created_by = "createdBy@hotmail.com",
                        //formId = 999999999, // เป็น Int ขั้นต่ำ 9 ตัว
                    };

                    // POST เข้าไปเลย
                    await documentRunningRef.CreateAsync(newDocumentRunning);

                    // 2. สร้าง docCode ใหม่ตาม default และ ใส่ไปใน docCode ของ document_request

                    int newRunning = 1; // สร้างเลขรันนิ่งใหม่
                    string runningAdd0 = newRunning.ToString().PadLeft(defaultRunningLength, '0');

                    var docCodeDefault = $"{defaultPrefix}{defaultYearMonthToDate}{runningAdd0}";
                    docCodeNewNew = docCodeDefault;

                    // 3. ใส่ collection id ของ POST ใน runningDocId ของ document_request
                    runningDocId = documentRunningRef.Id;

                    Console.WriteLine($"New [POST] DocumentRunning ID: {documentRunningRef.Id}");
                }

                // ===== เมื่อจัดการกับ docCode และ runningDocId เสร็จ ===== //
                newDocumentRequest.runningDocId = runningDocId;
                newDocumentRequest.docCode = docCodeNewNew;
                newDocumentRequest.status = defaultStatus;

                // 2. POST ลงไปใน document_request
                await documentRequestRef.CreateAsync(newDocumentRequest);
                Console.WriteLine($"New [POST] DocumentRequesting ID: {documentRequestRef.Id}");

                // ------------------------------ สำหรับ history ------------------------------ //  

                await AddHistoryAsync(newDocumentRequest, documentRequestRef, actionMethod);

                // ------------------------------ สำหรับ etax_bill ------------------------------ //        

                //await CreateEtaxBillAsync(newDocumentRequest, runningDocId, docCodeNewNew, corpData, documentRequestRef);  
                
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        private async Task AddHistoryAsync(DocumentRequestForPost newDocumentRequest, DocumentReference documentRequestRef, string actionMethod)
        {
            var subcollection = documentRequestRef.Collection("history");
            var subdocumentRef = subcollection.Document();
            
            var newHistory = new History
            {
                reason = actionMethod == "POST" ? "Create Document" : "Update Document",
                actionBy = newDocumentRequest.requestBy,
                data = newDocumentRequest,
                action = actionMethod,
            };
            
            await subdocumentRef.CreateAsync(newHistory);

            // Print the actual History document ID too if needed
            Console.WriteLine($"History ID: {subdocumentRef.Id}");
        }

        private async Task CreateEtaxBillAsync(DocumentRequestForPost newDocumentRequest, string runningDocId, string docCodeNewNew, Dictionary<string, object> corpData, DocumentReference documentReference)
        {
            // Instantiate the converters
            var englishConverter = new NumberToEnglishWordConverter();
            var thaiConverter = new NumberToThaiWordConverter();

            // Calculate total amount
            double totalAmount = newDocumentRequest.items?.Sum(item => item.productTotal) ?? 0;

            // Convert total amount to words
            var englishTotalWords = englishConverter.ConvertToWords(totalAmount);
            var thaiTotalWords = thaiConverter.ConvertToWords(totalAmount);

            // Retrieve corp tax ID
            var corpTaxId = corpData.ContainsKey("corpId") ? corpData["corpId"] as string : string.Empty;

            // Create the EtaxBill object
            var newEtaxBill = new EtaxBill
            {
                runningDocId = runningDocId,
                docCode = docCodeNewNew,
                documentType = newDocumentRequest.documentType,
                billDate = newDocumentRequest.billDate ?? DateTime.UtcNow,
                createDate = newDocumentRequest.createDate ?? DateTime.UtcNow,
                corpId = newDocumentRequest.corpId,
                corpTaxId = corpTaxId,
                branchCode = newDocumentRequest.branchCode,
                buyerEmail = newDocumentRequest.buyerEmail,
                totalAmount = newDocumentRequest.totalAmount,
                englishBathText = englishTotalWords,
                thaiBathText = thaiTotalWords,
                files = newDocumentRequest.files,
                items = newDocumentRequest.items,
                status = newDocumentRequest.status,
            };

            // Get the Firestore collection and document reference
            var etaxCollection = _firestoreDb.Collection("etax_bill");
            var etaxDocumentRef = etaxCollection.Document(documentReference.Id); // Use the document ID from the reference

            // Create the EtaxBill document in Firestore
            await etaxDocumentRef.CreateAsync(newEtaxBill);

            Console.WriteLine($"EtaxBill ID: {etaxDocumentRef.Id}");
        }

        // ============================== สำหรับ PUT  ============================== //
        
        public async Task<bool> UpdateDocumentRequestAsync(DocumentRequestForPost documentRequest, string actionMethod)
        {
            try
            {
                var documentsRef = _firestoreDb.Collection("document_request");

                // Retrieve the document snapshot
                var querySnapshot = await documentsRef
                    .WhereEqualTo("corpId", documentRequest.corpId)
                    .WhereEqualTo("documentType", documentRequest.documentType)
                    .WhereEqualTo("docCode", documentRequest.docCode)
                    .GetSnapshotAsync();

                // Get the first document from the query result
                var document = querySnapshot.Documents.FirstOrDefault();

                if (document != null)
                {
                    // Retrieve existing document data
                    var existingData = document.ConvertTo<DocumentRequestForPost>();

                    // ลงวันที่อัพเดทเป็นปัจจุบัน
                    //existingData.updateDate = DateTime.UtcNow;
                    //existingData.updated_at = DateTime.UtcNow; เลิกใช้

                    // Update each property if it has a setter
                    foreach (var property in typeof(DocumentRequestForPost).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        // Ensure property has a setter
                        if (property.CanWrite)
                        {
                            var newValue = property.GetValue(documentRequest);
                            var existingValue = property.GetValue(existingData);

                            // Special handling for List<string> and List<Item>
                            if (property.PropertyType == typeof(List<string>) || property.PropertyType == typeof(List<Item>))
                            {
                                if (!Equals(newValue, existingValue))
                                {
                                    Console.WriteLine($"Field '{property.Name}' mismatch: existing: {existingValue}, new: {newValue}");
                                    property.SetValue(existingData, newValue);
                                }
                            }
                            else
                            {
                                // ตอนเรา Update อ่ะ เราจะไม่ Update ตัว created_at และ createDate นะ อย่าลืมแก้
                                if (newValue != null && !newValue.Equals(existingValue))
                                {
                                    Console.WriteLine($"Field '{property.Name}' mismatch: existing: {existingValue}, new: {newValue}");
                                    property.SetValue(existingData, newValue);
                                }
                            }
                        }
                    }

                    // Update the document with the modified data
                    await document.Reference.SetAsync(existingData, SetOptions.MergeAll);

                    Console.WriteLine($"Update [PUT] DocumentRequest ID: {document.Id}");

                    // Create and add history
                    var newHistory = new History
                    {
                        reason = actionMethod == "PUT" ? "Update Document" : "Incorrect method",
                        actionBy = documentRequest.requestBy,
                        data = documentRequest,
                        action = actionMethod
                    };

                    var historyCollection = document.Reference.Collection("history");
                    var historyDocument = historyCollection.Document();
                    await historyDocument.SetAsync(newHistory);

                    Console.WriteLine($"History ID: {historyDocument.Id}");

                    return true;
                }
                else
                {
                    Console.WriteLine("Document not found.");
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        // ============================== สำหรับ GET  ============================== //

        public async Task<ServiceResponse<List<DocumentRequestForGet>, DateRangeForReponse>> GetDocumentDataByCorpIdAndDateRangeAsync(
            string corpCollectionId, DateRangeForQuery dateRangeSchema)
        {
            var serviceResponse = new ServiceResponse<List<DocumentRequestForGet>, DateRangeForReponse>();

            try
            {
                // Validate date range
                if (!ValidateDateRange(dateRangeSchema, out var validationMessage, out var validationCode))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = validationMessage ?? string.Empty;
                    serviceResponse.Meta = null;
                    return serviceResponse;
                }

                // Get all document_request that match
                var documentsRef = _firestoreDb.Collection("document_request");
                var corpQuerySnapshot = await documentsRef
                    .WhereEqualTo("corpId", corpCollectionId)
                    .GetSnapshotAsync();

                // Check if any documents were retrieved
                if (corpQuerySnapshot.Documents.Count > 0)
                {
                    foreach (var document in corpQuerySnapshot.Documents)
                    {
                        //Console.WriteLine($"Document ID: {document.Id}");
                        
                        // Iterate through each field in the document
                        foreach (var field in document.ToDictionary())
                        {
                            if(field.Key == "billDate" || field.Key == "createDate" || field.Key == "updateDate" )
                            {
                                //Console.WriteLine($"{field.Key}: {field.Value}");
                            }
                            
                        }
                        Console.WriteLine(); // Add a new line for better readability between documents
                    }
                }
                else
                {
                    Console.WriteLine("No documents found.");
                }

                // Convert the Firestore documents to DocumentRequestForGet
                var documents = corpQuerySnapshot.Documents
                    .Select(doc => doc.ConvertTo<DocumentRequestForGet>())
                    .ToList();

                // Filter and paginate documents
                var (filteredDocuments, dateRangeMetadata) = FilterDocuments(documents, dateRangeSchema);
                var (paginatedDocuments, updatedDateRange) = PaginateDocuments(filteredDocuments, dateRangeSchema);

                if (!paginatedDocuments.Any())
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No documents found for the provided corpId.";
                    serviceResponse.Meta = new DateRangeForReponse { nextOffset = updatedDateRange.nextOffset, total = paginatedDocuments.Count };
                    return serviceResponse;
                }

                // Set the response data
                serviceResponse.Data = paginatedDocuments;
                serviceResponse.Meta = new DateRangeForReponse { nextOffset = updatedDateRange.nextOffset, total = filteredDocuments.Count };
                serviceResponse.Success = true;
                serviceResponse.Message = "Documents retrieved successfully.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error retrieving documents: {ex.Message}";
                serviceResponse.Meta = null;
            }

            return serviceResponse;
        }

        // ============================== Reusable Methods ============================== //

        public bool ValidateDateRange(DateRangeForQuery dateRangeSchema, out string? validationMessage, out int validationCode)
        {
            DateTime startDateTime = dateRangeSchema.startDate.Date;
            DateTime endDateTime = dateRangeSchema.endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            TimeSpan maxDateRange = TimeSpan.FromDays(31);
            int maxLimit = 20;

            (validationMessage, validationCode) = (startDateTime > endDateTime) switch
            {
                true => ("Start date cannot be after end date.", 1),
                _ => (endDateTime - startDateTime > maxDateRange) switch
                {
                    true => ($"Date range cannot exceed {maxDateRange.Days} days.", 2),
                    _ => (dateRangeSchema.limitList > maxLimit) switch
                    {
                        true => ($"Limit cannot exceed {maxLimit} items.", 3),
                        _ => (null, 0)
                    }
                }
            };
            return validationCode == 0;
        }

        public (List<DocumentRequestForGet> Items, DateRangeForReponse Meta) FilterDocuments(
            List<DocumentRequestForGet> documents, DateRangeForQuery dateRangeSchema)
        {
            // Filter documents in-memory based on date range
            var filteredDocuments = documents
            .Where(doc => doc.createDate != DateTime.MinValue
                        && doc.createDate >= dateRangeSchema.startDate.Date // Ensure full day comparison
                        && doc.createDate <= dateRangeSchema.endDate.Date.AddDays(1).AddTicks(-1)) // Include end of day
            .ToList();

            // Create DateRangeForReponse metadata
            var total = filteredDocuments.Count;
            var nextOffset = dateRangeSchema.offSet + dateRangeSchema.limitList;
            if (nextOffset >= total)
            {
                nextOffset = -1; // No more pages
            }

            var dateRangeForReponse = new DateRangeForReponse
            {
                nextOffset = nextOffset,
                total = total
            };

            return (filteredDocuments, dateRangeForReponse);
        }

        public (List<DocumentRequestForGet> Items, DateRangeForReponse DateRange) PaginateDocuments(
            List<DocumentRequestForGet> filteredDocuments, DateRangeForQuery dateRangeSchema)
        {
            // Validate limitList and offSet
            if (dateRangeSchema.limitList < 0) dateRangeSchema.limitList = 0;
            if (dateRangeSchema.offSet < 0) dateRangeSchema.offSet = 0;
                    
            // Apply pagination
            var paginatedItems = filteredDocuments
                .Skip(dateRangeSchema.offSet)
                .Take(dateRangeSchema.limitList)
                .ToList();

            // Calculate nextOffset
            int totalItems = filteredDocuments.Count;
            int nextOffset = dateRangeSchema.offSet + dateRangeSchema.limitList;
            if (nextOffset >= totalItems || dateRangeSchema.limitList == 0 || dateRangeSchema.offSet >= totalItems)
            {
                nextOffset = -1; // No more pages
            }

            // Update DateRange metadata
            var updatedDateRange = new DateRangeForReponse
            {
                nextOffset = nextOffset,
                total = totalItems
            };

            return (paginatedItems, updatedDateRange);
        }
    }
}
