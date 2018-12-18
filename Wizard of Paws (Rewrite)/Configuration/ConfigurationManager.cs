using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace WizardOfPaws.Configuration
{
	public static class ConfigurationManager
	{
		private static object Load(string path, Type type)
		{
			path = Path.Combine("config", path);
			
			if (!File.Exists(path)) throw new FileNotFoundException("Unable to load configuration file", path);

			return JsonConvert.DeserializeObject(path, type);
		}

		public static T Load<T>(string path) => (T)Load(path, typeof(T));
	}
}
