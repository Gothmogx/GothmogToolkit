namespace GothmogToolkit.Tools.Helpers.Logger
{
	public interface IGothmogLogger
	{
		bool IsLogging { get; set; }
		void Log(string message, object sender = default, Color color = default);
		void LogError(string message, object sender = default);
		public enum Color
		{
			Default,
			Red,
			Green,
			Blue,
			Yellow,
			Cyan,
			Magenta,
			White,
			Black
		}
	}
}