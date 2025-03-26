﻿using ConsoleAppFramework;

using TouchSenderTablet.Core.Extensions;
using TouchSenderTablet.Core.Services;

namespace TouchSenderTablet.CUI
{
    internal class Program
    {
        static readonly CancellationTokenSource s_cts = new();
        static async Task Main(string[] args)
        {

            await ConsoleApp.RunAsync(args, RunnerAsync);
        }

        static async Task RunnerAsync(int portNumber = 50000, CancellationToken cancellationToken = default)
        {
            cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(s_cts.Token, cancellationToken).Token;

            var cancelTask = Task.Run(() =>
            {
                Console.WriteLine("Press Q to cancel.");
                while (Console.ReadKey().Key != ConsoleKey.Q) ;
                Console.WriteLine("\nCanceling...");
                s_cts.Cancel();
            }, cancellationToken).WaitAsync(cancellationToken);

            var touchSenderTask = ListenTouchSenderAsync(portNumber, cancellationToken);
            try
            {
                await Task.WhenAll(touchSenderTask, cancelTask);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Successfully Canceled.");
            }
        }
        static async Task ListenTouchSenderAsync(int portNumber, CancellationToken cancellationToken)
        {
            var touchSenderReceiver = new TouchSenderReceiver();
            touchSenderReceiver.AddFirstTimeOnlyReactor((e) =>
            {
                Console.WriteLine($"Received: {e.Payload}");
            });
            Console.WriteLine($"Listening on port {portNumber}...");
            await touchSenderReceiver.StartAsync(portNumber, cancellationToken);
        }
    }
}
