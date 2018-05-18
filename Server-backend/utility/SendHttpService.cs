using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;

namespace Server_backend.utility
{
    public interface ISendHttpService
    {
        string SendPost<T>(string url, ref T classToJson);
        string SendPost<T>(string url, ref T classToJson, string authToken);
        string SendGet(string url);
        string SendGet(string url, string authToken);
        T DeserializeJsonString<T>(string json);
    }

    public class SendHttpService : ISendHttpService
    {

        public string SendPost<T>(string url, ref T classToJson)
        {
            return this.SendPost(url, ref classToJson, "");
        }

        public string SendPost<T>(string url, ref T classToJson, string authToken)
        {
            url = "http://skjoldtoft.dk/daniel/g1e17/config_switch.php";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            if (authToken.Length > 0)
            {
                httpWebRequest.Headers.Add("AuthToken", authToken);
            }


            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //string jsonStr = new JsonResult<T>(classToJson).toString();
                string jsonStr = JsonConvert.SerializeObject(classToJson);
                streamWriter.Write(jsonStr);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }

        public string SendGet(string url)
        {
            return this.SendGet(url, "");
        }

        public string SendGet(string url, string authToken)
        {
            url = "http://skjoldtoft.dk/daniel/g1e17/config_switch.php";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            if (authToken.Length > 0)
            {
                httpWebRequest.Headers.Add("AuthToken", authToken);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }

        public T DeserializeJsonString<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /*public void testCurl()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://skjoldtoft.dk/daniel/g1e17/config_switch.php");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"classStr\":\"iod_ipconfig\"," +
                              "\"method\":\"setIp\"," +
                              "\"ip\":\"foslenFisker\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine(result);
            }
        }

        public async Task<HttpResponseMessage> PostResult()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    string url = "http://skjoldtoft.dk/daniel/g1e17/config_switch.php";
                    string json = "{\"class\":\"iod_ipconfig\"," +
                                 "\"method\":\"setIp\"," +
                                 "\"ip\":\"foslenFisker\"}";
                    HttpContent httpContent = new StringContent(json);
                    response = await client.PostAsync(url, httpContent);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                Console.WriteLine(response.Content);
                return response;
            }
        }*/

    }
}
