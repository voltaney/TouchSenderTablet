namespace TouchSenderTablet.GUI.Models;

public class ShowErrorDialogMessage
{
    public required string Title { get; set; }
    public required Exception Error { get; set; }
    public string? Message { get; set; }
}
