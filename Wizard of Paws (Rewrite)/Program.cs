using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using WizardOfPaws.Configuration;
using WizardOfPaws.Debug;
using WizardOfPaws.Modules;

//Built by Leif using Discord.Net
namespace WizardOfPaws
{

	public class Program
	{
		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		private DiscordSocketClient _instance;
		public static IServiceProvider Services;
		private CommandService _commands;
		private Logger _logger = new Logger();
		private Logger _discordLogger = new Logger();
		private CoreConfiguration _config;

		private async Task MainAsync()
		{
			try
			{
				this._config = ConfigurationManager.Load<CoreConfiguration>("core.json");

				this._logger = new Logger(_config.LogLevel);
				this._discordLogger = new Logger(_config.LogLevel, "Discord");
				this._instance = new DiscordSocketClient();
				this._commands = new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async });
				await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
				Program.Services = new ServiceCollection()
					.AddSingleton(_instance)
					.AddSingleton(_commands)
					.AddSingleton(new CommandModule(_config))
					.BuildServiceProvider();

				_instance.Log += msg => this._discordLogger.Log(msg.Message, msg.Severity);
				_instance.MessageReceived += ((CommandModule)Program.Services.GetService(typeof(CommandModule))).CommandMessageReceived;

				await _instance.LoginAsync(TokenType.Bot, _config.BotToken);
				await _instance.StartAsync();

				// Block this task until the program is closed.
				await Task.Delay(-1);
			}
			catch (FileNotFoundException exception)
			{
				await this._logger.Critical(exception.Message + " | File: " + exception.FileName);
			}
		}
	}
}
