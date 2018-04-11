Imports UwpPdf

Public NotInheritable Class MainPage
    Inherits Page

    Private Async Sub button_Click(sender As Object, e As RoutedEventArgs) Handles button.Click


        Dim pdf As Pdf = New Pdf
        pdf.SetFont(ListFont.Helvetica, ListFontStyle.N, False, 10)
        pdf.AddPage()
        pdf.Bookmark("Page 1", False)
        pdf.Bookmark("Paragraphe 1", False, 1, -1)
        pdf.Cell(0, 6, "Paragraphe 1")
        pdf.Ln(50)
        pdf.Bookmark("Paragraphe 2", False, 1, -1)
        pdf.Cell(0, 6, "Paragraphe 2")

        pdf.AddPage()
        pdf.Bookmark("Page 2", False)
        pdf.Bookmark("Paragraphe 3", False, 1, -1)
        pdf.Cell(0, 6, "Paragraphe 3")
        Await pdf.OutputAsync()
    End Sub

End Class