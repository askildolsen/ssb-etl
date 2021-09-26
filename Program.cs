using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Json;

namespace ssb_etl
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var store = new DocumentStore { Urls = new string[] { "http://ravendb:8080" }, Database = "Digitalisert" })
            {
                store.Conventions.FindCollectionName = t => t.Name;
                store.Initialize();

                var stopwatch = Stopwatch.StartNew();

                using (BulkInsertOperation bulkInsert = store.BulkInsert())
                {
                    bulkInsert.Store(
                        new {
                            Data = Csv.ExpandoStream(WebRequest.Create("https://data.ssb.no/api/v0/dataset/26975.csv"))
                        },
                        "SSB/26975",
                        new MetadataAsDictionary(new Dictionary<string, object> {{ "@collection", "SSB"}})
                    );
                }

                using (BulkInsertOperation bulkInsert = store.BulkInsert())
                {
                    bulkInsert.Store(
                        new {
                            Data = Csv.ExpandoStream(WebRequest.Create("https://data.ssb.no/api/v0/dataset/1108.csv?lang=no"))
                        },
                        "SSB/1108",
                        new MetadataAsDictionary(new Dictionary<string, object> {{ "@collection", "SSB"}})
                    );
                }

                new SSBResourceModel.SSBResourceIndex().Execute(store);

                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);
            }
        }
    }
}
