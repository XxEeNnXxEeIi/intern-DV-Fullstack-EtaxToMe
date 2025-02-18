using Google.Cloud.Firestore;

namespace MyFirestoreApi.Services
{
    public class CorpService
    {
        private FirestoreDb _firestoreDb;
        private FireStoreService _fireStoreService = new FireStoreService();
        public CorpService()
        {
            _firestoreDb = _fireStoreService.GetFirestoreDb();
        }   
        public async Task<Dictionary<string, object>> GetCorpIdByHeaderApiKeyAsync(string apiKey)
        {
            try
            {
                var apiSettingsRef = _firestoreDb.Collection("api_setting");
                var apiSettingsSnapshot = await apiSettingsRef
                    .WhereEqualTo("publicApiKey", apiKey)
                    .GetSnapshotAsync();

                if (apiSettingsSnapshot.Documents.Count == 0)
                {
                    return new Dictionary<string, object> { { "message", "@corp: corpId not found." } };
                }
                else
                {
                    var corpCollectionId = apiSettingsSnapshot.Documents.First().GetValue<string>("corpId");
                    return new Dictionary<string, object> { { "corpCollectionId", corpCollectionId ?? string.Empty } };
                }
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object> { { "message", $"Error retrieving corpId: {ex.Message}" } };
            }
        }

        public async Task<Dictionary<string, object>> GetCorpDataByCorpIdAsync(string corpCollectionId)
        {
            try
            {
                var corpsRef = _firestoreDb.Collection("corp");

                var docRef = corpsRef.Document(corpCollectionId);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return new Dictionary<string, object> { { "message", "@corp: corpCollectionId not found." } };
                }
                else
                {
                    return snapshot.ToDictionary();
                }
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object> { { "message", $"Error retrieving corpId: {ex.Message}" } };
            }
        }
    }
}
