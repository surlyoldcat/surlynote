using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SurlyNote.Shared.Models;

namespace SurlyNote.Client.Utility
{
    public class SurlyNoteApiClient
    {
        private readonly HttpClient http;

        public SurlyNoteApiClient(HttpClient client)
        {
            http = client;
        }

        public async Task<IEnumerable<DocListItem>> GetNoteList()
        {
            var response = await http.GetFromJsonAsync<IEnumerable<DocListItem>>("api/surlydocs");
            return response;
        }

        public async Task<SurlyDoc> GetNote(string id)
        {
            var response = await http.GetFromJsonAsync<SurlyDoc>($"api/surlydocs/{id}");
            return response;
        }

        public async Task UpdateNote(SurlyDoc doc)
        {
            await http.PutAsJsonAsync<SurlyDoc>($"api/surlydocs/{doc.Id}", doc);
        }

        public async Task CreateNote(SurlyDoc doc)
        {
            await http.PostAsJsonAsync<SurlyDoc>("api/surlydocs", doc);
        }
    }
}
