Partial Public Class Pdf
    Public Sub Circle(x, y, r, Optional style = "D")
        Ellipse(x, y, r, r, style)
    End Sub

    Public Sub Ellipse(x, y, rx, ry, Optional style = "D")
        Dim op As String
        If style = "F" Then
            op = "f"
        ElseIf (style = "FD" Or style = "DF") Then
            op = "B"
        Else
            op = "S"
        End If
        Dim lx = 4 / 3 * (Math.Sqrt(2) - 1) * rx
        Dim ly = 4 / 3 * (Math.Sqrt(2) - 1) * ry
        k = Me.k
        h = Me.h
        Me._out(String.Format("{0:0.00} {1:0.00} m {2:0.00} {3:0.00} {4:0.00} {5:0.00} {6:0.00} {7:0.00} c",
        (x + rx) * k, (h - y) * k,
        (x + rx) * k, (h - (y - ly)) * k,
        (x + lx) * k, (h - (y - ry)) * k,
        x * k, (h - (y - ry)) * k))

        Me._out(String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00} c",
        (x - lx) * k, (h - (y - ry)) * k,
        (x - rx) * k, (h - (y - ly)) * k,
        (x - rx) * k, (h - y) * k))

        Me._out(String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00} c",
        (x - rx) * k, (h - (y + ly)) * k,
        (x - lx) * k, (h - (y + ry)) * k,
        x * k, (h - (y + ry)) * k))

        Me._out(String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00} c {6}",
        (x + lx) * k, (h - (y + ry)) * k,
        (x + rx) * k, (h - (y + ly)) * k,
        (x + rx) * k, (h - y) * k,
        op))
    End Sub
End Class
