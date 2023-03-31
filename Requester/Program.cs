﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Requester
{
    internal class Program
    {
        private const string PORTAL_URL = "http://localhost:5000/Home/Index";
        private static readonly CancellationTokenSource source = new CancellationTokenSource();
        private static readonly CancellationToken token = source.Token;
        public static int NUMBER_OF_VIRTUAL_MACHINES = 50;

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += (o, e) => source.Cancel();

            do
            {
                Console.Write("Número de Requests concorrentes: ");

                int requestQty = int.Parse(Console.ReadLine());
                int count = 0;

                Parallel.ForEach(
                   new int[requestQty],
                   new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * Program.NUMBER_OF_VIRTUAL_MACHINES },
                   (int x) =>
                   {
                       var currentCount = Interlocked.Increment(ref count);

                       DoRequest(currentCount);
                       count++;
                   }
               );
            }
            while (!token.IsCancellationRequested);
        }

        private static void DoRequest(int requestId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var result = httpClient.GetStringAsync(PORTAL_URL).Result;
                    Console.WriteLine($"Request {requestId} => Ok: {!string.IsNullOrEmpty(result)}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request {requestId} => Erro: {ex.Message}");
            }
        }
    }
}