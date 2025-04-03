using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

using TouchSenderInterpreter.Models;

using TouchSenderReceiver.Reactors;
using TouchSenderReceiver.Services;

using TouchSenderTablet.GUI.Contracts.Services;
using TouchSenderTablet.GUI.Helpers;
using TouchSenderTablet.GUI.Models;

namespace TouchSenderTablet.GUI.Services;

public class TouchReceiverService(ILogger<TouchReceiverService> logger) : ITouchReceiverService
{
    private TouchReceiver? _receiver;
    private int _portNumber;
    private readonly ConcurrentDictionary<string, TouchSenderPayload> _payloads = new();
    private const string LatestPayloadKey = "latest";

    // Flutterの論理ピクセルは38pxで約1cm
    // https://api.flutter.dev/flutter/dart-ui/FlutterView/devicePixelRatio.html
    private const double FlutterPixelPerCm = 38.0;

    private int _currentPayloadId;

    /// <summary>
    /// ドロップした（処理できなかった）ペイロードの数。スレッドセーフ。
    /// </summary>
    public int DroppedPayloadCount { get; private set; }

    /// <summary>
    /// TouchReceiverServiceのオプションを設定します。
    /// </summary>
    /// <param name="options"></param>
    public void SetOptions(TouchReceiverServiceOptions options)
    {
        _portNumber = options.PortNumber;
        _receiver = new TouchReceiver();
        _receiver.AddReactor<SingleTouchReactor>((r) =>
        {
            r.OnWhileTouched += (e) =>
            {
                if (e.Offset is null) return;
                // 1cm動かしたら、Sensitivityの値分だけ動かす
                MouseHelper.MoveCursor(
                    (int)Math.Round(e.Offset.X * (options.HorizontalSensitivity) / FlutterPixelPerCm),
                    (int)Math.Round(e.Offset.Y * (options.VerticalSensitivity) / FlutterPixelPerCm));
            };
            r.OnTouched += (e) =>
            {
                if (options.LeftClickWhileTouched)
                {
                    MouseHelper.LeftClickDown();
                }
            };
            r.OnReleased += (e) =>
            {
                if (options.LeftClickWhileTouched)
                {
                    MouseHelper.LeftClickUp();
                }
            };
        });

        // ConcurrentDictionaryを使用したスレッドセーフなPayload受け渡し
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

    /// <summary>
    /// TouchReceiverServiceを開始します。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken token)
    {
        if (_receiver != null)
        {
            _currentPayloadId = DroppedPayloadCount = 0;
            logger.LogInformation("TouchReceiverService is starting");
            await _receiver.StartAsync(_portNumber, token);
        }
    }

    /// <summary>
    /// 最新のペイロードを取得します。スレッドセーフ。
    /// </summary>
    /// <param name="payload">最新のTouchSenderPayload</param>
    /// <returns>ConcurrentDictionaryから値を取得できたかどうか</returns>
    public bool TryGetLatestPayload(out TouchSenderPayload? payload)
    {
        return _payloads.TryGetValue(LatestPayloadKey, out payload);
    }
}
