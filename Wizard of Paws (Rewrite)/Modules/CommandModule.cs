using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using WizardOfPaws.Configuration;

namespace WizardOfPaws.Modules
{
	public class CommandModule
	{
		private CoreConfiguration _coreConfig { get; }
		private DiscordSocketClient _instance { get; }
		private CommandService _commands { get; }

		public CommandModule(CoreConfiguration coreConfig)
		{
			this._coreConfig = coreConfig;
			this._instance = (DiscordSocketClient)Program.Services.GetService(typeof(DiscordSocketClient));
			this._commands = (CommandService)Program.Services.GetService(typeof(CommandService));
		}

		public async Task CommandMessageReceived(SocketMessage arg)
		{
			// Don't process the message if it was a System message...
			if (!(arg is SocketUserMessage message)
			    // or if it was a bot message
			    || message.Author.Id == _instance.CurrentUser.Id
			    // or if the message doesn't start with the command prefix
			    || !message.Content.StartsWith(this._coreConfig.Prefix)) return;

			// Create a Command Context
			var context = new SocketCommandContext(_instance, message);
			// Execute the command. (result does not indicate a return value, rather an object stating if the command executed successfully)
			IResult result = await _commands.ExecuteAsync(context, this._coreConfig.Prefix.Length);
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
				};

				await context.Channel.SendMessageAsync("", false, embed.Build());
			}
		}
	}
}
