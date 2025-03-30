namespace TouchSenderTablet.GUI.Models;

public class TouchReceiverServiceOptions
{
    public int HorizontalSensitivity { get; set; } = 1000;
    public int VerticalSensitivity { get; set; } = 1000;
    public bool LeftClickWhileTouched { get; set; } = false;
    public int PortNumber { get; set; } = 50000;
}
