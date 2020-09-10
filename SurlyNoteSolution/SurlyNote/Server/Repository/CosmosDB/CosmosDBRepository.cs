using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SurlyNote.Shared.Models;


namespace SurlyNote.Server.Repository.CosmosDB
{
    public class CosmosDBRepository : IDocRepository
    {
        private ILogger<CosmosDBRepository> log;
        private CosmosDBConfig config = new CosmosDBConfig();
        private static readonly string CONFIG_SECTION = "CosmosDB";
        private static readonly string LIST_QUERY_BASE = "SELECT c.id, c.UserId, c.Title, c.Tags FROM c ";
        private readonly CosmosClient cosmos;

        public CosmosDBRepository(ILogger<CosmosDBRepository> logger, IConfiguration cfg)
        {
            log = logger;
            cfg.GetSection(CONFIG_SECTION).Bind(config);
            cosmos = new CosmosClient(config.EndpointUri, config.Key, new CosmosClientOptions { ApplicationName = config.AppName });
        }
        
        
        public async Task<SurlyDoc> Create(SurlyDoc doc)
        {
            var container = await SafeGetContainer();
            var pKey = new PartitionKey(doc.UserId);
            var options = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = true
            };
            var response = await container.CreateItemAsync(doc, pKey, options);
            
            //TODO check the response
            return response.Resource;
        }

        public async Task Delete(IDocumentKey key)
        {
            var container = await SafeGetContainer();
            var pKey = new PartitionKey(key.UserId);
            await container.DeleteItemAsync<SurlyDoc>(key.Id, pKey);
        }

        public async Task<SurlyDoc> Fetch(IDocumentKey key)
        {
            var container = await SafeGetContainer();
            var pKey = new PartitionKey(key.UserId);
            var response = await container.ReadItemAsync<SurlyDoc>(key.Id, pKey);
            return response.Resource;
            
        }
        public async Task<IEnumerable<DocListItem>> List(string userId, string searchText = null)
        {
            QueryDefinition qDef = null;
            if (String.IsNullOrEmpty(searchText))
            {
                qDef = new QueryDefinition($"{LIST_QUERY_BASE} WHERE c.UserId=@userId")
                    .WithParameter("@userId", userId);
            }
            else
            {
                throw new NotImplementedException();
            }
            return await RunListQuery(qDef, userId);
        }


        private async Task<IEnumerable<DocListItem>> RunListQuery(QueryDefinition queryDef, string userId)
        {
            var container = await SafeGetContainer();
            
            var options = new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(userId)
            };
            var listItems = new List<DocListItem>();
            using (FeedIterator<DocListItem> feed = container.GetItemQueryIterator<DocListItem>(queryDef, null, options))
            {
                while (feed.HasMoreResults)
                {
                    var response = await feed.ReadNextAsync();
                    listItems.AddRange(response.Resource);
                }
            }

            return listItems;
        }



        public async Task Update(IDocumentKey key, SurlyDoc doc)
        {
            var container = await SafeGetContainer();
            var pKey = new PartitionKey(key.UserId);
            await container.UpsertItemAsync<SurlyDoc>(doc, pKey);
        }

        private async Task<Container> SafeGetContainer()
        {
            var dbResponse = await cosmos.CreateDatabaseIfNotExistsAsync(config.DbName);
            var db = dbResponse.Database;
            var containerResponse = await db.CreateContainerIfNotExistsAsync(config.ContainerName, $"/UserId", config.Throughput);
            return containerResponse.Container;
        }

        private static List<DocListItem> ListItemsFromContentStream(Stream content)
        {
            List<DocListItem> docs = new List<DocListItem>();
            using (StreamReader sr = new StreamReader(content))
            using (JsonTextReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                dynamic items = serializer.Deserialize<dynamic>(jr).Documents;
                foreach (dynamic item in items)
                {
                    docs.Add(CreateListItemDynamic(item));
                }
            }
            return docs;
        }

        private static List<SurlyDoc> DocsFromContentStream(Stream content)
        {
            List<SurlyDoc> docs = new List<SurlyDoc>();
            using (StreamReader sr = new StreamReader(content))
            using (JsonTextReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                dynamic items = serializer.Deserialize<dynamic>(jr).Documents;
                foreach (dynamic item in items)
                {
                    docs.Add(CreateDocDynamic(item));
                }
            }
            return docs;
        }

        private static DocListItem CreateListItemDynamic(dynamic item)
        {
            return new DocListItem
            {
                Id = item.id,
                UserId = item.UserId,
                Title = item.Title
            };
        }

        private static SurlyDoc CreateDocDynamic(dynamic item)
        {
            return new SurlyDoc
            {
                Id = item.id,
                UserId = item.UserId,
                Title = item.Title,
                Tags = item.Tags,
                Body = item.Body,
                LastModified = item.LastModified,
                NumViews = item.NumViews
            };
        }


    }
}
