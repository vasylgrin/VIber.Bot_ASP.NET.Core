using Viber.Bot;
using VIber.Bot_ASP.NET.Core.Models;

namespace VIber.Bot_ASP.NET.Core.Services
{
    public class BotService
    {
        private readonly ViberBotClient _viberBotClient;

        public BotService()
        {
            _viberBotClient = new ViberBotClient(Constants.BOT_TOKEN);
        }

        public async Task SendMessageTextAsync(string userId, string message)
        {
            await _viberBotClient.SendTextMessageAsync(new TextMessage
            {
                Receiver = userId,
                Text = message
            });
        }

        public async Task SendKeyboardAsync(string userId, string message, string buttonName, string actionName)
        {
            await _viberBotClient.SendKeyboardMessageAsync(new KeyboardMessage
            {
                Text = message,
                Receiver= userId,
                Keyboard=new Keyboard
                {
                    Buttons = new[]
                    {
                        new KeyboardButton
                        {
                            Text = buttonName,
                            ActionBody = actionName
                        }
                    }

                }
            });
        }

        public async Task SendKeyboardAsync(string userId, string message, string buttonName1, string actionName1, string buttonName2, string actionName2)
        {
            await _viberBotClient.SendKeyboardMessageAsync(new KeyboardMessage
            {
                Text = message,
                Receiver = userId,
                Keyboard = new Keyboard
                {
                    Buttons = new[]
                    {
                        new KeyboardButton
                        {
                            Text = buttonName1,
                            ActionBody = actionName1
                        },
                        new KeyboardButton
                        {
                            Text = buttonName2,
                            ActionBody = actionName2
                        }
                    }

                }
            });
        }

        public async Task DeleteKeyboardAsync(string userId, string message)
        {
            await _viberBotClient.SendKeyboardMessageAsync(new KeyboardMessage
            {
                Text = message,
                Receiver= userId,
                Keyboard=null,
            });
        }
    }
}
