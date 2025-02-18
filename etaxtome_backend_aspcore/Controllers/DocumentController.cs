using Microsoft.AspNetCore.Mvc;
using MyFirestoreApi.Attributes;
using MyFirestoreApi.Services;
using MyFirestoreApi.Models;
using Google.Cloud.Firestore;

namespace MyFirestoreApi.Controllers
{
    [ApiController]
    [Route("api/document")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService = new DocumentService();

        [HttpPost("request/AddDocumentRequest")]
        [VerifyCorp]
        public async Task<IActionResult> AddDocumentRequest([FromBody] DocumentRequestForPost documentRequestForPost)
        {
            if (documentRequestForPost == null)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                if (HttpContext.Items.TryGetValue("corpCollectionId", out var corpCollectionIdObj))
                {
                    if (corpCollectionIdObj is string corpCollectionId)
                    {
                        documentRequestForPost.corpId = corpCollectionId;
                    }
                    
                }

                string actionMethod = HttpContext.Request.Method; // Get the action method (PUT, POST, GET, etc.)
                bool result = await _documentService.AddDocumentRequestAsync(documentRequestForPost, actionMethod);

                if (result)
                {
                    return Ok("Document request successfully added.");
                }
                else
                {
                    return StatusCode(500, "Failed to add document request.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("request/UpdateDocumentRequest")]
        [VerifyCorp]
        public async Task<IActionResult> UpdateDocumentRequest([FromBody] DocumentRequestForPost documentRequestForPost)
        {
            if (documentRequestForPost == null)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                if (HttpContext.Items.TryGetValue("corpCollectionId", out var corpCollectionIdObj))
                {
                    if (corpCollectionIdObj is string corpCollectionId)
                    {
                        documentRequestForPost.corpId = corpCollectionId;
                    }
                }

                string actionMethod = HttpContext.Request.Method; // Get the action method (PUT, POST, GET, etc.)
                bool result = await _documentService.UpdateDocumentRequestAsync(documentRequestForPost, actionMethod);

                if (result)
                {
                    return Ok("Document request successfully updated.");
                }
                else
                {
                    return StatusCode(500, "Failed to update document request.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("request/GetDocumentRequestDataFromCorpIdByDateRange")]
        [VerifyCorp]
        public async Task<IActionResult> GetDocumentRequestDataFromCorpIdByDateRange([FromQuery] DateRangeForQuery dateRange)
        {
            try
            {
                // Extract corpCollectionId from HttpContext
                if (!HttpContext.Items.TryGetValue("corpCollectionId", out var corpCollectionIdObj) || !(corpCollectionIdObj is string corpCollectionId))
                {
                    return BadRequest("Can't get or invalid corpId from HttpContext.");
                }

                // Call the service to get the document data
                var serviceResponse = await _documentService.GetDocumentDataByCorpIdAndDateRangeAsync(corpCollectionId, dateRange);

                // Check if the service response is successful
                if (!serviceResponse.Success)
                {
                    return NotFound(new { message = serviceResponse.Message });
                }

                // If successful, return the documents
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                // Return a 500 status for server errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
