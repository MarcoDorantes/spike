namespace spec;

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class rfcSpec
{
    #region SUT
    //static bool IsLetter(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z'); //https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#positional-pattern

    //https://www.studocu.com/es-mx/document/universidad-del-valle-de-mexico/administracion/algoritmo-para-generar-el-rfc-con-homoclave-para-personas-fisicas-y-morales/12002840
    static string map(char c)
    {
        c = char.ToUpper(c);
        if (c == ' ') return "00";
        if (c == '&') return "10";
        if (c == 'Ñ') return "40";
        if (char.IsDigit(c)) return $"0{c}";
        if (char.IsLetter(c))
        {
            if ((int)c < (int)'J') return $"{((int)c) - 54}";
            else if ((int)c >= (int)'J' && (int)c < (int)'S') return $"{((int)c) - 53}";
            else if ((int)c >= (int)'S') return $"{((int)c) - 51}";
            else throw new ArgumentException($"Unsupported letter ({c})");
        }
        throw new ArgumentException($"Unsupported char ({c})");
    }

    static IEnumerable<string> topair(string line)
    {
        for (int k = 1; k < line.Length; ++k) yield return line.Substring(k - 1, 2);
    }

    static int pairsum(IList<string> pair) => pair.Sum(p => int.Parse(p) * int.Parse(p.Substring(1, 1)));

    static char hom(int h)
    {
        if (h < 0 || h > 33) throw new ArgumentException($"Unsupported h ({h})");
        if (h < 9) return (char)(50 + (h - 1));
        if (h > 8 && h < 23) return (char)(56 + h);
        if (h > 22) return (char)(57 + h);
        throw new ArgumentException($"Unsupported h ({h})");
    }

    static int digv(char c)
    {
        var n = (int)c;
        if (c == '&') return 24;
        if (c == ' ') return 37;
        if (c == 'Ñ') return 38;
        if (n > 47 && n < 58) return n - 48;
        if (n > 64 && n < 79) return n - 55;
        if (n > 78 && n < 91) return n - 54;
        return 0;
    }

    static char checksum(int n)
    {
        if (n == 0) return '0';
        if (n == 10) return 'A';
        if (n > 0) return $"{11 - n}"[0];
        throw new ArgumentException($"Unsupported verif ({n})");
    }
    #endregion

    [TestMethod]
    public void map1()
    {
        Assert.AreEqual("00", map(' '));
        Assert.AreEqual("10", map('&'));
        Assert.AreEqual("07", map('7'));
        Assert.AreEqual("40", map('ñ'));
        Assert.AreEqual("11", map('A'));
        Assert.AreEqual("19", map('I'));
        Assert.AreEqual("21", map('J'));
        Assert.AreEqual("29", map('R'));
        Assert.AreEqual("32", map('S'));
        Assert.AreEqual("39", map('Z'));
    }

    [TestMethod]
    public void sum1()
    {
        var name = "GOMEZ DIAZ EMMA";
        var agg = name.Aggregate(new StringBuilder("0"), (w, n) => w.AppendFormat("{0}", map(n)));
        var line = $"{agg}";
        Assert.AreEqual("0172624153900141911390015242411", line);

        var pair = topair(line).ToList();
        var lin_ = pair.Aggregate(new System.Text.StringBuilder(), (w, n) => w.AppendFormat("{0}", n));
        Assert.AreEqual("011772266224411553399000011441199111133990000115522442244111", $"{lin_}");

        var sum = pairsum(pair);
        Assert.AreEqual(2535, sum);

        var suffix = int.Parse($"{sum}".Substring($"{sum}".Length - 3, 3));
        var m1 = suffix / 34;
        var m2 = suffix % 34;
        var h1 = hom(m1);
        var h2 = hom(m2);
        Assert.AreEqual('G', h1);
        Assert.AreEqual('R', h2);
        Assert.AreEqual('1', hom(0));
        Assert.AreEqual('P', hom(23));

        var rfc_12 = new StringBuilder("GODE561231");
        rfc_12.Append(h1);
        rfc_12.Append(h2);
        var rfc12 = $"{rfc_12}";
        Assert.AreEqual(00, digv('0'));
        Assert.AreEqual(10, digv('A'));
        Assert.AreEqual(23, digv('N'));
        Assert.AreEqual(24, digv('&'));
        Assert.AreEqual(25, digv('O'));
        Assert.AreEqual(36, digv('Z'));
        Assert.AreEqual(37, digv(' '));
        Assert.AreEqual(38, digv('Ñ'));
        Assert.AreEqual(00, digv('|'));

        int sumdigv = 0;
        for (int k = 0; k < rfc12.Length; ++k)
        {
            sumdigv += (digv(rfc12[k]) * (13 - k));
        }
        Assert.AreEqual(1026, sumdigv);

        var verif = sumdigv % 11;
        Assert.AreEqual('8', checksum(verif));
        Assert.AreEqual('0', checksum(0));
        Assert.AreEqual('A', checksum(10));
    }

    [TestMethod, Ignore]
    public void rfc2()
    {
        var name = "wn";
        var agg = name.Aggregate(new System.Text.StringBuilder("0"), (w, n) => w.AppendFormat("{0}", map(n)));
        var line = $"{agg}";

        var pair = topair(line).ToList();

        var sum = pairsum(pair);

        var suffix = int.Parse($"{sum}".Substring($"{sum}".Length - 3, 3));
        var m1 = suffix / 34;
        var m2 = suffix % 34;
        Assert.AreEqual('x', hom(m1));
        Assert.AreEqual('x', hom(m2));

        var rfc_12 = new StringBuilder("wn-");
        rfc_12.Append('x');
        rfc_12.Append('x');
        var rfc12 = $"{rfc_12}";

        int sumdigv = 0;
        for (int k = 0; k < rfc12.Length; ++k)
        {
            sumdigv += (digv(rfc12[k]) * (13 - k));
        }

        var verif = sumdigv % 11;
        Assert.AreEqual('x', checksum(verif));
    }

    [TestMethod]
    public void rfc3()
    {
        var name = "DIAZ WAYNE BRUNO";
        var agg = name.Aggregate(new System.Text.StringBuilder("0"), (w, n) => w.AppendFormat("{0}", map(n)));
        var line = $"{agg}";

        var pair = topair(line).ToList();

        var sum = pairsum(pair);

        var suffix = int.Parse($"{sum}".Substring($"{sum}".Length - 3, 3));
        var m1 = suffix / 34;
        var m2 = suffix % 34;
        Assert.AreEqual('1', hom(m1));
        Assert.AreEqual('W', hom(m2));

        var rfc_12 = new StringBuilder("DIWB650211");
        rfc_12.Append('1');
        rfc_12.Append('W');
        var rfc12 = $"{rfc_12}";

        int sumdigv = 0;
        for (int k = 0; k < rfc12.Length; ++k)
        {
            sumdigv += (digv(rfc12[k]) * (13 - k));
        }

        var verif = sumdigv % 11;
        Assert.AreEqual('3', checksum(verif));
    }
}