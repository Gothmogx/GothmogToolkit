using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GothmogToolkit.Tools.Helpers.Logger
{
	public class GothmogLoggerBase : IGothmogLogger
	{
		public bool IsLogging { get; set; } = true;
		private readonly StringBuilder _stringBuilder = new();

		private readonly Dictionary<IGothmogLogger.Color, string> _colorDictionary = new()
		{
			{ IGothmogLogger.Color.Default, string.Empty },
			{ IGothmogLogger.Color.Red, "ff0000ff" },
			{ IGothmogLogger.Color.Green, "00ff00ff" },
			{ IGothmogLogger.Color.Blue, "0000ffff" },
			{ IGothmogLogger.Color.Yellow, "ffff00ff" },
			{ IGothmogLogger.Color.Cyan, "00ffffff" },
			{ IGothmogLogger.Color.Magenta, "ff00ffff" },
			{ IGothmogLogger.Color.White, "ffffffff" },
			{ IGothmogLogger.Color.Black, "000000ff" },
		};

		public virtual void Log(string message, object sender = default,
			IGothmogLogger.Color color = default)
		{
			if (!IsLogging) return;

			_stringBuilder.Clear();

			if (color != default)
				_stringBuilder.Append(GetColorString(color));

			_stringBuilder.Append(message);
			_stringBuilder.Append(".");

			if (sender != default)
			{
				_stringBuilder.Append(" Sender: ");
				_stringBuilder.Append(sender);
				_stringBuilder.Append(".");
			}

			if (color != default)
				_stringBuilder.Append("</color>");

			Debug.Log(_stringBuilder.ToString());
		}

		private string GetColorString(IGothmogLogger.Color color) 
			=> $"<color=#{_colorDictionary[color]}>";
	}
}