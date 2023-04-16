using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using Viber.Bot;
using VIber.Bot_ASP.NET.Core.Enums;
using VIber.Bot_ASP.NET.Core.Extentions;
using VIber.Bot_ASP.NET.Core.Helper;
using VIber.Bot_ASP.NET.Core.Models;

namespace VIber.Bot_ASP.NET.Core.Services
{
    public sealed class ViberService
    {
        #region Fields

        private static Dictionary<string, NavigationEnum>? _navigationDictionary;
        private static Dictionary<string, string>? _imeiDictionary;
        private static Dictionary<string, List<Walk>>? _walksDictionary;
        private string _userId;
        private string _userMessage;
        private readonly BotService _botService;
        private CallbackData _callbackData;

        #endregion

        public ViberService()
        {
            _navigationDictionary = new Dictionary<string, NavigationEnum>();
            _imeiDictionary = new Dictionary<string, string>();
            _walksDictionary = new Dictionary<string, List<Walk>>();
            _botService = new BotService();
        }

        public async Task CheckRequestTextAsync(CallbackData callbackData)
        {
            _callbackData = callbackData;
            SetUserMessageAndId();

            if (_userMessage == "Back")
                _navigationDictionary.SetValue(_userId, NavigationEnum.Back);
            _navigationDictionary.TryGetValue(_userId, out NavigationEnum navigationEnum);

            if (navigationEnum == NavigationEnum.enterImei)
            {
                EnterImeiAsync().Wait();
                return;
            }
            else if(navigationEnum == NavigationEnum.getImei)
            {
                GetImeiAsync().Wait();
                return;
            }
            else if(navigationEnum == NavigationEnum.getTop10Walks)
            {
                GetTop10WalksAsync().Wait();
                return;
            }
            else if (navigationEnum == NavigationEnum.Back)
            {
                await BackButtonAsync();
            }
            else
            {
                await _botService.SendMessageTextAsync(_userId, "Невідома команда...");
            }
        }


        private void SetUserMessageAndId()
        {
            _userId = _callbackData.Sender.Id;
            var text = (TextMessage)_callbackData.Message;
            _userMessage = text.Text;
        }


        private async Task EnterImeiAsync()
        {
            await _botService.DeleteKeyboardAsync(_userId, "Enter IMEI:");
            _navigationDictionary.SetValue(_userId, NavigationEnum.getImei);
        }


        private async Task GetImeiAsync()
        {       
            if(!double.TryParse(_userMessage, out double value))
            {
                await _botService.SendMessageTextAsync(_userId, "Incorrect IMEI.");
                await BackButtonAsync();
                return;
            }

            _imeiDictionary.SetValue(_userId, _userMessage);

            var walksJToken = await GetWalksJToken();
            if (walksJToken == null)
                return;
            var walksList = CreateWalks(walksJToken);

            _navigationDictionary.SetValue(_userId, NavigationEnum.getTop10Walks);
            _walksDictionary.SetValue(_imeiDictionary[_userId], walksList);

            await SendInformationAboutWalks(walksList);

            return;
        }

        private async Task<JToken> GetWalksJToken()
        {
            string URL_FOR_API_REQUEST = $"https://localhost:7100/geAllWalks?IMEI={_userMessage}";
            var requestString = await ReciveRequest.ReciveToRequest(URL_FOR_API_REQUEST);
            if (requestString == "[]")
            {
                await _botService.SendMessageTextAsync(_userId, "Incorrect IMEI...");
                await BackButtonAsync();
                return null;
            }
            return JToken.Parse(requestString);
        }

        private List<Walk> CreateWalks(JToken walksJToken)
        {
            var walksList = new List<Walk>();

            foreach (var walkJToken in walksJToken)
            {
                var kilometers = double.Parse(walkJToken["kilometersWalked"].ToString());
                var duration = TimeSpan.Parse(walkJToken["durationOfWalk"].ToString()).TotalMinutes;

                kilometers = Math.Floor(kilometers);
                duration = Math.Floor(duration);

                var walk = new Walk(kilometers, duration);
                walksList.Add(walk);
            }

            return walksList;
        }
        
        private double GetTotalKilometersWalked(List<Walk> walksList)
        {
            double total = 0;

            foreach (var walk in walksList)
            {
                total += walk.KilometersWalked;
            }

            return total;
        }
        
        private double GetDurationOfWalksInMinutes(List<Walk> walksList)
        {
            double duration = 0;

            foreach (var walk in walksList)
            {
                duration += walk.DurationWalk;
            }

            return duration;
        }

        private async Task SendInformationAboutWalks(List<Walk> walksList)
        {
            var totalKilometersWalked = GetTotalKilometersWalked(walksList);
            var durationOfWalksInMinutes = GetDurationOfWalksInMinutes(walksList);

            string message = $"Amount of walk: {walksList.Count}{Environment.NewLine}" +
                $"Total kilometers walked: {totalKilometersWalked}{Environment.NewLine}km." +
                $"Duration of walks, minutes: {durationOfWalksInMinutes}m.";

            _botService.SendKeyboardAsync(_userId, message,
                "GetTop10Walks", ButtonEnum.getTop10Walks.ToString(),
                "Back", ButtonEnum.Back.ToString()).Wait();
        }


        private async Task GetTop10WalksAsync()
        {
            var imeiString = _imeiDictionary[_userId];
            var walksList = _walksDictionary[imeiString];

            var walkArray = walksList.OrderByDescending(w => w.DurationWalk).Take(10).ToArray();
            var messageString = CreateMessageAboutTopWalks(walkArray);

            await _botService.SendKeyboardAsync(_userId, messageString, "Back", NavigationEnum.Back.ToString());
        }

        private string CreateMessageAboutTopWalks(Walk[] walksArray)
        {
            string message = "";
            for (int i = 0; i < walksArray.Length; i++)
            {
                message +=
                    $"Top: {i + 1}{Environment.NewLine}" +
                    $"Kilometers: {walksArray[i].KilometersWalked}{Environment.NewLine}" +
                    $"Minutes: {walksArray[i].DurationWalk}{Environment.NewLine}{Environment.NewLine}";
            }
            return message;
        }


        private async Task BackButtonAsync()
        {
            _navigationDictionary.SetValue(_userId, NavigationEnum.enterImei);

            var s = _callbackData.Message as TextMessage;
            s.Text = "";

            await CheckRequestTextAsync(_callbackData);
        }
    }
}
