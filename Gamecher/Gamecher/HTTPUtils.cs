using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Gamecher
{
    class HTTPUtils
    {
        
        //static variable of an ip to connect to the api
        public readonly static string IP = "localhost";

        //executes a insert of a json on the database
        public static string HTTPPost(string url, StringContent json)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };
                client.DefaultRequestHeaders.Accept.Clear();

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PostAsync(url, json).Result;
                response.EnsureSuccessStatusCode();

                // return URI of the created resource.
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return json.ToString();
            }

        }

        //get a object from a database with a request url
        public static string HTTPGet(string url, string urlParameters)
        {
            try
            {
                string json = "";
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };
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
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return 500 + "";
            }
        }

        //Request an insert into the databases to execute an update of the object
        public static string HTTPPut(string url, string urlParameters, StringContent json)
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(url)
                };
                client.DefaultRequestHeaders.Accept.Clear();

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PutAsync(urlParameters, json).Result;
                response.EnsureSuccessStatusCode();

                // Deserialize the updated product from the response body.
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return json.ToString();                
            }
        }

        //TODO method that delete a object into the databases
        public static async Task<HttpStatusCode> DeleteProductAsync(string url, string urlParameters)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.DeleteAsync(urlParameters);
            return response.StatusCode;
        }
    }
}