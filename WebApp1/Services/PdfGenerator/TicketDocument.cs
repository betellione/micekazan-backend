using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace WebApp1.Services.PdfGenerator;

public class TicketDocument : IDocument
{
    private static readonly TextStyle InitialsStyled = TextStyle.Default.FontFamily("Montserrat").FontSize(30).Bold();
    private readonly TicketDocumentModel _model;

    public TicketDocument(TicketDocumentModel model)
    {
        _model = model;
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
                        .AlignLeft()
                        .MaxWidth(30, Unit.Millimetre)
                        .MaxHeight(20, Unit.Millimetre)
                        .Image(_model.LogoPath)
                        .FitArea();
                }

                if (_model.QrStream is not null)
                {
                    layers.Layer()
                        .AlignRight()
                        .AlignBottom()
                        .Background("#ffffff")
                        .Padding(2, Unit.Millimetre)
                        .Width(30, Unit.Millimetre)
                        .Height(30, Unit.Millimetre)
                        .Image(_model.QrStream)
                        .FitArea();
                }

                layers.PrimaryLayer()
                    .AlignMiddle()
                    .MaxWidth(80, Unit.Millimetre)
                    .Text(FullName())
                    .LineHeight(1)
                    .Style(InitialsStyled)
                    .FontColor(_model.FontColor);
            });
        });
    }

    public void Compose(IDocumentContainer container)
    {
        if (!_model.IsHorizontal)
        {
            ComposeVertical(container);
            return;
        }

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
                        .AlignLeft()
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
                        .Background("#ffffff")
                        .Padding(2, Unit.Millimetre)
                        .Width(30, Unit.Millimetre)
                        .Height(30, Unit.Millimetre)
                        .Image(_model.QrStream)
                        .FitArea();
                }

                layers.PrimaryLayer()
                    .AlignMiddle()
                    .MaxWidth(75, Unit.Millimetre)
                    .Text(FullName())
                    .LineHeight(1)
                    .Style(InitialsStyled)
                    .FontColor(_model.FontColor);
            });
        });
    }
}