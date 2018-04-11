Imports System.Text

Partial Public Class Pdf
    Protected T128 As List(Of List(Of Integer))                                        ' Tableau des codes 128
    Protected ABCset As String = ""                                  ' jeu des caractères éligibles au C128
    Protected Aset As String = ""                                    ' Set A du jeu des caractères éligibles
    Protected Bset As String = ""                                    ' Set B du jeu des caractères éligibles
    Protected Cset As String = ""                                    ' Set C du jeu des caractères éligibles
    Protected SetFrom = New Dictionary(Of String, String) From {{"A", ""}, {"B", ""}, {"C", ""}}                                  ' Convertisseur source des jeux vers le tableau
    Protected SetTo = New Dictionary(Of String, String) From {{"A", ""}, {"B", ""}, {"C", ""}}                                        ' Convertisseur destination des jeux vers le tableau
    Protected JStart = New Dictionary(Of String, Integer) From {{"A", 103}, {"B", 104}, {"C", 105}} ' Caractères de sélection de jeu au début du C128
    Protected JSwap = New Dictionary(Of String, Integer) From {{"A", 101}, {"B", 100}, {"C", 99}}   ' Caractères de changement de jeu

    '____________________________ Extension du constructeur _______________________
    Protected Sub _construct128()
        T128 = New List(Of List(Of Integer))

        Me.T128.Add(New List(Of Integer) From {2, 1, 2, 2, 2, 2})           '0 : [ ]               ' composition des caractères
        Me.T128.Add(New List(Of Integer) From {2, 2, 2, 1, 2, 2})           '1 : [!]
        Me.T128.Add(New List(Of Integer) From {2, 2, 2, 2, 2, 1})           '2 : ["]
        Me.T128.Add(New List(Of Integer) From {1, 2, 1, 2, 2, 3})           '3 : [#]
        Me.T128.Add(New List(Of Integer) From {1, 2, 1, 3, 2, 2})           '4 : []
        Me.T128.Add(New List(Of Integer) From {1, 3, 1, 2, 2, 2})           '5 : [%]
        Me.T128.Add(New List(Of Integer) From {1, 2, 2, 2, 1, 3})           '6 : [&]
        Me.T128.Add(New List(Of Integer) From {1, 2, 2, 3, 1, 2})           '7 : [']
        Me.T128.Add(New List(Of Integer) From {1, 3, 2, 2, 1, 2})           '8 : [(]
        Me.T128.Add(New List(Of Integer) From {2, 2, 1, 2, 1, 3})           '9 : [)]
        Me.T128.Add(New List(Of Integer) From {2, 2, 1, 3, 1, 2})           '10 : [*]
        Me.T128.Add(New List(Of Integer) From {2, 3, 1, 2, 1, 2})           '11 : [+]
        Me.T128.Add(New List(Of Integer) From {1, 1, 2, 2, 3, 2})           '12 : [,]
        Me.T128.Add(New List(Of Integer) From {1, 2, 2, 1, 3, 2})           '13 : [-]
        Me.T128.Add(New List(Of Integer) From {1, 2, 2, 2, 3, 1})           '14 : [.]
        Me.T128.Add(New List(Of Integer) From {1, 1, 3, 2, 2, 2})           '15 : [/]
        Me.T128.Add(New List(Of Integer) From {1, 2, 3, 1, 2, 2})           '16 : [0]
        Me.T128.Add(New List(Of Integer) From {1, 2, 3, 2, 2, 1})           '17 : [1]
        Me.T128.Add(New List(Of Integer) From {2, 2, 3, 2, 1, 1})           '18 : [2]
        Me.T128.Add(New List(Of Integer) From {2, 2, 1, 1, 3, 2})           '19 : [3]
        Me.T128.Add(New List(Of Integer) From {2, 2, 1, 2, 3, 1})           '20 : [4]
        Me.T128.Add(New List(Of Integer) From {2, 1, 3, 2, 1, 2})           '21 : [5]
        Me.T128.Add(New List(Of Integer) From {2, 2, 3, 1, 1, 2})           '22 : [6]
        Me.T128.Add(New List(Of Integer) From {3, 1, 2, 1, 3, 1})           '23 : [7]
        Me.T128.Add(New List(Of Integer) From {3, 1, 1, 2, 2, 2})           '24 : [8]
        Me.T128.Add(New List(Of Integer) From {3, 2, 1, 1, 2, 2})           '25 : [9]
        Me.T128.Add(New List(Of Integer) From {3, 2, 1, 2, 2, 1})           '26 : [:]
        Me.T128.Add(New List(Of Integer) From {3, 1, 2, 2, 1, 2})           '27 : []
        Me.T128.Add(New List(Of Integer) From {3, 2, 2, 1, 1, 2})           '28 : [<]
        Me.T128.Add(New List(Of Integer) From {3, 2, 2, 2, 1, 1})           '29 : [=]
        Me.T128.Add(New List(Of Integer) From {2, 1, 2, 1, 2, 3})           '30 : [>]
        Me.T128.Add(New List(Of Integer) From {2, 1, 2, 3, 2, 1})           '31 : [?]
        Me.T128.Add(New List(Of Integer) From {2, 3, 2, 1, 2, 1})           '32 : [@]
        Me.T128.Add(New List(Of Integer) From {1, 1, 1, 3, 2, 3})           '33 : [A]
        Me.T128.Add(New List(Of Integer) From {1, 3, 1, 1, 2, 3})           '34 : [B]
        Me.T128.Add(New List(Of Integer) From {1, 3, 1, 3, 2, 1})           '35 : [C]
        Me.T128.Add(New List(Of Integer) From {1, 1, 2, 3, 1, 3})           '36 : [D]
        Me.T128.Add(New List(Of Integer) From {1, 3, 2, 1, 1, 3})           '37 : [E]
        Me.T128.Add(New List(Of Integer) From {1, 3, 2, 3, 1, 1})           '38 : [F]
        Me.T128.Add(New List(Of Integer) From {2, 1, 1, 3, 1, 3})           '39 : [G]
        Me.T128.Add(New List(Of Integer) From {2, 3, 1, 1, 1, 3})           '40 : [h]
        Me.T128.Add(New List(Of Integer) From {2, 3, 1, 3, 1, 1})           '41 : [I]
        Me.T128.Add(New List(Of Integer) From {1, 1, 2, 1, 3, 3})           '42 : [J]
        Me.T128.Add(New List(Of Integer) From {1, 1, 2, 3, 3, 1})           '43 : [k]
        Me.T128.Add(New List(Of Integer) From {1, 3, 2, 1, 3, 1})           '44 : [L]
        Me.T128.Add(New List(Of Integer) From {1, 1, 3, 1, 2, 3})           '45 : [M]
        Me.T128.Add(New List(Of Integer) From {1, 1, 3, 3, 2, 1})           '46 : [n]
        Me.T128.Add(New List(Of Integer) From {1, 3, 3, 1, 2, 1})           '47 : [O]
        Me.T128.Add(New List(Of Integer) From {3, 1, 3, 1, 2, 1})           '48 : [P]
        Me.T128.Add(New List(Of Integer) From {2, 1, 1, 3, 3, 1})           '49 : [Q]
        Me.T128.Add(New List(Of Integer) From {2, 3, 1, 1, 3, 1})           '50 : [R]
        Me.T128.Add(New List(Of Integer) From {2, 1, 3, 1, 1, 3})           '51 : [S]
        Me.T128.Add(New List(Of Integer) From {2, 1, 3, 3, 1, 1})           '52 : [T]
        Me.T128.Add(New List(Of Integer) From {2, 1, 3, 1, 3, 1})           '53 : [U]
        Me.T128.Add(New List(Of Integer) From {3, 1, 1, 1, 2, 3})           '54 : [V]
        Me.T128.Add(New List(Of Integer) From {3, 1, 1, 3, 2, 1})           '55 : [w]
        Me.T128.Add(New List(Of Integer) From {3, 3, 1, 1, 2, 1})           '56 : [x]
        Me.T128.Add(New List(Of Integer) From {3, 1, 2, 1, 1, 3})           '57 : [y]
        Me.T128.Add(New List(Of Integer) From {3, 1, 2, 3, 1, 1})           '58 : [Z]
        Me.T128.Add(New List(Of Integer) From {3, 3, 2, 1, 1, 1})           '59 : [[]
        Me.T128.Add(New List(Of Integer) From {3, 1, 4, 1, 1, 1})           '60 : [\]
        Me.T128.Add(New List(Of Integer) From {2, 2, 1, 4, 1, 1})           '61 : []]
        Me.T128.Add(New List(Of Integer) From {4, 3, 1, 1, 1, 1})           '62 : [^]
        Me.T128.Add(New List(Of Integer) From {1, 1, 1, 2, 2, 4})           '63 : [_]
        Me.T128.Add(New List(Of Integer) From {1, 1, 1, 4, 2, 2})           '64 : [`]
        Me.T128.Add(New List(Of Integer) From {1, 2, 1, 1, 2, 4})           '65 : [a]
        Me.T128.Add(New List(Of Integer) From {1, 2, 1, 4, 2, 1})           '66 : [b]
        Me.T128.Add(New List(Of Integer) From {1, 4, 1, 1, 2, 2})           '67 : [c]
        Me.T128.Add(New List(Of Integer) From {1, 4, 1, 2, 2, 1})           '68 : [d]
        Me.T128.Add(New List(Of Integer) From {1, 1, 2, 2, 1, 4})           '69 : [e]
        Me.T128.Add(New List(Of Integer) From {1, 1, 2, 4, 1, 2})           '70 : [f]
        Me.T128.Add(New List(Of Integer) From {1, 2, 2, 1, 1, 4})           '71 : [g]
        Me.T128.Add(New List(Of Integer) From {1, 2, 2, 4, 1, 1})           '72 : [h]
        Me.T128.Add(New List(Of Integer) From {1, 4, 2, 1, 1, 2})           '73 : [i]
        Me.T128.Add(New List(Of Integer) From {1, 4, 2, 2, 1, 1})           '74 : [j]
        Me.T128.Add(New List(Of Integer) From {2, 4, 1, 2, 1, 1})           '75 : [k]
        Me.T128.Add(New List(Of Integer) From {2, 2, 1, 1, 1, 4})           '76 : [l]
        Me.T128.Add(New List(Of Integer) From {4, 1, 3, 1, 1, 1})           '77 : [m]
        Me.T128.Add(New List(Of Integer) From {2, 4, 1, 1, 1, 2})           '78 : [n]
        Me.T128.Add(New List(Of Integer) From {1, 3, 4, 1, 1, 1})           '79 : [o]
        Me.T128.Add(New List(Of Integer) From {1, 1, 1, 2, 4, 2})           '80 : [p]
        Me.T128.Add(New List(Of Integer) From {1, 2, 1, 1, 4, 2})           '81 : [q]
        Me.T128.Add(New List(Of Integer) From {1, 2, 1, 2, 4, 1})           '82 : [r]
        Me.T128.Add(New List(Of Integer) From {1, 1, 4, 2, 1, 2})           '83 : [s]
        Me.T128.Add(New List(Of Integer) From {1, 2, 4, 1, 1, 2})           '84 : [t]
        Me.T128.Add(New List(Of Integer) From {1, 2, 4, 2, 1, 1})           '85 : [u]
        Me.T128.Add(New List(Of Integer) From {4, 1, 1, 2, 1, 2})           '86 : [v]
        Me.T128.Add(New List(Of Integer) From {4, 2, 1, 1, 1, 2})           '87 : [w]
        Me.T128.Add(New List(Of Integer) From {4, 2, 1, 2, 1, 1})           '88 : [x]
        Me.T128.Add(New List(Of Integer) From {2, 1, 2, 1, 4, 1})           '89 : [y]
        Me.T128.Add(New List(Of Integer) From {2, 1, 4, 1, 2, 1})           '90 : [z]
        Me.T128.Add(New List(Of Integer) From {4, 1, 2, 1, 2, 1})           '91 : [{]
        Me.T128.Add(New List(Of Integer) From {1, 1, 1, 1, 4, 3})           '92 : [|]
        Me.T128.Add(New List(Of Integer) From {1, 1, 1, 3, 4, 1})           '93 : [}]
        Me.T128.Add(New List(Of Integer) From {1, 3, 1, 1, 4, 1})           '94 : [~]
        Me.T128.Add(New List(Of Integer) From {1, 1, 4, 1, 1, 3})           '95 : [DEL]
        Me.T128.Add(New List(Of Integer) From {1, 1, 4, 3, 1, 1})           '96 : [FNC3]
        Me.T128.Add(New List(Of Integer) From {4, 1, 1, 1, 1, 3})           '97 : [FNC2]
        Me.T128.Add(New List(Of Integer) From {4, 1, 1, 3, 1, 1})           '98 : [SHIFT]
        Me.T128.Add(New List(Of Integer) From {1, 1, 3, 1, 4, 1})           '99 : [Cswap]
        Me.T128.Add(New List(Of Integer) From {1, 1, 4, 1, 3, 1})           '100 : [Bswap]
        Me.T128.Add(New List(Of Integer) From {3, 1, 1, 1, 4, 1})           '101 : [Aswap]
        Me.T128.Add(New List(Of Integer) From {4, 1, 1, 1, 3, 1})           '102 : [FNC1]
        Me.T128.Add(New List(Of Integer) From {2, 1, 1, 4, 1, 2})           '103 : [Astart]
        Me.T128.Add(New List(Of Integer) From {2, 1, 1, 2, 1, 4})           '104 : [Bstart]
        Me.T128.Add(New List(Of Integer) From {2, 1, 1, 2, 3, 2})           '105 : [Cstart]
        Me.T128.Add(New List(Of Integer) From {2, 3, 3, 1, 1, 1})           '106 : [STOP]
        Me.T128.Add(New List(Of Integer) From {2, 1})                       '107 : [END BAR]

        For i = 32 To 95                                           ' jeux de caractères
            Me.ABCset &= ChrW(i)
        Next
        Me.Aset = Me.ABCset
        Me.Bset = Me.ABCset

        For i = 0 To 31
            Me.ABCset &= ChrW(i)
            Me.Aset &= ChrW(i)
        Next
        For i = 96 To 127
            Me.ABCset &= ChrW(i)
            Me.Bset &= ChrW(i)
        Next
        For i = 200 To 210                                            ' controle 128
            Me.ABCset &= ChrW(i)
            Me.Aset &= ChrW(i)
            Me.Bset &= ChrW(i)
        Next
        Me.Cset = "0123456789" & ChrW(206)

        For i = 0 To 95                                                  ' convertisseurs des jeux A & B
            Me.SetFrom("A") &= ChrW(i)
            Me.SetFrom("B") &= ChrW(i + 32)
            Me.SetTo("A") &= ChrW(If(i < 32, i + 64, i - 32))
            Me.SetTo("B") &= ChrW(i)
        Next
        For i = 96 To 106                                                 ' contrôle des jeux A & B
            Me.SetFrom("A") &= ChrW(i + 104)
            Me.SetFrom("B") &= ChrW(i + 104)
            Me.SetTo("A") &= ChrW(i)
            Me.SetTo("B") &= ChrW(i)
        Next
    End Sub

    '________________ Fonction encodage et dessin du code 128 _____________________
    Public Sub Code128(x As Integer, y As Integer, code As String, w As Integer, h As Integer)
        _construct128()
        Dim Aguid = ""                                                                      ' Création des guides de choix ABC
        Dim Bguid = ""
        Dim Cguid = ""
        Dim i As Integer
        For i = 0 To code.Length - 1
            Dim needle = code.Substring(i, 1)
            Aguid &= If(Me.Aset.Contains(needle) = False, "N", "O")
            Bguid &= If(Me.Bset.Contains(needle) = False, "N", "O")
            Cguid &= If(Me.Cset.Contains(needle) = False, "N", "O")
        Next

        Dim SminiC = "OOOO"
        Dim IminiC = 4

        Dim crypt = ""
        While code > ""
            ' BOUCLE PRINCIPALE DE CODAGE
            i = Cguid.IndexOf(SminiC)                                                ' forçage du jeu C, si possible
            If Not i = -1 Then
                _changeCharacter(Aguid, "N", i)
                _changeCharacter(Bguid, "N", i)
            End If
            Dim jeu As String
            Dim made As Integer
            If Cguid.Substring(0, IminiC) = SminiC Then                             ' jeu C
                crypt &= ChrW(If(crypt > "", Me.JSwap("C"), Me.JStart("C")))  ' début Cstart, sinon Cswap
                made = Cguid.IndexOf("N")                                             ' étendu du set C
                If made = False Then
                    made = Cguid.Length
                End If
                If ((made Mod 2) = 1) Then
                    made -= made                                                            ' seulement un nombre pair
                End If
                For i = 0 To made Step 2
                    crypt &= ChrW(CInt(code.Substring(i, 2)))                        ' conversion 2 par 2
                Next
                jeu = "C"
            Else
                Dim madeA = Aguid.IndexOf("N")                                            ' étendu du set A
                If madeA = -1 Then
                    madeA = Aguid.Length
                End If
                Dim madeB = Bguid.IndexOf("N")                                            ' étendu du set B
                If madeB = -1 Then
                    madeB = Bguid.Length
                End If
                made = If(madeA < madeB, madeB, madeA)                         ' étendu traitée
                jeu = If(madeA < madeB, "B", "A")                                ' Jeu en cours

                crypt &= ChrW(If(crypt > "", Me.JSwap(jeu), Me.JStart(jeu))) ' début start, sinon swap

                crypt &= code.Substring(0, made).Replace(Me.SetFrom(jeu), Me.SetTo(jeu)) ' conversion selon jeu

            End If
            code = code.Substring(made)                                           ' raccourcir légende et guides de la zone traitée
            Aguid = Aguid.Substring(made)
            Bguid = Bguid.Substring(made)
            Cguid = Cguid.Substring(made)
        End While                                                                       ' FIN BOUCLE PRINCIPALE

        Dim check = AscW(crypt(0))                                                   ' calcul de la somme de contrôle
        For i = 0 To crypt.Length - 1
            check += (AscW(crypt(i)) * i)
        Next
        check = check Mod 103

        crypt &= ChrW(check) & ChrW(106) & ChrW(107)                               ' Chaine cryptée complète

        i = crypt.Length * 11 - 8                                            ' calcul de la largeur du module
        Dim modul = w / i

        For i = 0 To crypt.Length - 1                        ' BOUCLE D"IMPRESSION
            Dim c = Me.T128(AscW(crypt(i)))
            For j = 0 To c.Count - 1
                Me.Rect(x, y, c(j) * modul, h, "F")
                x += (c(j) + c(j + 1)) * modul
                j += 1
            Next
        Next
    End Sub

    Protected Function _changeCharacter(s As String, replaceWith As Char, idx As Integer) As String
        Dim sb As New StringBuilder(s)
        sb(idx) = replaceWith
        Return sb.ToString()
    End Function
End Class
