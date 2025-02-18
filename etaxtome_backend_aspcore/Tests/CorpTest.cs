using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MyFirestoreApi.Services;
using Xunit;

namespace MyFirestoreApi.Tests
{
    public class CorpTest
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly CorpService _cropService = new CorpService();
        private readonly string _publicApiKeySample = "896410c6bfddc6ea9d1048e33bc8ffc5473302e0689b6f5360a916e5573285e7664e2d0152dc2b5d1dad9ad779a039efab42";
        private readonly string _corpCollectionIdSample = "Vc4eynY6soMVvualWlMT";

        public CorpTest()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseSolutionRelativeContentRoot(""); 
                });
        }

        private HttpClient CreateClient(string header)
        {
            var client = _factory.CreateClient();
            
            // Check if the client was created successfully
            if (client == null)
            {
                throw new InvalidOperationException("Failed to create HttpClient.");
            }

            // Set the x-api-key header
            client.DefaultRequestHeaders.Clear(); // Clear existing headers
            client.DefaultRequestHeaders.Add("x-api-key", header);
            
            return client;
        }
        
        [Fact]
        public async Task CorpControllerAndServiceEndpointTest()
        {
            // Arrange
            var client = CreateClient(_publicApiKeySample);
            var response = await client.GetAsync("/api/corp/getcorpdata");

            // Act, Assert
            if (response != null && response.IsSuccessStatusCode)
            {
                var serviceProvider = _factory.Services;
        
                if (serviceProvider.GetService(typeof(IHttpContextAccessor)) is IHttpContextAccessor httpContextAccessor)
                {
                    var httpContext = httpContextAccessor.HttpContext;
                    
                    if (httpContext != null && httpContext.Items.TryGetValue("corpCollectionId", out var corpCollectionIdObj))
                    {
                        if (corpCollectionIdObj is string corpCollectionId)
                        {
                            if (string.IsNullOrEmpty(corpCollectionId))
                            {
                                Assert.Fail("The request succeeded, but corpCollectionId is empty.");
                            }
                            else if (corpCollectionId.Length != _corpCollectionIdSample.Length)
                            {
                                Assert.Fail("The request succeeded, but corpCollectionId length is invalid.");
                            }
                            else
                            {
                                Console.WriteLine($"Correct corpCollectionId context retrieved: {corpCollectionId}");

                                var corpData = await _cropService.GetCorpDataByCorpIdAsync(corpCollectionId);
                                
                                Assert.NotNull(corpData);
                                Assert.True(response.IsSuccessStatusCode, "The request was successful as expected.");
                        
                                // Print the corpData to the console
                                Console.WriteLine($"Service response data: {corpData}");
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
    }
}


/* นอกเหนือจาก console.WriteLine ใช้ตัวนี้ได้นะ
//private readonly ITestOutputHelper _output;

public CorpTest(ITestOutputHelper output)
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseSolutionRelativeContentRoot(""); 
                });
            _output = output;
        }

_output.WriteLine("The request succeeded, but corpCollectionId is empty.");
*/