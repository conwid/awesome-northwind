using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateStoredProcedures
{
    class Program
    {
        static readonly string databaseName = "Northwind";
        static readonly string collectionName = "NorthwindItems";
        static readonly string serverUri = "https://localhost:8081";
        static readonly string resourceKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        static readonly string storedProcedureFilesPath = @"stored_procedures.txt";
        static async Task Main(string[] args)
        {
            using (DocumentClient client = new DocumentClient(new Uri(serverUri), resourceKey))
            {
                var storedProcedures = File.ReadAllText(storedProcedureFilesPath).Split(new[] { "--------------------------------------------------------------------------------------------" }, StringSplitOptions.RemoveEmptyEntries);
                var db = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });
                var collection = await client.CreateDocumentCollectionIfNotExistsAsync(db.Resource.SelfLink, new DocumentCollection { Id = collectionName });
                foreach (var storedProcedure in storedProcedures)
                {
                    var trimmedProc = storedProcedure.Trim();
                    string storedProcedureName = trimmedProc.Substring(9, trimmedProc.IndexOf('(') - 9);
                    await client.CreateStoredProcedureAsync(collection.Resource.SelfLink, new StoredProcedure { Id = storedProcedureName, Body = trimmedProc });
                }
            }
        }
    }
}
