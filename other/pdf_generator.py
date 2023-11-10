#!chapter_002/src/snippet_034.py
from borb.pdf import Document
from borb.pdf import Page
from borb.pdf import SingleColumnLayout
from borb.pdf import PageLayout
from borb.pdf import Paragraph
from borb.pdf import PDF
from borb.pdf import Barcode, BarcodeType

from borb.pdf import FlexibleColumnWidthTable, FixedColumnWidthTable
from borb.pdf import TableCell

from borb.pdf.canvas.font.simple_font.true_type_font import TrueTypeFont
from borb.pdf.canvas.font.font import Font
from borb.pdf.canvas.layout.layout_element import Alignment

from decimal import Decimal
from pathlib import Path


qr_link = 'test.ru'
first_name = 'Илья'
last_name = 'Никифоров'

qr_alignment = 'left'

page_w = 400
page_h = 300

font_size = Decimal(30)
font_name = 'Roboto-Medium'


def generation_pdf(
        qr_link=qr_link,
        first_name=first_name,
        last_name=last_name,
        qr_alignment=qr_alignment,
        page_w=page_w,
        page_h=page_h,
        font_size=font_size,
        font_name=font_name,
):
    doc: Document = Document()
    page: Page = Page(Decimal(page_w), Decimal(page_h))
    doc.add_page(page)
    layout: PageLayout = SingleColumnLayout(page)

    font_path: Path = Path(__file__).parent / f"font/{font_name}.ttf"
    custom_font: Font = TrueTypeFont.true_type_font_from_file(font_path)

    layout_qrcode = Barcode(
        qr_link,
        width=Decimal(128),
        height=Decimal(128),
        type=BarcodeType.QR,
    )

    if qr_alignment == "left":
        layout_first_name = Paragraph(first_name, font=custom_font, font_size=Decimal(font_size),
                                      horizontal_alignment=Alignment.RIGHT,
                                      vertical_alignment=Alignment.BOTTOM)
        layout_last_name = Paragraph(last_name, font=custom_font, font_size=Decimal(font_size),
                                     horizontal_alignment=Alignment.RIGHT,
                                     vertical_alignment=Alignment.TOP)
    else:
        layout_first_name = Paragraph(first_name, font=custom_font, font_size=Decimal(font_size),
                                      horizontal_alignment=Alignment.LEFT,
                                      vertical_alignment=Alignment.BOTTOM)
        layout_last_name = Paragraph(last_name, font=custom_font, font_size=Decimal(font_size),
                                     horizontal_alignment=Alignment.LEFT,
                                     vertical_alignment=Alignment.TOP)

    if qr_alignment == "left":
        layout.add(
            FixedColumnWidthTable(number_of_columns=2, number_of_rows=2, vertical_alignment=Alignment.MIDDLE)
            .add(layout_qrcode)
            .add(layout_first_name)
            .add(TableCell(layout_last_name, column_span=2))
            .set_padding_on_all_cells(Decimal(2), Decimal(2), Decimal(2), Decimal(2))
            .no_borders()
        )
    else:
        layout.add(
            FixedColumnWidthTable(number_of_columns=2, number_of_rows=2, vertical_alignment=Alignment.MIDDLE)
            .add(layout_first_name)
            .add(layout_qrcode)
            .add(TableCell(layout_last_name, column_span=2))
            .set_padding_on_all_cells(Decimal(2), Decimal(2), Decimal(2), Decimal(2))
            .no_borders()
        )

    with open("output.pdf", "wb") as pdf_file_handle:
        PDF.dumps(pdf_file_handle, doc)


if __name__ == "__main__":
    generation_pdf()
