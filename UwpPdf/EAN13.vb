Partial Public Class Pdf
    Public Sub EAN13(x As Double, y As Double, barcode As String, Optional h As Double = 16, Optional w As Double = 0.35)
        _barcode(x, y, barcode, h, w, 13)
    End Sub

    Public Sub UPC_A(x, y, barcode, Optional h = 16, Optional w = 0.35)
        _barcode(x, y, barcode, h, w, 12)
    End Sub

    Protected Function _getCheckDigit(barcode)

        'Calcule le chiffre de contrôle
        Dim sum = 0
        For i = 1 To 11 Step 2
            sum += 3 * Convert.ToInt32(barcode(i))
        Next
        For i = 0 To 10 Step 2
            sum += Convert.ToInt32(barcode(i))
        Next
        Dim r = sum Mod 10
        If (r > 0) Then
            r = 10 - r
        End If
        Return r
    End Function

    Protected Function _testCheckDigit(barcode) As Boolean

        'Vérifie le chiffre de contrôle
        Dim sum = 0
        For i = 1 To 11 Step 2
            sum += 3 * Convert.ToInt32(barcode(i))
        Next
        For i = 0 To 10 Step 2
            sum += Convert.ToInt32(barcode(i))
        Next
        Return (sum + barcode(12)) Mod 10 = 0
    End Function

    Protected Sub _barcode(x As Double, y As Double, barcode As String, h As Double, w As Double, len As Double)

        'Ajoute des 0 si nécessaire
        barcode.PadLeft(len - 1, "0")
        If len = 12 Then
            barcode = "0" & barcode
        End If
        'Ajoute ou teste le chiffre de contrôle
        If barcode.Length = 12 Then
            barcode &= _getCheckDigit(barcode)
        ElseIf Not _testCheckDigit(barcode) Then
            Erreur("Incorrect check digit")
        End If
        'Convertit les chiffres en barres
        Dim codes = New Dictionary(Of String, Dictionary(Of String, String)) From {
        {"A", New Dictionary(Of String, String) From {
            {"0", "0001101"}, {"1", "0011001"}, {"2", "0010011"}, {"3", "0111101"}, {"4", "0100011"},
            {"5", "0110001"}, {"6", "0101111"}, {"7", "0111011"}, {"8", "0110111"}, {"9", "0001011"}}},
        {"B", New Dictionary(Of String, String) From {
            {"0", "0100111"}, {"1", "0110011"}, {"2", "0011011"}, {"3", "0100001"}, {"4", "0011101"},
            {"5", "0111001"}, {"6", "0000101"}, {"7", "0010001"}, {"8", "0001001"}, {"9", "0010111"}}},
        {"C", New Dictionary(Of String, String) From {
            {"0", "1110010"}, {"1", "1100110"}, {"2", "1101100"}, {"3", "1000010"}, {"4", "1011100"},
            {"5", "1001110"}, {"6", "1010000"}, {"7", "1000100"}, {"8", "1001000"}, {"9", "1110100"}}}}

        Dim parities = New Dictionary(Of String, List(Of String)) From {
        {"0", New List(Of String) From {"A", "A", "A", "A", "A", "A"}},
        {"1", New List(Of String) From {"A", "A", "B", "A", "B", "B"}},
        {"2", New List(Of String) From {"A", "A", "B", "B", "A", "B"}},
        {"3", New List(Of String) From {"A", "A", "B", "B", "B", "A"}},
        {"4", New List(Of String) From {"A", "B", "A", "A", "B", "B"}},
        {"5", New List(Of String) From {"A", "B", "B", "A", "A", "B"}},
        {"6", New List(Of String) From {"A", "B", "B", "B", "A", "A"}},
        {"7", New List(Of String) From {"A", "B", "A", "B", "A", "B"}},
        {"8", New List(Of String) From {"A", "B", "A", "B", "B", "A"}},
        {"9", New List(Of String) From {"A", "B", "B", "A", "B", "A"}}}

        Dim code As String = "101"
        Dim p As List(Of String) = parities(barcode(0))
        For i = 1 To 6
            code &= codes(p(i - 1))(barcode(i))
        Next
        code &= "01010"
        For i = 7 To 12
            code &= codes("C")(barcode(i))
        Next
        code &= "101"
        'Dessine les barres
        For i = 0 To code.Length - 1
            If code(i) = "1" Then
                Rect(x + i * w, y, w, h, "F")
            End If
        Next
        'Imprime le texte sous le code-barres
        SetFont(ListFont.Helvetica, ListFontStyle.N, False, 12)
        Text(x, y + h + 11 / Me.k, barcode.Substring(barcode.Length - len, len))
    End Sub
End Class