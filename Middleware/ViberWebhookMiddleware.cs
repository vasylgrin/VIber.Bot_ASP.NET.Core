using Microsoft.Owin;
using Newtonsoft.Json;
using Viber.Bot;
using VIber.Bot_ASP.NET.Core.Services;
using static System.Net.Mime.MediaTypeNames;

namespace VIber.Bot_ASP.NET.Core.Middleware
{
    public class ViberWebhookMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ViberService _viberService;
        private readonly ViberBotClient _botClient;

        public ViberWebhookMiddleware(RequestDelegate next)
        {
            _next = next;
            _viberService = new ViberService();
            _botClient = new ViberBotClient("50df9ee8d5e7e54f-f2096351553a8357-e1842b7a91617a2a");
        }

        public async Task Invoke(HttpContext context)
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                await _next.Invoke(context);
                return;
            }
            
            var isSignatureValid = _botClient.ValidateWebhookHash(
                context.Request.Headers[ViberBotClient.XViberContentSignatureHeader],
                body);
            
            if (!isSignatureValid)
            {
                throw new Exception("Invalid viber content signature");
            }

            var callbackData = JsonConvert.DeserializeObject<CallbackData>(body);

            if (callbackData.Event == EventType.Message)
            {
                _viberService.CheckRequestTextAsync(callbackData).Wait();
            }
        }
    }
}
