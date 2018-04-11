Partial Public Class Pdf
    Protected angle = 0

    Public Sub Rotate(angle, Optional x = -1, Optional y = -1)
        If x = -1 Then
            x = Me.x
        End If
        If y = -1 Then
            y = Me.y
        End If
        If Not Me.angle = 0 Then
            Me._out("Q")
        End If
        Me.angle = angle
        If Not angle = 0 Then
            angle *= Math.PI / 180
            Dim c = Math.Cos(angle)
            Dim s = Math.Sin(angle)
            Dim cx = x * Me.k
            Dim cy = (Me.h - y) * Me.k
            Me._out(String.Format("q {0:0.00000} {1:0.00000} {2:0.00000} {3:0.00000} {4:0.00} {5:0.00} cm 1 0 0 1 {6:0.00} {7:0.00} cm", c, s, -s, c, cx, cy, -cx, -cy))
        End If
    End Sub

End Class
