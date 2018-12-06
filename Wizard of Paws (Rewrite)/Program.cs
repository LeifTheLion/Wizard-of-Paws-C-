using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
//Built by Leif using Discord.Net
namespace WizardBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();


        DiscordSocketClient WizardBot;
        IServiceProvider _services;
        CommandService _commands;
        readonly char _cmdPrefix = '>';
        readonly Random rnd = new Random();
        Color RandomColor() => new Color(rnd.Next(256), rnd.Next(256), rnd.Next(256));

        public async Task MainAsync()
        {
            WizardBot = new DiscordSocketClient();

            _commands = new CommandService(new CommandServiceConfig() { DefaultRunMode = RunMode.Async });
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            _services = new ServiceCollection()
                .AddSingleton(WizardBot)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            //WizardBot.Log += Log;
            WizardBot.MessageReceived += CommandMessageReceived;

            await WizardBot.LoginAsync(TokenType.Bot, "");
            await WizardBot.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }



        async Task CommandMessageReceived(SocketMessage arg)
        {
            // Don't process the message if it was a System message
            if (!(arg is SocketUserMessage message)) return;
            // Don't process the message if it was from the bot
            if (message.Author.Id == WizardBot.CurrentUser.Id) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command based on if it starts with '>'
            if (!message.HasCharPrefix(_cmdPrefix, ref argPos)) return;
            // Create a Command Context
            var context = new SocketCommandContext(WizardBot, message);
            // Execute the command. (result does not indicate a return value, rather an object stating if the command executed successfully)
            IResult result = await _commands.ExecuteAsync(context, argPos);
            if (!result.IsSuccess)
            {
                Console.WriteLine(result.ErrorReason);
                var embed = new EmbedBuilder()
                {
                    Title = "Error: Couldn't execute command ",
                    //Author = { Name = context.User.Username },
                    Description = result.ErrorReason,
                    Color = new Color(255, 175, 215),
                    Timestamp = DateTimeOffset.UtcNow,
                    ThumbnailUrl = "https://mbtskoudsalg.com/images/error-transparent-4.png",
                };

                await context.Channel.SendMessageAsync("", false, embed.Build());
            }
            /*
            Task Log(LogMessage msg)
            {
                Console.WriteLine(msg.ToString());
                return Task.CompletedTask;
            }
            */
        }
    }
}