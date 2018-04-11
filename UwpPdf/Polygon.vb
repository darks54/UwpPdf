Partial Public Class Pdf
    Public Sub Polygon(points As List(Of Integer), Optional style As String = "D")
        Dim op As String = ""
        'Draw a polygon
        If style = "F" Then
            op = "f"
        ElseIf (style = "FD" Or style = "DF") Then
            op = "b"
        Else
            op = "s"
        End If
        h = Me.h
        k = Me.k

        Dim points_string = ""

        For i = 0 To points.Count - 1 Step 2
            points_string &= String.Format("{0:0.00} {1:0.00}", points(i) * k, (h - points(i + 1)) * k)
            If i = 0 Then
                points_string &= " m "
            Else
                points_string &= " l "
            End If
        Next
        Me._out(points_string & op)
    End Sub
End Class
