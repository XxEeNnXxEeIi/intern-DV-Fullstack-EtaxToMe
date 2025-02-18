using Microsoft.AspNetCore.Mvc;
using MyFirestoreApi.Services;
using MyFirestoreApi.Attributes;

namespace MyFirestoreApi.Controllers
{
    [ApiController]
    [Route("api/firestore")]
    public class FirestoreController : ControllerBase
    {
        private readonly FireStoreService _firestoreService;

        public FirestoreController(FireStoreService firestoreService)
        {
            _firestoreService = firestoreService;
        }

        [HttpGet("TestConnection")]
        public async Task<IActionResult> TestConnection()
        {
            bool isConnected = await _firestoreService.TestConnectionAsync(); // Use _firestoreService here
            return isConnected ? Ok("Connection successful.") : StatusCode(500, "Connection failed.");
        }

        [HttpGet("GetAllCollection")]
        public async Task<IActionResult> GetAllCollections()
        {
            try
            {
                var collections = await _firestoreService.GetAllCollectionsAsync();

                if (collections != null && collections.Any())
                {
                    return Ok(collections);
                }
                else
                {
                    // Return 404 Not Found if no collections are found
                    return NotFound("No collections found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving collections: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving collections.");
            }
        }

        [HttpGet("GetAllCollectionAndDocument")]
        public async Task<IActionResult> GetAllCollectionsAndDocuments()
        {
            try
            {
                var collections = await _firestoreService.GetAllCollectionsAndDocumentsAsync();

                if (collections != null && collections.Any())
                {
                    return Ok(collections);
                }
                else
                {
                    return NotFound("No collections found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving collections: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving collections.");
            }
        }

        [HttpGet("GetAllCollectionAndDocumentAndData")]
        public async Task<IActionResult> GetAllCollectionsAndDocumentsAndData()
        {
            try
            {
                var collections = await _firestoreService.GetAllCollectionsAndDocumentsAndDataAsync();

                if (collections != null && collections.Any())
                {
                    return Ok(collections);
                }
                else
                {
                    // Return 404 Not Found if no collections are found
                    return NotFound("No collections found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error retrieving collections: {ex.Message}");

                // Return a 500 Internal Server Error response with a relevant error message
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving collections.");
            }
        }

        [HttpGet("GetAllCollectionAndDocumentAndDataAndSubcollection")]
        public async Task<IActionResult> GetAllCollectionsAndDocumentsAndSubCollections()
        {
            try
            {
                var collections = await _firestoreService.GetAllCollectionsAndDocumentsAndSubCollectionsAsync();

                if (collections != null && collections.Any())
                {
                    return Ok(collections);
                }
                else
                {
                    return NotFound("No collections found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error retrieving collections: {ex.Message}");

                // Return a 500 Internal Server Error response with a relevant error message
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving collections.");
            }
        }

        [HttpGet("GetSubcollection/{collectionId}")]
        public async Task<IActionResult> GetSubCollectionsFromCollectionIdAsync(string collectionId)
        {
            try
            {
                var subCollections = await _firestoreService.GetSubCollectionsFromCollectionIdAsync(collectionId);

                if (subCollections != null && subCollections.Any())
                {
                    return Ok(subCollections);
                }
                else
                {
                    // Return 404 Not Found if no subcollections are found
                    return NotFound("No subcollections found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error retrieving subcollections: {ex.Message}");

                // Return a 500 Internal Server Error response with a relevant error message
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving subcollections.");
            }
        }

    }
}