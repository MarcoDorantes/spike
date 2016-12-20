using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;

namespace expressionTree_specs
{
  class B
  {
    public int n;
    public B(int d) { n = d;/*WriteLine($"B.B({d})");*/}
    public override string ToString() => $"{n}";
  }
  static class A
  {
    public static void f1() { Trace.WriteLine("A.f1()"); }
    public static int f2() => 144;
    public static int g1(this B b) => b.n;
  }

  [TestClass]
  public class expressionTree_spec
  {
    static IEnumerable<IEnumerable<KeyValuePair<int, string>>> filter1(IEnumerable<List<KeyValuePair<int, string>>> msgs)
    {
      return msgs.Where(msg => msg.Any());
    }
    static IEnumerable<IEnumerable<KeyValuePair<int, string>>> filter1(IEnumerable<List<KeyValuePair<int, string>>> msgs, Func<IEnumerable<KeyValuePair<int,string>>,bool> predicate)
    {
      return msgs.Where(predicate);
    }
    #region tries
    /*static IEnumerable<IEnumerable<KeyValuePair<int, string>>> filter2(IEnumerable<List<KeyValuePair<int, string>>> msgs)
    {
      IQueryable<IEnumerable<KeyValuePair<int, string>>> Q = msgs.AsQueryable<IEnumerable<KeyValuePair<int, string>>>();
      if (Q == null) throw new Exception("AsQueryable returned null");

      ParameterExpression msg = Expression.Parameter(typeof(IEnumerable<KeyValuePair<int, string>>), "msg");
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 1).MakeGenericMethod(typeof(KeyValuePair<int, string>));
      Expression filter_expression = Expression.Call(any_method, msg);

      var selection = Expression.Lambda<Func<IEnumerable<KeyValuePair<int, string>>, bool>>(filter_expression, new ParameterExpression[] { msg });
      MethodCallExpression where_call = Expression.Call(
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        selection);

      return Q.Provider.CreateQuery<IEnumerable<KeyValuePair<int, string>>>(where_call);
    }*/
    /*static IEnumerable<IEnumerable<KeyValuePair<int, string>>> filter2(IEnumerable<List<KeyValuePair<int, string>>> msgs, string filter_config)
    {
      IQueryable<IEnumerable<KeyValuePair<int, string>>> Q = msgs.AsQueryable<IEnumerable<KeyValuePair<int, string>>>();
      if (Q == null) throw new Exception("AsQueryable returned null");

      //new Func<IEnumerable<KeyValuePair<int, string>>, bool>(msg => msg.Any(pair => pair.Key == 35 && pair.Value == "8"))
      ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");
      ParameterExpression msg = Expression.Parameter(typeof(IEnumerable<KeyValuePair<int, string>>), "msg");
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(KeyValuePair<int, string>));

      Expression key_left = Expression.Property(pair, typeof(KeyValuePair<int,string>).GetProperty("Key"));
      Expression key_right = Expression.Constant(35, typeof(int));
      Expression key_expr = Expression.Equal(key_left, key_right);
      Expression value_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Value"));
      Expression value_right = Expression.Constant("8", typeof(string));
      Expression value_expr = Expression.Equal(value_left, value_right);

      var pair_filter_and = Expression.And(key_expr, value_expr);
      var pair_filter = Expression.Lambda<Func<KeyValuePair<int,string>, bool>>(pair_filter_and, new ParameterExpression[] { pair });
      Expression filter_expression = Expression.Call(any_method, msg, pair_filter);
      var selection = Expression.Lambda<Func<IEnumerable<KeyValuePair<int, string>>, bool>>(filter_expression, new ParameterExpression[] { msg });
      MethodCallExpression where_call = Expression.Call(
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        selection);

      return Q.Provider.CreateQuery<IEnumerable<KeyValuePair<int, string>>>(where_call);
    }*/
    /*static IEnumerable<IEnumerable<KeyValuePair<int, string>>> filter3(IEnumerable<List<KeyValuePair<int, string>>> msgs, string filter_config)
    {
      IQueryable<IEnumerable<KeyValuePair<int, string>>> Q = msgs.AsQueryable<IEnumerable<KeyValuePair<int, string>>>();
      if (Q == null) throw new Exception("AsQueryable returned null");

      //new Func<IEnumerable<KeyValuePair<int, string>>, bool>(msg => msg.Any(pair => pair.Key == 35 && pair.Value == "j"))
      ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");
      ParameterExpression msg = Expression.Parameter(typeof(IEnumerable<KeyValuePair<int, string>>), "msg");
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(KeyValuePair<int, string>));

      Expression key_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Key"));
      Expression key_right = Expression.Constant(35, typeof(int));
      Expression key_expr = Expression.Equal(key_left, key_right);
      Expression value_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Value"));
      Expression value_right = Expression.Constant("j", typeof(string));
      Expression value_expr = Expression.Equal(value_left, value_right);
      var pair_filter_and = Expression.And(key_expr, value_expr);

      var pair_filter = Expression.Lambda<Func<KeyValuePair<int, string>, bool>>(pair_filter_and, new ParameterExpression[] { pair });
      Expression filter_expression = Expression.Call(any_method, msg, pair_filter);
      var selection = Expression.Lambda<Func<IEnumerable<KeyValuePair<int, string>>, bool>>(filter_expression, new ParameterExpression[] { msg });
      MethodCallExpression where_call = Expression.Call(
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        selection);

      return Q.Provider.CreateQuery<IEnumerable<KeyValuePair<int, string>>>(where_call);
    }*/
    #endregion
    static Expression parse(string input)
    {
      return null;
    }
    static IEnumerable<IEnumerable<KeyValuePair<int, string>>> filter4(IEnumerable<List<KeyValuePair<int, string>>> msgs, string filter_config = null)
    {
      IQueryable<IEnumerable<KeyValuePair<int, string>>> Q = msgs.AsQueryable<IEnumerable<KeyValuePair<int, string>>>();

      ParameterExpression msg = Expression.Parameter(typeof(IEnumerable<KeyValuePair<int, string>>), "msg");

      Expression filter_expression = null;
      if (string.IsNullOrWhiteSpace(filter_config) == true)
      {
        var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 1).MakeGenericMethod(typeof(KeyValuePair<int, string>));
        filter_expression = Expression.Call(any_method, msg);
      }
      else
      {
        var filter_tags = filter_config.Split('=');
        int filter_tag = int.Parse(filter_tags[0]);
        string filter_value = filter_tags[1];

        ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");

        Expression key_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Key"));
        Expression key_right = Expression.Constant(filter_tag, typeof(int));
        Expression key_expr = Expression.Equal(key_left, key_right);
        Expression value_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Value"));
        Expression value_right = Expression.Constant(filter_value, typeof(string));
        Expression value_expr = Expression.Equal(value_left, value_right);
        var pair_filter_and = Expression.And(key_expr, value_expr);
        var pair_filter_subtree = pair_filter_and;
        var pair_filter = Expression.Lambda<Func<KeyValuePair<int, string>, bool>>(pair_filter_subtree, new ParameterExpression[] { pair });

        var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(KeyValuePair<int, string>));
        filter_expression = Expression.Call(any_method, msg, pair_filter);
      }

      var selection = Expression.Lambda<Func<IEnumerable<KeyValuePair<int, string>>, bool>>(filter_expression, new ParameterExpression[] { msg });
      MethodCallExpression where_call = Expression.Call
      (
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        selection
      );
      return Q.Provider.CreateQuery<IEnumerable<KeyValuePair<int, string>>>(where_call);
    }

    [TestMethod]
    public void call_static_Ia()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var output=filter1(L).Aggregate(new StringBuilder(),(whole,msg)=>whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,j) (55,WALMEX V) | (35,j) (55,AMX L) | (35,8) (55,X) | ", output.ToString());
    }
    [TestMethod]
    public void call_static_Ib()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = filter1(L, new Func<IEnumerable<KeyValuePair<int, string>>, bool>(msg => msg.Any(pair => pair.Key == 35 && pair.Value == "8")));
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,8) (55,X) | ", output.ToString());
    }
    [TestMethod]
    public void call_static_IIa()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var output = filter4(L).Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,j) (55,WALMEX V) | (35,j) (55,AMX L) | (35,8) (55,X) | ", output.ToString());
    }
    [TestMethod]
    public void call_static_IIb()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = filter4(L, "35=8");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,8) (55,X) | ", output.ToString());
    }
    [TestMethod]
    public void call_static_IIc()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = filter4(L, "35=j");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<string>("(35,j) (55,WALMEX V) | (35,j) (55,AMX L) | ", output.ToString());
    }
    [TestMethod]
    public void call_static_00a()
    {
      Expression<Func<int, int, int>> e = (a, b) => a + b;
      var f = e.Compile();
      var output = $"{f(13, 2)}";
      Assert.AreEqual<string>("15", output);
    }
    [TestMethod]
    public void call_static_00b()
    {
      Expression<Func<int, B>> e = n => new B(n);
      var f = e.Compile();
      var result = f(132);
      var output = $"{result}";
      Assert.AreEqual<string>("132", output);
    }
    [TestMethod]
    public void call_static_00c()
    {
      var E = new List<B> { new B(0), new B(5), new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      Expression<Func<int, B>> e = n => new B(n);
      InvocationExpression invoke = Expression.Invoke(e, Expression.Constant(132));
      var result = Q.Provider.Execute(invoke);
      var output = $"{result}";

      Assert.AreEqual<string>("132", output);
    }
    [TestMethod]
    public void call_static_01()
    {
      var E = new List<B> { new B(0), new B(5), new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      MethodCallExpression c = Expression.Call(typeof(A).GetMethod("f1"));
      LambdaExpression l = Expression.Lambda(c);
      var result = Q.Provider.Execute(l);
      ((Action)result).Invoke();
      var output = $"{result}";

      Assert.AreEqual<string>("System.Action", output);
    }
    [TestMethod]
    public void call_static_02()
    {
      MethodCallExpression c = Expression.Call(typeof(A).GetMethod("f1"));
      LambdaExpression l = Expression.Lambda(c);
      var f = l.Compile();
      ((Action)f).Invoke();

      var output = $"{f}";
      Assert.AreEqual<string>("System.Action", output);
    }
    [TestMethod]
    public void call_static_03()
    {
      MethodCallExpression c = Expression.Call(typeof(A).GetMethod("f2"));
      LambdaExpression l = Expression.Lambda(c);
      var f = l.Compile();
      var r = ((Func<int>)f).Invoke();

      var output = $"{r}";
      Assert.AreEqual<string>("144", output);
    }
    [TestMethod]
    public void call_static_04()
    {
      //B b=new B(12); WriteLine(b.g1());

      ParameterExpression Btype = Expression.Parameter(typeof(B));
      MethodCallExpression c = Expression.Call(typeof(A).GetMethod("g1"), Btype);
      LambdaExpression l = Expression.Lambda<Func<B, int>>(c, new ParameterExpression[] { Btype });
      var f = l.Compile();
      var r = ((Func<B, int>)f).Invoke(new B(321));

      var output = $"{r}";
      Assert.AreEqual<string>("321", output);
    }
    [TestMethod]
    public void call_static_05()
    {
      var E = new List<B> { new B(0), new B(5), new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      //WriteLine($"{E.Count()}");

      //var m=typeof(Enumerable).GetMethods().Where(n=>n.Name=="Count").ElementAt(0).MakeGenericMethod(typeof(B));
      //WriteLine(m);
      //WriteLine(m.Invoke(null,new object[]{E}));return;

      //var fn=new Func<IEnumerable<B>,int>(t=>t.Count());
      //int r=fn(E);
      //WriteLine(r);return;
      ParameterExpression Btypes = Expression.Parameter(typeof(IEnumerable<B>));
      var count_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Count" && m.GetParameters().Count() == 1).MakeGenericMethod(typeof(B));
      MethodCallExpression c = Expression.Call(count_method, Btypes);
      LambdaExpression l = Expression.Lambda<Func<IEnumerable<B>, int>>(c, new ParameterExpression[] { Btypes });

      var f = l.Compile();

      //var f=Q.Provider.Execute(l);

      var result = ((Func<IEnumerable<B>, int>)f).Invoke(E);

      var output = $"{result}";
      Assert.AreEqual<string>("3", output);
    }
    [TestMethod]
    public void call_static_06()
    {
      var E = new List<B> { new B(0), new B(5), new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      //WriteLine($"{E.Any()}");

      ParameterExpression Btypes = Expression.Parameter(typeof(IEnumerable<B>));
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 1).MakeGenericMethod(typeof(B));
      MethodCallExpression c =Expression.Call(any_method, Btypes);
      LambdaExpression l=Expression.Lambda<Func<IEnumerable<B>,bool>>(c,new ParameterExpression[] {Btypes});
      var f=l.Compile();
      var result=((Func<IEnumerable<B>,bool>)f).Invoke(E);

      var output = $"{result}";
      Assert.AreEqual<string>("True", output);
    }
    [TestMethod]
    public void call_static_07()
    {
      var E = new List<B> { new B(0), new B(5), new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      ParameterExpression Btypes = Expression.Parameter(typeof(IQueryable<B>));
      var any_method = typeof(Queryable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 1).MakeGenericMethod(typeof(B));
      MethodCallExpression c = Expression.Call(any_method, Btypes);
      LambdaExpression l = Expression.Lambda<Func<IQueryable<B>, bool>>(c, new ParameterExpression[] { Btypes });
      var f = l.Compile();
      var result = ((Func<IQueryable<B>, bool>)f).Invoke(Q);

      var output = $"{result}";
      Assert.AreEqual<string>("True", output);
    }
    [TestMethod]
    public void call_static_07a()
    {
      var any_methods_1 = typeof(Enumerable).GetMethods().Where(m => m.Name.Contains("Any")).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}({1}:{2})|", n.Name, n.GetParameters().Length, n.GetParameters().Aggregate(new StringBuilder(), (w1, n1) => w1.AppendFormat("{0},", n1.ParameterType.Name))));
      var any_methods_2 = typeof(Queryable).GetMethods().Where(m => m.Name.Contains("Any")).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}({1}:{2})|", n.Name, n.GetParameters().Length,n.GetParameters().Aggregate(new StringBuilder(),(w1,n1)=>w1.AppendFormat("{0},",n1.ParameterType.Name))));
      Assert.AreEqual<string>("Any(1:IEnumerable`1,)|Any(2:IEnumerable`1,Func`2,)|", any_methods_1.ToString());
      Assert.AreEqual<string>("Any(1:IQueryable`1,)|Any(2:IQueryable`1,Expression`1,)|", any_methods_2.ToString());
    }
    [TestMethod]
    public void call_static_08()
    {
      var E = new List<B> { new B(0), new B(5), new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      ParameterExpression b = Expression.Parameter(typeof(B), "b");
      Expression left = b;
      Expression right = Expression.Constant(null);
      Expression expr = Expression.NotEqual(left, right);
      var selection= Expression.Lambda<Func<B, bool>>(expr, new ParameterExpression[] { b });
      var q = Q.Where(selection);
      var result = q.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n)).ToString();

      var output = $"{result}";
      Assert.AreEqual<string>("0|5|125|", output);
    }
    [TestMethod]
    public void call_static_09()
    {
      var E = new List<B> { new B(0), new B(5), null, new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      ParameterExpression b = Expression.Parameter(typeof(B), "b");
      Expression left = b;
      Expression right = Expression.Constant(null);
      Expression expr = Expression.NotEqual(left, right);
      var selection = Expression.Lambda<Func<B, bool>>(expr, new ParameterExpression[] { b });

      MethodCallExpression where_expression = Expression.Call(
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        selection);

      var q = Q.Provider.CreateQuery<B>(where_expression);
      var result = q.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n)).ToString();

      var output = $"{result}";
      Assert.AreEqual<string>("0|5|125|", output);
    }
    [TestMethod]
    public void call_static_10()
    {
      var E = new List<List<B>> { new List<B> { new B(0), new B(5), new B(125) } };
      IQueryable<IEnumerable<B>> Q = E.AsQueryable<IEnumerable<B>>();

      ParameterExpression bb = Expression.Parameter(typeof(IEnumerable<B>), "bb");
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 1).MakeGenericMethod(typeof(B));
      MethodCallExpression c = Expression.Call(any_method, bb);
      var selection = Expression.Lambda<Func<IEnumerable<B>, bool>>(c, new ParameterExpression[] { bb });

      MethodCallExpression where_expression = Expression.Call(
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        selection);

      var q = Q.Provider.CreateQuery<IEnumerable<B>>(where_expression);
      var result = q.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n.Count())).ToString();

      var output = $"{result}";
      Assert.AreEqual<string>("3|", output);
    }
    [TestMethod]
    public void call_static_11()
    {
      var E = new List<List<B>>
      {
        new List<B> { new B(0), new B(5), new B(125) },
        new List<B> { new B(1), new B(6), new B(126) },
        new List<B> { new B(2), new B(5), new B(127) }
      };
      IQueryable<IEnumerable<B>> Q = E.AsQueryable<IEnumerable<B>>();

      ParameterExpression b = Expression.Parameter(typeof(B), "b");
      ParameterExpression bb = Expression.Parameter(typeof(IEnumerable<B>), "bb");
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(B));

      Expression left = Expression.Field(b, typeof(B).GetField("n"));
      Expression right = Expression.Constant(5, typeof(int));
      Expression expr = Expression.Equal(left, right);
      var b_filter = Expression.Lambda<Func<B, bool>>(expr, new ParameterExpression[] { b });

      MethodCallExpression c = Expression.Call(any_method, bb, b_filter);
      var selection = Expression.Lambda<Func<IEnumerable<B>, bool>>(c, new ParameterExpression[] { bb });

      MethodCallExpression where_expression = Expression.Call(
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        selection);

      var q = Q.Provider.CreateQuery<IEnumerable<B>>(where_expression);
      var result = q.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n.Count())).ToString();

      var output = $"{result}";
      Assert.AreEqual<string>("3|3|", output);
    }
    [TestMethod]
    public void parse_expr1()
    {
      var input = "35=8";
      Expression exp = parse(input);
      Assert.IsNotNull(exp);
    }
  }
}
/*
https://msdn.microsoft.com/en-us/library/bb882637(v=vs.100).aspx
https://msdn.microsoft.com/en-us/library/mt654263.aspx
https://blogs.msdn.microsoft.com/charlie/2008/01/31/expression-tree-basics
	https://code.msdn.microsoft.com/csharpsamples
https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression.call(v=vs.110).aspx
http://stackoverflow.com/questions/7857451/error-static-method-requires-null-instance-non-static-method-requires-non-null
https://msdn.microsoft.com/en-us/library/system.linq.expressions.blockexpression(v=vs.110).aspx

is a generic method definition at System.Linq.Expressions.Expression.ValidateMethodInfo(MethodInfo method)
http://stackoverflow.com/questions/25845477/expression-call-with-any-method-throws-exception
http://stackoverflow.com/questions/25884233/how-to-use-expressions-to-invoke-a-method-call-with-a-generic-list-as-the-parame
http://stackoverflow.com/questions/31063891/enumerable-select-by-expression-tree

Static method requires null instance, non-static method requires non-null instance.
Enumerable.Any(this) static
Enumerable.Where(this, lambda) static
*/