using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SurlyNote.Shared.Models
{
    public class SurlyDoc : IDocumentKey
    {
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }

        [Required]
        [MinLength(1)]
        public string[] Tags { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public DateTime LastModified { get; set; }
        public short NumViews { get; set; }
        
        [Required]
        public string Body { get; set; }

        public string UserId { get; set; }
    }
}
