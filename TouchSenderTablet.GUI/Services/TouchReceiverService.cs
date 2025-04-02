using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

using TouchSenderInterpreter.Models;

using TouchSenderReceiver.Reactors;
using TouchSenderReceiver.Services;

using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Models;

using WindowsInput;

namespace TouchSenderTablet.GUI.Services;

public class TouchReceiverService(ILogger<TouchReceiverService> logger) : ITouchReceiverService
{
    private TouchReceiver? _receiver;
    private int _portNumber;
    static readonly InputSimulator s_inputSimulator = new InputSimulator();
    private readonly ConcurrentDictionary<string, TouchSenderPayload> _payloads = new();
    private const string LatestPayloadKey = "latest";
    private int _currentPayloadId;
    public int DroppedPayloadCount { get; private set; }

    public void SetOptions(TouchReceiverServiceOptions options)
    {
        _portNumber = options.PortNumber;
        _receiver = new TouchReceiver();
        _receiver.AddReactor<SingleTouchReactor>((r) =>
        {
            r.OnWhileTouched += (e) =>
            {
                if (e.Offset is null) return;
                if (options.LeftClickWhileTouched)
                {
                    s_inputSimulator.Mouse.LeftButtonDown();
                }
                // Flutterの論理ピクセルは38pxで約1cm
                // 1cm動かしたら、Sensitivityの値分だけ動かす
                // https://api.flutter.dev/flutter/dart-ui/FlutterView/devicePixelRatio.html
                s_inputSimulator.Mouse.MoveMouseBy(
                    (int)Math.Round(e.Offset.X * (options.HorizontalSensitivity) / 38.0),
                    (int)Math.Round(e.Offset.Y * (options.VerticalSensitivity) / 38.0));
            };
            r.OnReleased += (e) =>
            {
                if (options.LeftClickWhileTouched)
                {
                    s_inputSimulator.Mouse.LeftButtonUp();
                }
            };
        });
        // TODO: 1000hzくらいのときに外部からデータを取れない。
        // スレッドセーフでないため。
        _receiver.AddReactor<RawPayloadReactor>((r) =>
        {
            r.OnReceive += (e) =>
            {
                // データを複製して保存
                _payloads[LatestPayloadKey] = e.Payload with { };
                // ペイロードIDが連続していない場合はドロップとしてカウント
                if (e.Payload.Id != 0 && _currentPayloadId != 0 && e.Payload.Id != _currentPayloadId + 1)
                {
                    DroppedPayloadCount++;
                }
                _currentPayloadId = e.Payload.Id;
            };
        });
    }

    public async Task StartAsync(CancellationToken token)
    {
        if (_receiver != null)
        {
            _currentPayloadId = DroppedPayloadCount = 0;
            logger.LogInformation("TouchReceiverService is starting");
            await _receiver.StartAsync(_portNumber, token);
        }
    }

    public bool TryGetLatestPayload(out TouchSenderPayload? payload)
    {
        return _payloads.TryGetValue(LatestPayloadKey, out payload);
    }
}
