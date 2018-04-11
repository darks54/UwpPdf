Partial Public Class Pdf
    Public Sub SetDash(Optional black As Integer? = Nothing, Optional white As Integer? = Nothing)
        Dim s As String = ""
        If black IsNot Nothing Then
            s = String.Format("[{0:0.000} {1:0.000}] 0 d", black * Me.k, white * Me.k)
        Else
            s = "[] 0 d"
        End If
        Me._out(s)
    End Sub
End Class
