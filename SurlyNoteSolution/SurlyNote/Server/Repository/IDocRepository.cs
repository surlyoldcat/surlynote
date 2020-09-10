using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurlyNote.Shared.Models;

namespace SurlyNote.Server.Repository
{
    public interface IDocRepository
    {
        Task<SurlyDoc> Fetch(IDocumentKey key);
        Task<SurlyDoc> Create(SurlyDoc doc);
        Task Update(IDocumentKey key, SurlyDoc doc);
        Task Delete(IDocumentKey key);
        Task<IEnumerable<DocListItem>> List(string userId, string searchText = null);

    }
}
