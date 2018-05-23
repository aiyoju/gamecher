using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Gamecher
{
    class HTTPUtils
    {
        

        public static async Task<string> HTTPPost(string url, StringContent json)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync(url, json);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Content.ToString();
        }

        public static string HTTPGet(string url, string urlParameters)
        {
            string json = "";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                json = response.Content.ReadAsStringAsync().Result;
            }
            return json;
        }

        public static async Task<String> HTTPPut(string url, string urlParameters, StringContent json)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PutAsync(urlParameters, json);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            return response.Content.ToString();
        }

        public static async Task<HttpStatusCode> DeleteProductAsync(string url, string urlParameters)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.DeleteAsync(urlParameters);
            return response.StatusCode;
        }
    }
}