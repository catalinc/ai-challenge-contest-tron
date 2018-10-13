
using System;
using System.IO;

public class Log
{
	public static bool ENABLED = false;
	
	private static TextWriter tw;
	
	public static void Print(string message)
	{
		if (ENABLED)
		{
			if (tw == null)
			{
				tw = new StreamWriter("bot.log");
			}
			tw.WriteLine(message);
			tw.Flush();
		}
	}
}

