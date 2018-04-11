Partial Public Class Pdf
    Public Sub RoundedRect(x, y, w, h, r, Optional style = "")

        Dim k = Me.k
        Dim hp = Me.h
        Dim op As String
        If style = "F" Then
            op = "f"
        ElseIf style = "FD" Or style = "DF" Then
            op = "B"
        Else
            op = "S"
        End If
        Dim MyArc = 4 / 3 * (Math.Sqrt(2) - 1)
        Me._out(String.Format("{0:0.00} {1:0.00} m", (x + r) * k, (hp - y) * k))
        Dim xc = x + w - r
        Dim yc = y + r
        Me._out(String.Format("{0:0.00} {1:0.00} l", xc * k, (hp - y) * k))
        Me._Arc(xc + r * MyArc, yc - r, xc + r, yc - r * MyArc, xc + r, yc)
        xc = x + w - r
        yc = y + h - r
        Me._out(String.Format("{0:0.00} {1:0.00} l", (x + w) * k, (hp - yc) * k))
        Me._Arc(xc + r, yc + r * MyArc, xc + r * MyArc, yc + r, xc, yc + r)
        xc = x + r
        yc = y + h - r
        Me._out(String.Format("{0:0.00} {1:0.00} l", xc * k, (hp - (y + h)) * k))
        Me._Arc(xc - r * MyArc, yc + r, xc - r, yc + r * MyArc, xc - r, yc)
        xc = x + r
        yc = y + r
        Me._out(String.Format("{0:0.00} {1:0.00} l", (x) * k, (hp - yc) * k))
        Me._Arc(xc - r, yc - r * MyArc, xc - r * MyArc, yc - r, xc, yc - r)
        Me._out(op)
    End Sub

    Protected Sub _Arc(x1, y1, x2, y2, x3, y3)
        Dim h = Me.h
        Me._out(String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00} c",
                              x1 * Me.k,
                              (h - y1) * Me.k,
                              x2 * Me.k,
                              (h - y2) * Me.k,
                              x3 * Me.k,
                              (h - y3) * Me.k))
    End Sub
End Class
