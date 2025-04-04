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

    static async Task RunnerAsync(int portNumber = 50000, int sensitivity = 300, bool leftClickWhileTouched = false, CancellationToken cancellationToken = default)
    {
        cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(s_cts.Token, cancellationToken).Token;

        var cancelTask = Task.Run(() =>
        {
            Console.WriteLine("Press Q to cancel.");
            while (Console.ReadKey().Key != ConsoleKey.Q) ;
            Console.WriteLine("\nCanceling...");
            s_cts.Cancel();
        }, cancellationToken).WaitAsync(cancellationToken);

        var touchSenderTask = ListenTouchSenderAsync(portNumber, sensitivity, leftClickWhileTouched, cancellationToken);
        try
        {
            await Task.WhenAll(touchSenderTask, cancelTask);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Successfully Canceled.");
        }
    }
    static async Task ListenTouchSenderAsync(int portNumber, int sensitivity, bool leftClickWhileTouched, CancellationToken cancellationToken)
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
        double flutterPixelPerCm = 38.0;
        touchReceiver.AddReactor<SingleTouchReactor>(r =>
        {
            r.OnReleased += (e) =>
            {
                if (leftClickWhileTouched)
                    inputSimulator.Mouse.LeftButtonUp();
            };
            r.OnTouched += (e) =>
            {
                if (leftClickWhileTouched)
                    inputSimulator.Mouse.LeftButtonDown();
            };
            r.OnWhileTouched += (e) =>
            {
                if (e.Offset is null)
                {
                    return;
                }
                inputSimulator.Mouse.MoveMouseBy(
                    (int)(e.Offset.X / flutterPixelPerCm * sensitivity),
                    (int)(e.Offset.Y / flutterPixelPerCm * sensitivity));
            };
        });
        Console.WriteLine($"Listening on port {portNumber}...");
        await touchReceiver.StartAsync(portNumber, cancellationToken);
    }
}
