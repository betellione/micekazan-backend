using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace WebApp1.Services.PdfGenerator;

public class TicketDocument : IDocument
{
    private static readonly TextStyle InitialsStyle = TextStyle.Default.FontFamily("Montserrat").FontSize(30).Bold();
    private readonly TicketDocumentModel _model;

    public TicketDocument(TicketDocumentModel model)
    {
        _model = model;
    }

    public void Compose(IDocumentContainer container)
    {
        if (_model.IsHorizontal) ComposeHorizontal(container);
        else ComposeVertical(container);
    }

    private string FullName()
    {
        return (string.IsNullOrEmpty(_model.Name), string.IsNullOrEmpty(_model.Surname)) switch
        {
            (false, false) => $"{_model.Name}\n{_model.Surname}",
            (true, false) => _model.Surname!,
            (false, true) => _model.Name!,
            _ => string.Empty,
        };
    }

    private void ComposeVertical(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(90, 120, Unit.Millimetre);
            page.Margin(5, Unit.Millimetre);

            if (_model.BackgroundPath is not null)
            {
                page.Background()
                    .AlignMiddle()
                    .AlignCenter()
                    .Image(_model.BackgroundPath)
                    .FitArea();
            }

            page.Content().Layers(layers =>
            {
                if (_model.LogoPath is not null)
                {
                    layers.Layer()
                        .AlignTop()
                        .AlignCenter()
                        .MaxWidth(30, Unit.Millimetre)
                        .MaxHeight(20, Unit.Millimetre)
                        .Image(_model.LogoPath)
                        .FitArea();
                }

                if (_model.QrStream is not null)
                {
                    layers.Layer()
                        .AlignBottom()
                        .AlignCenter()
                        .Background("#FFFFFF")
                        .Padding(2, Unit.Millimetre)
                        .Width(30, Unit.Millimetre)
                        .Height(30, Unit.Millimetre)
                        .Image(_model.QrStream)
                        .FitArea();
                }

                layers.PrimaryLayer()
                    .AlignMiddle()
                    .AlignCenter()
                    .MaxWidth(80, Unit.Millimetre)
                    .Text(text =>
                    {
                        text.DefaultTextStyle(InitialsStyle.FontColor(_model.FontColor).FontSize(10));
                        text.AlignCenter();
                        text.Span(FullName());
                        text.ParagraphSpacing(1);
                    });
            });
        });
    }

    private void ComposeHorizontal(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(120, 90, Unit.Millimetre);
            page.Margin(5, Unit.Millimetre);

            if (_model.BackgroundPath is not null)
            {
                page.Background()
                    .AlignMiddle()
                    .AlignCenter()
                    .Image(_model.BackgroundPath)
                    .FitArea();
            }

            page.Content().Layers(layers =>
            {
                if (_model.LogoPath is not null)
                {
                    layers.Layer()
                        .AlignTop()
                        .AlignCenter()
                        .MaxWidth(30, Unit.Millimetre)
                        .MaxHeight(20, Unit.Millimetre)
                        .Image(_model.LogoPath)
                        .FitArea();
                }

                if (_model.QrStream is not null)
                {
                    layers.Layer()
                        .AlignMiddle()
                        .AlignRight()
                        .Background("#FFFFFF")
                        .Padding(2, Unit.Millimetre)
                        .Width(30, Unit.Millimetre)
                        .Height(30, Unit.Millimetre)
                        .Image(_model.QrStream)
                        .FitArea();
                }

                layers.PrimaryLayer()
                    .AlignMiddle()
                    .AlignLeft()
                    .MaxWidth(75, Unit.Millimetre)
                    .Text(text =>
                    {
                        text.DefaultTextStyle(InitialsStyle.FontColor(_model.FontColor));
                        text.AlignRight();
                        text.Span(FullName());
                        text.ParagraphSpacing(1);
                    });
            });
        });
    }
}