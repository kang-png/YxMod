using System.Text;
using UnityEngine;

namespace DigitalOpus.MB.Core;

public class LODLog
{
	private int pos;

	private string[] logMessages;

	public LODLog(short bufferSize)
	{
		logMessages = new string[bufferSize];
	}

	public void Log(MB2_LogLevel l, string msg, MB2_LogLevel currentThreshold)
	{
		MB2_Log.Log(l, msg, currentThreshold);
		if (logMessages.Length != 0 && l <= currentThreshold)
		{
			logMessages[pos] = $"frm={Time.frameCount} {l} {msg}";
			pos++;
			if (pos >= logMessages.Length)
			{
				pos = 0;
			}
		}
	}

	public string Dump()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		if (logMessages == null || logMessages.Length < 1)
		{
			return string.Empty;
		}
		if (logMessages[logMessages.Length - 1] != null)
		{
			num = pos;
		}
		for (int i = 0; i < logMessages.Length; i++)
		{
			int num2 = (num + i) % logMessages.Length;
			if (logMessages[num2] == null)
			{
				break;
			}
			stringBuilder.AppendLine(logMessages[num2]);
		}
		return stringBuilder.ToString();
	}
}
