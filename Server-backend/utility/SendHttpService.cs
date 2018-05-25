using System.Net;
using System.IO;
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
        /**
         * Basically overloads the sendpost method, but without an authentication token.
         */
        public string SendPost<T>(string url, ref T classToJson)
        {
            return this.SendPost(url, ref classToJson, "");
        }

        /**
         * This sends a HTTP POST method request. This sends a generic class (since they are JSON encoded, it doesn't matter whatever class you attempt to send), to the URL.
         * It does this by creating a HTTPWebRequest and then writing to the request stream of this HTTPWebRequest. THen it reads the response stream from the the request.
         */
        public string SendPost<T>(string url, ref T classToJson, string authToken)
        {
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

        /**
         * Overloaded to be able to send GET requests without an authentication token.
         */
        public string SendGet(string url)
        {
            return this.SendGet(url, "");
        }

        /**
         * This method creates a HTTPWebRequest and then calls upon it. It then reads the responsestream. No stream is written to the url, since this is a GET request, where parameters go in the URL.
         */
        public string SendGet(string url, string authToken)
        {
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

        /**
         * Uses Newtonsoft JSON deserializer, that basically converts a string to whatever class you want.
         */
        public T DeserializeJsonString<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
