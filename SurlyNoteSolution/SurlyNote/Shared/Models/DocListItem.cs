using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SurlyNote.Shared.Models
{
    public class DocListItem : IDocumentKey
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Title { get; set; }
        public string UserId { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
