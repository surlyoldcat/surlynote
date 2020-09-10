using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SurlyNote.Shared.Models
{
    public interface IDocumentKey
    {
        string Id { get; set; }
        string UserId { get; set; }
    }

    public class DocumentKey : IDocumentKey
    {
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        public string UserId { get; set; }
    }
}
