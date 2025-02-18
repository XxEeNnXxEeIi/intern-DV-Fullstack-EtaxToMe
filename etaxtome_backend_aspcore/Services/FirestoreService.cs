using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace MyFirestoreApi.Services
{
    public class FireStoreService
    {
        private FirestoreDb _firestoreDb;

        public FireStoreService()
        {
            // Load environment variables from .env file ไม่จำเป็นเพราะใส่ใน .env แล้ว
            //DotNetEnv.Env.Load();

            string? privateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY");
            string? clientEmail = Environment.GetEnvironmentVariable("CLIENT_EMAIL");
            string? projectId = Environment.GetEnvironmentVariable("PROJECT_ID");
            string? databaseId = Environment.GetEnvironmentVariable("DATABASE_ID");
        
            //Console.WriteLine($"PRIVATE_KEY: {privateKey}");
            //Console.WriteLine($"CLIENT_EMAIL: {clientEmail}");
            //Console.WriteLine($"PROJECT_ID: {projectId}");
            //Console.WriteLine($"DATABASE_ID: {databaseId}");

            if (string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(clientEmail) || 
                string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(databaseId))
            {
                //throw new InvalidOperationException("One or more environment variables are not set.");

                // สำหรับ dotnet test, dotnet test จะมีปัญหากับ .env file มันจะไม่ยอมอ่าน เราเลยต้องกำหนดให้มันอ่านค่าใน .env เอง

                // Navigate up to the project root directory (adjust number of ".." based on your folder structure)
                string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                string envFilePath = Path.Combine(projectRoot, ".env");

                // Read and parse the .env file
                var envVars = LoadEnvVariables(envFilePath);

                // Read values from parsed environment variables
                privateKey = envVars.GetValueOrDefault("PRIVATE_KEY", "default-private-key");
                clientEmail = envVars.GetValueOrDefault("CLIENT_EMAIL", "default-client-email");
                projectId = envVars.GetValueOrDefault("PROJECT_ID", "default-project-id");
                databaseId = envVars.GetValueOrDefault("DATABASE_ID", "default-database-id");

                Console.WriteLine($"private_key: {privateKey}");
                Console.WriteLine($"client_email: {clientEmail}");
                Console.WriteLine($"PROJECT_ID: {projectId}");
                Console.WriteLine($"DATABASE_ID: {databaseId}");
            }

            // Construct the API key JSON
            var apiKeyJson = new
            {
                type = "service_account",
                private_key = privateKey.Replace("\\n", "\n"),
                client_email = clientEmail,
                project_id = projectId
            };

            // Serialize the JSON object to a string
            var apiKeyJsonString = JsonConvert.SerializeObject(apiKeyJson);

            // Create a GoogleCredential object from the JSON string
            GoogleCredential credential;
            using (var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(apiKeyJsonString)))
            {
                credential = GoogleCredential.FromStream(jsonStream);
            }

            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                DatabaseId = databaseId,
                Credential = credential
            }.Build();
        }

        public FirestoreDb GetFirestoreDb()
        {
            return _firestoreDb;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                // Attempt to get a collection reference
                var collections = await _firestoreDb.ListRootCollectionsAsync().ToListAsync();

                return collections != null; // If collections are retrieved, the connection is working
            }
            catch
            {
                return false; // An exception occurred, so connection is likely not working
            }
        }

        public async Task<List<string>> GetAllCollectionsAsync()
        {
            var collectionNames = new List<string>();

            // Example: List top-level collections
            var collections = _firestoreDb.ListRootCollectionsAsync();
            await foreach (var collection in collections)
            {
                collectionNames.Add(collection.Id);
            }

            return collectionNames;
        }

        public async Task<Dictionary<string, List<string>>> GetAllCollectionsAndDocumentsAsync()
        {
            var result = new Dictionary<string, List<string>>();

            try
            {
                // List all collections
                var collections = await _firestoreDb.ListRootCollectionsAsync().ToListAsync();

                foreach (var collection in collections)
                {
                    var collectionName = collection.Id;
                    var documentList = new List<string>();

                    // List all documents in the collection
                    var snapshot = await collection.GetSnapshotAsync();
                    foreach (var document in snapshot.Documents)
                    {
                        documentList.Add(document.Id);
                    }

                    result.Add(collectionName, documentList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting collections and documents: {ex.Message}");
                // Optionally handle the exception or add an error message to the result
            }

            return result;
        }

        public async Task<Dictionary<string, Dictionary<string, Dictionary<string, object>>>> GetAllCollectionsAndDocumentsAndDataAsync()
        {
            var result = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

            try
            {
                // List all root collections
                var collections = await _firestoreDb.ListRootCollectionsAsync().ToListAsync();

                foreach (var collection in collections)
                {
                    var collectionName = collection.Id;
                    var documents = new Dictionary<string, Dictionary<string, object>>();

                    // Get all documents in the collection
                    var collectionRef = _firestoreDb.Collection(collectionName);
                    var collectionSnapshot = await collectionRef.GetSnapshotAsync();

                    foreach (var doc in collectionSnapshot.Documents)
                    {
                        var documentName = doc.Id;
                        var documentData = doc.ToDictionary(); // Convert document to dictionary

                        documents[documentName] = documentData;
                    }

                    result[collectionName] = documents;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting collections, documents, and data: {ex.Message}");
            }

            return result;
        }
        public async Task<Dictionary<string, object>> GetAllCollectionsAndDocumentsAndSubCollectionsAsync()
        {
            var result = new Dictionary<string, object>();
            
            // Retrieve top-level collections
            var topLevelCollections = await _firestoreDb.ListRootCollectionsAsync().ToListAsync();

            foreach (var collection in topLevelCollections)
            {
                var collectionData = new Dictionary<string, object>();
                
                // Retrieve documents in the top-level collection
                var documents = await collection.GetSnapshotAsync();
                
                foreach (var doc in documents.Documents)
                {
                    var docData = doc.ToDictionary();
                    var subCollections = await doc.Reference.ListCollectionsAsync().ToListAsync();
                    
                    var subCollectionData = new Dictionary<string, object>();
                    
                    foreach (var subCollection in subCollections)
                    {
                        var subCollectionDocuments = await subCollection.GetSnapshotAsync();
                        var subDocs = new Dictionary<string, object>();
                        
                        foreach (var subDoc in subCollectionDocuments.Documents)
                        {
                            subDocs[subDoc.Id] = subDoc.ToDictionary();
                        }
                        
                        subCollectionData[subCollection.Id] = subDocs;
                    }
                    
                    docData["subCollections"] = subCollectionData;
                    collectionData[doc.Id] = docData;
                }
                
                result[collection.Id] = collectionData;
            }
            return result;
        }

        public async Task<Dictionary<string, object>> GetSubCollectionsFromCollectionIdAsync(string collectionId)
        {
            var result = new Dictionary<string, object>();

            try
            {
                // Get a reference to the top-level collection
                var topLevelCollection = _firestoreDb.Collection(collectionId);

                // Retrieve documents in the specified collection
                var documents = await topLevelCollection.GetSnapshotAsync();

                foreach (var doc in documents.Documents)
                {
                    var docData = doc.ToDictionary();
                    
                    // Retrieve subcollections for each document
                    var subCollections = await doc.Reference.ListCollectionsAsync().ToListAsync();
                    var subCollectionData = new Dictionary<string, object>();
                    
                    foreach (var subCollection in subCollections)
                    {
                        var subCollectionDocuments = await subCollection.GetSnapshotAsync();
                        var subDocs = new Dictionary<string, object>();
                        
                        foreach (var subDoc in subCollectionDocuments.Documents)
                        {
                            subDocs[subDoc.Id] = subDoc.ToDictionary();
                        }
                        
                        subCollectionData[subCollection.Id] = subDocs;
                    }
                    
                    docData["subCollections"] = subCollectionData;
                    result[doc.Id] = docData;
                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                Console.WriteLine($"Error retrieving subcollections: {ex.Message}");
                // Handle the exception or rethrow as needed
            }

            return result;
        }

        public async Task<List<string>> GetCollectionsUnderDocumentAsync(string documentPath)
        {
            var documentRef = _firestoreDb.Document(documentPath);
            var collections = documentRef.ListCollectionsAsync();
            var collectionNames = new List<string>();

            await foreach (var collection in collections)
            {
                collectionNames.Add(collection.Id);
            }

            return collectionNames;
        }

        private static Dictionary<string, string> LoadEnvVariables(string filePath)
        {
            var envVars = new Dictionary<string, string>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file {filePath} was not found.");

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    envVars[key] = value;
                }
            }

            return envVars;
        }
    }
}