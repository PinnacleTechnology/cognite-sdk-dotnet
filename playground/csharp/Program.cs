﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Cognite.Sdk;
using Cognite.Sdk.Api;
using Cognite.Sdk.Timeseries;


namespace csharp
{
    class Program
    {
        /// <summary>
        /// Get Assets example.
        /// </summary>
        /// <param name="client">Fusion client.</param>
        /// <returns>Task</returns>
        static async Task GetAssetsExample(Client client)
        {
            var assetArgs =
                AssetArgs.Empty()
                .Name("string3");

            var result = await client.GetAssetsAsync(assetArgs);

            Console.WriteLine("{0}", result.Items.First().TryGetParentId(out long parentId));
            Console.WriteLine("{0}", parentId);
            Console.WriteLine("{0}", result);
        }

        static async Task QueryTimeseriesDataExample(Client client)
        {
            var defaultQuery =
                QueryData.Create()
                .Aggregates(new List<Aggregate> { Aggregate.Average });

            var query = new List<Tuple<long, QueryData>> ();

            var result = await client.GetTimeseriesDataAsync(defaultQuery, query);

            Console.WriteLine("{0}", result.First().DataPoints.First().TryGetValue(out long value));
            Console.WriteLine("{0}", value);
            Console.WriteLine("{0}", result);
        }

        static async Task CreateTimeseriesDataExample(Client client)
        {
            var timeseries =
                Timeseries.Create()
                    .SetName("Testing");

            var result = await client.CreateTimeseriesAsync(new List<TimeseriesCreateDto> { timeseries });

            Console.WriteLine("{0}", result);

            var points = new List<DataPoints> {
                new DataPoints {
                    Identity = Identity.ExternalId("test"),
                    DataPoints = new List<DataPoint> {
                        DataPoint.Float(0L, 1.0)
                    }
                }
            };
            var result2 = await client.InsertDataAsync(points);
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("C# Client");

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            await CreateTimeseriesDataExample(client);
        }
    }
}
