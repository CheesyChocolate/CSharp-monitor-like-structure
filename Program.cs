using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
	static void Main()
	{
		object bufferLock = new object();
		Queue<int> buffer = new Queue<int>();
		const int BufferCapacity = 100;
		const int NumProducers = 5;
		const int NumConsumers = 5;
		const int NumbersToProduce = 50;
		string fileName = "Numbers.txt";

		List<Thread> producers = new List<Thread>();
		List<Thread> consumers = new List<Thread>();

		// Create producer threads
		for (int i = 0; i < NumProducers; i++)
		{
			Thread producerThread = new Thread(() =>
					ProducerModule.Produce(buffer, bufferLock, NumbersToProduce, BufferCapacity));
			producers.Add(producerThread);
			producerThread.Start();
		}

		// Create consumer threads
		for (int i = 0; i < NumConsumers; i++)
		{
			Thread consumerThread = new Thread(() =>
					ConsumerModule.Consume(buffer, bufferLock, fileName));
			consumers.Add(consumerThread);
			consumerThread.Start();
		}

		foreach (Thread producer in producers)
		{
			producer.Join();
		}

		lock (bufferLock)
		{
			Monitor.PulseAll(bufferLock);
		}

		foreach (Thread consumer in consumers)
		{
			consumer.Join();
		}

		Console.WriteLine("All producers and consumers finished their tasks.");
	}
}
