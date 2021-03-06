﻿Public Class SymbolN
    Public type = "Core"
    Public name = "Symbol"
    Public up = -100
    Public ut = 50
    Public cw As New Dictionary(Of Char, Integer) From {
    {ChrW(0), 250}, {ChrW(1), 250}, {ChrW(2), 250}, {ChrW(3), 250}, {ChrW(4), 250}, {ChrW(5), 250}, {ChrW(6), 250}, {ChrW(7), 250}, {ChrW(8), 250}, {ChrW(9), 250}, {ChrW(10), 250}, {ChrW(11), 250}, {ChrW(12), 250}, {ChrW(13), 250}, {ChrW(14), 250}, {ChrW(15), 250}, {ChrW(16), 250}, {ChrW(17), 250}, {ChrW(18), 250}, {ChrW(19), 250}, {ChrW(20), 250}, {ChrW(21), 250},
    {ChrW(22), 250}, {ChrW(23), 250}, {ChrW(24), 250}, {ChrW(25), 250}, {ChrW(26), 250}, {ChrW(27), 250}, {ChrW(28), 250}, {ChrW(29), 250}, {ChrW(30), 250}, {ChrW(31), 250}, {" ", 250}, {"!", 333}, {"""", 713}, {"#", 500}, {"$", 549}, {"%", 833}, {"&", 778}, {"'", 439}, {"(", 333}, {")", 333}, {"*", 500}, {"+", 549},
    {",", 250}, {"-", 549}, {".", 250}, {"/", 278}, {"0", 500}, {"1", 500}, {"2", 500}, {"3", 500}, {"4", 500}, {"5", 500}, {"6", 500}, {"7", 500}, {"8", 500}, {"9", 500}, {":", 278}, {";", 278}, {"<", 549}, {"=", 549}, {">", 549}, {"?", 444}, {"@", 549}, {"A", 722},
    {"B", 667}, {"C", 722}, {"D", 612}, {"E", 611}, {"F", 763}, {"G", 603}, {"H", 722}, {"I", 333}, {"J", 631}, {"K", 722}, {"L", 686}, {"M", 889}, {"N", 722}, {"O", 722}, {"P", 768}, {"Q", 741}, {"R", 556}, {"S", 592}, {"T", 611}, {"U", 690}, {"V", 439}, {"W", 768},
    {"X", 645}, {"Y", 795}, {"Z", 611}, {"[", 333}, {"\", 863}, {"]", 333}, {"^", 658}, {"_", 500}, {"`", 500}, {"a", 631}, {"b", 549}, {"c", 549}, {"d", 494}, {"e", 439}, {"f", 521}, {"g", 411}, {"h", 603}, {"i", 329}, {"j", 603}, {"k", 549}, {"l", 549}, {"m", 576},
    {"n", 521}, {"o", 549}, {"p", 549}, {"q", 521}, {"r", 549}, {"s", 603}, {"t", 439}, {"u", 576}, {"v", 713}, {"w", 686}, {"x", 493}, {"y", 686}, {"z", 494}, {"{", 480}, {"|", 200}, {"}", 480}, {"~", 549}, {ChrW(127), 0}, {ChrW(128), 0}, {ChrW(129), 0}, {ChrW(130), 0}, {ChrW(131), 0},
    {ChrW(132), 0}, {ChrW(133), 0}, {ChrW(134), 0}, {ChrW(135), 0}, {ChrW(136), 0}, {ChrW(137), 0}, {ChrW(138), 0}, {ChrW(139), 0}, {ChrW(140), 0}, {ChrW(141), 0}, {ChrW(142), 0}, {ChrW(143), 0}, {ChrW(144), 0}, {ChrW(145), 0}, {ChrW(146), 0}, {ChrW(147), 0}, {ChrW(148), 0}, {ChrW(149), 0}, {ChrW(150), 0}, {ChrW(151), 0}, {ChrW(152), 0}, {ChrW(153), 0},
    {ChrW(154), 0}, {ChrW(155), 0}, {ChrW(156), 0}, {ChrW(157), 0}, {ChrW(158), 0}, {ChrW(159), 0}, {ChrW(160), 750}, {ChrW(161), 620}, {ChrW(162), 247}, {ChrW(163), 549}, {ChrW(164), 167}, {ChrW(165), 713}, {ChrW(166), 500}, {ChrW(167), 753}, {ChrW(168), 753}, {ChrW(169), 753}, {ChrW(170), 753}, {ChrW(171), 1042}, {ChrW(172), 987}, {ChrW(173), 603}, {ChrW(174), 987}, {ChrW(175), 603},
    {ChrW(176), 400}, {ChrW(177), 549}, {ChrW(178), 411}, {ChrW(179), 549}, {ChrW(180), 549}, {ChrW(181), 713}, {ChrW(182), 494}, {ChrW(183), 460}, {ChrW(184), 549}, {ChrW(185), 549}, {ChrW(186), 549}, {ChrW(187), 549}, {ChrW(188), 1000}, {ChrW(189), 603}, {ChrW(190), 1000}, {ChrW(191), 658}, {ChrW(192), 823}, {ChrW(193), 686}, {ChrW(194), 795}, {ChrW(195), 987}, {ChrW(196), 768}, {ChrW(197), 768},
    {ChrW(198), 823}, {ChrW(199), 768}, {ChrW(200), 768}, {ChrW(201), 713}, {ChrW(202), 713}, {ChrW(203), 713}, {ChrW(204), 713}, {ChrW(205), 713}, {ChrW(206), 713}, {ChrW(207), 713}, {ChrW(208), 768}, {ChrW(209), 713}, {ChrW(210), 790}, {ChrW(211), 790}, {ChrW(212), 890}, {ChrW(213), 823}, {ChrW(214), 549}, {ChrW(215), 250}, {ChrW(216), 713}, {ChrW(217), 603}, {ChrW(218), 603}, {ChrW(219), 1042},
    {ChrW(220), 987}, {ChrW(221), 603}, {ChrW(222), 987}, {ChrW(223), 603}, {ChrW(224), 494}, {ChrW(225), 329}, {ChrW(226), 790}, {ChrW(227), 790}, {ChrW(228), 786}, {ChrW(229), 713}, {ChrW(230), 384}, {ChrW(231), 384}, {ChrW(232), 384}, {ChrW(233), 384}, {ChrW(234), 384}, {ChrW(235), 384}, {ChrW(236), 494}, {ChrW(237), 494}, {ChrW(238), 494}, {ChrW(239), 494}, {ChrW(240), 0}, {ChrW(241), 329},
    {ChrW(242), 274}, {ChrW(243), 686}, {ChrW(244), 686}, {ChrW(245), 686}, {ChrW(246), 384}, {ChrW(247), 384}, {ChrW(248), 384}, {ChrW(249), 384}, {ChrW(250), 384}, {ChrW(251), 384}, {ChrW(252), 494}, {ChrW(253), 494}, {ChrW(254), 494}, {ChrW(255), 0}}

    Public uv = New Dictionary(Of Integer, Object) From {
    {32, 160},
    {33, 33},
    {34, 8704},
    {35, 35},
    {36, 8707},
    {37, New Dictionary(Of Integer, Object) From {{0, 37}, {1, 2}}},
    {39, 8715},
    {40, New Dictionary(Of Integer, Object) From {{0, 40}, {1, 2}}},
    {42, 8727},
    {43, New Dictionary(Of Integer, Object) From {{0, 43}, {1, 2}}},
    {45, 8722},
    {46, New Dictionary(Of Integer, Object) From {{0, 46}, {1, 18}}},
    {64, 8773},
    {65, New Dictionary(Of Integer, Object) From {{0, 913}, {1, 2}}},
    {67, 935},
    {68, New Dictionary(Of Integer, Object) From {{0, 916}, {1, 2}}},
    {70, 934},
    {71, 915},
    {72, 919},
    {73, 921},
    {74, 977},
    {75, New Dictionary(Of Integer, Object) From {{0, 922}, {1, 4}}},
    {79, New Dictionary(Of Integer, Object) From {{0, 927}, {1, 2}}},
    {81, 920},
    {82, 929},
    {83, New Dictionary(Of Integer, Object) From {{0, 931}, {1, 3}}},
    {86, 962},
    {87, 937},
    {88, 926},
    {89, 936},
    {90, 918},
    {91, 91},
    {92, 8756},
    {93, 93},
    {94, 8869},
    {95, 95},
    {96, 63717},
    {97, New Dictionary(Of Integer, Object) From {{0, 945}, {1, 2}}},
    {99, 967},
    {100, New Dictionary(Of Integer, Object) From {{0, 948}, {1, 2}}},
    {102, 966},
    {103, 947},
    {104, 951},
    {105, 953},
    {106, 981},
    {107, New Dictionary(Of Integer, Object) From {{0, 954}, {1, 4}}},
    {111, New Dictionary(Of Integer, Object) From {{0, 959}, {1, 2}}},
    {113, 952},
    {114, 961},
    {115, New Dictionary(Of Integer, Object) From {{0, 963}, {1, 3}}},
    {118, 982},
    {119, 969},
    {120, 958},
    {121, 968},
    {122, 950},
    {123, New Dictionary(Of Integer, Object) From {{0, 123}, {1, 3}}},
    {126, 8764},
    {160, 8364},
    {161, 978},
    {162, 8242},
    {163, 8804},
    {164, 8725},
    {165, 8734},
    {166, 402},
    {167, 9827},
    {168, 9830},
    {169, 9829},
    {170, 9824},
    {171, 8596},
    {172, New Dictionary(Of Integer, Object) From {{0, 8592}, {1, 4}}},
    {176, New Dictionary(Of Integer, Object) From {{0, 176}, {1, 2}}},
    {178, 8243},
    {179, 8805},
    {180, 215},
    {181, 8733},
    {182, 8706},
    {183, 8226},
    {184, 247},
    {185, New Dictionary(Of Integer, Object) From {{0, 8800}, {1, 2}}},
    {187, 8776},
    {188, 8230},
    {189, New Dictionary(Of Integer, Object) From {{0, 63718}, {1, 2}}},
    {191, 8629},
    {192, 8501},
    {193, 8465},
    {194, 8476},
    {195, 8472},
    {196, 8855},
    {197, 8853},
    {198, 8709},
    {199, New Dictionary(Of Integer, Object) From {{0, 8745}, {1, 2}}},
    {201, 8835},
    {202, 8839},
    {203, 8836},
    {204, 8834},
    {205, 8838},
    {206, New Dictionary(Of Integer, Object) From {{0, 8712}, {1, 2}}},
    {208, 8736},
    {209, 8711},
    {210, 63194},
    {211, 63193},
    {212, 63195},
    {213, 8719},
    {214, 8730},
    {215, 8901},
    {216, 172},
    {217, New Dictionary(Of Integer, Object) From {{0, 8743}, {1, 2}}},
    {219, 8660},
    {220, New Dictionary(Of Integer, Object) From {{0, 8656}, {1, 4}}},
    {224, 9674},
    {225, 9001},
    {226, New Dictionary(Of Integer, Object) From {{0, 63720}, {1, 3}}},
    {229, 8721},
    {230, New Dictionary(Of Integer, Object) From {{0, 63723}, {1, 10}}},
    {241, 9002},
    {242, 8747},
    {243, 8992},
    {244, 63733},
    {245, 8993},
    {246, New Dictionary(Of Integer, Object) From {{0, 63734}, {1, 9}}}}

    Public Function GetFont() As Dictionary(Of String, Object)
        Return New Dictionary(Of String, Object) From {{"type", type}, {"name", name}, {"up", up}, {"ut", ut}, {"enc", Nothing}, {"cw", cw}, {"uv", uv}}
    End Function
End Class
