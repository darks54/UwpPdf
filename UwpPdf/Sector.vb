Partial Public Class Pdf
    Public Sub Sector(xc, yc, r, a, b, Optional style = "FD", Optional cw = True, Optional o = 90)
        Dim d
        Dim MyArc
        Dim d0 = a - b
        If (cw) Then
            d = b
            b = o - a
            a = o - d
        Else
            b += o
            a += o
        End If
        While (a < 0)
            a += 360
        End While
        While (a > 360)
            a -= 360
        End While
        While (b < 0)
            b += 360
        End While
        While (b > 360)
            b -= 360
        End While
        If (a > b) Then
            b += 360
        End If
        b = b / 360 * 2 * Math.PI
        a = a / 360 * 2 * Math.PI
        d = b - a
        If (d = 0 And Not d0 = 0) Then
            d = 2 * Math.PI
        End If
        k = Me.k
        Dim hp = Me.h
        If Math.Sin(d / 2) Then
            MyArc = 4 / 3 * (1 - Math.Cos(d / 2)) / Math.Sin(d / 2) * r
        Else
            MyArc = 0
        End If
        'first put the center

        Me._out(String.Format("{0:0.00} {1:0.00} m", (xc) * k, (hp - yc) * k))
        'put the first point
        Me._out(String.Format("{0:0.00} {1:0.00} l", (xc + r * Math.Cos(a)) * k, ((hp - (yc - r * Math.Sin(a))) * k)))
        'draw the arc
        If (d < Math.PI / 2) Then
            Me._Arc(xc + r * Math.Cos(a) + MyArc * Math.Cos(Math.PI / 2 + a),
                        yc - r * Math.Sin(a) - MyArc * Math.Sin(Math.PI / 2 + a),
                        xc + r * Math.Cos(b) + MyArc * Math.Cos(b - Math.PI / 2),
                        yc - r * Math.Sin(b) - MyArc * Math.Sin(b - Math.PI / 2),
                        xc + r * Math.Cos(b),
                        yc - r * Math.Sin(b)
                        )
        Else
            b = a + d / 4
            MyArc = 4 / 3 * (1 - Math.Cos(d / 8)) / Math.Sin(d / 8) * r
            Me._Arc(xc + r * Math.Cos(a) + MyArc * Math.Cos(Math.PI / 2 + a),
            yc - r * Math.Sin(a) - MyArc * Math.Sin(Math.PI / 2 + a),
            xc + r * Math.Cos(b) + MyArc * Math.Cos(b - Math.PI / 2),
            yc - r * Math.Sin(b) - MyArc * Math.Sin(b - Math.PI / 2),
            xc + r * Math.Cos(b),
            yc - r * Math.Sin(b)
            )
            a = b
            b = a + d / 4
            Me._Arc(xc + r * Math.Cos(a) + MyArc * Math.Cos(Math.PI / 2 + a),
                yc - r * Math.Sin(a) - MyArc * Math.Sin(Math.PI / 2 + a),
                xc + r * Math.Cos(b) + MyArc * Math.Cos(b - Math.PI / 2),
                yc - r * Math.Sin(b) - MyArc * Math.Sin(b - Math.PI / 2),
                xc + r * Math.Cos(b),
                yc - r * Math.Sin(b)
                )
            a = b
            b = a + d / 4
            Me._Arc(xc + r * Math.Cos(a) + MyArc * Math.Cos(Math.PI / 2 + a),
            yc - r * Math.Sin(a) - MyArc * Math.Sin(Math.PI / 2 + a),
            xc + r * Math.Cos(b) + MyArc * Math.Cos(b - Math.PI / 2),
            yc - r * Math.Sin(b) - MyArc * Math.Sin(b - Math.PI / 2),
            xc + r * Math.Cos(b),
            yc - r * Math.Sin(b)
            )
            a = b
            b = a + d / 4
            Me._Arc(xc + r * Math.Cos(a) + MyArc * Math.Cos(Math.PI / 2 + a),
            yc - r * Math.Sin(a) - MyArc * Math.Sin(Math.PI / 2 + a),
            xc + r * Math.Cos(b) + MyArc * Math.Cos(b - Math.PI / 2),
            yc - r * Math.Sin(b) - MyArc * Math.Sin(b - Math.PI / 2),
            xc + r * Math.Cos(b),
            yc - r * Math.Sin(b)
            )
        End If
        'terminate drawing
        Dim op As String = ""
        If style = "F" Then
            op = "f"
        ElseIf style = "FD" Or style = "DF" Then
            op = "b"
        Else
            op = "s"
        End If
        Me._out(op)
    End Sub

End Class
