using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;

namespace expressionTree_specs//UnitTestProject1
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
    static IEnumerable<List<KeyValuePair<int, string>>> filter1(IEnumerable<List<KeyValuePair<int, string>>> msgs)
    {
      return msgs.Where(msg => msg.Any());
      //return msgs.Where(msg=>msg.Any(pair=>pair.Key==35) && msg.First(pair=>pair.Key==35).Value=="j");
    }
    static IEnumerable<List<KeyValuePair<int, string>>> filter2(IEnumerable<List<KeyValuePair<int, string>>> msgs)
    {
      IQueryable<List<KeyValuePair<int, string>>> Q = msgs.AsQueryable<List<KeyValuePair<int, string>>>();
      if (Q == null) throw new Exception("AsQueryable returned null");

      ParameterExpression msg = Expression.Parameter(typeof(List<KeyValuePair<int, string>>), "msg");
      //ParameterExpression msgs_parameter = Expression.Parameter(typeof(IQueryable<List<KeyValuePair<int,string>>>), "msgs_parameter");
      Expression filter_expression = Expression.Call(typeof(Queryable).GetMethods().Where(n => n.Name == "Any").ElementAt(0).MakeGenericMethod(typeof(List<KeyValuePair<int, string>>)), msg);

      MethodCallExpression where_call = Expression.Call(
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType }, //new Type[] { typeof(List<KeyValuePair<int,string>>) },
        Q.Expression,
        Expression.Lambda<Func<List<KeyValuePair<int, string>>, bool>>(filter_expression, new ParameterExpression[] { msg }));

      return Q.Provider.CreateQuery<List<KeyValuePair<int, string>>>(where_call);
    }
    [TestMethod]
    public void call_static_I()
    {
      //var t=new List<string>{"uno"};
      //Console.WriteLine(t.Any());return;

      //foreach(var m in typeof(List<string>).GetMethods()) Console.Write($"{m.Name}, ");return;
      //foreach(var m in typeof(Enumerable).GetMethods().Where(n=>n.Name=="Any")) Console.WriteLine($"{m}");return;

      //Console.WriteLine("System.Collections.Generic.IEnumerable`1[TSource]");
      //Console.WriteLine(typeof(IEnumerable<>).FullName);return;
      //Console.WriteLine(Type.GetType("System.Collections.Generic.IEnumerable`1[]").FullName);return;

      //var x=typeof(Enumerable).GetMethod("Any", new Type[]{typeof(IEnumerable<>)});//, System.Type.EmptyTypes
      /*var x=typeof(Enumerable).GetMethods().Where(n=>n.Name=="Any").ElementAt(0).MakeGenericMethod(typeof(string));
      if(x==null) Console.WriteLine("null");else Console.WriteLine(x.Name);
      return;*/
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      //foreach (var msg in filter1(L)) Trace.WriteLine(msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value)));
      var output=filter1(L).Aggregate(new StringBuilder(),(whole,msg)=>whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,j) (55,WALMEX V) | (35,j) (55,AMX L) | (35,8) (55,X) | ", output.ToString());
    }
    [TestMethod]
    public void call_static_II()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var output = filter2(L).Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,j) (55,WALMEX V) | (35,j) (55,AMX L) | (35,8) (55,X) | ", output.ToString());
    }
    [TestMethod]
    public void call_static_0a()
    {
      Expression<Func<int, int, int>> e = (a, b) => a + b;
      var f = e.Compile();
      var output = $"{f(13, 2)}";
      Assert.AreEqual<string>("15", output);
    }
    [TestMethod]
    public void call_static_0b()
    {
      Expression<Func<int, B>> e = n => new B(n);
      var f = e.Compile();
      var result = f(132);
      var output = $"{result}";
      Assert.AreEqual<string>("132", output);
    }
    [TestMethod]
    public void call_static_0c()
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
    public void call_static_1()
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
    public void call_static_2()
    {
      MethodCallExpression c = Expression.Call(typeof(A).GetMethod("f1"));
      LambdaExpression l = Expression.Lambda(c);
      var f = l.Compile();
      ((Action)f).Invoke();

      var output = $"{f}";
      Assert.AreEqual<string>("System.Action", output);
    }
    [TestMethod]
    public void call_static_3()
    {
      MethodCallExpression c = Expression.Call(typeof(A).GetMethod("f2"));
      LambdaExpression l = Expression.Lambda(c);
      var f = l.Compile();
      var r = ((Func<int>)f).Invoke();

      var output = $"{r}";
      Assert.AreEqual<string>("144", output);
    }
    [TestMethod]
    public void call_static_4()
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
    public void call_static_5()
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
      MethodCallExpression c = Expression.Call(typeof(Enumerable).GetMethods().Where(n => n.Name == "Count").ElementAt(0).MakeGenericMethod(typeof(B)), Btypes);
      LambdaExpression l = Expression.Lambda<Func<IEnumerable<B>, int>>(c, new ParameterExpression[] { Btypes });

      var f = l.Compile();

      //var f=Q.Provider.Execute(l);

      var result = ((Func<IEnumerable<B>, int>)f).Invoke(E);

      var output = $"{result}";
      Assert.AreEqual<string>("3", output);
    }
    [TestMethod]
    public void call_static_6()
    {
      var E = new List<B> { new B(0), new B(5), new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      //WriteLine($"{E.Any()}");

      ParameterExpression Btypes = Expression.Parameter(typeof(IEnumerable<B>));
      MethodCallExpression c=Expression.Call(typeof(Enumerable).GetMethods().Where(n=>n.Name=="Any").ElementAt(0).MakeGenericMethod(typeof(B)), Btypes);
      LambdaExpression l=Expression.Lambda<Func<IEnumerable<B>,bool>>(c,new ParameterExpression[] {Btypes});
      var f=l.Compile();
      var result=((Func<IEnumerable<B>,bool>)f).Invoke(E);

      var output = $"{result}";
      Assert.AreEqual<string>("True", output);
    }
    [TestMethod]
    public void call_static_7()
    {
      var E = new List<B> { new B(0), new B(5), new B(125) };
      IQueryable<B> Q = E.AsQueryable<B>();

      ParameterExpression Btypes = Expression.Parameter(typeof(IQueryable<B>));
      MethodCallExpression c = Expression.Call(typeof(Queryable).GetMethods().Where(n => n.Name == "Any").ElementAt(0).MakeGenericMethod(typeof(B)), Btypes);
      LambdaExpression l = Expression.Lambda<Func<IQueryable<B>, bool>>(c, new ParameterExpression[] { Btypes });
      var f = l.Compile();
      var result = ((Func<IQueryable<B>, bool>)f).Invoke(Q);

      var output = $"{result}";
      Assert.AreEqual<string>("True", output);
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