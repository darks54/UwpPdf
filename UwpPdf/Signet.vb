Partial Public Class Pdf

    Protected outlines As New List(Of Dictionary(Of String, Object))
    Protected outlineRoot As Integer

    Public Sub Bookmark(txt As String, Optional isUTF8 As Boolean = False, Optional level As Integer = 0, Optional y As Integer = 0)

        If Not isUTF8 Then
            txt = _utf8encode(txt)
        End If
        If y = -1 Then
            y = Me.GetY()
        End If
        Me.outlines.Add(New Dictionary(Of String, Object) From {{"t", txt}, {"l", level}, {"y", (Me.h - y) * Me.k}, {"p", Me.PageNo()}})
    End Sub

    Public Sub _putbookmarks()
        Dim nb As Integer = Me.outlines.Count
        If nb = 0 Then
            Return
        End If
        Dim lru As New Dictionary(Of Integer, Integer)
        Dim level As Integer = 0
        Dim i = 0
        For Each tab1 In Me.outlines
            Dim o As Dictionary(Of String, Object) = tab1
            If o("l") > 0 Then
                Dim parent = lru(o("l") - 1)
                ' Set parent And last pointers
                Me.outlines(i)("parent") = parent
                Me.outlines(parent)("last") = i
                If o("l") > level Then
                    ' Level increasing: set first pointer
                    Me.outlines(parent)("first") = i
                End If
            Else
                Me.outlines(i)("parent") = nb
            End If
            If o("l") <= level AndAlso i > 0 Then
                ' Set prev And next pointers
                Dim prev = lru(o("l"))
                Me.outlines(prev)("next") = i
                Me.outlines(i)("prev") = prev
            End If
            If lru.ContainsKey(CInt(o("l"))) Then
                lru(CInt(o("l"))) = i
            Else
                lru.Add(CInt(o("l")), i)
            End If
            level = o("l")
            i += 1
        Next
        ' Outline items
        Dim n = Me.n + 1
        i = 0
        For Each tab2 In Me.outlines
            Dim o = tab2
            Me._newobj()
            Me._put("<</Title " & Me._textstring(o("t")))
            Me._put("/Parent " & (n + o("parent")) & " 0 R")
            If o.ContainsKey("prev") Then
                Me._put("/Prev " & (n + o("prev")) & " 0 R")
            End If
            If o.ContainsKey("next") Then
                Me._put("/Next " & (n + o("next")) & " 0 R")
            End If
            If o.ContainsKey("first") Then
                Me._put("/First " & (n + o("first")) & " 0 R")
            End If
            If o.ContainsKey("last") Then
                Me._put("/Last " & (n + o("last")) & " 0 R")
            End If
            'Me._put(sprintf("/Dest [%d 0 R /XYZ 0 %.2F null]", Me.PageInfo(o("p"))("n"), o("y")))
            Me._put(String.Format("/Dest [{0} 0 R /XYZ 0 {1:0.00} null]", Me.PageInfo(o("p"))("n"), o("y")))
            Me._put("/Count 0>>")
            Me._put("endobj")
            i += 1
        Next
        ' Outline root
        Me._newobj()
        Me.outlineRoot = Me.n
        Me._put("<</Type /Outlines /First " & n & " 0 R")
        Me._put("/Last " & (n + lru(0)) & " 0 R>>")
        Me._put("endobj")
    End Sub
End Class
