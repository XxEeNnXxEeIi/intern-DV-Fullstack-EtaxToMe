using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MyFirestoreApi.Services;
using MyFirestoreApi.Models;
using Xunit;
using Moq;
using System.Net;

namespace MyFirestoreApi.Tests
{
    public class DocumentTest
    {   
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DocumentService _documentService = new DocumentService();

        // ใช้สำหรับ Controller, Service ทดสอบ
        private readonly string _publicApiKeySample = "896410c6bfddc6ea9d1048e33bc8ffc5473302e0689b6f5360a916e5573285e7664e2d0152dc2b5d1dad9ad779a039efab42"; 

        // ใช้สำหรับ Length เฉยๆ เทียบความยาวอักขระ
        private readonly string _corpCollectionIdSample = "Vc4eynY6soMVvualWlMT"; 

        private List<DocumentRequestForGet> _testDocumentRequestForGet = new List<DocumentRequestForGet>();

        public DocumentTest()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseSolutionRelativeContentRoot(""); 
                });

            ApplyTestModel();    
        }

        // ============================== กำนนด ชุดข้อมูลสำหรับทดสอบ ============================== //

        private void ApplyTestModel()
        {
            // ชุดข้อมูลสำหรับทดสอบใช้สำหรับ GetFilteredDocumentsAsync_ShouldHandleAllCases และ ApplyPagination_ShouldHandleAllScenarios

            // สำหรับ User ให้ใส่เอกสารเพื่อใช้ทดสอบได้เลยนะ ไม่ใส่ก็ได้
            _testDocumentRequestForGet = new List<DocumentRequestForGet>
            {
                // ตัวอย่าง:
                // new DocumentRequestForGet { createDate = DateTime.UtcNow.AddDays(-5).Date },

            };

            // กรณีที่ถ้า User ไม่ใส่เอกสารอะไรมาให้ทดสอบเลย 
            if(_testDocumentRequestForGet.Count <= 0)
            {
                // กำหนดจำนวนสูงสุดของเอกสาร
                const int maxDocumentCount = 20; 

                // สุ่มจำนวนเอกสารไม่เกินจำนวนสูงสุด
                var random = new Random();
                int documentCount = random.Next(1, maxDocumentCount); 

                // เริ่มการเพิ่มเอกสารที่สุ่มเข้าไป
                for (int i = 1; i <= documentCount; i++) 
                {
                    _testDocumentRequestForGet.Add(new DocumentRequestForGet
                    {
                        createDate = DateTime.UtcNow.Date.AddDays(-documentCount + i)
                    });
                }
            }
        }

        // ============================== สร้าง Client + ใส่ Header ============================== //

        private HttpClient CreateClient(string header)
        {
            var client = _factory.CreateClient();

            if (client == null)
            {
                throw new InvalidOperationException("Failed to create HttpClient.");
            }

            // Set the x-api-key header
            client.DefaultRequestHeaders.Clear(); // Clear existing headers
            client.DefaultRequestHeaders.Add("x-api-key", header);
            
            return client;
        }

        // ============================== Test สำหรับ Service + Controller ============================== //
        
        // งานอัพเดท 13 / 9 / 67
        // เตรียมเพิ่ม DocumentRequest_Schema_Failed หลายๆแบบเพื่อใช้ทดสอบการตอบสนองหลายๆอย่างด้วย
        
        [Fact] // POST Test
        public async Task DocumentControllerAndServiceEndpointTest_AddDocumentRequest()
        {
            // Arrange
            var client = CreateClient(_publicApiKeySample);

            var documentRequest = new DocumentRequest_Schema();

            // Act
            var response = await client.PostAsJsonAsync("/api/document/request/AddDocumentRequest", documentRequest);

            // Assert
            Assert.NotNull(response); // Ensure the response is not null
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.BadRequest, 
                "Response should be successful or a Bad Request status code.");
        }

        [Fact] // PUT Test
        public async Task DocumentControllerAndServiceEndpointTest_UpdateDocumentRequest()
        {
            // Arrange
            var client = CreateClient(_publicApiKeySample);

            var documentRequest = new DocumentRequest_Schema();

            // Act
            var response = await client.PutAsJsonAsync("/api/document/request/UpdateDocumentRequest", documentRequest);

            // Assert
            Assert.NotNull(response); // Ensure the response is not null
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.BadRequest, 
                "Response should be successful or a Bad Request status code.");
        }

        [Fact] // GET Test
        public async Task DocumentControllerAndServiceEndpointTest_GetDocumentRequest_IdAndData_FromCorpIdByDateRange()
        {
            // Arrange
            var client = CreateClient(_publicApiKeySample);
            var _dateRange = new DateRangeForQuery
            {
                // ตัวอย่างแต่ละบรรทัดนะ
                //startDate = DateTime.UtcNow.AddDays(-29),
                //endDate = DateTime.UtcNow,
                //limitList = 20,
                //offset = 0, 
            };

            // Build the request URL with parameters
            var requestUrl = $"/api/document/request/GetDocumentRequestDataFromCorpIdByDateRange?startDate={_dateRange.startDate.ToString("yyyy-MM-dd")}&endDate={_dateRange.endDate.ToString("yyyy-MM-dd")}&limitList={_dateRange.limitList}&offset={_dateRange.offSet}";

            HttpResponseMessage? response = await client.GetAsync(requestUrl);

            if (response != null)
            {
                var serviceProvider = _factory.Services;

                if (serviceProvider.GetService(typeof(IHttpContextAccessor)) is IHttpContextAccessor httpContextAccessor)
                {
                    var httpContext = httpContextAccessor.HttpContext;

                    if (httpContext != null && httpContext.Items.TryGetValue("corpCollectionId", out var corpCollectionIdObj))
                    {
                        var existingCorpCollectionId = corpCollectionIdObj as string; // Corrected this line

                        if (string.IsNullOrEmpty(existingCorpCollectionId))
                        {
                            Assert.Fail("The request succeeded, but corpCollectionId is empty.");
                        }
                        else if (existingCorpCollectionId.Length != _corpCollectionIdSample.Length)
                        {
                            Assert.Fail("The request succeeded, but corpCollectionId length is invalid.");
                        }
                        else
                        {
                            // Assert for document_request Data
                            var resultForData = await _documentService.GetDocumentDataByCorpIdAndDateRangeAsync(
                                existingCorpCollectionId, _dateRange);

                            Assert.NotNull(resultForData);
                            Assert.True(resultForData.Success, "Result should indicate success.");

                            if (resultForData.Data != null)
                            {
                                var documentsData = resultForData.Data; // Data is already of type List<DocumentRequestForPost>
                                Assert.NotNull(documentsData);
                                Assert.InRange(documentsData.Count, 0, _dateRange.limitList);
                            }
                            else
                            {
                                Assert.Contains("No documents found", resultForData.Message);
                            }
                        }
                    }
                    else
                    {
                        Assert.Fail("The request succeeded, but corpCollectionId is missing."); 
                    }
                }
            }
            else
            {
                Assert.Fail("The request failed: publicApiKey is invalid.");
            }
        }

        // ============================== อันนี้ Test ตัวเล็ก Method ที่แยกย่อยออกมา ============================== //

        [Fact]
        public void ValidateDateRange_ShouldHandleAllValidationScenarios()
        {
            // Act
            // Define test cases
            var testCases = new List<(DateRangeForQuery dateRangeSchema, bool expectedResult, int expectedCode)>
            {
                // case 1: Start date after end date (validationCode 1)
                (new DateRangeForQuery
                {
                    startDate = DateTime.UtcNow.Date.AddDays(1),
                    endDate = DateTime.UtcNow,
                    limitList = 10
                }, false, 1),

                // case 2: Date range exceeds limit (validationCode 2)
                (new DateRangeForQuery
                {
                    startDate = DateTime.UtcNow.Date.AddDays(-31),
                    endDate = DateTime.UtcNow,
                    limitList = 10
                }, false, 2),

                // case 3: Limit exceeds max limit (validationCode 3)
                (new DateRangeForQuery
                {
                    startDate = DateTime.UtcNow.Date.AddDays(-10),
                    endDate = DateTime.UtcNow,
                    limitList = 21
                }, false, 3),

                // case 4: Valid case (validationCode 0)
                (new DateRangeForQuery
                {
                    startDate = DateTime.UtcNow.Date.AddDays(-10),
                    endDate = DateTime.UtcNow,
                    limitList = 10
                }, true, 0)
            };

            // Act and Assert
            foreach (var (dateRangeSchema, expectedResult, expectedCode) in testCases)
            {
                var result = _documentService.ValidateDateRange(dateRangeSchema, out _, out int validationCode);

                Assert.Equal(expectedResult, result);
                Assert.Equal(expectedCode, validationCode);
            }
        }

        [Fact]
        public void GetFilteredDocumentsAsync_ShouldHandleAllCases()
        {
            // Arrange
            var testDocuments = _testDocumentRequestForGet;

            // Create test cases with dynamic expectations
            var testCases = new List<(DateRangeForQuery dateRangeSchema, Func<List<DocumentRequestForGet>, int> expectedCountFunc)>
            {
                // Case 1: Valid date range with documents
                (new DateRangeForQuery
                {
                    startDate = DateTime.UtcNow.Date.AddDays(-10),
                    endDate = DateTime.UtcNow.Date
                }, documents => documents.Count(doc => doc.createDate >= DateTime.UtcNow.Date.AddDays(-10).Date && doc.createDate <= DateTime.UtcNow.Date)),

                // Case 2: Date range with no matching documents
                (new DateRangeForQuery
                {
                    startDate = DateTime.UtcNow.AddDays(-30).Date,
                    endDate = DateTime.UtcNow.AddDays(-20).Date
                }, documents => 0), // No documents expected

                // Case 3: Start date after end date
                (new DateRangeForQuery
                {
                    startDate = DateTime.UtcNow.Date,
                    endDate = DateTime.UtcNow.Date.AddDays(-10)
                }, documents => 0), // No documents expected

                // Case 4: Boundary cases with documents exactly on the start and end date
                (new DateRangeForQuery
                {
                    startDate = DateTime.UtcNow.Date.AddDays(-5),
                    endDate = DateTime.UtcNow.Date.AddDays(-3)
                }, documents => documents.Count(doc => doc.createDate >= DateTime.UtcNow.Date.AddDays(-5) && doc.createDate <= DateTime.UtcNow.Date.AddDays(-3)))
            };

            foreach (var (dateRangeSchema, expectedCountFunc) in testCases)
            {
                // Act
                var result = _documentService.FilterDocuments(testDocuments, dateRangeSchema);

                // Assert
                int expectedCount = expectedCountFunc(testDocuments); // Calculate expected count based on the actual documents
                Assert.Equal(expectedCount, result.Items.Count); // Check if the count matches
            }
        }
        
        [Fact]
        public void ApplyPagination_ShouldHandleAllScenarios()
        {
            // Arrange
            var testDocuments = _testDocumentRequestForGet;
            int totalDocuments = testDocuments.Count;

            // offSet is index of array, offSet 2 is index of 2 or 3rd of item in array
            var testCases = new List<(DateRangeForQuery dateRangeSchema, List<DocumentRequestForGet> expectedItems, int expectedTotal, int expectedNextOffSet, string caseDescription)>
            {
                // Case 1: Normal pagination: Offset is 0, Should return all items but not more than limit list
                (new DateRangeForQuery { offSet = 0, limitList = 10 },
                    testDocuments.Take(10).ToList(), 
                    totalDocuments, 
                    (10 < totalDocuments) ? 10 : -1, 
                    "Offset is 0, Should return all items but not more than limit list"),
                
                // Case 2: Advance pagination: Offset is set, should return items starting from the OffSet
                (new DateRangeForQuery { offSet = 2, limitList = 3 }, // เอา 2 3 4 มา 
                    totalDocuments > 2 ? testDocuments.Skip(2).Take(3).ToList() : new List<DocumentRequestForGet>(), // ต้องเป็น 2 3 4 รวม 3
                    totalDocuments, 
                    (2 + 3 < totalDocuments) ? 5 : -1,
                    "Offset is set, should return items starting from the OffSet"),
                
                // Case 3: Bad pagination: Offset is set but greater than Limitlist
                (new DateRangeForQuery { offSet = 1, limitList = 0 },
                    new List<DocumentRequestForGet>(),
                    totalDocuments,
                    -1,
                    "Offset is set but greater than Limitlist"),

                // Case 4: Bad pagination: Offset beyond list length; should return empty list
                (new DateRangeForQuery { offSet = totalDocuments+1, limitList = 1 }, 
                    new List<DocumentRequestForGet>(), 
                    totalDocuments,
                    -1, 
                    "Offset beyond list length, should return empty list")
            };

            // Act and Assert
            foreach (var (dateRangeSchema, expectedItems, expectedTotal, expectedNextOffSet, caseDescription) in testCases)
            {
                var result = _documentService.PaginateDocuments(testDocuments, dateRangeSchema);

                // Check if the list of items is equal
                Assert.Collection(result.Items,
                    expectedItems.Select((item, index) => new Action<DocumentRequestForGet>(resultItem =>
                    {
                        Assert.Equal(item, resultItem);
                    })).ToArray()
                );

                // Check if the nextOffset is calculated correctly
                Assert.Equal(expectedNextOffSet, result.DateRange.nextOffset);

                // Check if the total is correct
                Assert.Equal(expectedTotal, result.DateRange.total);
            }
        }
    }
}
