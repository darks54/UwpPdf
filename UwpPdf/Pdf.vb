Imports Windows.Storage
Imports System.Text
Imports System.Text.RegularExpressions
Imports Ionic.Zlib
Imports System.Globalization
Imports Windows.Storage.Streams
Imports Windows.Storage.FileProperties

Public Class Pdf

    Private UWPPDF_VERSION = "1.0"

    Protected page As Integer                                                   ' current page number
    Protected n As Integer                                                      ' current object number
    Protected offsets As New Dictionary(Of Integer, Integer)                    ' array of object offsets
    Protected buffer As Byte()                                                  ' buffer holding in-memory PDF
    Protected pages As Dictionary(Of Integer, Object)                           ' array containing pages
    Protected state As Integer                                                  ' current document state
    Protected compress As Boolean                                               ' compression flag
    Protected k As Double                                                       ' scale factor (number of points in user unit)
    Protected DefOrientation As Orientation                                     ' default orientation
    Protected CurOrientation As Orientation                                     ' current orientation
    Protected StdPageSizes As Dictionary(Of ListPageSize, Double())             ' standard page sizes
    Protected DefPageSize As Double()                                           ' default page size
    Protected CurPageSize As Double()                                           ' current page size
    Protected CurRotation As Integer                                            ' current page rotation
    Protected PageInfo As Dictionary(Of Integer, Dictionary(Of String, Object)) ' page-related data
    Protected wPt, hPt                                                          ' dimensions of current page in points
    Protected w, h                                                              ' dimensions of current page in user unit
    Protected lMargin                                                           ' left margin
    Protected tMargin                                                           ' top margin
    Protected rMargin                                                           ' right margin
    Protected bMargin                                                           ' page break margin
    Protected cMargin                                                           ' cell margin
    Protected x, y                                                              ' current position in user unit
    Protected lasth                                                             ' height of last printed cell
    Protected LineWidth                                                         ' line width in user unit
    Protected fonts As Dictionary(Of String, Object)                            ' array of used fonts
    Protected FontFiles As Dictionary(Of String, Object)                        ' array of font files
    Protected encodings As Dictionary(Of String, Object)                        ' array of encodings
    Protected cmaps As Dictionary(Of String, Object)                            ' array of ToUnicode CMaps
    Protected FontFamily As ListFont                                            ' current font family
    Protected FontStyle As ListFontStyle                                        ' current font style
    Protected underline As Boolean                                              ' underlining flag
    Protected CurrentFont As Dictionary(Of String, Object)                      ' current font info
    Protected FontSizePt As Double                                              ' current font size in points
    Protected FontSize                                                          ' current font size in user unit
    Protected DrawColor As String                                               ' commands for drawing color
    Protected FillColor As String                                               ' commands for filling color
    Protected TextColor As String                                               ' commands for text color
    Protected ColorFlag As Boolean                                              ' indicates whether fill And text colors are different
    Protected WithAlpha As Boolean                                              ' indicates whether alpha channel Is used
    Protected ws                                                                ' word spacing
    Protected images As Dictionary(Of String, Object)                           ' array of used images
    Protected PageLinks As New Dictionary(Of Integer, List(Of ArrayList))       ' array of links in pages
    Protected links As ArrayList                                                ' array of internal links
    Protected AutoPageBreak As Boolean                                          ' automatic page breaking
    Protected PageBreakTrigger                                                  ' threshold used to trigger page breaks
    Protected InHeader As Boolean                                               ' flag set when processing header
    Protected InFooter As Boolean                                               ' flag set when processing footer
    Protected _AliasNbPages As String                                           ' alias for total number of pages
    Protected ZoomMode As Integer                                               ' zoom display mode
    Protected LayoutMode As Layout                                              ' layout display mode
    Protected metadata As New Dictionary(Of String, Object)                     ' document properties
    Protected PDFVersion As String                                              ' PDF version number

    '*******************************************************************************
    '*                               Public methods                                 *
    '*******************************************************************************

    Public Sub New(Optional orientation As Orientation = Orientation.Portrait, Optional unit As Unit = Unit.millimètre, Optional size As Object = ListPageSize.A4)

        ' Initialization of properties
        Me.state = 0
        Me.page = 0
        Me.n = 2
        Me.buffer = New Byte() {}
        Me.pages = New Dictionary(Of Integer, Object)
        Me.PageInfo = New Dictionary(Of Integer, Dictionary(Of String, Object))
        Me.fonts = New Dictionary(Of String, Object)
        Me.FontFiles = New Dictionary(Of String, Object)
        Me.encodings = New Dictionary(Of String, Object)
        Me.cmaps = New Dictionary(Of String, Object)
        Me.images = New Dictionary(Of String, Object)
        Me.links = New ArrayList
        Me.InHeader = False
        Me.InFooter = False
        Me.lasth = 0
        Me.FontFamily = ListFont.Null
        Me.FontStyle = ListFontStyle.N
        Me.FontSizePt = 12
        Me.underline = False
        Me.DrawColor = "0 G"
        Me.FillColor = "0 g"
        Me.TextColor = "0 g"
        Me.ColorFlag = False
        Me.WithAlpha = False
        Me.ws = 0
        If (unit = Unit.point) Then
            Me.k = 1
        ElseIf (unit = Unit.millimètre) Then
            Me.k = 72 / 25.4
        ElseIf (unit = Unit.centimètre) Then
            Me.k = 72 / 2.54
        ElseIf (unit = Unit.pouce) Then
            Me.k = 72
        End If
        ' Page sizes
        Me.StdPageSizes = New Dictionary(Of ListPageSize, Double()) From {{ListPageSize.A3, {841.89, 1190.55}}, {ListPageSize.A4, {595.28, 841.89}}, {ListPageSize.A5, {420.94, 595.28}}, {ListPageSize.Letter, {612, 792}}, {ListPageSize.Legal, {612, 1008}}}
        size = Me._getpagesize(size)
        Me.DefPageSize = size
        Me.CurPageSize = size
        ' Page orientation
        If orientation = Orientation.Portrait Then
            Me.DefOrientation = Orientation.Portrait
            Me.w = size(0)
            Me.h = size(1)
        ElseIf orientation = Orientation.Landscape Then
            Me.DefOrientation = Orientation.Landscape
            w = size(1)
            h = size(0)
        End If
        Me.CurOrientation = Me.DefOrientation
        Me.wPt = Me.w * Me.k
        Me.hPt = Me.h * Me.k
        ' Page rotation
        Me.CurRotation = 0
        ' Page margins (1 cm)
        Dim margin = 28.35 / Me.k
        Me.SetMargins(margin, margin)
        ' Interior cell margin (1 mm)
        Me.cMargin = margin / 10
        ' Line width (0.2 mm)
        Me.LineWidth = 0.567 / Me.k
        ' Automatic page break
        Me.SetAutoPageBreak(True, 2 * margin)
        ' Default display mode
        Me.SetDisplayMode(Zoom.DefaultZoom)
        ' Enable compression
        Me.SetCompression(False)
        ' Set default PDF version number
        Me.PDFVersion = "1.3"
    End Sub

    ''' <summary>
    ''' Fixe les marges gauche, haute et droite. Par défaut, elles valent 1 cm. Appelez cette méthode si vous désirez les changer.
    ''' </summary>
    ''' <param name="left">Marge gauche.</param>
    ''' <param name="top">Marge haute.</param>
    ''' <param name="right">Marge droite. Par défaut, est égale à la gauche.</param>
    Public Sub SetMargins(left As Double, top As Double, Optional right As Double = Nothing)
        ' Set left, top And right margins
        Me.lMargin = left
        Me.tMargin = top
        If right = Nothing Then
            right = left
        End If
        Me.rMargin = right
    End Sub

    ''' <summary>
    ''' Fixe la marge gauche. La méthode peut être appelée avant de créer la première page. 
    ''' Si l 'abscisse courante se retrouve hors page, elle est ramenée à la marge.
    ''' </summary>
    ''' <param name="margin">La marge.</param>
    Public Sub SetLeftMargin(margin As Double)
        ' Set left margin
        Me.lMargin = margin
        If Me.page > 0 AndAlso Me.x < margin Then
            Me.x = margin
        End If
    End Sub

    ''' <summary>
    ''' Fixe la marge haute. La méthode peut être appelée avant de créer la première page.
    ''' </summary>
    ''' <param name="margin">La marge.</param>
    Public Sub SetTopMargin(margin As Double)
        ' Set top margin
        Me.tMargin = margin
    End Sub

    ''' <summary>
    ''' Fixe la marge droite. La méthode peut être appelée avant de créer la première page.
    ''' </summary>
    ''' <param name="margin">La marge.</param>
    Public Sub SetRightMargin(margin As Double)
        ' Set right margin
        Me.rMargin = margin
    End Sub

    ''' <summary>
    ''' Active ou désactive le mode saut de page automatique. En cas d'activation, le second paramètre représente la distance par rapport au bas de la page qui déclenche le saut. Par défaut, le mode est activé et la marge vaut 2 cm.
    ''' </summary>
    ''' <param name="auto">Booléen indiquant si le mode doit être activé.</param>
    ''' <param name="margin">Distance par rapport au bas de la page.</param>
    Public Sub SetAutoPageBreak(auto As Boolean, Optional margin As Double = 0)
        ' Set auto page break mode And triggering margin
        Me.AutoPageBreak = auto
        Me.bMargin = margin
        Me.PageBreakTrigger = Me.h - margin
    End Sub

    ''' <summary>
    ''' Contrôle la manière dont le document sera affiché par le lecteur. On peut régler le niveau de zoom : afficher la page en entier, occuper toute la largeur de la fenêtre, utiliser la taille réelle, choisir un facteur de zoom particulier ou encore utiliser la valeur par défaut de l'utilisateur (celle paramétrée dans le menu Préférences d'Adobe Reader). On peut également spécifier la disposition des pages : une seule à la fois, affichage continu, pages sur deux colonnes ou réglage par défaut.
    ''' </summary>
    ''' <param name="zoom">Le zoom à utiliser.</param>
    ''' <param name="layout">La disposition des pages.</param>
    Public Sub SetDisplayMode(zoom As Zoom, Optional layout As Layout = Layout.DefaultLayout)
        ' Set display mode in viewer
        Me.ZoomMode = zoom
        Me.LayoutMode = layout
    End Sub

    ''' <summary>
    ''' Contrôle la manière dont le document sera affiché par le lecteur. On peut régler le niveau de zoom : afficher la page en entier, occuper toute la largeur de la fenêtre, utiliser la taille réelle, choisir un facteur de zoom particulier ou encore utiliser la valeur par défaut de l'utilisateur (celle paramétrée dans le menu Préférences d'Adobe Reader). On peut également spécifier la disposition des pages : une seule à la fois, affichage continu, pages sur deux colonnes ou réglage par défaut.
    ''' </summary>
    ''' <param name="zoom">Le zoom à utiliser.</param>
    ''' <param name="layout">La disposition des pages.</param>
    Public Sub SetDisplayMode(zoom As Integer, Optional layout As Layout = Layout.DefaultLayout)
        ' Set display mode in viewer
        Me.ZoomMode = zoom
        Me.LayoutMode = layout
    End Sub

    ''' <summary>
    ''' Active ou désactive la compression des pages. Lorsqu'elle est activée, la représentation interne de chaque page est compressée, ce qui donne généralement un taux de compression de l'ordre de 2 pour le document résultant. 
    ''' La compression est désactivée par défaut. 
    ''' </summary>
    ''' <param name="compress">Booléen indiquant si la compression doit être activée.</param>
    Public Sub SetCompression(compress As Boolean)
        ' Set page compression
        Me.compress = compress
    End Sub

    ''' <summary>
    ''' Définit le titre du document.
    ''' </summary>
    ''' <param name="title">Le titre.</param>
    ''' <param name="isUTF8">Indique si la chaîne est encodée en ISO-8859-1 (false) ou en UTF-8 (true).</param>
    Public Sub SetTitle(title As String, Optional isUTF8 As Boolean = False)
        ' Title of document
        If isUTF8 Then
            Me.metadata("Title") = title
        Else
            Me.metadata("Title") = _utf8encode(title)
        End If
    End Sub

    ''' <summary>
    ''' Définit l'auteur du document.
    ''' </summary>
    ''' <param name="author">Le nom de l'auteur.</param>
    ''' <param name="isUTF8">Indique si la chaîne est encodée en ISO-8859-1 (false) ou en UTF-8 (true).</param>
    Public Sub SetAuthor(author As String, Optional isUTF8 As Boolean = False)
        ' Author of document
        If isUTF8 Then
            Me.metadata("Author") = author
        Else
            Me.metadata("Author") = _utf8encode(author)
        End If
    End Sub

    ''' <summary>
    ''' Définit le sujet du document.
    ''' </summary>
    ''' <param name="subject">L'intitulé du sujet.</param>
    ''' <param name="isUTF8">Indique si la chaîne est encodée en ISO-8859-1 (false) ou en UTF-8 (true).</param>
    Public Sub SetSubject(subject As String, Optional isUTF8 As Boolean = False)
        ' Subject of document
        If isUTF8 Then
            Me.metadata("Subject") = subject
        Else
            Me.metadata("Subject") = _utf8encode(subject)
        End If
    End Sub

    ''' <summary>
    ''' Associe des mot-clés au document, généralement sous la forme 'mot-clé1 mot-clé2 ...'.
    ''' </summary>
    ''' <param name="keywords">La liste de mots-clés.</param>
    ''' <param name="isUTF8">Indique si la chaîne est encodée en ISO-8859-1 (false) ou en UTF-8 (true).</param>
    Public Sub SetKeywords(keywords As String, Optional isUTF8 As Boolean = False)
        ' Keywords of document
        If isUTF8 Then
            Me.metadata("Keywords") = keywords
        Else
            Me.metadata("Keywords") = _utf8encode(keywords)
        End If
    End Sub

    ''' <summary>
    ''' Définit le créateur du document. Il s'agit typiquement du nom de l'application qui génère le PDF.
    ''' </summary>
    ''' <param name="creator">Le nom du créateur.</param>
    ''' <param name="isUTF8">Indique si la chaîne est encodée en ISO-8859-1 (false) ou en UTF-8 (true).</param>
    Public Sub SetCreator(creator As String, Optional isUTF8 As Boolean = False)
        ' Creator of document
        If isUTF8 Then
            Me.metadata("Creator") = creator
        Else
            Me.metadata("Creator") = _utf8encode(creator)
        End If
    End Sub

    ''' <summary>
    ''' Définit un alias pour le nombre total de pages. Cet alias sera substitué lors de la fermeture du document.
    ''' </summary>
    ''' <param name="_alias">L'alias.</param>
    Public Sub AliasNbPages(Optional _alias As String = "{nb}")
        ' Define an alias for total number of pages
        Me._AliasNbPages = _alias
    End Sub

    ''' <summary>
    ''' Cette méthode est appelée automatiquement en cas d'erreur fatale ; elle se contente de lancer une exception avec le message fourni.
    ''' </summary>
    ''' <param name="msg">Le message d'erreur.</param>
    Public Sub Erreur(msg As String)
        ' Fatal error
        Throw New Exception("FPDF error: " & msg)
    End Sub

    ''' <summary>
    ''' Cette méthode termine le document PDF. Il n'est pas nécessaire de l'appeler explicitement car Output() le fait automatiquement. 
    ''' Si le document ne contient aucune page, AddPage() est appelé pour éviter d'obtenir un document invalide.
    ''' </summary>
    Public Sub Close()
        ' Terminate document
        If Me.state = 3 Then
            Return
        End If
        If Me.page = 0 Then
            Me.AddPage()
        End If
        ' Page footer
        Me.InFooter = True
        Me.Footer()
        Me.InFooter = False
        ' Close page
        Me._endpage()
        ' Close document
        Me._enddoc()
    End Sub

    ''' <summary>
    ''' Ajoute une nouvelle page au document. Si une page était en cours, la méthode Footer() est appelée pour traiter le pied de page. Puis la page est ajoutée, la position courante mise en haut à gauche en fonction des marges gauche et haute, et Header() est appelée pour afficher l'en-tête. 
    ''' La police qui était en cours au moment de l'appel est automatiquement restaurée. Il n'est donc pas nécessaire d'appeler à nouveau SetFont() si vous souhaitez continuer avec la même police. Même chose pour les couleurs et l'épaisseur du trait. 
    ''' L'origine du système de coordonnées est en haut à gauche et les ordonnées croissantes vont vers le bas.
    ''' </summary>
    ''' <param name="orientation">Orientation de la page.</param>
    ''' <param name="size">Format de la page.</param>
    ''' <param name="rotation">Angle de rotation de la page. La valeur doit être un multiple de 90 ; la rotation se fait dans le sens horaire.</param>
    Public Sub AddPage(Optional orientation As Orientation = Orientation.Portrait, Optional size As Object = "", Optional rotation As Integer = 0)
        CultureInfo.CurrentCulture = New CultureInfo("en-US")
        ' Start a New page
        If Me.state = 3 Then
            Me.Erreur("The document is closed")
        End If
        Dim family = Me.FontFamily
        Dim style = Me.FontStyle
        Dim fontSize = Me.FontSizePt
        Dim lw = Me.LineWidth
        Dim dc = Me.DrawColor
        Dim fc = Me.FillColor
        Dim tc = Me.TextColor
        Dim cf = Me.ColorFlag
        If Me.page > 0 Then
            ' Page footer
            Me.InFooter = True
            Me.Footer()
            Me.InFooter = False
            ' Close page
            Me._endpage()
        End If
        ' Start New page
        Me._beginpage(orientation, size, rotation)
        ' Set line cap style to square
        Me._out("2 J")
        ' Set line width
        Me.LineWidth = lw
        Me._out(String.Format("{0:0.00} w", lw * Me.k))
        ' Set font
        If family Then
            Me.SetFont(family, style, Me.underline, fontSize)
        End If
        ' Set colors
        Me.DrawColor = dc
        If Not dc = "0 G" Then
            Me._out(dc)
        End If
        Me.FillColor = fc
        If Not fc = "0 g" Then
            Me._out(fc)
        End If
        Me.TextColor = tc
        Me.ColorFlag = cf
        ' Page header
        Me.InHeader = True
        Me.Header()
        Me.InHeader = False
        ' Restore line width
        If Not Me.LineWidth = lw Then
            Me.LineWidth = lw
            Me._out(String.Format("{0:0.00} w", lw * Me.k))
        End If
        ' Restore font
        If family Then
            Me.SetFont(family, style, Me.underline, fontSize)
        End If
        ' Restore colors
        If Not Me.DrawColor = dc Then
            Me.DrawColor = dc
            Me._out(dc)
        End If
        If Not Me.FillColor = fc Then
            Me.FillColor = fc
            Me._out(fc)
        End If
        Me.TextColor = tc
        Me.ColorFlag = cf
    End Sub

    ''' <summary>
    ''' Cette méthode permet de définir l'en-tête de page. Elle est appelée automatiquement par AddPage() et ne devrait donc pas être appelée explicitement par l'application. L'implémentation de Header() dans FPDF est vide, donc vous devez dériver la classe et redéfinir la méthode si vous voulez un traitement particulier pour vos en-têtes.
    ''' </summary>
    Public Sub Header()
        ' To be implemented in your own inherited class
    End Sub

    ''' <summary>
    ''' Cette méthode permet de définir le pied de page. Elle est appelée automatiquement par AddPage() et Close() et ne devrait donc pas être appelée explicitement par l'application. L'implémentation de Footer() dans FPDF est vide, donc vous devez dériver la classe et redéfinir la méthode si vous voulez un traitement particulier pour vos pieds de page.
    ''' </summary>
    Public Sub Footer()
        ' To be implemented in your own inherited class
    End Sub

    ''' <summary>
    ''' Renvoie le numéro de page courant.
    ''' </summary>
    ''' <returns></returns>
    Public Function PageNo() As Integer
        ' Get current page number
        Return Me.page
    End Function

    ''' <summary>
    ''' Fixe la couleur pour toutes les opérations de tracé (lignes, rectangles et contours de cellules). Elle peut être indiquée en composantes RGB ou en niveau de gris. La méthode peut être appelée avant que la première page ne soit créée et la valeur est conservée de page en page.
    ''' </summary>
    ''' <param name="r">Si g et b sont renseignés, composante de rouge; sinon, indique le niveau de gris. Valeur comprise entre 0 et 255.</param>
    ''' <param name="g">Composante de vert (entre 0 et 255).</param>
    ''' <param name="b">Composante de bleu (entre 0 et 255).</param>
    Public Sub SetDrawColor(r As Integer, Optional g As Integer? = Nothing, Optional b As Integer? = Nothing)
        ' Set color for all stroking operations
        If (r = 0 AndAlso g = 0 AndAlso b = 0) OrElse g Is Nothing Then
            Me.DrawColor = String.Format("{0:0.000} G", r / 255)
        Else
            Me.DrawColor = String.Format("{0:0.000} {1:0.000} {2:0.000} RG", r / 255, g / 255, b / 255)
        End If
        If (Me.page > 0) Then
            Me._out(Me.DrawColor)
        End If
    End Sub

    ''' <summary>
    ''' Fixe la couleur pour toutes les opérations de remplissage (rectangles pleins et fonds de cellules). Elle peut être indiquée en composantes RGB ou en niveau de gris. La méthode peut être appelée avant que la première page ne soit créée et la valeur est conservée de page en page.
    ''' </summary>
    ''' <param name="r">Si g et b sont renseignés, composante de rouge; sinon, indique le niveau de gris. Valeur comprise entre 0 et 255.</param>
    ''' <param name="g">Composante de vert (entre 0 et 255).</param>
    ''' <param name="b">Composante de bleu (entre 0 et 255).</param>
    Public Sub SetFillColor(r As Integer, Optional g As Integer? = Nothing, Optional b As Integer? = Nothing)
        ' Set color for all filling operations
        If (r = 0 AndAlso g = 0 AndAlso b = 0) OrElse g Is Nothing Then
            Me.FillColor = String.Format("{0:0.000} g", r / 255)
        Else
            Me.FillColor = String.Format("{0:0.000} {1:0.000} {2:0.000} rg", r / 255, g / 255, b / 255)
        End If
        Me.ColorFlag = Not String.Equals(Me.FillColor, Me.TextColor)
        If (Me.page > 0) Then
            Me._out(Me.FillColor)
        End If
    End Sub

    ''' <summary>
    ''' Fixe la couleur pour le texte. Elle peut être indiquée en composantes RGB ou en niveau de gris. La méthode peut être appelée avant que la première page ne soit créée et la valeur est conservée de page en page.
    ''' </summary>
    ''' <param name="r">Si g et b sont renseignés, composante de rouge; sinon, indique le niveau de gris. Valeur comprise entre 0 et 255.</param>
    ''' <param name="g">Composante de vert (entre 0 et 255).</param>
    ''' <param name="b">Composante de bleu (entre 0 et 255).</param>
    Public Sub SetTextColor(r As Integer, Optional g As Integer? = Nothing, Optional b As Integer? = Nothing)
        ' Set color for text
        If (r = 0 AndAlso g = 0 AndAlso b = 0) OrElse g Is Nothing Then
            Me.TextColor = String.Format("{0:0.000} g", r / 255)
        Else
            Me.TextColor = String.Format("{0:0.000} {1:0.000} {2:0.000} rg", r / 255, g / 255, b / 255)
        End If
        Me.ColorFlag = Not String.Equals(Me.FillColor, Me.TextColor)
    End Sub

    ''' <summary>
    ''' Renvoie la longueur d'une chaîne en unité utilisateur. Une police doit être sélectionnée.
    ''' </summary>
    ''' <param name="s">La chaîne dont on veut déterminer la longueur.</param>
    ''' <returns></returns>
    Public Function GetStringWidth(s As String) As Double
        ' Get width of a string in the current font
        'Dim cw = & Me.CurrentFont("cw")
        w = 0
        Dim l = s.Length
        For i = 0 To l - 1
            w += Me.CurrentFont("cw")(s(i))
        Next
        Return w * Me.FontSize / 1000
    End Function

    ''' <summary>
    ''' Fixe l'épaisseur des traits. La valeur est par défaut égale à 0,2 mm. La méthode peut être appelée avant que la première page ne soit créée et la valeur est conservée de page en page.
    ''' </summary>
    ''' <param name="width">L'épaisseur.</param>
    Public Sub SetLineWidth(width As Double)
        ' Set line width
        Me.LineWidth = width
        If Me.page > 0 Then
            Me._out(String.Format("{0:0.00} w", width * Me.k))
        End If
    End Sub

    ''' <summary>
    ''' Trace une ligne entre deux points.
    ''' </summary>
    ''' <param name="x1">Abscisse du premier point.</param>
    ''' <param name="y1">Ordonnée du premier point.</param>
    ''' <param name="x2">Abscisse du second point.</param>
    ''' <param name="y2">Ordonnée du second point.</param>
    Public Sub Line(x1 As Double, y1 As Double, x2 As Double, y2 As Double)
        ' Draw a line
        Me._out(String.Format("{0:0.00} {1:0.00} m {2:0.00} {3:0.00} l S", x1 * Me.k, (Me.h - y1) * Me.k, x2 * Me.k, (Me.h - y2) * Me.k))
    End Sub

    ''' <summary>
    ''' Dessine un rectangle à partir de son coin supérieur gauche. Il peut être tracé, rempli ou les deux à la fois.
    ''' </summary>
    ''' <param name="x">Abscisse du coin supérieur gauche.</param>
    ''' <param name="y">Ordonnée du coin supérieur gauche.</param>
    ''' <param name="w">Largeur.</param>
    ''' <param name="h">Hauteur.</param>
    ''' <param name="style">Manière de tracer le rectangle. Les valeurs possibles sont :
    ''' D ou chaîne vide : contour (draw). C'est la valeur par défaut.
    ''' F: remplissage (fill)
    ''' DF ou FD : contour et remplissage</param>
    Public Sub Rect(x As Double, y As Double, w As Double, h As Double, Optional style As String = "")
        Dim op As String
        ' Draw a rectangle
        If style = "F" Then
            op = "f"
        ElseIf style = "FD" OrElse style = "DF" Then
            op = "B"
        Else
            op = "S"
        End If
        Me._out(String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00} re {4}", x * Me.k, (Me.h - y) * Me.k, w * Me.k, -h * Me.k, op))
    End Sub

    ''' <summary>
    ''' Importe une police TrueType, OpenType ou Type1 et la rend disponible. Il faut au préalable avoir généré un fichier de définition de police avec l'utilitaire MakeFont. 
    ''' Le fichier de définition (ainsi que le fichier de police en cas d'incorporation) doit être présent dans le répertoire des polices. S'il n'est pas trouvé, l'erreur "Could not include font definition file" est renvoyée.
    ''' </summary>
    ''' <param name="family">Famille de la police. Le nom peut être choisi arbitrairement. S'il est celui d'une famille standard, la police correspondante sera masquée.</param>
    ''' <param name="style">Style de la police.</param>
    ''' <param name="file">Le fichier de définition de police. 
    ''' Par défaut, le nom est construit à partir du nom de la famille et du style, en minuscules et sans espace.</param>
    Public Sub AddFont(family As ListFont, Optional style As ListFontStyle = ListFontStyle.N, Optional file As String = "")

        ' Add a TrueType, OpenType Or Type1 font
        'family = family.ToLower
        'If file = "" Then
        '    file = family.Replace(" ", "") & style.ToLower & ".php"
        'End If
        'style = style.ToString.ToUpper
        'If style = "IB" Then
        '    style = "BI"
        'End If
        Dim fontkey = family.ToString & style.ToString
        'If Not Me.fonts(fontkey) = Nothing Then
        '    Return
        'End If

        Dim info = Me._loadfont(family, style)
        info.Add("i", Me.fonts.Count + 1)
        If info.ContainsKey("file") Then
            ' Embedded font
            If info("type") = "TrueType" Then
                If Me.FontFiles.ContainsKey(info("file")) Then
                    Me.FontFiles(info("file")) = New Dictionary(Of String, Object) From {{"length1", info("originalsize")}}
                Else
                    Me.FontFiles.Add(info("file"), New Dictionary(Of String, Object) From {{"length1", info("originalsize")}})
                End If
            Else
                If Me.FontFiles.ContainsKey(info("file")) Then
                    Me.FontFiles(info("file")) = New Dictionary(Of String, Object) From {{"length1", info("size1")}, {"length2", info("size2")}}
                Else
                    Me.FontFiles.Add(info("file"), New Dictionary(Of String, Object) From {{"length1", info("size1")}, {"length2", info("size2")}})
                End If
            End If
        End If
        Me.fonts.Add(fontkey, info)
    End Sub

    ''' <summary>
    ''' Fixe la police utilisée pour imprimer les chaînes de caractères. Il est obligatoire d'appeler cette méthode au moins une fois avant d'imprimer du texte, sinon le document résultant ne sera pas valide. 
    ''' La police peut être soit une police standard, soit une police ajoutée à l'aide de la méthode AddFont(). Les polices standard utilisent l'encodage Windows cp1252 (Europe de l'ouest). 
    ''' La méthode peut être appelée avant que la première page ne soit créée et la police est conservée de page en page. 
    ''' Si vous souhaitez juste changer la taille courante, il est plus simple d'appeler SetFontSize(). 
    ''' </summary>
    ''' <param name="family">Famille de la police.</param>
    ''' <param name="style">Style de la police.</param>
    ''' <param name="underline">Police souligné.</param>
    ''' <param name="size">Taille de la police en points. </param>
    Public Sub SetFont(family As ListFont, Optional style As ListFontStyle = ListFontStyle.N, Optional underline As Boolean = False, Optional size As Double = 0)

        ' Select a font; size given in points
        If family = ListFont.Null Then
            family = Me.FontFamily
            'Else
            '    family = family.ToLower()
        End If
        If underline Then
            Me.underline = True
            'style = style.Replace("U", "")
        Else
            Me.underline = False
        End If
        'If style = "IB" Then
        '    style = "BI"
        'End If
        If size = 0 Then
            size = Me.FontSizePt
        End If
        ' Test if font Is already selected
        If Me.FontFamily = family AndAlso Me.FontStyle = style AndAlso Me.FontSizePt = size Then
            Return
        End If
        ' Test if font Is already loaded
        Dim fontkey = family.ToString & style.ToString
        If Not Me.fonts.ContainsKey(fontkey) Then
            ' Test if one of the core fonts
            'If family = "arial" Then
            '    family = "helvetica"
            'End If
            'If Me.CoreFonts.Contains(family) Then
            If family = ListFont.Symbol OrElse family = ListFont.ZapfDingbats Then
                style = ListFontStyle.N
            End If
            fontkey = family.ToString & style.ToString
            If Not Me.fonts.ContainsKey(fontkey) Then
                Me.AddFont(family, style)
            End If
            'Else
            '    Me.Erreur("Undefined font: " & family & " " & style)
            'End If
        End If
        ' Select it
        Me.FontFamily = family
        Me.FontStyle = style
        Me.FontSizePt = size
        Me.FontSize = size / Me.k
        Me.CurrentFont = Me.fonts(fontkey)
        If Me.page > 0 Then
            Me._out(String.Format("BT /F{0} {1:0.00} Tf ET", Me.CurrentFont("i"), Me.FontSizePt))
        End If
    End Sub

    ''' <summary>
    ''' Fixe la taille de la police courante.
    ''' </summary>
    ''' <param name="size">La taille (en points).</param>
    Public Sub SetFontSize(size As Double)

        ' Set font size in points
        If Me.FontSizePt = size Then
            Return
        End If
        Me.FontSizePt = size
        Me.FontSize = size / Me.k
        If Me.page > 0 Then
            Me._out(String.Format("BT /F{0} {1:0.00} Tf ET", Me.CurrentFont("i"), Me.FontSizePt))
        End If
    End Sub

    ''' <summary>
    ''' Crée un nouveau lien interne et renvoie son identifiant. Un lien interne est une zone cliquable qui amène à un autre endroit dans le document. 
    ''' L'identifiant pourra être transmis aux méthodes Cell(), Write(), Image() ou Link(). La destination est définie à l'aide de SetLink().
    ''' </summary>
    ''' <returns></returns>
    Public Function AddLink()
        ' Create a New internal link
        Dim n = Me.links.Count + 1
        Me.links(n) = New ArrayList From {0, 0}
        Return n
    End Function

    ''' <summary>
    ''' Indique la page et la position vers lesquelles pointe un lien interne.
    ''' </summary>
    ''' <param name="link">Identifiant du lien retourné par AddLink().</param>
    ''' <param name="y">Ordonnée de la position; -1 indique la position courante. La valeur par défaut est 0 (haut de la page).</param>
    ''' <param name="page">Numéro de la page; -1 indique la page courante. C'est la valeur par défaut.</param>
    Public Sub SetLink(link As Integer, Optional y As Double = 0, Optional page As Integer = -1)
        ' Set destination of internal link
        If y = -1 Then
            y = Me.y
        End If
        If page = -1 Then
            page = Me.page
        End If
        Me.links(link) = New ArrayList() From {page, y}
    End Sub

    ''' <summary>
    ''' Place un lien sur une zone rectangulaire de la page. Les liens textes ou images se posent généralement via Cell(), Write() ou Image(), mais cette méthode peut être utile par exemple pour placer une zone cliquable à l'intérieur d'une image.
    ''' </summary>
    ''' <param name="x">Abscisse du coin supérieur gauche du rectangle.</param>
    ''' <param name="y">Ordonnée du coin supérieur gauche du rectangle.</param>
    ''' <param name="w">Largeur du rectangle.</param>
    ''' <param name="h">Hauteur du rectangle.</param>
    ''' <param name="link">URL ou identifiant retourné par AddLink().</param>
    Public Sub Link(x As Double, y As Double, w As Double, h As Double, link As Object)
        ' Put a link on the page
        If Me.PageLinks.ContainsKey(Me.page) Then
            Me.PageLinks.Add(Me.page, New List(Of ArrayList))
        End If
        Me.PageLinks(Me.page).Add(New ArrayList From {x * Me.k, Me.hPt - y * Me.k, w * Me.k, h * Me.k, link})
    End Sub

    ''' <summary>
    ''' Imprime une chaîne de caractères. L'origine est à gauche du premier caractère, sur la ligne de base. Cette méthode permet de positionner précisément une chaîne dans la page, mais il est généralement plus simple d'utiliser Cell(), MultiCell() ou Write() qui sont les méthodes standard pour imprimer du texte.
    ''' </summary>
    ''' <param name="x">Abscisse de l'origine.</param>
    ''' <param name="y">Ordonnée de l'origine.</param>
    ''' <param name="txt">Chaîne à imprimer.</param>
    Public Sub Text(x As Double, y As Double, txt As String)
        ' Output a string
        If Me.CurrentFont Is Nothing Then
            Me.Erreur("No font has been set")
        End If
        Dim s = String.Format("BT {0:0.00} {1:0.00} Td ({2}) Tj ET", x * Me.k, (Me.h - y) * Me.k, Me._escape(txt))
        If Me.underline AndAlso Not txt = "" Then
            s &= " " & Me._dounderline(x, y, txt)
        End If
        If Me.ColorFlag Then
            s = "q " & Me.TextColor & " " & s & " Q"
        End If
        Me._out(s)
    End Sub

    ''' <summary>
    ''' Lorsqu'une condition de saut de page est remplie, la méthode est appelée, et en fonction de la valeur de retour, le saut est effectué ou non. L'implémentation par défaut renvoie une valeur selon le mode sélectionné par SetAutoPageBreak(). 
    ''' Cette méthode est appelée automatiquement et ne devrait donc pas être appelée directement par l'application.
    ''' </summary>
    ''' <returns></returns>
    Public Function AcceptPageBreak() As Boolean
        ' Accept automatic page break Or Not
        Return Me.AutoPageBreak
    End Function

    ''' <summary>
    ''' Imprime une cellule (zone rectangulaire) avec éventuellement des bords, un fond et une chaîne de caractères. Le coin supérieur gauche de la cellule correspond à la position courante. Le texte peut être aligné ou centré. Après l'appel, la position courante se déplace à droite ou un retour à la ligne est effectué. Il est possible de mettre un lien sur le texte. 
    ''' Si le saut de page automatique est activé et que la cellule dépasse le seuil de déclenchement, un saut de page est effectué avant de l'imprimer.
    ''' </summary>
    ''' <param name="w">Largeur de la cellule. Si elle vaut 0, la cellule s'étend jusqu'à la marge droite de la page.</param>
    ''' <param name="h">Hauteur de la cellule.</param>
    ''' <param name="txt">Chaîne à imprimer.</param>
    ''' <param name="border">Indique si des bords doivent être tracés autour de la cellule.</param>
    ''' <param name="ln">ndique où se déplace la position courante après l'appel à la méthode.</param>
    ''' <param name="align">Permet de centrer ou d'aligner le texte.</param>
    ''' <param name="fill">Indique si le fond de la cellule doit être coloré (true) ou transparent (false).</param>
    ''' <param name="link">URL ou identifiant retourné par AddLink().</param>
    Public Sub Cell(w As Double, Optional h As Double = 0, Optional txt As String = "", Optional border As Object = 0, Optional ln As Integer = 0, Optional align As String = "", Optional fill As Boolean = False, Optional link As Object = "")
        ' Output a cell
        k = Me.k
        If Me.y + h > Me.PageBreakTrigger AndAlso Not Me.InHeader AndAlso Not Me.InFooter AndAlso Me.AcceptPageBreak() Then

            ' Automatic page break
            Dim x = Me.x
            Dim ws = Me.ws
            If ws > 0 Then
                Me.ws = 0
                Me._out("0 Tw")
            End If
            Me.AddPage(Me.CurOrientation, Me.CurPageSize, Me.CurRotation)
            Me.x = x
            If ws > 0 Then
                Me.ws = ws
                Me._out(String.Format("{0:0.000} Tw", ws * k))
            End If
        End If
        If w = 0 Then
            w = Me.w - Me.rMargin - Me.x
        End If
        Dim s = ""
        If fill OrElse border = 1 Then
            Dim op As String
            If fill Then
                op = If(border = 1, "B", "f")
            Else
                op = "S"
            End If
            s = String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00} re {4} ", Me.x * k, (Me.h - Me.y) * k, w * k, -h * k, op)
        End If
        If border.GetType Is GetType(String) Then
            x = Me.x
            y = Me.y
            If border.ToString.Contains("L") Then
                s &= String.Format("{0:0.00} {1:0.00} m {2:0.00} {3:0.00} l S ", x * k, (Me.h - y) * k, x * k, (Me.h - (y + h)) * k)
            End If
            If border.ToString.Contains("T") Then
                s &= String.Format("{0:0.00} {1:0.00} m {2:0.00} {3:0.00} l S ", x * k, (Me.h - y) * k, (x + w) * k, (Me.h - y) * k)
            End If
            If border.ToString.Contains("R") Then
                s &= String.Format("{0:0.00} {1:0.00} m {2:0.00} {3:0.00} l S ", (x + w) * k, (Me.h - y) * k, (x + w) * k, (Me.h - (y + h)) * k)
            End If
            If border.ToString.Contains("B") Then
                s &= String.Format("{0:0.00} {1:0.00} m {2:0.00} {3:0.00} l S ", x * k, (Me.h - (y + h)) * k, (x + w) * k, (Me.h - (y + h)) * k)
            End If
        End If
        If Not txt = "" Then
            If Me.CurrentFont Is Nothing Then
                Me.Erreur("No font has been set")
            End If
            Dim dx As Integer
            If align = "R" Then
                dx = w - Me.cMargin - Me.GetStringWidth(txt)
            ElseIf align = "C" Then
                dx = (w - Me.GetStringWidth(txt)) / 2
            Else
                dx = Me.cMargin
            End If
            If Me.ColorFlag Then
                s &= "q " & Me.TextColor & " "
            End If
            s &= String.Format("BT {0:0.00} {1:0.00} Td ({2}) Tj ET", (Me.x + dx) * k, (Me.h - (Me.y + 0.5 * h + 0.3 * Me.FontSize)) * k, Me._escape(txt))
            If Me.underline Then
                s &= " " & Me._dounderline(Me.x + dx, Me.y + 0.5 * h + 0.3 * Me.FontSize, txt)
            End If
            If Me.ColorFlag Then
                s &= " Q"
            End If
            If Not link = "" Then
                Me.Link(Me.x + dx, Me.y + 0.5 * h - 0.5 * Me.FontSize, Me.GetStringWidth(txt), Me.FontSize, link)
            End If
        End If
        If Not s = "" Then
            Me._out(s)
        End If
        Me.lasth = h
        If ln > 0 Then
            ' Go to next line
            Me.y += h
            If ln = 1 Then
                Me.x = Me.lMargin
            End If
        Else
            Me.x += w
        End If
    End Sub

    ''' <summary>
    ''' Cette méthode permet d'imprimer du texte avec des retours à la ligne. Ceux-ci peuvent être automatiques (dès que le texte atteint le bord droit de la cellule) ou explicites (via le caractère \n). Autant de cellules que nécessaire sont imprimées, les unes en dessous des autres. 
    ''' Le texte peut être aligné, centré ou justifié. Le bloc de cellules peut être encadré et le fond coloré.
    ''' </summary>
    ''' <param name="w">Largeur des cellules. Si elle vaut 0, elles s'étendent jusqu'à la marge droite de la page.</param>
    ''' <param name="h">Hauteur des cellules.</param>
    ''' <param name="txt">Chaîne à imprimer.</param>
    ''' <param name="border">Indique si des bords doivent être tracés autour du bloc de cellules.</param>
    ''' <param name="align">Contrôle l'alignement du texte.</param>
    ''' <param name="fill">Indique si le fond des cellules doit être coloré (true) ou transparent (false).</param>
    Public Sub MultiCell(w As Double, h As Double, txt As String, Optional border As Object = 0, Optional align As String = "J", Optional fill As Boolean = False)

        ' Output text with automatic Or explicit line breaks
        If Me.CurrentFont Is Nothing Then
            Me.Erreur("No font has been set")
        End If
        Dim cw = Me.CurrentFont("cw")
        If w = 0 Then
            w = Me.w - Me.rMargin - Me.x
        End If
        Dim wmax = (w - 2 * Me.cMargin) * 1000 / Me.FontSize
        Dim s As String = txt.Replace("\r", "")
        Dim nb = s.Length
        If nb > 0 AndAlso s(nb - 1) = vbLf Then
            nb -= 1
        End If
        Dim b As Integer = 0
        Dim b2 As Integer
        If border Then

            If border = 1 Then
                border = "LTRB"
                b = "LRT"
                b2 = "LR"
            Else
                b2 = ""
                If border.ToString = "L" Then
                    b2 &= "L"
                End If
                If border.ToString = "R" Then
                    b2 &= "R"
                End If
                b = If(border.ToString.Contains("T"), b2 & "T", b2)
            End If

        End If
        Dim sep = -1
        Dim i = 0
        Dim j = 0
        Dim l = 0
        Dim ns = 0
        Dim nl = 1
        While i < nb

            ' Get next character
            Dim c = s(i)
            If c = vbLf Then

                ' Explicit line break
                If Me.ws > 0 Then
                    Me.ws = 0
                    Me._out("0 Tw")
                End If
                Me.Cell(w, h, s.Substring(j, i - j), b, 2, align, fill)
                i += 1
                sep = -1
                j = i
                l = 0
                ns = 0
                nl += 1
                If border AndAlso nl = 2 Then
                    b = b2
                End If
                Continue While
            End If
            Dim ls As Integer
            If c = " " Then
                sep = i
                ls = l
                ns += 1
            End If
            l += cw(c)
            If l > wmax Then

                ' Automatic line break
                If sep = -1 Then

                    If i = j Then
                        i += 1
                    End If
                    If Me.ws > 0 Then
                        Me.ws = 0
                        Me._out("0 Tw")
                    End If
                    Me.Cell(w, h, s.Substring(j, i - j), b, 2, align, fill)
                Else
                    If align = "J" Then
                        Me.ws = If(ns > 1, (wmax - ls) / 1000 * Me.FontSize / (ns - 1), 0)
                        Me._out(String.Format("{0:0.000} Tw", Me.ws * Me.k))
                    End If
                    Me.Cell(w, h, s.Substring(j, sep - j), b, 2, align, fill)
                    i = sep + 1
                End If
                sep = -1
                j = i
                l = 0
                ns = 0
                nl += 1
                If border AndAlso nl = 2 Then
                    b = b2
                End If
            Else
                i += 1
            End If
        End While
        ' Last chunk
        If Me.ws > 0 Then
            Me.ws = 0
            Me._out("0 Tw")
        End If
        If Not border AndAlso border.ToString = "B" Then
            b &= "B"
        End If
        Me.Cell(w, h, s.Substring(j, i - j), b, 2, align, fill)
        Me.x = Me.lMargin
    End Sub

    ''' <summary>
    ''' Cette méthode imprime du texte à partir de la position courante. Lorsque la marge droite est atteinte (ou que le caractère \n est rencontré), un saut de ligne est effectué et le texte continue à partir de la marge gauche. Au retour de la méthode, la position courante est située juste à la fin du texte. 
    ''' Il est possible de mettre un lien sur le texte.
    ''' </summary>
    ''' <param name="h">Hauteur de la ligne.</param>
    ''' <param name="txt">Chaîne à imprimer.</param>
    ''' <param name="link">URL ou identifiant retourné par AddLink().</param>
    Public Sub Write(h As Double, txt As String, Optional link As Object = "")

        ' Output text in flowing mode
        If Me.CurrentFont Is Nothing Then
            Me.Erreur("No font has been set")
        End If
        Dim cw = Me.CurrentFont("cw")
        Dim w = Me.w - Me.rMargin - Me.x
        Dim wmax = (w - 2 * Me.cMargin) * 1000 / Me.FontSize
        Dim s As String = txt.Replace("\r", "")
        Dim nb = s.Length
        Dim sep = -1
        Dim i = 0
        Dim j = 0
        Dim l = 0
        Dim nl = 1
        While i < nb
            ' Get next character
            Dim c = s(i)
            If c = vbLf Then
                ' Explicit line break
                Me.Cell(w, h, s.Substring(j, i - j), 0, 2, "", False, link)
                i += 1
                sep = -1
                j = i
                l = 0
                If nl = 1 Then

                    Me.x = Me.lMargin
                    w = Me.w - Me.rMargin - Me.x
                    wmax = (w - 2 * Me.cMargin) * 1000 / Me.FontSize
                End If
                nl += 1
                Continue While
            End If
            If c = " " Then
                sep = i
            End If
            l += cw(c)
            If l > wmax Then
                ' Automatic line break
                If sep = -1 Then
                    If (Me.x > Me.lMargin) Then

                        ' Move to next line
                        Me.x = Me.lMargin
                        Me.y += h
                        w = Me.w - Me.rMargin - Me.x
                        wmax = (w - 2 * Me.cMargin) * 1000 / Me.FontSize
                        i += 1
                        nl += 1
                        Continue While
                    End If
                    If i = j Then
                        i += 1
                    End If
                    Me.Cell(w, h, s.Substring(j, i - j), 0, 2, "", False, link)
                Else
                    Me.Cell(w, h, s.Substring(j, sep - j), 0, 2, "", False, link)
                    i = sep + 1
                End If
                sep = -1
                j = i
                l = 0
                If nl = 1 Then
                    Me.x = Me.lMargin
                    w = Me.w - Me.rMargin - Me.x
                    wmax = (w - 2 * Me.cMargin) * 1000 / Me.FontSize
                End If
                nl += 1
            Else
                i += 1
            End If
        End While
        ' Last chunk
        If Not i = j Then
            Me.Cell(l / 1000 * Me.FontSize, h, s.Substring(j), 0, 0, "", False, link)
        End If
    End Sub

    ''' <summary>
    ''' Effectue un saut de ligne. L'abscisse courante est ramenée à la valeur de la marge gauche et l'ordonnée augmente de la valeur indiquée en paramètre.
    ''' </summary>
    ''' <param name="h">L'amplitude du saut de ligne. Par défaut, la valeur est égale à la hauteur de la dernière cellule imprimée.</param>
    Public Sub Ln(Optional h As Double = Nothing)
        ' Line feed; default value Is the last cell height
        Me.x = Me.lMargin
        If h = Nothing Then
            Me.y += Me.lasth
        Else
            Me.y += h
        End If
    End Sub

    ''' <summary>
    ''' Place une image.
    ''' </summary>
    ''' <param name="file">Chemin ou URL de l'image.</param>
    ''' <param name="x">Abscisse du coin supérieur gauche. Si non précisée ou égale à null, l'abscisse courante est utilisée.</param>
    ''' <param name="y">Ordonnée du coin supérieur gauche. Si non précisée ou égale à null, l'ordonnée courante est utilisée ; de plus, un saut de page est d'abord effectué si nécessaire (en cas de saut de page automatique) ; puis, après l'appel, l'ordonnée courante est positionnée en bas de l'image.</param>
    ''' <param name="w">Largeur de l'image dans la page.</param>
    ''' <param name="h">Hauteur de l'image dans la page.</param>
    ''' <param name="type">Format de l'image.</param>
    ''' <param name="link">URL ou identifiant retourné par AddLink().</param>
    Public Async Function ImageAsync(file As String, Optional x As Double? = Nothing, Optional y As Double? = Nothing, Optional w As Double = 0, Optional h As Double = 0, Optional type As ListImageType = ListImageType.JPEG, Optional link As Object = "") As Task
        Dim info As Dictionary(Of String, Object) = Nothing
        ' Put an image on the page
        If file = "" Then
            Me.Erreur("Image file name is empty")
        End If
        If Not Me.images.ContainsKey(file) Then

            Select Case type
                Case ListImageType.JPEG
                    info = Await Me._parsejpg(file)
                Case ListImageType.PNG
                    info = Await Me._parsepng(file)
                    'Case ListImageType.GIF
                    '    info = Me._parsegif(file)
                Case Else
                    Me.Erreur("Unsupported image type.")
            End Select

            'info = Me.mtd(file)

            info("i") = Me.images.Count + 1
            Me.images.Add(file, info)
        Else
            info = Me.images(file)
        End If

        ' Automatic width And height calculation if needed
        If w = 0 AndAlso h = 0 Then
            ' Put image at 96 dpi
            w = -96
            h = -96
        End If
        If w < 0 Then
            w = -info("w") * 72 / w / Me.k
        End If
        If h < 0 Then
            h = -info("h") * 72 / h / Me.k
        End If
        If w = 0 Then
            w = h * info("w") / info("h")
        End If
        If h = 0 Then
            h = w * info("h") / info("w")
        End If

        ' Flowing mode
        If y Is Nothing Then
            If Me.y + h > Me.PageBreakTrigger AndAlso Not Me.InHeader AndAlso Not Me.InFooter AndAlso Me.AcceptPageBreak() Then

                ' Automatic page break
                Dim x2 = Me.x
                Me.AddPage(Me.CurOrientation, Me.CurPageSize, Me.CurRotation)
                Me.x = x2
            End If
            y = Me.y
            Me.y += h
        End If

        If x Is Nothing Then
            x = Me.x
        End If
        Me._out(String.Format("q {0:0.00} 0 0 {1:0.00} {2:0.00} {3:0.00} cm /I{4} Do Q", w * Me.k, h * Me.k, x * Me.k, (Me.h - (y + h)) * Me.k, info("i")))
        If Not link = "" Then
            Me.Link(x, y, w, h, link)
        End If
    End Function

    ''' <summary>
    ''' Renvoie la largeur de la page courante.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetPageWidth() As Double
        ' Get current page width
        Return Me.w
    End Function

    ''' <summary>
    ''' Renvoie la hauteur de la page courante.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetPageHeight() As Double
        ' Get current page height
        Return Me.h
    End Function

    ''' <summary>
    ''' Renvoie l'abscisse de la position courante.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetX() As Double
        ' Get x position
        Return Me.x
    End Function

    ''' <summary>
    ''' Fixe l'abscisse de la position courante. Si la valeur transmise est négative, elle est relative à l'extrémité droite de la page.
    ''' </summary>
    ''' <param name="x">La valeur de l'abscisse.</param>
    Public Sub SetX(x As Double)
        ' Set x position
        If x >= 0 Then
            Me.x = x
        Else
            Me.x = Me.w + x
        End If
    End Sub

    ''' <summary>
    ''' Renvoie l'ordonnée de la position courante.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetY() As Double
        ' Get y position
        Return Me.y
    End Function

    ''' <summary>
    ''' Fixe l'abscisse de la position courante. Si la valeur transmise est négative, elle est relative à l'extrémité droite de la page.
    ''' </summary>
    ''' <param name="y">La valeur de l'abscisse.</param>
    ''' <param name="resetX">Retour à la ligne.</param>
    Public Sub SetY(y As Double, Optional resetX As Boolean = True)
        ' Set y position And optionally reset x
        If y >= 0 Then
            Me.y = y
        Else
            Me.y = Me.h + y
        End If
        If (resetX) Then
            Me.x = Me.lMargin
        End If
    End Sub

    ''' <summary>
    ''' Fixe l'abscisse et l'ordonnée de la position courante. Si les valeurs transmises sont négatives, elles sont relatives respectivement aux extrémités droite et basse de la page.
    ''' </summary>
    ''' <param name="x">La valeur de l'abscisse.</param>
    ''' <param name="y">La valeur de l'ordonnée.</param>
    Public Sub SetXY(x As Double, y As Double)
        ' Set x And y positions
        Me.SetX(x)
        Me.SetY(y, False)
    End Sub

    ''' <summary>
    ''' Sauve le document dans un StorageFile.
    ''' La méthode commence par appeler Close() si nécessaire pour terminer le document.
    ''' </summary>
    ''' <param name="name">Le nom du fichier.</param>
    ''' <returns></returns>
    Public Async Function OutputAsync(Optional name As String = "doc.pdf") As Task(Of StorageFile)

        ' Output PDF to some destination
        Me.Close()

        ' Save to local file
        Dim storageFolder As StorageFolder = ApplicationData.Current.LocalFolder
        Dim sampleFile As StorageFile = Await storageFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting)
        Await FileIO.WriteBytesAsync(sampleFile, Me.buffer)
        Return sampleFile

    End Function

    '*******************************************************************************
    '*                              Protected methods                              *
    '*******************************************************************************

    Protected Function _getpagesize(size As Object) As Double()
        If size.GetType() Is GetType(Integer) Then
            size = size.ToString.ToLower
            Dim a = Me.StdPageSizes(size)
            Return {a(0) / Me.k, a(1) / Me.k}
        Else
            If size(0) > size(1) Then
                Return {size(1), size(0)}
            Else
                Return size
            End If
        End If
    End Function

    Protected Sub _beginpage(orientation As Orientation, size As Object, rotation As Integer)
        Me.page += 1
        Me.pages.Add(Me.page, "")
        Me.state = 2
        Me.x = Me.lMargin
        Me.y = Me.tMargin
        Me.FontFamily = ListFont.Null
        If size = "" Then
            size = Me.DefPageSize
        Else
            size = Me._getpagesize(size)
        End If
        If (Not orientation = Me.CurOrientation OrElse Not size(0) = Me.CurPageSize(0) OrElse Not size(1) = Me.CurPageSize(1)) Then
            ' New size Or orientation
            If (orientation = Orientation.Portrait) Then
                Me.w = size(0)
                Me.h = size(1)
            Else
                Me.w = size(1)
                Me.h = size(0)
            End If
            Me.wPt = Me.w * Me.k
            Me.hPt = Me.h * Me.k
            Me.PageBreakTrigger = Me.h - Me.bMargin
            Me.CurOrientation = orientation
            Me.CurPageSize = size
        End If
        If Not Me.PageInfo.ContainsKey(Me.page) Then
            Me.PageInfo.Add(Me.page, New Dictionary(Of String, Object))
        End If
        If Not orientation = Me.DefOrientation OrElse Not size(0) = Me.DefPageSize(0) OrElse Not size(1) = Me.DefPageSize(1) Then
            Me.PageInfo(Me.page).Add("size", New ArrayList From {Me.wPt, Me.hPt})
        End If
        If Not rotation = 0 Then
            If Not rotation Mod 90 = 0 Then
                Me.Erreur("Incorrect rotation value: " & rotation)
            End If
            Me.CurRotation = rotation
            Me.PageInfo(Me.page).Add("rotation", rotation)
        End If
    End Sub

    Protected Sub _endpage()
        If Not Me.angle = 0 Then

            Me.angle = 0
            Me._out("Q")
        End If
        Me.state = 1
    End Sub

    Protected Function _loadfont(font As ListFont, Optional style As ListFontStyle = ListFontStyle.N) As Dictionary(Of String, Object)
        Dim result As New Dictionary(Of String, Object)
        Select Case font
            Case ListFont.Courier
                Select Case style
                    Case ListFontStyle.N
                        Dim fontClass = New CourierN
                        result = fontClass.GetFont
                    Case ListFontStyle.B
                        Dim fontClass = New CourierB
                        result = fontClass.GetFont
                    Case ListFontStyle.I
                        Dim fontClass = New CourierI
                        result = fontClass.GetFont
                    Case ListFontStyle.BI
                        Dim fontClass = New CourierBI
                        result = fontClass.GetFont
                End Select
            Case ListFont.Helvetica
                Select Case style
                    Case ListFontStyle.N
                        Dim fontClass = New HelveticaN
                        result = fontClass.GetFont
                    Case ListFontStyle.B
                        Dim fontClass = New HelveticaB
                        result = fontClass.GetFont
                    Case ListFontStyle.I
                        Dim fontClass = New HelveticaI
                        result = fontClass.GetFont
                    Case ListFontStyle.BI
                        Dim fontClass = New HelveticaBI
                        result = fontClass.GetFont
                End Select
            Case ListFont.Times
                Select Case style
                    Case ListFontStyle.N
                        Dim fontClass = New TimesN
                        result = fontClass.GetFont
                    Case ListFontStyle.B
                        Dim fontClass = New TimesB
                        result = fontClass.GetFont
                    Case ListFontStyle.I
                        Dim fontClass = New TimesI
                        result = fontClass.GetFont
                    Case ListFontStyle.BI
                        Dim fontClass = New TimesBI
                        result = fontClass.GetFont
                End Select

            Case ListFont.Symbol
                Dim fontClass = New SymbolN
                result = fontClass.GetFont

            Case ListFont.ZapfDingbats
                Dim fontClass = New ZapfDingbatsN
                result = fontClass.GetFont

        End Select

        If Not result.ContainsKey("subsetted") Then
            result.Add("subsetted", False)
        End If
        Return result
    End Function

    Protected Function _isascii(s As String)
        ' Test if string Is ASCII
        Dim nb = s.Length
        For i = 0 To nb - 1
            If AscW(s(i)) > 127 Then
                Return False
            End If
        Next
        Return True
    End Function

    Protected Function _UTF8toUTF16(s As String)

        ' Convert UTF-8 to UTF-16BE with BOM
        Dim res = "\xFE\xFF"
        Dim nb = s.Length
        Dim i = 0
        While i < nb
            Dim c1 = AscW(s(i))
            Dim c2
            i += 1
            If c1 >= 224 Then
                ' 3-byte character
                c2 = AscW(s(i))
                i += 1
                Dim c3 = AscW(s(i))
                i += 1
                res &= ChrW(((c1 & &HF) << 4) + ((c2 & &H3C) >> 2))
                res &= ChrW(((c2 & &H3) << 6) + (c3 & &H3F))
            ElseIf c1 >= 192 Then
                ' 2-byte character
                c2 = AscW(s(i))
                i += 1
                res &= ChrW((c1 & &H1C) >> 2)
                res &= ChrW(((c1 & &H3) << 6) + (c2 & &H3F))
            Else
                ' Single-byte character
                res &= "\0" & ChrW(c1)
            End If
        End While
        Return res
    End Function

    Protected Function _escape(s As String) As String
        ' Escape special characters
        If Not s.IndexOf("(") = -1 OrElse Not s.IndexOf(")") = -1 OrElse Not s.IndexOf("\\") = -1 OrElse Not s.IndexOf("\r") = -1 Then
            Return s.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\").Replace("\r", "\\r")
        Else
            Return s
        End If
    End Function

    Protected Function _textstring(s) As String
        ' Format a text string
        If Not Me._isascii(s) Then
            s = Me._UTF8toUTF16(s)
        End If
        Return "(" & Me._escape(s) & ")"
    End Function

    Protected Function _dounderline(x As Double, y As Double, txt As String) As String
        ' Underline text
        Dim up = Me.CurrentFont("up")
        Dim ut = Me.CurrentFont("ut")
        Dim w = Me.GetStringWidth(txt) + Me.ws * txt.Split(" ").Count
        Return String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00} re f", x * Me.k, (Me.h - (y - up / 1000 * Me.FontSize)) * Me.k, w * Me.k, -ut / 1000 * Me.FontSizePt)
    End Function

    Protected Async Function _parsejpg(file As String, Optional channel As ListChannels = ListChannels.DeviceRGB, Optional bits As Integer = 8) As Task(Of Dictionary(Of String, Object))
        ' Extract info from a JPEG file
        Dim a = New BitmapImage(New Uri(file))
        If a Is Nothing Then
            Me.Erreur("Missing or incorrect image file: " & file)
        End If
        Dim colspace As String = channel.ToString
        Dim result = Await StorageFile.GetFileFromApplicationUriAsync(New Uri(file))

        Dim imageProperties As ImageProperties = Await result.Properties.GetImagePropertiesAsync()
        Dim dtImage As DateTimeOffset = imageProperties.DateTaken

        Dim data = Await _storageFileToBase64(result)
        Return New Dictionary(Of String, Object) From {{"w", imageProperties.Width}, {"h", imageProperties.Height}, {"cs", colspace}, {"bpc", bits}, {"f", "DCTDecode"}, {"data", data}}
    End Function

    Protected Async Function _parsepng(file As String) As Task(Of Dictionary(Of String, Object))
        ' Extract info from a PNG file
        Dim sampleFile As StorageFile = Await StorageFile.GetFileFromPathAsync(file)
        Using f As StreamReader = Await sampleFile.OpenAsync(FileAccessMode.ReadWrite)
            If f Is Nothing Then
                Me.Erreur("Can\'t open image file: " & file)
            End If
            Dim info = Me._parsepngstream(f, file)
            'fclose(f)
            Return info
        End Using
    End Function

    Protected Function _parsepngstream(f As StreamReader, file As String) As Dictionary(Of String, Object)

        ' Check signature
        If Not Me._readstream(f, 8) = ChrW(137) & "PNG" & ChrW(13) & ChrW(10) & ChrW(26) & ChrW(10) Then
            Me.Erreur("Not a PNG file: " & file)
        End If
        ' Read header chunk
        Me._readstream(f, 4)
        If Not Me._readstream(f, 4) = "IHDR" Then
            Me.Erreur("Incorrect PNG file: " & file)
        End If
        Dim w = Me._readint(f)
        Dim h = Me._readint(f)
        Dim bpc = AscW(Me._readstream(f, 1))
        If bpc > 8 Then
            Me.Erreur("16-bit depth not supported: " & file)
        End If
        Dim ct = AscW(Me._readstream(f, 1))
        Dim colspace As String = ""
        If ct = 0 OrElse ct = 4 Then
            colspace = "DeviceGray"
        ElseIf ct = 2 OrElse ct = 6 Then
            colspace = "DeviceRGB"
        ElseIf ct = 3 Then
            colspace = "Indexed"
        Else
            Me.Erreur("Unknown color type: " & file)
        End If
        If Not AscW(Me._readstream(f, 1)) = 0 Then
            Me.Erreur("Unknown compression method: " & file)
        End If
        If Not AscW(Me._readstream(f, 1)) = 0 Then
            Me.Erreur("Unknown filter method: " & file)
        End If
        If Not AscW(Me._readstream(f, 1)) = 0 Then
            Me.Erreur("Interlacing not supported: " & file)
        End If
        Me._readstream(f, 4)
        Dim dp = "/Predictor 15 /Colors " & If(colspace = "DeviceRGB", 3, 1) & " /BitsPerComponent " & bpc & " /Columns " & w

        ' Scan chunks looking for palette, transparency And image data
        Dim pal = ""
        Dim trns = New ArrayList()
        Dim data = ""
        Do

            Dim n = Me._readint(f)
            Dim type = Me._readstream(f, 4)
            If type = "PLTE" Then
                ' Read palette
                pal = Me._readstream(f, n)
                Me._readstream(f, 4)
            ElseIf type = "tRNS" Then
                ' Read transparency info
                Dim t As String = Me._readstream(f, n)
                If ct = 0 Then
                    trns = New ArrayList() From {AscW(t.Substring(1, 1))}
                ElseIf ct = 2 Then
                    trns = New ArrayList() From {AscW(t.Substring(1, 1)), AscW(t.Substring(3, 1)), AscW(t.Substring(5, 1))}
                Else
                    Dim pos = t.IndexOf(ChrW(0))
                    If Not pos = -1 Then
                        trns = New ArrayList() From {pos}
                    End If
                End If
                Me._readstream(f, 4)
            ElseIf type = "IDAT" Then

                ' Read image data block

                data &= Me._readstream(f, n)
                Me._readstream(f, 4)
            ElseIf type = "IEND" Then
                Exit Do
            Else
                Me._readstream(f, n + 4)
            End If
        Loop While n

        If colspace = "Indexed" AndAlso String.IsNullOrEmpty(pal) Then
            Me.Erreur("Missing palette in " & file)
        End If
        Dim info = New Dictionary(Of String, Object) From {{"w", w}, {"h", h}, {"cs", colspace}, {"bpc", bpc}, {"f", "FlateDecode"}, {"dp", dp}, {"pal", pal}, {"trns", trns}}
        If ct >= 4 Then

            ' Extract alpha channel
            data = _gzuncompress(data)
            Dim color = ""
            Dim alpha = ""
            If ct = 4 Then
                ' Gray image
                Dim len = 2 * w
                For i = 0 To h - 1
                    Dim pos = (1 + len) * i
                    color &= data(pos)
                    alpha &= data(pos)
                    Dim line = data.Substring(pos + 1, len)
                    color &= Regex.Replace(line, "/(.)./s", "$1")
                    alpha &= Regex.Replace(line, "/.(.)/s", "$1")
                Next
            Else
                ' RGB image
                Dim len = 4 * w
                For i = 0 To h - 1

                    Dim pos = (1 + len) * i
                    color &= data(pos)
                    alpha &= data(pos)
                    Dim line = data.Substring(pos + 1, len)
                    color &= Regex.Replace(line, "/(.{3})./s", "$1")
                    alpha &= Regex.Replace(line, "/.{3}(.)/s", "$1")
                Next
            End If
            'unset(data)
            data = _gzcompress(color)
            info("smask") = _gzcompress(alpha)
            Me.WithAlpha = True
            If Me.PDFVersion < 1.4 Then
                Me.PDFVersion = "1.4"
            End If
        End If
        'info("data") = data
        info.Add("data", data)
        Return info
    End Function

    Protected Function _readstream(ByRef f As StreamReader, n As Integer) As Char()
        ' Read n bytes from stream
        Dim tab(n) As Char
        Dim s = f.Read(tab, 0, n)
        If s = 0 Then
            Me.Erreur("Error while reading stream")
        End If
        Return tab
    End Function

    Protected Function _readint(f As StreamReader) As Integer
        ' Read a 4-byte integer from stream
        Dim a As Integer = CInt(Me._readstream(f, 4).ToString)
        Return a
    End Function

    'Protected Function _parsegif(file) As Dictionary(Of String, Object)
    '    ' Extract info from a GIF file (via PNG conversion)
    '    If Not function_exists("imagepng") Then
    '        Me.Erreur("GD extension is required for GIF support")
    '    End If
    '    If Not function_exists("imagecreatefromgif") Then
    '        Me.Erreur("GD has no GIF read support")
    '    End If
    '    Dim im = imagecreatefromgif(file)
    '    If Not im Then
    '        Me.Error("Missing or incorrect image file: " & file)
    '    End If
    '    imageinterlace(im, 0)
    '    ob_start()
    '    imagepng(im)
    '    Dim data = ob_get_clean()
    '    imagedestroy(im)
    '    Dim f = fopen("php://temp", "rb+")
    '    If Not f Then
    '        Me.Erreur("Unable to create memory stream")
    '    End If
    '    fwrite(f, data)
    '    rewind(f)
    '    Dim info = Me._parsepngstream(f, file)
    '    fclose(f)
    '    Return info
    'End Function

    Protected Sub _out(s)
        ' Add a line to the document
        If Me.state = 2 Then
            Me.pages(Me.page) &= s & vbLf
        ElseIf Me.state = 1 Then
            Me._put(s)
        ElseIf Me.state = 0 Then
            Me.Erreur("No page has been added yet")
        ElseIf Me.state = 3 Then
            Me.Erreur("The document is closed")
        End If
    End Sub

    Protected Sub _put(s As String)
        Dim tempBuffer As Byte() = System.Text.UnicodeEncoding.UTF8.GetBytes(s & vbLf)
        'Me.buffer &= s & vbLf
        Me.buffer = Me.buffer.Concat(tempBuffer).ToArray
    End Sub
    Protected Sub _putBytes(s As Byte())
        Me.buffer = Me.buffer.Concat(s).ToArray
        Dim tempBuffer As Byte() = System.Text.UnicodeEncoding.UTF8.GetBytes(vbLf)
        Me.buffer = Me.buffer.Concat(tempBuffer).ToArray
    End Sub

    Protected Function _getoffset() As Integer
        Return Me.buffer.Length
    End Function

    Protected Sub _newobj(Optional n = Nothing)
        ' Begin a New object
        If n = Nothing Then
            Me.n += 1
            n = Me.n
        End If
        If Me.offsets.ContainsKey(n) Then
            Me.offsets(n) = Me._getoffset()
        Else
            Me.offsets.Add(n, Me._getoffset())
        End If
        Me._put(n & " 0 obj")
    End Sub

    Protected Sub _putstream(data As String)
        Me._put("stream")
        Me._put(data)
        Me._put("endstream")
    End Sub

    Protected Sub _putstream(data As Byte())
        Me._put("stream")
        Me._putBytes(data)
        Me._put("endstream")
    End Sub

    Protected Sub _putstreamobject(data As String)
        Dim entries As String
        If Me.compress Then
            entries = "/Filter /FlateDecode "
            data = _gzcompress(data)
        Else
            entries = ""
        End If
        entries &= "/Length " & data.Length
        Me._newobj()
        Me._put("<<" & entries & ">>")
        Me._putstream(data)
        Me._put("endobj")
    End Sub

    Protected Sub _putpage(n As Integer)

        Me._newobj()
        Me._put("<</Type /Page")
        Me._put("/Parent 1 0 R")
        If Me.PageInfo(n).ContainsKey("size") Then
            Me._put(String.Format("/MediaBox [0 0 {0:0.00} {1:0.00}]", Me.PageInfo(n)("size")(0), Me.PageInfo(n)("size")(1)))
        End If
        If Me.PageInfo(n).ContainsKey("rotation") Then
            Me._put("/Rotate " & Me.PageInfo(n)("rotation"))
        End If
        Me._put("/Resources 2 0 R")
        If Me.PageLinks.ContainsKey(n) Then

            ' Links
            Dim annots = "/Annots ["
            For Each pl As ArrayList In Me.PageLinks(n)

                Dim rect = String.Format("{0:0.00} {1:0.00} {2:0.00} {3:0.00}", pl(0), pl(1), pl(0) + pl(2), pl(1) - pl(3))
                annots &= "<</Type /Annot /Subtype /Link /Rect [" & rect & "] /Border [0 0 0] "
                If pl(4).GetType Is GetType(String) Then
                    annots &= "/A <</S /URI /URI " & Me._textstring(pl(4)) & ">>>>"
                Else

                    Dim l = Me.links(pl(4))
                    If Not Me.PageInfo(l(0))("size") = Nothing Then
                        Dim h = Me.PageInfo(l(0))("size")(1)
                    Else
                        h = If(Me.DefOrientation = Orientation.Portrait, Me.DefPageSize(1) * Me.k, Me.DefPageSize(0) * Me.k)
                    End If
                    annots &= String.Format("/Dest [{0} 0 R /XYZ 0 {1:0.00} null]>>", Me.PageInfo(l(0))("n"), h - l(1) * Me.k)
                End If
            Next
            Me._put(annots & "]")
        End If
        If (Me.WithAlpha) Then
            Me._put("/Group <</Type /Group /S /Transparency /CS /DeviceRGB>>")
        End If
        Me._put("/Contents " & (Me.n + 1) & " 0 R>>")
        Me._put("endobj")
        ' page content
        If Not String.IsNullOrEmpty(Me._AliasNbPages) Then
            Me.pages(n) = Me.pages(n).ToString.Replace(Me._AliasNbPages, Me.page.ToString)
        End If
        Me._putstreamobject(Me.pages(n))
    End Sub

    Protected Sub _putpages()
        Dim nb = Me.page
        For m = 1 To nb
            Me.PageInfo(m).Add("n", Me.n + 1 + 2 * (m - 1))
        Next
        For m = 1 To nb
            Me._putpage(m)
        Next
        ' Pages root
        Me._newobj(1)
        Me._put("<</Type /Pages")
        Dim kids = "/Kids ["
        For m = 1 To nb
            kids &= Me.PageInfo(m)("n") & " 0 R "
        Next
        Me._put(kids & "]")
        Me._put("/Count " & nb)
        If Me.DefOrientation = Orientation.Portrait Then
            w = Me.DefPageSize(0)
            h = Me.DefPageSize(1)
        Else
            w = Me.DefPageSize(1)
            h = Me.DefPageSize(0)
        End If
        Me._put(String.Format("/MediaBox [0 0 {0:0.00} {1:0.00}]", w * Me.k, h * Me.k))
        Me._put(">>")
        Me._put("endobj")
    End Sub

    Protected Sub _putfonts()

        For Each tab1 In Me.FontFiles
            Dim file = tab1.Key
            Dim info = tab1.Value
            ' Font file embedding
            Me._newobj()
            Me.FontFiles(file)("n") = Me.n
            Dim font As String = "" 'file_get_contents(Me.fontpath & file, True)
            If Not font Then
                Me.Erreur("Font file not found: " & file)
            End If
            Dim compressed = file.Substring(-2) = ".z"
            If Not compressed AndAlso Not info("length2") = Nothing Then
                font = font.Substring(6, info("length1")) & font.Substring(6 + info("length1") + 6, info("length2"))
            End If
            Me._put("<</Length " & font.Length)
            If compressed Then
                Me._put("/Filter /FlateDecode")
            End If
            Me._put("/Length1 " & info("length1"))
            If Not info("length2") = Nothing Then
                Me._put("/Length2 " & info("length2") & " /Length3 0")
            End If
            Me._put(">>")
            Me._putstream(font)
            Me._put("endobj")
        Next

        For Each tab2 In Me.fonts
            Dim k = tab2.Key
            Dim font As Dictionary(Of String, Object) = tab2.Value
            ' Encoding
            If font.ContainsKey("diff") Then
                If Me.encodings.ContainsKey(font("enc")) Then
                    Me._newobj()
                    Me._put("<</Type /Encoding /BaseEncoding /WinAnsiEncoding /Differences [" & font("diff") & "]>>")
                    Me._put("endobj")
                    Me.encodings(font("enc")) = Me.n
                End If
            End If
            ' ToUnicode CMap
            Dim cmapkey As String = ""
            If font.ContainsKey("uv") Then
                If Not font.ContainsKey("enc") Then
                    cmapkey = font("enc")
                Else
                    cmapkey = font("name")
                End If
                If Not Me.cmaps.ContainsKey(cmapkey) Then
                    Dim cmap = Me._tounicodecmap(font("uv"))
                    Me._putstreamobject(cmap)
                    Me.cmaps(cmapkey) = Me.n
                End If
            End If

            ' Font object
            Me.fonts(k).Add("n", Me.n + 1)
            Dim type As String = font("type")
            Dim name = font("name")
            If font("subsetted") Then
                name = "AAAAAA+" & name
            End If
            If type = "Core" Then

                ' Core font
                Me._newobj()
                Me._put("<</Type /Font")
                Me._put("/BaseFont /" & name)
                Me._put("/Subtype /Type1")
                If Not name = "Symbol" AndAlso Not name = "ZapfDingbats" Then
                    Me._put("/Encoding /WinAnsiEncoding")
                End If
                If font.ContainsKey("uv") Then
                    Me._put("/ToUnicode " & Me.cmaps(cmapkey) & " 0 R")
                End If
                Me._put(">>")
                Me._put("endobj")

            ElseIf type = "Type1" OrElse type = "TrueType" Then

                ' Additional Type1 Or TrueType/OpenType font
                Me._newobj()
                Me._put("<</Type /Font")
                Me._put("/BaseFont /" & name)
                Me._put("/Subtype /" & type)
                Me._put("/FirstChar 32 /LastChar 255")
                Me._put("/Widths " & (Me.n + 1) & " 0 R")
                Me._put("/FontDescriptor " & (Me.n + 2) & " 0 R")
                If Not font.ContainsKey("diff") Then
                    Me._put("/Encoding " & Me.encodings(font("enc")) & " 0 R")
                Else
                    Me._put("/Encoding /WinAnsiEncoding")
                End If
                If Not font.ContainsKey("uv") Then
                    Me._put("/ToUnicode " & Me.cmaps(cmapkey) & " 0 R")
                End If
                Me._put(">>")
                Me._put("endobj")
                ' Widths
                Me._newobj()
                Dim cw = font("cw")
                Dim s = "["
                For i = 32 To 255
                    s &= cw(ChrW(i)) & " "
                Next
                Me._put(s & "]")
                Me._put("endobj")
                ' Descriptor
                Me._newobj()
                s = "<</Type /FontDescriptor /FontName /" & name
                'For Each (font("desc") as k=>v)
                For Each tab1 As KeyValuePair(Of String, Object) In font("desc")
                    k = tab1.Key
                    Dim v = tab1.Value
                    s &= " /" & k & " " & v
                Next
                If Not String.IsNullOrEmpty(font("file")) Then
                    s &= " /FontFile" & If(type = "Type1", "", "2") & " " & Me.FontFiles(font("file"))("n") & " 0 R"
                End If
                Me._put(s & ">>")
                Me._put("endobj")
            Else
                ' Allow for additional types
                'mtd = "_put" & type.ToLower
                'If Not method_exists(this, mtd) Then
                Me.Erreur("Unsupported font type: " & type)
                'End If
                'Me.mtd(font)
            End If
        Next
    End Sub

    Protected Function _tounicodecmap(uv As Dictionary(Of Integer, Object))

        Dim ranges = ""
        Dim nbr = 0
        Dim chars = ""
        Dim nbc = 0
        'For Each (uv as c=>v)
        For Each tab1 In uv
            Dim c = tab1.Key
            Dim v = tab1.Value
            If v.GetType IsNot GetType(Integer) Then
                ranges &= String.Format("<{0:X}> <{1:X}> <{2:X4}>" & vbLf, c, c + CInt(v(1)) - 1, CInt(v(0)))
                nbr += 1
            Else
                chars &= String.Format("<{0:X}> <{1:X4}>" & vbLf, c, CInt(v))
                nbc += 1
            End If
        Next
        Dim s = "/CIDInit /ProcSet findresource begin" & vbLf
        s &= "12 dict begin" & vbLf
        s &= "begincmap" & vbLf
        s &= "/CIDSystemInfo" & vbLf
        s &= "<</Registry (Adobe)" & vbLf
        s &= "/Ordering (UCS)" & vbLf
        s &= "/Supplement 0" & vbLf
        s &= ">> def" & vbLf
        s &= "/CMapName /Adobe-Identity-UCS def" & vbLf
        s &= "/CMapType 2 def" & vbLf
        s &= "1 begincodespacerange" & vbLf
        s &= "<00> <FF>" & vbLf
        s &= "endcodespacerange" & vbLf
        If nbr > 0 Then

            s &= nbr & " beginbfrange" & vbLf
            s &= ranges
            s &= "endbfrange" & vbLf
        End If
        If nbc > 0 Then

            s &= nbc & " beginbfchar" & vbLf
            s &= chars
            s &= "endbfchar" & vbLf
        End If
        s &= "endcmap" & vbLf
        s &= "CMapName currentdict /CMap defineresource pop" & vbLf
        s &= "end" & vbLf
        s &= "end"
        Return s
    End Function

    Protected Sub _putimages()
        For Each tab1 In Me.images
            Dim file As Dictionary(Of String, Object) = tab1.Value
            Me._putimage(file)
            file("data") = Nothing
            If file.ContainsKey("smask") Then
                file("smask") = Nothing
            Else
                file.Add("smask", Nothing)
            End If
        Next
    End Sub

    Protected Sub _putimage(ByRef info As Dictionary(Of String, Object))

        Me._newobj()
        info("n") = Me.n
        Me._put("<</Type /XObject")
        Me._put("/Subtype /Image")
        Me._put("/Width " & info("w"))
        Me._put("/Height " & info("h"))
        If info("cs") = "Indexed" Then
            Me._put("/ColorSpace [/Indexed /DeviceRGB " & info("pal").ToString.Length / 3 - 1 & " " & (Me.n + 1) & " 0 R]")
        Else

            Me._put("/ColorSpace /" & info("cs"))
            If info("cs") = "DeviceCMYK" Then
                Me._put("/Decode [1 0 1 0 1 0 1 0]")
            End If
        End If
        Me._put("/BitsPerComponent " & info("bpc"))
        If info.ContainsKey("f") Then
            Me._put("/Filter /" & info("f"))
        End If
        If info.ContainsKey("dp") Then
            Me._put("/DecodeParms <<" & info("dp") & ">>")
        End If
        'If Not info("trns") = Nothing AndAlso is_array(info("trns")) Then
        If info.ContainsKey("trns") Then

            Dim trns = ""
            For i = 0 To info("trns").count - 1
                trns &= info("trns")(i) & " " & info("trns")(i) & " "
            Next
            Me._put("/Mask [" & trns & "]")

        End If
        If info.ContainsKey("smask") Then
            Me._put("/SMask " & (Me.n + 1) & " 0 R")
        End If
        Me._put("/Length " & info("data").ToString.Length & ">>")
        Dim data As Byte() = info("data")
        Me._putstream(data)
        Me._put("endobj")
        ' Soft mask
        If info.ContainsKey("smask") Then

            Dim dp = "/Predictor 15 /Colors 1 /BitsPerComponent 8 /Columns " & info("w")
            Dim smask = New Dictionary(Of String, Object) From {{"w", info("w")}, {"h", info("h")}, {"cs", "DeviceGray"}, {"bpc", 8}, {"f", info("f")}, {"dp", dp}, {"data", info("smask")}}
            Me._putimage(smask)

        End If
        ' Palette
        If info("cs") = "Indexed" Then
            Me._putstreamobject(info("pal"))
        End If
    End Sub

    Protected Sub _putxobjectdict()
        For Each tab1 In Me.images
            Dim _image = tab1.Value
            Me._put("/I" & _image("i") & " " & _image("n") & " 0 R")
        Next
    End Sub

    Protected Sub _putresourcedict()
        Me._put("/ProcSet [/PDF /Text /ImageB /ImageC /ImageI]")
        Me._put("/Font <<")
        For Each font In Me.fonts.Values
            Me._put("/F" & font("i") & " " & font("n") & " 0 R")
        Next
        Me._put(">>")
        Me._put("/XObject <<")
        Me._putxobjectdict()
        Me._put(">>")
    End Sub

    Protected Sub _putresources()
        Me._putfonts()
        Me._putimages()
        ' Resource dictionary
        Me._newobj(2)
        Me._put("<<")
        Me._putresourcedict()
        Me._put(">>")
        Me._put("endobj")

        Me._putbookmarks()
    End Sub

    Protected Sub _putinfo()
        Me.metadata.Add("Producer", "UwpPdf " & UWPPDF_VERSION)
        'Me.metadata.Add("CreationDate", "D:" & New DateTime().ToString("YmdHis"))
        Me.metadata.Add("CreationDate", "D:" & DateTime.Now().ToString("yyyyMMddhhmmss"))
        'For Each (Me.metadata As key=>value)
        For Each tab1 In Me.metadata
            Dim key = tab1.Key
            Dim value = tab1.Value
            Me._put("/" & key & " " & Me._textstring(value))
        Next
    End Sub

    Protected Sub _putcatalog()
        Dim n = Me.PageInfo(1)("n")
        Me._put("/Type /Catalog")
        Me._put("/Pages 1 0 R")
        If Me.ZoomMode = Zoom.Fullpage Then
            Me._put("/OpenAction [" & n & " 0 R /Fit]")
        ElseIf Me.ZoomMode = Zoom.Fullwidth Then
            Me._put("/OpenAction [" & n & " 0 R /FitH null]")
        ElseIf Me.ZoomMode = Zoom.Real Then
            Me._put("/OpenAction [" & n & " 0 R /XYZ null null 1]")
        ElseIf Not Me.ZoomMode.GetType Is GetType(Integer) AndAlso Me.ZoomMode > 10 Then
            Me._put("/OpenAction [" & n & " 0 R /XYZ null null " & String.Format("{0:0.00}", Me.ZoomMode / 100) & "]")
        End If
        If Me.LayoutMode = Layout.SingleLayout Then
            Me._put("/PageLayout /SinglePage")
        ElseIf Me.LayoutMode = Layout.Continuous Then
            Me._put("/PageLayout /OneColumn")
        ElseIf Me.LayoutMode = Layout.Two Then
            Me._put("/PageLayout /TwoColumnLeft")
        End If

        If Me.outlines.Count > 0 Then
            Me._put("/Outlines " & Me.outlineRoot & " 0 R")
            Me._put("/PageMode /UseOutlines")
        End If
    End Sub

    Protected Sub _putheader()
        Me._put("%PDF-" & Me.PDFVersion)
    End Sub

    Protected Sub _puttrailer()
        Me._put("/Size " & (Me.n + 1))
        Me._put("/Root " & Me.n & " 0 R")
        Me._put("/Info " & (Me.n - 1) & " 0 R")
    End Sub

    Protected Sub _enddoc()
        Me._putheader()
        Me._putpages()
        Me._putresources()
        ' Info
        Me._newobj()
        Me._put("<<")
        Me._putinfo()
        Me._put(">>")
        Me._put("endobj")
        ' Catalog
        Me._newobj()
        Me._put("<<")
        Me._putcatalog()
        Me._put(">>")
        Me._put("endobj")
        ' Cross-ref
        Dim offset = Me._getoffset()
        Me._put("xref")
        Me._put("0 " & (Me.n + 1))
        Me._put("0000000000 65535 f ")
        For i = 1 To Me.n
            Me._put(String.Format("{0:0000000000} 00000 n ", Me.offsets(i)))
        Next
        ' Trailer
        Me._put("trailer")
        Me._put("<<")
        Me._puttrailer()
        Me._put(">>")
        Me._put("startxref")
        Me._put(offset)
        Me._put("%%EOF")
        Me.state = 3
    End Sub

    Protected Function _utf8encode(ByVal str As String) As String
        Dim utf8Encoding As New UTF8Encoding(True)
        Dim encodedString() As Byte
        encodedString = utf8Encoding.GetBytes(str)
        Return utf8Encoding.GetString(encodedString)
    End Function

    Protected Async Function _storageFileToBase64(file As StorageFile) As Task(Of Byte())
        Dim byteArray As Byte() = New Byte() {}
        If file IsNot Nothing Then
            Dim fileStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
            Dim reader = New DataReader(fileStream.GetInputStreamAt(0))
            Await reader.LoadAsync(CUInt(fileStream.Size))
            byteArray = New Byte(fileStream.Size - 1) {}
            reader.ReadBytes(byteArray)
            'For Each bit In byteArray
            '    Base64String += ChrW(bit)
            'Next
        End If

        'Return Base64String
        Return byteArray
    End Function

    Protected Function _gzcompress(ByVal txt As String) As String
        Dim result As String = ""
        For Each bit In ZlibStream.CompressBuffer(Encoding.Unicode.GetBytes(txt))
            result &= ChrW(bit)
        Next
        Return result
    End Function

    Protected Function _gzuncompress(ByVal txt As String) As String
        Dim result As String = ""
        Return ZlibStream.UncompressString(Encoding.Unicode.GetBytes(txt))
    End Function
End Class
