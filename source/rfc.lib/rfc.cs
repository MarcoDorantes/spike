namespace rfc.lib;

using System.Text;

public static class rfc
{
    //static bool IsLetter(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z'); //https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#positional-pattern

    //https://www.studocu.com/es-mx/document/universidad-del-valle-de-mexico/administracion/algoritmo-para-generar-el-rfc-con-homoclave-para-personas-fisicas-y-morales/12002840
    public static string map(char c)
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

    public static IEnumerable<string> topair(string line)
    {
        for (int k = 1; k < line.Length; ++k) yield return line.Substring(k - 1, 2);
    }

    public static int pairsum(IList<string> pair) => pair.Sum(p => int.Parse(p) * int.Parse(p.Substring(1, 1)));

    public static char hom(int h)
    {
        if (h < 0 || h > 33) throw new ArgumentException($"Unsupported h ({h})");
        if (h < 9) return (char)(50 + (h - 1));
        if (h > 8 && h < 23) return (char)(56 + h);
        if (h > 22) return (char)(57 + h);
        throw new ArgumentException($"Unsupported h ({h})");
    }

    public static int digv(char c)
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

    public static char checksum(int n)
    {
        if (n == 0) return '0';
        if (n == 10) return 'A';
        if (n > 0) return $"{11 - n}"[0];
        throw new ArgumentException($"Unsupported verif ({n})");
    }

    public static string getfullrfc(string name, string simple)
    {
        var agg = name.Aggregate(new System.Text.StringBuilder("0"), (w, n) => w.AppendFormat("{0}", map(n)));
        var line = $"{agg}";

        var pair = topair(line).ToList();

        var sum = pairsum(pair);

        var suffix = int.Parse($"{sum}".Substring($"{sum}".Length - 3, 3));
        var m1 = suffix / 34;
        var m2 = suffix % 34;

        var rfc_12 = new StringBuilder(simple);
        rfc_12.Append(hom(m1));
        rfc_12.Append(hom(m2));
        var rfc12 = $"{rfc_12}";

        int sumdigv = 0;
        for (int k = 0; k < rfc12.Length; ++k)
        {
            sumdigv += (digv(rfc12[k]) * (13 - k));
        }

        var verif = sumdigv % 11;
        return rfc12 + checksum(verif);
    }
}