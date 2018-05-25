using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Gamecher
{
    class HTTPUtils
    {
        

        public static string HTTPPost(string url, StringContent json)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PostAsync(url, json).Result;
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Content.ReadAsStringAsync().Result;
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

        public static string HTTPPut(string url, string urlParameters, StringContent json)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PutAsync(urlParameters, json).Result;
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            return response.Content.ReadAsStringAsync().Result;
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