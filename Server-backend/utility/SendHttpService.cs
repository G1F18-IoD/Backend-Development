using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;

namespace Server_backend.utility
{
    public interface ISendHttpService
    {

    }

    public class SendHttpService : ISendHttpService
    {
        public void testCurl()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/test_class/");
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
        }

    }
}
