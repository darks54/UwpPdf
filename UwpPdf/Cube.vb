Partial Public Class Pdf
    Public Sub Cube(Optional a = 10, Optional b = 10, Optional c = 10, Optional scale = 1, Optional alfax = 10, Optional alfay = 10, Optional alfaz = 10)

        Dim x = (Me.w - Me.rMargin) / 2 - (a / 2)
        Dim y = (Me.h - Me.tMargin) / 2 - (b / 2)

        Dim cubo = New List(Of List(Of Double)) From {
            New List(Of Double) From {x + a, y + b, c},
            New List(Of Double) From {x + a, y + b, -c},
            New List(Of Double) From {x - a, y + b, -c},
            New List(Of Double) From {x - a, y + b, c},
            New List(Of Double) From {x + a, y - b, c},
            New List(Of Double) From {x + a, y - b, -c},
            New List(Of Double) From {x - a, y - b, -c},
            New List(Of Double) From {x - a, y - b, c}}

        Dim senx = Math.Sin(alfax * Math.PI / 180)
        Dim cosx = Math.Cos(alfax * Math.PI / 180)
        Dim seny = Math.Sin(alfay * Math.PI / 180)
        Dim cosy = Math.Cos(alfay * Math.PI / 180)
        Dim senz = Math.Sin(alfaz * Math.PI / 180)
        Dim cosz = Math.Cos(alfaz * Math.PI / 180)
        a = cosy * cosz
        b = cosy * senz
        c = -seny
        Dim d = senx * seny * cosz - cosx * senz
        Dim e = senx * seny * senz + cosx * cosz
        Dim f = senx * cosy
        Dim g = cosx * seny * cosz + senx * senz
        Dim h = cosx * seny * senz - senx * cosz
        Dim k = cosx * cosy

        For i = 0 To cubo.Count - 1
            cubo(i)(0) = (a * cubo(i)(0) + b * cubo(i)(1) + c * cubo(i)(2)) * scale
            cubo(i)(1) = (d * cubo(i)(0) + e * cubo(i)(1) + f * cubo(i)(2)) * scale
            cubo(i)(2) = (g * cubo(i)(0) + h * cubo(i)(1) + k * cubo(i)(2)) * scale
        Next

        'down
        Me.Line(cubo(0)(0), cubo(0)(1), cubo(1)(0), cubo(1)(1))
        Me.Line(cubo(1)(0), cubo(1)(1), cubo(2)(0), cubo(2)(1))
        Me.Line(cubo(2)(0), cubo(2)(1), cubo(3)(0), cubo(3)(1))
        Me.Line(cubo(3)(0), cubo(3)(1), cubo(0)(0), cubo(0)(1))

        'up
        Me.Line(cubo(4)(0), cubo(4)(1), cubo(5)(0), cubo(5)(1))
        Me.Line(cubo(5)(0), cubo(5)(1), cubo(6)(0), cubo(6)(1))
        Me.Line(cubo(6)(0), cubo(6)(1), cubo(7)(0), cubo(7)(1))
        Me.Line(cubo(7)(0), cubo(7)(1), cubo(4)(0), cubo(4)(1))

        'Lateral
        Me.Line(cubo(0)(0), cubo(0)(1), cubo(4)(0), cubo(4)(1))
        Me.Line(cubo(1)(0), cubo(1)(1), cubo(5)(0), cubo(5)(1))
        Me.Line(cubo(2)(0), cubo(2)(1), cubo(6)(0), cubo(6)(1))
        Me.Line(cubo(3)(0), cubo(3)(1), cubo(7)(0), cubo(7)(1))

    End Sub
End Class