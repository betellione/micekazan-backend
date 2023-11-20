using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace WebApp1.Services.PdfGenerator;

public class TicketDocument(TicketDocumentModel model) : IDocument
{
    private static readonly TextStyle InitialsStyled = TextStyle.Default.FontFamily("Montserrat").FontSize(30).Bold();

    private string FullName()
    {
        return (string.IsNullOrEmpty(model.Name), string.IsNullOrEmpty(model.Surname)) switch
        {
            (false, false) => $"{model.Name}\n{model.Surname}",
            (true, false) => model.Surname!,
            (false, true) => model.Name!,
            _ => string.Empty,
        };
    }

    private void ComposeVertical(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(90, 120, Unit.Millimetre);
            page.Margin(5, Unit.Millimetre);

            if (model.BackgroundPath is not null)
            {
                page.Background()
                    .AlignMiddle()
                    .AlignCenter()
                    .Image(model.BackgroundPath)
                    .FitArea();
            }

            page.Content().Layers(layers =>
            {
                if (model.LogoPath is not null)
                {
                    layers.Layer()
                        .AlignTop()
                        .AlignLeft()
                        .MaxWidth(30, Unit.Millimetre)
                        .MaxHeight(20, Unit.Millimetre)
                        .Image(model.LogoPath)
                        .FitArea();
                }

                if (model.QrPath is not null)
                {
                    layers.Layer()
                        .AlignRight()
                        .AlignBottom()
                        .Background("#ffffff")
                        .Padding(2, Unit.Millimetre)
                        .Width(30, Unit.Millimetre)
                        .Height(30, Unit.Millimetre)
                        .Image(model.QrPath)
                        .FitArea();
                }

                layers.PrimaryLayer()
                    .AlignMiddle()
                    .MaxWidth(80, Unit.Millimetre)
                    .Text(FullName())
                    .LineHeight(1)
                    .Style(InitialsStyled)
                    .FontColor(model.FontColor);
            });
        });
    }

    public void Compose(IDocumentContainer container)
    {
        if (!model.IsHorizontal)
        {
            ComposeVertical(container);
            return;
        }

        container.Page(page =>
        {
            page.Size(120, 90, Unit.Millimetre);
            page.Margin(5, Unit.Millimetre);

            if (model.BackgroundPath is not null)
            {
                page.Background()
                    .AlignMiddle()
                    .AlignCenter()
                    .Image(model.BackgroundPath)
                    .FitArea();
            }

            page.Content().Layers(layers =>
            {
                if (model.LogoPath is not null)
                {
                    layers.Layer()
                        .AlignTop()
                        .AlignLeft()
                        .MaxWidth(30, Unit.Millimetre)
                        .MaxHeight(20, Unit.Millimetre)
                        .Image(model.LogoPath)
                        .FitArea();
                }

                if (model.QrPath is not null)
                {
                    layers.Layer()
                        .AlignMiddle()
                        .AlignRight()
                        .Background("#ffffff")
                        .Padding(2, Unit.Millimetre)
                        .Width(30, Unit.Millimetre)
                        .Height(30, Unit.Millimetre)
                        .Image(model.QrPath)
                        .FitArea();
                }

                layers.PrimaryLayer()
                    .AlignMiddle()
                    .MaxWidth(75, Unit.Millimetre)
                    .Text(FullName())
                    .LineHeight(1)
                    .Style(InitialsStyled)
                    .FontColor(model.FontColor);
            });
        });
    }
}