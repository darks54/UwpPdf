Partial Public Class Pdf
    Dim legends
    Dim wLegend
    Dim sum
    Dim NbVal

    Private Sub _setLegends(data As Dictionary(Of String, Integer), format As String)

        Me.legends = Array()
        Me.wLegend = 0
        Me.sum = array_sum(data)
        Me.NbVal = count(data)
        For Each (data As l=>val)        
            Dim p = sprintf("%.2f", val / Me.sum * 100) & "%"
            Dim legend = str_replace(Array("%l", "%v", "%p"), Array(l, val, p), format)
            Me.legends[]=legend
            Me.wLegend = max(Me.GetStringWidth(legend), Me.wLegend)
        Next
    End Sub

    Public Sub DiagCirculaire(largeur As Integer, hauteur As Integer, data As Dictionary(Of String, Integer), format As String, Optional couleurs As List(Of Integer()) = Nothing)

        Me.SetFont("Courier", "", 10)
        Me._setLegends(data, format)

        Dim XPage = Me.GetX()
        Dim YPage = Me.GetY()
        Dim marge = 2
        Dim hLegende = 5
        Dim rayon = Math.Min(largeur - marge * 4 - hLegende - Me.wLegend, hauteur - marge * 2)
        rayon = Math.Floor(rayon / 2)
        Dim XDiag = XPage + marge + rayon
        Dim YDiag = YPage + marge + rayon
        If couleurs Is Nothing Then
            For i = 0 To Me.NbVal - 1
                Dim gray = i * intval(255 / Me.NbVal)
                couleurs(i) = Array(gray, gray, gray)
            Next
        End If

        'Secteurs
        Me.SetLineWidth(0.2)
        Dim angleDebut = 0
        Dim angleFin = 0
        Dim i = 0
        For Each val In data.Values
            angle = (val * 360) / doubleval(Me.sum)
            If (angle! = 0) Then
                angleFin = angleDebut + angle
                Me.SetFillColor(couleurs(i)(0), couleurs(i)(1), couleurs(i)(2))
                Me.Sector(XDiag, YDiag, rayon, angleDebut, angleFin)
                angleDebut += angle
            End If
            i += 1
        Next

        'Légendes
        Me.SetFont("Courier", "", 10)
        Dim x1 = XPage + 2 * rayon + 4 * marge
        Dim x2 = x1 + hLegende + marge
        Dim y1 = YDiag - rayon + (2 * rayon - Me.NbVal * (hLegende + marge)) / 2
        For i = 0 To Me.NbVal - 1
            Me.SetFillColor(couleurs(i)(0), couleurs(i)(1), couleurs(i)(2))
            Me.Rect(x1, y1, hLegende, hLegende, "DF")
            Me.SetXY(x2, y1)
            Me.Cell(0, hLegende, Me.legends(i))
            y1 += hLegende + marge
        Next
    End Sub

    Public Sub DiagBatons(largeur As Integer, hauteur As Integer, data As Dictionary(Of String, Integer), format As String, Optional couleur As List(Of Integer()) = Nothing, Optional maxValRepere As Integer = 0, Optional nbIndRepere As Integer = 4)

        Me.SetFont("Courier", "", 10)
        Me._setLegends(data, format)

        Dim XPage = Me.GetX()
        Dim YPage = Me.GetY()
        Dim marge = 2
        Dim YDiag = YPage + marge
        Dim hDiag = Math.Floor(hauteur - marge * 2)
        Dim XDiag = XPage + marge * 2 + Me.wLegend
        Dim lDiag = Math.Floor(largeur - marge * 3 - Me.wLegend)
        If couleur Is Nothing Then
            couleur = Array(155, 155, 155)
        End If
        If maxValRepere = 0 Then
            maxValRepere = Math.Max(data)
        End If
        Dim valIndRepere = ceil(maxValRepere / nbIndRepere)
        maxValRepere = valIndRepere * nbIndRepere
        Dim lRepere = Math.Floor(lDiag / nbIndRepere)
        lDiag = lRepere * nbIndRepere
        Dim unite = lDiag / maxValRepere
        Dim hBaton = Math.Floor(hDiag / (Me.NbVal + 1))
        hDiag = hBaton * (Me.NbVal + 1)
        Dim eBaton = Math.Floor(hBaton * 80 / 100)

        Me.SetLineWidth(0.2)
        Me.Rect(XDiag, YDiag, lDiag, hDiag)

        Me.SetFont("Courier", "", 10)
        Me.SetFillColor(couleur(0), couleur(1), couleur(2))
        Dim i = 0
        For Each val In data.Values
            'Barre
            Dim xval = XDiag
            Dim lval = CInt(val * unite)
            Dim yval = YDiag + (i + 1) * hBaton - eBaton / 2
            Dim hval = eBaton
            Me.Rect(xval, yval, lval, hval, "DF")
            'Légende
            Me.SetXY(0, yval)
            Me.Cell(xval - marge, hval, Me.legends(i), 0, 0, "R")
            i += 1
        Next

        'Echelles
        For i = 0 To nbIndRepere
            Dim xpos = XDiag + lRepere * i
            Me.Line(xpos, YDiag, xpos, YDiag + hDiag)
            Dim val = i * valIndRepere
            xpos = XDiag + lRepere * i - Me.GetStringWidth(val) / 2
            Dim ypos = YDiag + hDiag - marge
            Me.Text(xpos, ypos, val)
        Next
    End Sub
End Class
