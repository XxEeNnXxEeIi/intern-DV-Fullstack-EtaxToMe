using Microsoft.AspNetCore.Mvc;
using MyFirestoreApi.Attributes;
using MyFirestoreApi.Services;

namespace MyFirestoreApi.Controllers
{
    [ApiController]
    [Route("api/corp")]
    public class CorpController : ControllerBase
    {
        private readonly CorpService _cropService;

        public CorpController(CorpService cropService)
        {
            _cropService = cropService;
        }

        [HttpGet("getcorpid")]
        [VerifyCorp]
        public IActionResult GetCorpId()
        {
            try
            {
                if (HttpContext.Items.TryGetValue("corpCollectionId", out var corpCollectionIdObj))
                {
                    if (corpCollectionIdObj is string corpCollectionId)
                    {
                        return Ok(new { corpCollectionId });
                    }
                    else
                    {
                        return BadRequest("Invalid corpId format.");
                    }
                }
                else
                {
                    return BadRequest("Can't get corpId from HttpContext.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getcorpdata")]
        [VerifyCorp]
        public async Task<IActionResult> GetCorpData()
        {
            try
            {
                if (HttpContext.Items.TryGetValue("corpCollectionId", out var corpCollectionIdObj))
                {
                    if (corpCollectionIdObj is string corpCollectionId)
                    {
                        var corpData = await _cropService.GetCorpDataByCorpIdAsync(corpCollectionId);
                        return Ok(corpData);
                    }
                    else
                    {
                        return BadRequest("Invalid corpId format.");
                    }
                }
                else
                {
                    return BadRequest("Can't get corpId from HttpContext.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
