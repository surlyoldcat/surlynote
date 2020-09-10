using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using SurlyNote.Shared.Models;

namespace SurlyNote.Client.Utility
{
    public class ValidatableSurlyDoc
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string TagString { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public DateTime LastModified { get; set; }
        public short NumViews { get; set; }

        [Required]
        public string Body { get; set; }

        public static implicit operator SurlyDoc(ValidatableSurlyDoc d)
        {
            string[] tags = String.IsNullOrEmpty(d.TagString) ? null : d.TagString.Split(' ');
            return new SurlyDoc
            {
                Id = d.Id,
                UserId = d.UserId,
                Tags = tags,
                Title = d.Title,
                LastModified = d.LastModified,
                Body = d.Body,
                NumViews = d.NumViews
            };
        }

        public static implicit operator ValidatableSurlyDoc(SurlyDoc d)
        {
            string tagString = String.Empty;
            if (null != d.Tags && d.Tags.Length > 0)
            {
                tagString = String.Join(' ', d.Tags);
            }
            return new ValidatableSurlyDoc
            {
                Id = d.Id,
                UserId = d.UserId,
                TagString = tagString,
                Title = d.Title,
                LastModified = d.LastModified,
                Body = d.Body,
                NumViews = d.NumViews
            };
        }
    }
}
