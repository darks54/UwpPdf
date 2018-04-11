Public Class CourierB
    Public type = "Core"
    Public name = "Courier-Bold"
    Public up = -100
    Public ut = 50
    Public enc = "cp1252"
    Public cw As New Dictionary(Of Char, Integer)()
    Public uv = New Dictionary(Of Integer, Object) From {
        {0, New Dictionary(Of Integer, Object) From {{0, 0}, {1, 128}}},
        {128, 8364},
        {130, 8218},
        {131, 402},
        {132, 8222},
        {134, New Dictionary(Of Integer, String) From {{0, 8224}, {1, 2}}},
        {136, 710},
        {137, 8240},
        {138, 352},
        {139, 8249},
        {140, 338},
        {142, 381},
        {145, New Dictionary(Of Integer, String) From {{0, 8216}, {1, 2}}},
        {147, New Dictionary(Of Integer, String) From {{0, 8220}, {1, 2}}},
        {149, 8226},
        {150, New Dictionary(Of Integer, String) From {{0, 8211}, {1, 2}}},
        {152, 732},
        {153, 8482},
        {154, 353},
        {155, 8250},
        {156, 339},
        {158, 382},
        {159, 376},
        {160, New Dictionary(Of Integer, String) From {{0, 160}, {1, 96}}}}

    Public Sub New()
        For i = 0 To 255
            cw.Add(ChrW(i), 600)
        Next
    End Sub

    Public Function GetFont() As Dictionary(Of String, Object)
        Return New Dictionary(Of String, Object) From {{"type", type}, {"name", name}, {"up", up}, {"ut", ut}, {"enc", enc}, {"cw", cw}, {"uv", uv}}
    End Function
End Class
