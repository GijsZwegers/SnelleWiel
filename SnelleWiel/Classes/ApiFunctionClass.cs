using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace SnelleWiel.api
{
    class ApiFunctionClass
    {
        public static async Task<string> PostDataAsync(string path, List<KeyValuePair<string, string>> postData)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new FormUrlEncodedContent(postData);
            HttpResponseMessage response = await client.PostAsync(request.RequestUri, request.Content);
            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<string> GetDataAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        }

        public static T DeserializeJSon<T>(string Json)
        {
            return JsonConvert.DeserializeObject<T>(Json);
        }
    }
}
