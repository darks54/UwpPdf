﻿Public Class TimesN
    Public type = "Core"
    Public name = "Times-Roman"
    Public up = -100
    Public ut = 50
    Public enc = "cp1252"
    Public cw As New Dictionary(Of Char, Integer) From {
    {ChrW(0), 250}, {ChrW(1), 250}, {ChrW(2), 250}, {ChrW(3), 250}, {ChrW(4), 250}, {ChrW(5), 250}, {ChrW(6), 250}, {ChrW(7), 250}, {ChrW(8), 250}, {ChrW(9), 250}, {ChrW(10), 250}, {ChrW(11), 250}, {ChrW(12), 250}, {ChrW(13), 250}, {ChrW(14), 250}, {ChrW(15), 250}, {ChrW(16), 250}, {ChrW(17), 250}, {ChrW(18), 250}, {ChrW(19), 250}, {ChrW(20), 250}, {ChrW(21), 250},
    {ChrW(22), 250}, {ChrW(23), 250}, {ChrW(24), 250}, {ChrW(25), 250}, {ChrW(26), 250}, {ChrW(27), 250}, {ChrW(28), 250}, {ChrW(29), 250}, {ChrW(30), 250}, {ChrW(31), 250}, {" ", 250}, {"!", 333}, {"""", 408}, {"#", 500}, {"$", 500}, {"%", 833}, {"&", 778}, {"'", 180}, {"(", 333}, {")", 333}, {"*", 500}, {"+", 564},
    {",", 250}, {"-", 333}, {".", 250}, {"/", 278}, {"0", 500}, {"1", 500}, {"2", 500}, {"3", 500}, {"4", 500}, {"5", 500}, {"6", 500}, {"7", 500}, {"8", 500}, {"9", 500}, {":", 278}, {";", 278}, {"<", 564}, {"=", 564}, {">", 564}, {"?", 444}, {"@", 921}, {"A", 722},
    {"B", 667}, {"C", 667}, {"D", 722}, {"E", 611}, {"F", 556}, {"G", 722}, {"H", 722}, {"I", 333}, {"J", 389}, {"K", 722}, {"L", 611}, {"M", 889}, {"N", 722}, {"O", 722}, {"P", 556}, {"Q", 722}, {"R", 667}, {"S", 556}, {"T", 611}, {"U", 722}, {"V", 722}, {"W", 944},
    {"X", 722}, {"Y", 722}, {"Z", 611}, {"[", 333}, {"\", 278}, {"]", 333}, {"^", 469}, {"_", 500}, {"`", 333}, {"a", 444}, {"b", 500}, {"c", 444}, {"d", 500}, {"e", 444}, {"f", 333}, {"g", 500}, {"h", 500}, {"i", 278}, {"j", 278}, {"k", 500}, {"l", 278}, {"m", 778},
    {"n", 500}, {"o", 500}, {"p", 500}, {"q", 500}, {"r", 333}, {"s", 389}, {"t", 278}, {"u", 500}, {"v", 500}, {"w", 722}, {"x", 500}, {"y", 500}, {"z", 444}, {"{", 480}, {"|", 200}, {"}", 480}, {"~", 541}, {ChrW(127), 350}, {ChrW(128), 500}, {ChrW(129), 350}, {ChrW(130), 333}, {ChrW(131), 500},
    {ChrW(132), 444}, {ChrW(133), 1000}, {ChrW(134), 500}, {ChrW(135), 500}, {ChrW(136), 333}, {ChrW(137), 1000}, {ChrW(138), 556}, {ChrW(139), 333}, {ChrW(140), 889}, {ChrW(141), 350}, {ChrW(142), 611}, {ChrW(143), 350}, {ChrW(144), 350}, {ChrW(145), 333}, {ChrW(146), 333}, {ChrW(147), 444}, {ChrW(148), 444}, {ChrW(149), 350}, {ChrW(150), 500}, {ChrW(151), 1000}, {ChrW(152), 333}, {ChrW(153), 980},
    {ChrW(154), 389}, {ChrW(155), 333}, {ChrW(156), 722}, {ChrW(157), 350}, {ChrW(158), 444}, {ChrW(159), 722}, {ChrW(160), 250}, {ChrW(161), 333}, {ChrW(162), 500}, {ChrW(163), 500}, {ChrW(164), 500}, {ChrW(165), 500}, {ChrW(166), 200}, {ChrW(167), 500}, {ChrW(168), 333}, {ChrW(169), 760}, {ChrW(170), 276}, {ChrW(171), 500}, {ChrW(172), 564}, {ChrW(173), 333}, {ChrW(174), 760}, {ChrW(175), 333},
    {ChrW(176), 400}, {ChrW(177), 564}, {ChrW(178), 300}, {ChrW(179), 300}, {ChrW(180), 333}, {ChrW(181), 500}, {ChrW(182), 453}, {ChrW(183), 250}, {ChrW(184), 333}, {ChrW(185), 300}, {ChrW(186), 310}, {ChrW(187), 500}, {ChrW(188), 750}, {ChrW(189), 750}, {ChrW(190), 750}, {ChrW(191), 444}, {ChrW(192), 722}, {ChrW(193), 722}, {ChrW(194), 722}, {ChrW(195), 722}, {ChrW(196), 722}, {ChrW(197), 722},
    {ChrW(198), 889}, {ChrW(199), 667}, {ChrW(200), 611}, {ChrW(201), 611}, {ChrW(202), 611}, {ChrW(203), 611}, {ChrW(204), 333}, {ChrW(205), 333}, {ChrW(206), 333}, {ChrW(207), 333}, {ChrW(208), 722}, {ChrW(209), 722}, {ChrW(210), 722}, {ChrW(211), 722}, {ChrW(212), 722}, {ChrW(213), 722}, {ChrW(214), 722}, {ChrW(215), 564}, {ChrW(216), 722}, {ChrW(217), 722}, {ChrW(218), 722}, {ChrW(219), 722},
    {ChrW(220), 722}, {ChrW(221), 722}, {ChrW(222), 556}, {ChrW(223), 500}, {ChrW(224), 444}, {ChrW(225), 444}, {ChrW(226), 444}, {ChrW(227), 444}, {ChrW(228), 444}, {ChrW(229), 444}, {ChrW(230), 667}, {ChrW(231), 444}, {ChrW(232), 444}, {ChrW(233), 444}, {ChrW(234), 444}, {ChrW(235), 444}, {ChrW(236), 278}, {ChrW(237), 278}, {ChrW(238), 278}, {ChrW(239), 278}, {ChrW(240), 500}, {ChrW(241), 500},
    {ChrW(242), 500}, {ChrW(243), 500}, {ChrW(244), 500}, {ChrW(245), 500}, {ChrW(246), 500}, {ChrW(247), 564}, {ChrW(248), 500}, {ChrW(249), 500}, {ChrW(250), 500}, {ChrW(251), 500}, {ChrW(252), 500}, {ChrW(253), 500}, {ChrW(254), 500}, {ChrW(255), 500}}

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

    Public Function GetFont() As Dictionary(Of String, Object)
        Return New Dictionary(Of String, Object) From {{"type", type}, {"name", name}, {"up", up}, {"ut", ut}, {"enc", enc}, {"cw", cw}, {"uv", uv}}
    End Function
End Class
