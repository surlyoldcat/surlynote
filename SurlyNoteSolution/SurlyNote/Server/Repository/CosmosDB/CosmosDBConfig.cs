using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurlyNote.Server.Repository.CosmosDB
{
    public class CosmosDBConfig
    {
        public string EndpointUri { get; set; }
        public string Key { get; set; }
        public string AppName { get; set; }
        public string DbName { get; set; }
        public string ContainerName { get; set; }
        public short Throughput { get; set; }
    }
}
