using System;
using System.Collections.Generic;
using System.Threading;

public static class ProducerModule
{
	public static void Produce(Queue<int> buffer, object bufferLock, int NumbersToProduce, int BufferCapacity)
	{
		Random rand = new Random();
		for (int i = 0; i < NumbersToProduce; i++)
		{
			int number = rand.Next(1, 101); // Generate a random number between 1 and 100

			lock (bufferLock)
			{
				while (buffer.Count == BufferCapacity)
				{
					Monitor.Wait(bufferLock); // Wait if buffer is full
				}
				buffer.Enqueue(number);
				Monitor.Pulse(bufferLock); // Signal consumers that new data is available
			}
		}
	}
}
