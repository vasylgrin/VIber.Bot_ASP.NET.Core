using System.Net;

namespace VIber.Bot_ASP.NET.Core.Helper
{
    public static class ReciveRequest
    {
        private static string _URLString = "";


        public static async Task<string?> ReciveToRequest(string urlString)
        {
            _URLString = urlString;
            return await Task.FromResult(await ReadStreamAsync());
        }

        private static async Task<string?> ReadStreamAsync()
        {
            Stream stream = null;
            try
            {
                stream = await CreateStreamAsync();
            }
            catch (Exception)
            {
                return null;
            }

            if (stream != null)
                return await new StreamReader(stream).ReadToEndAsync();
            else
                return null;
        }

        private static async Task<Stream?> CreateStreamAsync()
        {
            var respopnse = await CreateWebResponseAsync();
            return respopnse?.GetResponseStream();
        }

        private static async Task<HttpWebResponse?> CreateWebResponseAsync()
        {
            return await CreateRequest()?.GetResponseAsync() as HttpWebResponse;
        }

        private static HttpWebRequest? CreateRequest()
        {
            var _request = WebRequest.Create(_URLString) as HttpWebRequest;
            _request.Method = "GET";
            return _request;
        }
    }
}
