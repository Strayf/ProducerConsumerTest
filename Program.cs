using System;
using System.Threading;

namespace ProducerConsumer
{
    class Program
    {
        private const int bufferSize = 10; // Tamanho do buffer.
        private const int numberOfConsumers = 3; // Quantidade de threads de consumidores.
        private static int[] numbersBuffer = new int[bufferSize]; // Inicializando buffer com o tamanho especificado.
        private static int itemCount = 0; // Inicializando contador de itens do buffer com 0.
        private static Random rng = new Random(); // Inicializando gerador de números aleatórios.
        private static Semaphore semaphore = new Semaphore(1, 1); // Inicializando classe do semáforo.

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing app...");

            // Inicializando threads de produtor e consumidores.
            Thread producer = new Thread(AddToQueue);
            producer.Start();
            for (int i = 0; i < numberOfConsumers; i++)
            {
                new Thread(ReadFromQueue).Start();
            }
        }

        public static void AddToQueue()
        {
            while (true)
            {
                // Se ainda houver espaço no buffer
                if (itemCount < bufferSize)
                {
                    // Tentando acessar a seção crítica através de semáforo.
                    semaphore.WaitOne();
                    numbersBuffer[itemCount] = rng.Next(1, 100); // Inserindo número gerado aleatoriamente entre 1 a 100 no buffer.
                    itemCount++; // Incrementando o contador do buffer.
                    semaphore.Release(); // Liberando a seção crítica.
                }
            }
        }

        public static void ReadFromQueue()
        {
            while (true)
            {
                // Tentando acessar a seção crítica através de semáforo.
                semaphore.WaitOne();
                // Se ainda houver itens para leitura no buffer.
                if (itemCount > 0)
                {
                    int numberToRead = numbersBuffer[itemCount - 1]; // Ler último item do buffer.
                    itemCount--; // Decrementar o contador do buffer.
                    Console.WriteLine($"Thread ({Thread.CurrentThread.ManagedThreadId}): {numberToRead}"); // Imprimindo no console o item lido do buffer.
                    Thread.Sleep(100); // Suspendendo thread por 100 milissegundos.
                }
                semaphore.Release(); // Liberando a seção crítica.
            }
        }
    }
}