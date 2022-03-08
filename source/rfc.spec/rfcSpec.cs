namespace spec;

using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static rfc.lib.rfc;

[TestClass]
public class rfcSpec
{
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

    [TestMethod]
    public void rfc3a()
    {
        var name = "DIAZ WAYNE BRUNO";
        var simple = "DIWB650211";

        Assert.AreEqual("DIWB6502111W3", getfullrfc(name, simple));
    }
}