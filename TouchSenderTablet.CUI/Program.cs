using ConsoleAppFramework;

using TouchSenderReceiver.Reactors;
using TouchSenderReceiver.Services;

using WindowsInput;

namespace TouchSenderTablet.CUI;

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
        var touchReceiver = new TouchReceiver();
        var inputSimulator = new InputSimulator();
        touchReceiver.AddReactor<FirstTimeOnlyReactor>((r) =>
        {
            r.OnFirstReceive += (e) =>
            {
                Console.WriteLine($"Received: {e.Payload}");
            };
        });
        int sensitivity = 3000;
        touchReceiver.AddReactor<SingleTouchReactor>(r =>
        {
            r.OnReleased += (e) =>
            {
                //Console.WriteLine($"Released!{count++}");
                inputSimulator.Mouse.LeftButtonUp();
            };
            r.OnTouched += (e) =>
            {
                //Console.WriteLine($"Touched! {count++}");
            };
            r.OnWhileTouched += (e) =>
            {
                //Console.WriteLine($"{e.OffsetRatio}");
                if (e.OffsetRatio is null) return;
                inputSimulator.Mouse.LeftButtonDown();
                inputSimulator.Mouse.MoveMouseBy(
                    (int)(e.OffsetRatio.X * sensitivity),
                    (int)(e.OffsetRatio.Y * sensitivity * 0));
            };
        });
        Console.WriteLine($"Listening on port {portNumber}...");
        await touchReceiver.StartAsync(portNumber, cancellationToken);
    }
}
