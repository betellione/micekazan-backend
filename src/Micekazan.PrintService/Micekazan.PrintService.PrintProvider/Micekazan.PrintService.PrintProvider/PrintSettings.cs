namespace Micekazan.PrintService.PrintProvider;

public sealed class PrintSettings
{
    /// <summary>
    ///     Paper width in mm.
    /// </summary>
    public int PaperWidth { get; init; }

    /// <summary>
    ///     Paper height in mm.
    /// </summary>
    public int PaperHeight { get; init; }

    /// <summary>
    ///     Array of document margins in mm starting from top of the page clockwise (top - right - bottom - left).
    /// </summary>
    public int[] Margins { get; init; } = Array.Empty<int>();

    /// <summary>
    ///     Printing start point from top left corner of the page in mm.
    /// </summary>
    public (int X, int Y) StartPoint { get; init; }
}