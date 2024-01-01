using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public static class ConsumerModule
{
	public static void Consume(Queue<int> buffer, object bufferLock, string fileName)
	{
		while (true)
		{
			int numberToWrite = 0;

			lock (bufferLock)
			{
				while (buffer.Count == 0)
				{
					if (!Monitor.Wait(bufferLock, TimeSpan.FromSeconds(1)))
					{
						if (Monitor.TryEnter(bufferLock))
						{
							if (buffer.Count == 0)
							{
								Monitor.Exit(bufferLock);
								break;
							}
							Monitor.Exit(bufferLock);
						}
					}
				}

				if (buffer.Count == 0)
					break;

				numberToWrite = buffer.Dequeue();
				Monitor.Pulse(bufferLock);
			}

			lock (bufferLock)
			{
				string logEntry = $"Timestamp: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}, Thread ID: {Thread.CurrentThread.ManagedThreadId}, Number: {numberToWrite}";

				using (StreamWriter writer = File.AppendText(fileName))
				{
					writer.WriteLine(logEntry);
				}
			}
		}
	}
}
