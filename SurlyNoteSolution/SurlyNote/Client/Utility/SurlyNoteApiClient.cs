using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;
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
            IEnumerable<DocListItem> items = null;
            try
            {
                items = await http.GetFromJsonAsync<IEnumerable<DocListItem>>("api/surlydocs");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            return items;
        }

        public async Task<SurlyDoc> GetNote(string id)
        {
            SurlyDoc doc = null;
            try
            {
                doc = await http.GetFromJsonAsync<SurlyDoc>($"api/surlydocs/{id}");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            
            return doc;
        }

        public async Task UpdateNote(SurlyDoc doc)
        {
            try
            {
                await http.PutAsJsonAsync<SurlyDoc>($"api/surlydocs/{doc.Id}", doc);

            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public async Task CreateNote(SurlyDoc doc)
        {
            try
            {
                await http.PostAsJsonAsync<SurlyDoc>("api/surlydocs", doc);

            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
