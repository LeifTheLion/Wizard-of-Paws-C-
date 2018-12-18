using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Discord;

namespace WizardOfPaws.Configuration
{
	public class CoreConfiguration
	{
		public string BotToken { get; set; }
		public string Prefix { get; set; }
		public LogSeverity LogLevel { get; set; }
	}
}
