using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

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
    static IEnumerable<IEnumerable<KeyValuePair<int, string>>> filter_by_simple_WhereAny(IEnumerable<List<KeyValuePair<int, string>>> msgs)
    {
      return msgs.Where(msg => msg.Any());
    }
    static IEnumerable<IEnumerable<KeyValuePair<int, string>>> filter_by_WherePredicate(IEnumerable<List<KeyValuePair<int, string>>> msgs, Func<IEnumerable<KeyValuePair<int, string>>, bool> predicate)
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

    #region Filter by LINQ Expression Trees
    public class Tree<K, V> : Dictionary<K, Tree<K, V>>
    {
      public V Value { get; set; }
      public override string ToString()
      {
        string value = Value?.ToString();
        if (value == null)
        {
          value = "";
        }
        var result = new System.Xml.Linq.XDocument(new System.Xml.Linq.XElement("tree", new System.Xml.Linq.XAttribute("value", value)));
        this.Aggregate(result, (whole, next) =>
        {
          var subtree = new System.Xml.Linq.XElement("tree", new System.Xml.Linq.XAttribute("key", next.Key));
          subtree.Add(System.Xml.Linq.XElement.Parse(next.Value.ToString()));
          whole.Root.Add(subtree);
          return whole; });
        return result.ToString();
      }
    }
    public class Tree<V> : HashSet<Tree<V>>
    {
      public V Value { get; set; }
      public override string ToString()
      {
        string value = Value?.ToString();
        if (value == null)
        {
          value = "";
        }
        var result = new System.Xml.Linq.XDocument(new System.Xml.Linq.XElement("tree", new System.Xml.Linq.XAttribute("value", value)));
        this.Aggregate(result, (whole, next) =>
        {
          var xml = next?.ToString();
          if (!string.IsNullOrWhiteSpace(xml)) whole.Root.Add(System.Xml.Linq.XElement.Parse(xml));
          return whole;
        });
        return result.ToString();
      }
    }
    static IEnumerable<string> get_expression_tokens(string input)
    {
      //return get_expression_tokens_regex(input);
      return get_expression_tokens_alt1(input);
    }
    static IEnumerable<string> get_expression_tokens_regex(string input)
    {
      if (input == null)
      {
        throw new ArgumentNullException(nameof(input));
      }
      /*
       35=8
       35=j
       35=8 AND 35=j
       35=8 OR 35=j
       35=8 OR 421=3 AND 34=12
       35=8 OR (421=3 AND 34=12)
       35=8 AND 421=3 OR 34=12
       35=8 AND (421=3 OR 34=12)
       35=8 AND 55=AMX L AND 34=12
       (34>4566 AND 34<5000) AND 35=8 AND (39=1 OR 39=2)

      12=A123 55=1
      (?<pair>(?<tag>\d+)=(?<val>\w+))+

      (?<pair>(?<tag>\d+)=(?<right>(?<value>[_0-9A-Za-z]+))(?<sep>&|))+

      12=A123 OR 55=1 AND 34=123
      (?<expr>((?<pair>(?<tag>\d+)=(?<val>\w+))\s?(?<op>AND|OR)?))+
      http://regexstorm.net/tester
      */
      var regex = new Regex(@"(?<pair>(?<tag>\d+)=(?<val>\w+))|(?<op>AND|OR)");
      foreach (Match match in regex.Matches(input))
      {
        var token = match.Value?.Trim();
        if (string.IsNullOrWhiteSpace(token) == false)
        {
          yield return token;
        }
      }
    }
    static IEnumerable<string> get_expression_tokens_alt1(string input, IEnumerable<string> operators = null)
    {
      if (operators == null)
      {
        operators = new string[] { "AND", "OR" };
      }

      var tokens = get_tokens_alt1(input).ToArray();
      int k = 0;
      while (k < tokens.Length)
      {
        KeyValuePair<Token, string> token = tokens[k];
        switch (token.Key)
        {
          case Token.Text:
            if (operators.Any(op => op == token.Value))
            {
              yield return token.Value;
              ++k;
            }
            else
            {
              if (k + 2 < tokens.Length && tokens[k+1].Key == Token.Equals && tokens[k + 2].Key == Token.Text)
              {
                yield return token.Value + tokens[++k].Value + tokens[++k].Value;
                ++k;
              }
              else throw new Exception($"Bad syntax starting at: {token.Value}");
            }
            break;

          default:
            throw new Exception($"Bad syntax starting at: {token.Value}");
        }
      }
    }
    enum Token { Text, Equals, LeftParenthesis, RightParenthesis, LessThan, GreaterThan }
    static IEnumerable<KeyValuePair<Token,string>> get_tokens_alt1(string input)
    {
      int k = 0;
      while (k < input.Length)
      {
        char c = input[k];
        if (char.IsLetterOrDigit(c))
        {
          yield return new KeyValuePair<Token, string>(Token.Text, input.Substring(k, count(input, ref k, next => char.IsLetterOrDigit(next))));
        }
        else if (char.IsSeparator(c))
        {
          ++k;
        }
        else if (c == '=')
        {
          ++k;
          yield return new KeyValuePair<Token, string>(Token.Equals, new string(c, 1));
        }
        else if (c == '(')
        {
          ++k;
          yield return new KeyValuePair<Token, string>(Token.LeftParenthesis, new string(c, 1));
        }
        else if (c == ')')
        {
          ++k;
          yield return new KeyValuePair<Token, string>(Token.RightParenthesis, new string(c, 1));
        }
        else if (c == '<')
        {
          ++k;
          yield return new KeyValuePair<Token, string>(Token.LessThan, new string(c, 1));
        }
        else if (c == '>')
        {
          ++k;
          yield return new KeyValuePair<Token, string>(Token.GreaterThan, new string(c, 1));
        }
        else throw new Exception($"Bad syntax: {c}");
      }
    }
    static int count(string input, ref int k, Func<char, bool> is_included)
    {
      int length = 1;
      while (++k < input.Length && is_included(input[k])) ++length;
      return length;
    }
    static int IndexOfLogicalOperator(IEnumerable<string> operators, string[] expr_tokens, out string operator_token)
    {
      int result = -1;
      operator_token = null;
      bool found = false;
      string current_token = null;
      int index = -1;
      while (found == false && index + 1 < expr_tokens.Length)
      {
        ++index;
        current_token = expr_tokens[index];
        found = operators.Any(op => op == current_token);
      }
      if (found)
      {
        result = index;
        operator_token = current_token;
      }
      return result;
    }
    static string reverse_sub_expression(string[] expr_tokens, int start_index, int count = 0)
    {
      var result = "";
      if (start_index < 0 || start_index >= expr_tokens.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(start_index));
      }
      if (count < 0 || count > expr_tokens.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(count));
      }
      if (count == 0)
      {
        count = expr_tokens.Length - start_index;
      }
      for (int k = 0; k < count; ++k)
      {
        result = $"{expr_tokens[start_index++]} {result}";
      }
      return result;
    }
    static bool are_parenthesis_balanced(string input)
    {
      int level = 0;
      foreach (char c in input)
      {
        switch (c)
        {
          case '(':
            ++level;
            break;
          case ')':
            --level;
            break;
        }
        if (level < 0)
        {
          return false;
        }
      }
      if (level != 0)
      {
        return false;
      }
      return true;
    }
    static Tree<int, StringBuilder> tree_parse_parenthesis(string input, ref int k)
    {
      var result = new Tree<int, StringBuilder> { Value = new StringBuilder() };
      int current = 0;
      result[current] = new Tree<int, StringBuilder> { Value = new StringBuilder() };

      while (k < input.Length)
      {
        char c = input[k];
        ++k;
        if (c == '(')
        {
          var subtree = tree_parse_parenthesis(input, ref k);
          result[++current] = subtree;
        }
        else if (c == ')')
        {
          break;
        }
        else
        {
          result.Value.Append(c);
        }
      }
      return result;
    }
    static Tree<int, string> trim_nodes(Tree<int, StringBuilder> raw_node)
    {
      var result = new Tree<int, string> { Value = raw_node.Value?.ToString() };
      int index = 0;
      var keys = raw_node.Keys.OrderBy(k => k);
      foreach (int subnode_key in keys)
      {
        Tree<int, StringBuilder> subnode = raw_node[subnode_key];
        string value = subnode.Value?.ToString();
        if ((subnode_key == keys.First() || subnode_key == keys.Last()) && string.IsNullOrWhiteSpace(value) == true)
        {
          continue;
        }
        result[index++] = trim_nodes(subnode);
      }
      return result;
    }
    static Tree<int, string> tree_parse_parenthesis_pass1(string input)
    {
      if (string.IsNullOrWhiteSpace(input))
      {
        throw new ArgumentNullException(nameof(input));
      }
      if (are_parenthesis_balanced(input) == false)
      {
        throw new Exception("Malformed expression. Check your parenthesis.");
      }
      var tree = new Tree<int, StringBuilder> { Value = new StringBuilder() };
      int current = 0;
      tree[current] = new Tree<int, StringBuilder> { Value = new StringBuilder() };

      int k = 0;
      while (k < input.Length)
      {
        char c = input[k];
        ++k;
        switch (c)
        {
          case '(':
            {
              var subtree = tree_parse_parenthesis(input, ref k);
              tree[++current] = subtree;
              tree[++current] = new Tree<int, StringBuilder> { Value = new StringBuilder() };
            }
            break;
          case ')':
            throw new Exception("Bad syntax. Check parenthesis.");
          default:
            tree[current].Value.Append(c);
            break;
        }
      }
      return trim_nodes(tree);
    }
    /*Tree<string> tree_parse_pass2(IEnumerable<string> rexpr_tokens, IEnumerable<string> operators)
    {
      var result = new Tree<string>();
      if (operators.Any(op => op == rexpr_tokens.Last()))
      {
        result.Value = rexpr_tokens.Last();
        result.Add(tree_parse_pass2(null, operators));
      }

      return result;
    }*/
    static Tree<string> tree_parse_parenthesis_pass2(Tree<int, string> pass1, IEnumerable<string> operators = null)
    {
      if (pass1 == null)
      {
        throw new ArgumentNullException(nameof(pass1));
      }
Trace.WriteLine($"{nameof(pass1)}:\n{pass1}");
      if (operators == null)
      {
        operators = new string[] { "AND", "OR" };
      }

      Tree<string> result_head = null;
      Tree<string> pending_node = null;
      int start_index = pass1.Count - 1;
      for (int k = start_index; k >= 0; --k)
      {
        string expr = pass1[k].Value.Trim();
Trace.WriteLine($"\nexpr in turn:[{expr}]\n");
        string[] expr_tokens = get_expression_tokens(expr).ToArray();
        if (expr_tokens.Length == 0)
        {
          throw new Exception("Invalid syntax: No expression.");
        }

        Tree<string> current_node = null;
        //IEnumerable<string> left = null;
        //IEnumerable<string> right = null;

        parse_pass1_node(expr_tokens, operators, ref result_head, ref current_node , ref pending_node);

        //result.Add(tree_parse_parenthesis_pass2(right, operators));
        //result.Add(tree_parse_parenthesis_pass2(left, operators));
        if (current_node != null)
        {
          result_head = current_node;
        }
        //tree_parse_parenthesis_pass2(child);
      }

      //Tree<string> result = tree_parse_pass2(rexpr_tokens, operators);
      //result.Add(tree_parse_parenthesis_pass2(pass1[0]));
      //result.Add(tree_parse_parenthesis_pass2(pass1[1]));
      //result.Add(tree_parse_parenthesis_pass2(pass1[2]));

      /*ref pattern:
        if (operators.Any(op => op == rexpr_tokens.First()) || operators.Any(op => op == rexpr_tokens.Last()))
        {
          throw new Exception("Invalid syntax: malformed logical operator.");
        }

        var result = new Tree<string>();
        string operator_token = null;
        int operator_index = IndexOfLogicalOperator(operators, rexpr_tokens, out operator_token);
        if (operator_index >= 0)
        {
          result.Value = operator_token;
          result.Add(tree_parse(reverse_sub_expression(rexpr_tokens, operator_index + 1)));
          result.Add(tree_parse(reverse_sub_expression(rexpr_tokens, 0, operator_index)));
        }
        else if (rexpr_tokens.Length == 1)
        {
          result.Value = rexpr_tokens[0];
        }
        else throw new Exception("Invalid syntax.");
      */
      if (pending_node != null)
      {
        throw new ArgumentException("Bad syntax.", $"{pass1}");
      }
      return result_head;
    }
    static void parse_pass1_node_with_suffix_operator(string[] expr_tokens, IEnumerable<string> operators, ref Tree<string> result_head, ref Tree<string> current_node, ref Tree<string> pending_node)
    {
      int taken = expr_tokens.Count() - 1;
      if (taken > 0)
      {
        if (operators.Any(op => op == expr_tokens.First()))
        {
          pending_node = new Tree<string> { Value = expr_tokens.Last() };
          pending_node.Add(new Tree<string> { Value = "<pending>" });
          var node = new Tree<string> { Value = expr_tokens.First() };
          node.Add(tree_parse(expr_tokens.Where((token, index) => index > 0 && index < expr_tokens.Length - 1).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim(), operators));
          if (node.Add(result_head) == false)
          {
            throw new Exception($"Node {result_head} has been already added to the tree: {node}");
          }
          result_head = null;
          pending_node.Add(node);
Trace.WriteLine($"\nexpr in turn: pending_node:{pending_node}\ncurrent_node:{current_node}");
        }
        else
        {
          current_node = new Tree<string> { Value = expr_tokens.Last() };
          current_node.Add(tree_parse(expr_tokens.Take(taken).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim(), operators));
          //current_node.Add(tree_parse_parenthesis_pass2(expr_tokens.Take(taken).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim())));
          if (current_node.Add(result_head) == false)
          {
            throw new Exception($"Node {result_head} has been already added to the tree: {current_node}");
          }
Trace.WriteLine($"\nexpr in turn: taken:{taken}\n");
        }
      }
      else
      {
        pending_node = new Tree<string> { Value = expr_tokens.First() };
        pending_node.Add(new Tree<string> { Value = "<pending>" });
        if (pending_node.Add(result_head) == false)
        {
          throw new Exception($"Node {result_head} has been already added to the tree: {pending_node}");
        }
Trace.WriteLine($"\nexpr in turn: pending_node:{pending_node}\ncurrent_node:{current_node}");
      }
    }
    static void parse_pass1_node_with_prefix_operator(string[] expr_tokens, IEnumerable<string> operators, ref Tree<string> result_head, ref Tree<string> current_node, ref Tree<string> pending_node)
    {
      int taken = expr_tokens.Count() - 1;
      if (taken > 0)
      {
        pending_node = new Tree<string> { Value = expr_tokens.First() };
        pending_node.Add(new Tree<string> { Value = "<pending>" });
        pending_node.Add(tree_parse(expr_tokens.SkipWhile((_, index) => index == 0).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim(), operators));
        //ForAll: result_head == null?
Trace.WriteLine($"\nexpr in turn: pending_node:{pending_node}\ncurrent_node:{current_node}");
      }
      else
      {
        pending_node = new Tree<string> { Value = expr_tokens.First() };
        pending_node.Add(new Tree<string> { Value = "<pending>" });
        if (pending_node.Add(result_head) == false)
        {
          throw new Exception($"Node {result_head} has been already added to the tree: {pending_node}");
        }
Trace.WriteLine($"\nexpr in turn: pending_node:{pending_node}\ncurrent_node:{current_node}");
      }
    }
    static void parse_pass1_standalone_node(string[] expr_tokens, IEnumerable<string> operators, ref Tree<string> result_head, ref Tree<string> current_node, ref Tree<string> pending_node)
    {
      var current_expression_node = tree_parse(expr_tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim(), operators);
      if (pending_node != null)
      {
        if (result_head != null)
        {
          current_node = new Tree<string> { Value = pending_node.Value };
          current_node.Add(current_expression_node);
          if (current_node.Add(result_head) == false)
          {
            throw new Exception($"Node {result_head} has been already added to the tree: {current_node}");
          }
Trace.WriteLine($"\nexpr in turn: pending_node processed (result_head added):{current_node}\n");
        }
        else
        {
          current_node = new Tree<string> { Value = pending_node.Value };
          current_node.Add(current_expression_node);
          if (current_node.Add(pending_node.ElementAt(1)) == false)
          {
            throw new Exception($"Node {pending_node.ElementAt(1)} has been already added to the tree: {current_node}");
          }
Trace.WriteLine($"\nexpr in turn: pending_node processed (result_head is null):{current_node}\n");
        }
        pending_node = null;
      }
      else
      {
        current_node = current_expression_node;
Trace.WriteLine($"\nexpr in turn: current_expression_node processed:{current_node}\n");
      }
    }
    static void parse_pass1_node(string[] expr_tokens, IEnumerable<string> operators, ref Tree<string> result_head, ref Tree<string> current_node, ref Tree<string> pending_node)
    {
      if (operators.Any(op => op == expr_tokens.Last()))
      {
        parse_pass1_node_with_suffix_operator(expr_tokens, operators, ref result_head, ref current_node, ref pending_node);
      }
      else if (operators.Any(op => op == expr_tokens.First()))
      {
        parse_pass1_node_with_prefix_operator(expr_tokens, operators, ref result_head, ref current_node, ref pending_node);
      }
      else //no operators at the head or tail.
      {
        parse_pass1_standalone_node(expr_tokens, operators, ref result_head, ref current_node, ref pending_node);
      }
    }
    static Tree<string> tree_parse(string input, IEnumerable<string> operators = null)
    {
      input = input?.Trim();
      if (string.IsNullOrWhiteSpace(input))
      {
        throw new ArgumentNullException(nameof(input));
      }
      if (operators == null)
      {
        operators = new string[] { "AND", "OR" };
      }

      var rexpr_tokens = get_expression_tokens(input).Reverse().ToArray();
      if (rexpr_tokens.Length == 0)
      {
        throw new Exception("Invalid syntax: No expression.");
      }
      if (operators.Any(op => op == rexpr_tokens.First()) || operators.Any(op => op == rexpr_tokens.Last()))
      {
        throw new Exception("Invalid syntax: malformed logical operator.");
      }

      var result = new Tree<string>();
      string operator_token = null;
      int operator_index = IndexOfLogicalOperator(operators, rexpr_tokens, out operator_token);
      if (operator_index >= 0)
      {
        result.Value = operator_token;
        result.Add(tree_parse(reverse_sub_expression(rexpr_tokens, operator_index + 1), operators));
        result.Add(tree_parse(reverse_sub_expression(rexpr_tokens, 0, operator_index), operators));
      }
      else if (rexpr_tokens.Length == 1)
      {
        result.Value = rexpr_tokens[0];
      }
      else throw new Exception("Invalid syntax.");
      return result;
    }
    static Expression build_filter_expression(Tree<string> t, ParameterExpression pair, ParameterExpression msg)
    {
      if (t.Value == "AND")
      {
        return Expression.And(build_filter_expression(t.ElementAt(0), pair, msg), build_filter_expression(t.ElementAt(1), pair, msg));
      }
      else if (t.Value == "OR")
      {
        return Expression.Or(build_filter_expression(t.ElementAt(0), pair, msg), build_filter_expression(t.ElementAt(1), pair, msg));
      }
      else
      {
        var filter_tags = t.Value.Split('=');
        int filter_tag = int.Parse(filter_tags[0]);
        string filter_value = filter_tags[1];

        var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(KeyValuePair<int, string>));

        Expression key_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Key"));
        Expression key_right = Expression.Constant(filter_tag, typeof(int));
        Expression key_expr = Expression.Equal(key_left, key_right);
        Expression value_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Value"));
        Expression value_right = Expression.Constant(filter_value, typeof(string));
        Expression value_expr = Expression.Equal(value_left, value_right);
        var pair_filter_and = Expression.And(key_expr, value_expr);
        var pair_filter = Expression.Lambda<Func<KeyValuePair<int, string>, bool>>(pair_filter_and, new ParameterExpression[] { pair });
        return Expression.Call(any_method, msg, pair_filter);
      }
    }
    static bool is_binary_tree(Tree<string> tree)
    {
      bool result = false;
      if (tree != null)
      {
        result = tree.Count == 0 || tree.Count == 2;
        var k = tree.GetEnumerator();
        while (result == true && k.MoveNext())
        {
          result = is_binary_tree(k.Current);
        }
      }
      return result;
    }
    public static IEnumerable<IEnumerable<KeyValuePair<int, string>>> dynamic_filter1(IEnumerable<IEnumerable<KeyValuePair<int, string>>> msgs, string filter_config = null)
    {
      IQueryable<IEnumerable<KeyValuePair<int, string>>> Q = msgs.AsQueryable<IEnumerable<KeyValuePair<int, string>>>();

      ParameterExpression msg = Expression.Parameter(typeof(IEnumerable<KeyValuePair<int, string>>), "msg");

      Expression filter_expression = null;
      filter_config = filter_config?.Trim();
      if (string.IsNullOrWhiteSpace(filter_config) == true)
      {
        var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 1).MakeGenericMethod(typeof(KeyValuePair<int, string>));
        filter_expression = Expression.Call(any_method, msg);
      }
      else
      {
        //Tree<string> tree = tree_parse(filter_config);
        //Tree<string> tree_prev = tree_parse(filter_config);

        Tree<string> tree = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1(filter_config));
        //Trace.WriteLine($"\ntree_parse:\n{tree_prev}\n\ntree_parse_parenthesis_pass2:\n{tree}");

        #region old steps
        /*
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
        */

        /*
                var filter_tags = tree.ElementAt(0).Value.Split('=');
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

                var filter_tags_ = tree.ElementAt(1).Value.Split('=');
                int filter_tag_ = int.Parse(filter_tags_[0]);
                string filter_value_ = filter_tags_[1];

                //ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");

                Expression key_left_ = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Key"));
                Expression key_right_ = Expression.Constant(filter_tag_, typeof(int));
                Expression key_expr_ = Expression.Equal(key_left_, key_right_);
                Expression value_left_ = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Value"));
                Expression value_right_ = Expression.Constant(filter_value_, typeof(string));
                Expression value_expr_ = Expression.Equal(value_left_, value_right_);
                var pair_filter_and_ = Expression.And(key_expr_, value_expr_);

                //t.Value == "OR"
                Expression or = Expression.Or(pair_filter_and, pair_filter_and_);

                var pair_filter_subtree = or;
        */
        #endregion

        if (is_binary_tree(tree) == false)
        {
          throw new ArgumentException("Bad syntax.",$"{filter_config}");
        }
        ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");

        var pair_filter_subtree = build_filter_expression(tree, pair, msg);
        var pair_filter = Expression.Lambda<Func<KeyValuePair<int, string>, bool>>(pair_filter_subtree, new ParameterExpression[] { pair });
        var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(KeyValuePair<int, string>));
        filter_expression = Expression.Call(any_method, msg, pair_filter);
      }

      var where_selection = Expression.Lambda<Func<IEnumerable<KeyValuePair<int, string>>, bool>>(filter_expression, new ParameterExpression[] { msg });
Trace.WriteLine($"\n\nwhere_selection:{where_selection}");
      MethodCallExpression where_call = Expression.Call
      (
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        where_selection
      );
      return Q.Provider.CreateQuery<IEnumerable<KeyValuePair<int, string>>>(where_call);
    }
    #endregion

    [TestMethod]
    public void fixed_filter_by_WhereAny()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = filter_by_simple_WhereAny(L);
      var output= filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<int>(4, filtered.Count());
      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,j) (55,WALMEX V) | (35,j) (55,AMX L) | (35,8) (55,X) | ", output.ToString());
    }
    [TestMethod]
    public void fixed_filter_by_fixed_Predicate_1()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") }
      };

      var filtered = filter_by_WherePredicate(L, new Func<IEnumerable<KeyValuePair<int, string>>, bool>(msg => msg.Any(pair => pair.Key == 35 && pair.Value == "8") && msg.Any(pair => pair.Key == 55 && pair.Value == "AMX L")));
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<int>(2, filtered.Count());
      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,8) (55,AMX L) | ", output.ToString());
    }
    [TestMethod]
    public void fixed_filter_by_fixed_Predicate_2()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"9"), new KeyValuePair<int,string>(55,"AMX L") }
      };

      var filtered = filter_by_WherePredicate(L, new Func<IEnumerable<KeyValuePair<int, string>>, bool>(msg => msg.Any(pair => pair.Key == 35 && pair.Value == "8") || msg.Any(pair => pair.Key == 35 && pair.Value == "j")));

      Assert.AreEqual<int>(5, filtered.Count());
    }
    [TestMethod]
    public void dynamic_filter_by_default_Where()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = dynamic_filter1(L);
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<int>(4, filtered.Count());
      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,j) (55,WALMEX V) | (35,j) (55,AMX L) | (35,8) (55,X) | ", output.ToString());
    }
    [TestMethod]
    public void dynamic_filter_by_configured_Where_1()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = dynamic_filter1(L, "35=8");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<int>(2, filtered.Count());
      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,8) (55,X) | ", output.ToString());

      Assert.AreEqual<int>(2, L.Where(LL => LL.First(pair => pair.Key == 35).Value == "8").Count());
      Assert.AreEqual<int>(2, L.Where(LL => LL.Any(pair => pair.Key == 35 && pair.Value == "8")).Count());
    }
    [TestMethod]
    public void dynamic_filter_by_configured_Where_2()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = dynamic_filter1(L, "35=j");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<int>(2, filtered.Count());
      Assert.AreEqual<string>("(35,j) (55,WALMEX V) | (35,j) (55,AMX L) | ", output.ToString());

      //var fixlog = L.Where(LL => LL.First(pair => pair.Key == 56).Value == "GBM10DC1" && LL.First(pair => pair.Key == 35).Value == "8" && LL.First(pair => pair.Key == 39).Value == "2");
      Assert.AreEqual<int>(2, L.Where(LL => LL.First(pair => pair.Key == 35).Value == "j").Count());
      Assert.AreEqual<int>(2, L.Where(LL => LL.Any(pair => pair.Key == 35 && pair.Value == "j")).Count());
    }
    [TestMethod]
    public void dynamic_filter_by_configured_Where_3()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = dynamic_filter1(L, "35=8 AND 35=j");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<int>(0, filtered.Count());
      Assert.AreEqual<string>("", output.ToString());

      Assert.AreEqual<int>(0, L.Where(LL => LL.First(pair => pair.Key == 35).Value == "8" && LL.First(pair => pair.Key == 35).Value == "j").Count());
      Assert.AreEqual<int>(0, L.Where(LL => LL.Any(pair => pair.Key == 35 && pair.Value == "8") && LL.Any(pair => pair.Key == 35 && pair.Value == "j")).Count());
    }
    [TestMethod]
    public void dynamic_filter_by_configured_Where_4()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"WALMEX V") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(55,"AMX L") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(55,"X") }
      };

      var filtered = dynamic_filter1(L, "35=8 OR 35=j");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<int>(4, filtered.Count());
      Assert.AreEqual<string>("(35,8) (55,AMX L) | (35,j) (55,WALMEX V) | (35,j) (55,AMX L) | (35,8) (55,X) | ", output.ToString());

      Assert.AreEqual<int>(4, L.Where(LL => LL.First(pair => pair.Key == 35).Value == "8" || LL.First(pair => pair.Key == 35).Value == "j").Count());
      Assert.AreEqual<int>(4, L.Where(LL => LL.Any(pair => pair.Key == 35 && pair.Value == "8") || LL.Any(pair => pair.Key == 35 && pair.Value == "j")).Count());
    }
    [TestMethod]
    public void dynamic_filter_by_configured_Where_5()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"0") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"j") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(56, "VX"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> { new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") }
      };

      var filtered = dynamic_filter1(L, "56=DC1 AND 35=8 AND 39=2");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))));

      Assert.AreEqual<int>(2, filtered.Count());
      Assert.AreEqual<string>("(56,DC1) (35,8) (39,2) | (56,DC1) (35,8) (39,2) | ", output.ToString());
    }
    [TestMethod]
    public void dynamic_filter_by_configured_Where_6()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "A"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"0") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"0") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"j") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "VX"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "A"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") }
      };

      var L2 = L.Where(msg => msg.First(pair => pair.Key == -1).Value == "IN" && "8|j".Contains(msg.First(pair => pair.Key == 35).Value)).Cast<IEnumerable<KeyValuePair<int, string>>>();
      var filtered = dynamic_filter1(L2, "56=DC1 AND 35=8 AND 39=2");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => n.Key != -1 ? w.AppendFormat("({0},{1}) ", n.Key, n.Value) : w)));

      Assert.AreEqual<int>(6, L2.Count());
      Assert.AreEqual<int>(2, filtered.Count());
      Assert.AreEqual<string>("(56,DC1) (35,8) (39,2) | (56,DC1) (35,8) (39,2) | ", output.ToString());
    }
    [TestMethod]
    public void dynamic_filter_by_configured_Where_7()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "A"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"0") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"0") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"j") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "VX"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "A"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"1") }
      };

      var L2 = L.Where(msg => msg.First(pair => pair.Key == -1).Value == "IN" && "8|j".Contains(msg.First(pair => pair.Key == 35).Value)).Cast<IEnumerable<KeyValuePair<int, string>>>();
      var filtered = dynamic_filter1(L2, "56=DC1 AND 35=8 AND (39=1 OR 39=2)");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => n.Key != -1 ? w.AppendFormat("({0},{1}) ", n.Key, n.Value) : w)));

      Assert.AreEqual<int>(7, L2.Count());
      Assert.AreEqual<int>(3, filtered.Count());
      Assert.AreEqual<string>("(56,DC1) (35,8) (39,2) | (56,DC1) (35,8) (39,2) | (56,DC1) (35,8) (39,1) | ", output.ToString());
    }
    [TestMethod]
    public void dynamic_filter_by_configured_Where_8()
    {
      IEnumerable<List<KeyValuePair<int, string>>> L = new List<List<KeyValuePair<int, string>>>
      {
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "A"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"0") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"0") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"j"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"j") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "VX"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "A"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"2") },
      new List<KeyValuePair<int,string>> {new KeyValuePair<int, string>(-1, "IN"), new KeyValuePair<int, string>(56, "DC1"), new KeyValuePair<int,string>(35,"8"), new KeyValuePair<int,string>(39,"1") }
      };

      var L2 = L.Where(msg => msg.First(pair => pair.Key == -1).Value == "IN" && "8|j".Contains(msg.First(pair => pair.Key == 35).Value)).Cast<IEnumerable<KeyValuePair<int, string>>>();
      var filtered = dynamic_filter1(L2, "(56=DC1 OR 56=VX) AND 35=8 AND (39=1 OR 39=2)");
      var output = filtered.Aggregate(new StringBuilder(), (whole, msg) => whole.AppendFormat("{0}| ", msg.Aggregate(new StringBuilder(), (w, n) => n.Key != -1 ? w.AppendFormat("({0},{1}) ", n.Key, n.Value) : w)));

      Assert.AreEqual<int>(7, L2.Count());
      Assert.AreEqual<int>(4, filtered.Count());
      Assert.AreEqual<string>("(56,VX) (35,8) (39,2) | (56,DC1) (35,8) (39,2) | (56,DC1) (35,8) (39,2) | (56,DC1) (35,8) (39,1) | ", output.ToString());
    }
    [TestMethod]
    public void expr1()
    {
      var regex = new Regex(@"(?<pair>(?<tag>\d+)=(?<val>\w+))|(?<op>AND|OR)");
      var input = "12=A123 OR 55=1 AND 34=123";

      var matches = regex.Matches(input);

      Assert.AreEqual<int>(5, matches.Count);
      Assert.AreEqual<string>("12=A123", matches[0].Value);
      Assert.AreEqual<string>("OR", matches[1].Value);
      Assert.AreEqual<string>("55=1", matches[2].Value);
      Assert.AreEqual<string>("AND", matches[3].Value);
      Assert.AreEqual<string>("34=123", matches[4].Value);

      Assert.AreEqual<string>("12=A123", matches[0].Groups["pair"].Value);
      Assert.AreEqual<string>("12", matches[0].Groups["tag"].Value);
      Assert.AreEqual<string>("A123", matches[0].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[0].Groups["op"].Value);

      Assert.AreEqual<string>("", matches[1].Groups["pair"].Value);
      Assert.AreEqual<string>("", matches[1].Groups["tag"].Value);
      Assert.AreEqual<string>("", matches[1].Groups["val"].Value);
      Assert.AreEqual<string>("OR", matches[1].Groups["op"].Value);

      Assert.AreEqual<string>("55=1", matches[2].Groups["pair"].Value);
      Assert.AreEqual<string>("55", matches[2].Groups["tag"].Value);
      Assert.AreEqual<string>("1", matches[2].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[2].Groups["op"].Value);

      Assert.AreEqual<string>("", matches[3].Groups["pair"].Value);
      Assert.AreEqual<string>("", matches[3].Groups["tag"].Value);
      Assert.AreEqual<string>("", matches[3].Groups["val"].Value);
      Assert.AreEqual<string>("AND", matches[3].Groups["op"].Value);

      Assert.AreEqual<string>("34=123", matches[4].Groups["pair"].Value);
      Assert.AreEqual<string>("34", matches[4].Groups["tag"].Value);
      Assert.AreEqual<string>("123", matches[4].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[4].Groups["op"].Value);
    }
    [TestMethod]
    public void expr2()
    {
      var regex = new Regex(@"(?<pair>(?<tag>\d+)=(?<val>\w+))|(?<op>AND|OR)");
      var input = "OR 35=8 OR 421=3 AND 34=12 AND";

      var matches = regex.Matches(input);

      Assert.AreEqual<int>(7, matches.Count);
      Assert.AreEqual<string>("OR", matches[0].Value);
      Assert.AreEqual<string>("35=8", matches[1].Value);
      Assert.AreEqual<string>("OR", matches[2].Value);
      Assert.AreEqual<string>("421=3", matches[3].Value);
      Assert.AreEqual<string>("AND", matches[4].Value);
      Assert.AreEqual<string>("34=12", matches[5].Value);
      Assert.AreEqual<string>("AND", matches[6].Value);

      Assert.AreEqual<string>("", matches[0].Groups["pair"].Value);
      Assert.AreEqual<string>("", matches[0].Groups["tag"].Value);
      Assert.AreEqual<string>("", matches[0].Groups["val"].Value);
      Assert.AreEqual<string>("OR", matches[0].Groups["op"].Value);

      Assert.AreEqual<string>("35=8", matches[1].Groups["pair"].Value);
      Assert.AreEqual<string>("35", matches[1].Groups["tag"].Value);
      Assert.AreEqual<string>("8", matches[1].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[1].Groups["op"].Value);

      Assert.AreEqual<string>("", matches[2].Groups["pair"].Value);
      Assert.AreEqual<string>("", matches[2].Groups["tag"].Value);
      Assert.AreEqual<string>("", matches[2].Groups["val"].Value);
      Assert.AreEqual<string>("OR", matches[2].Groups["op"].Value);

      Assert.AreEqual<string>("421=3", matches[3].Groups["pair"].Value);
      Assert.AreEqual<string>("421", matches[3].Groups["tag"].Value);
      Assert.AreEqual<string>("3", matches[3].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[3].Groups["op"].Value);

      Assert.AreEqual<string>("", matches[4].Groups["pair"].Value);
      Assert.AreEqual<string>("", matches[4].Groups["tag"].Value);
      Assert.AreEqual<string>("", matches[4].Groups["val"].Value);
      Assert.AreEqual<string>("AND", matches[4].Groups["op"].Value);

      Assert.AreEqual<string>("34=12", matches[5].Groups["pair"].Value);
      Assert.AreEqual<string>("34", matches[5].Groups["tag"].Value);
      Assert.AreEqual<string>("12", matches[5].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[5].Groups["op"].Value);

      Assert.AreEqual<string>("", matches[6].Groups["pair"].Value);
      Assert.AreEqual<string>("", matches[6].Groups["tag"].Value);
      Assert.AreEqual<string>("", matches[6].Groups["val"].Value);
      Assert.AreEqual<string>("AND", matches[6].Groups["op"].Value);
    }
    [TestMethod]
    public void expr3()
    {
      var regex = new Regex(@"(?<pair>(?<tag>\d+)=(?<val>\w+))|(?<op>AND|OR)");
      var input = "35=8 OR";

      var matches = regex.Matches(input);

      Assert.AreEqual<int>(2, matches.Count);
      Assert.AreEqual<string>("35=8", matches[0].Value);
      Assert.AreEqual<string>("OR", matches[1].Value);

      Assert.AreEqual<string>("35=8", matches[0].Groups["pair"].Value);
      Assert.AreEqual<string>("35", matches[0].Groups["tag"].Value);
      Assert.AreEqual<string>("8", matches[0].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[0].Groups["op"].Value);

      Assert.AreEqual<string>("", matches[1].Groups["pair"].Value);
      Assert.AreEqual<string>("", matches[1].Groups["tag"].Value);
      Assert.AreEqual<string>("", matches[1].Groups["val"].Value);
      Assert.AreEqual<string>("OR", matches[1].Groups["op"].Value);
    }
    [TestMethod]
    public void expr4()
    {
      var regex = new Regex(@"(?<pair>(?<tag>\d+)=(?<val>\w+))|(?<op>AND|OR)");
      var input = "AND 35=8";

      var matches = regex.Matches(input);

      Assert.AreEqual<int>(2, matches.Count);
      Assert.AreEqual<string>("AND", matches[0].Value);
      Assert.AreEqual<string>("35=8", matches[1].Value);

      Assert.AreEqual<string>("", matches[0].Groups["pair"].Value);
      Assert.AreEqual<string>("", matches[0].Groups["tag"].Value);
      Assert.AreEqual<string>("", matches[0].Groups["val"].Value);
      Assert.AreEqual<string>("AND", matches[0].Groups["op"].Value);

      Assert.AreEqual<string>("35=8", matches[1].Groups["pair"].Value);
      Assert.AreEqual<string>("35", matches[1].Groups["tag"].Value);
      Assert.AreEqual<string>("8", matches[1].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[1].Groups["op"].Value);
    }
    [TestMethod]
    public void expr5()
    {
      var regex = new Regex(@"(?<pair>(?<tag>\d+)=(?<val>\w+))|(?<op>AND|OR)");
      var input = "35=8";

      var matches = regex.Matches(input);

      Assert.AreEqual<int>(1, matches.Count);
      Assert.AreEqual<string>("35=8", matches[0].Value);

      Assert.AreEqual<string>("35=8", matches[0].Groups["pair"].Value);
      Assert.AreEqual<string>("35", matches[0].Groups["tag"].Value);
      Assert.AreEqual<string>("8", matches[0].Groups["val"].Value);
      Assert.AreEqual<string>("", matches[0].Groups["op"].Value);
    }
    [TestMethod]
    public void expression_tokens1()
    {
      var input = "35=8";
      var tokens = get_expression_tokens(input).Reverse().ToList().Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      Assert.AreEqual<string>("35=8,", tokens);
    }
    [TestMethod]
    public void expression_tokens2()
    {
      var input = "12=A123 OR 55=1 AND 34=123";
      var tokens = get_expression_tokens(input).Reverse().ToList().Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      Assert.AreEqual<string>("34=123,AND,55=1,OR,12=A123,", tokens);
    }
    [TestMethod]
    public void expression_tokens3()
    {
      var input = "AND 55=1 OR";
      var tokens = get_expression_tokens(input).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      Assert.AreEqual<string>("AND,55=1,OR,", tokens);
    }
    [TestMethod, TestCategory("alt1")]
    public void expression_tokens_alt1_a()
    {
      var input = "AND 55=1 OR";
      var tokens = get_expression_tokens_alt1(input).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      Assert.AreEqual<string>("AND,55=1,OR,", tokens);
    }
    [TestMethod, TestCategory("alt1")]
    public void tokens_alt1_a()
    {
      var input = "55=1";
      var tokens = get_tokens_alt1(input);

      Assert.AreEqual<string>("55,=,1,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Value)).ToString());
      Assert.AreEqual<string>("Text,Equals,Text,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Key)).ToString());
    }
    [TestMethod, TestCategory("alt1")]
    public void tokens_alt1_b()
    {
      var input = " 55 =  1  ";
      var tokens = get_tokens_alt1(input);

      Assert.AreEqual<string>("55,=,1,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Value)).ToString());
      Assert.AreEqual<string>("Text,Equals,Text,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Key)).ToString());
    }
    [TestMethod, TestCategory("alt1")]
    public void tokens_alt1_c()
    {
      var input = " 55 =  1  OR 2=ABC";
      var tokens = get_tokens_alt1(input);

      Assert.AreEqual<string>("55,=,1,OR,2,=,ABC,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Value)).ToString());
      Assert.AreEqual<string>("Text,Equals,Text,Text,Text,Equals,Text,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Key)).ToString());
    }
    [TestMethod, TestCategory("alt1")]
    public void tokens_alt1_d()
    {
      var input = " 55 =  1  OR (2=ABC)";
      var tokens = get_tokens_alt1(input);

      Assert.AreEqual<string>("55,=,1,OR,(,2,=,ABC,),", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Value)).ToString());
      Assert.AreEqual<string>("Text,Equals,Text,Text,LeftParenthesis,Text,Equals,Text,RightParenthesis,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Key)).ToString());
    }
    [TestMethod, TestCategory("alt1")]
    public void tokens_alt1_e()
    {
      var input = "NOT 55 =  1  AND NOT(2=ABC)";
      var tokens = get_tokens_alt1(input);

      Assert.AreEqual<string>("NOT,55,=,1,AND,NOT,(,2,=,ABC,),", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Value)).ToString());
      Assert.AreEqual<string>("Text,Text,Equals,Text,Text,Text,LeftParenthesis,Text,Equals,Text,RightParenthesis,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Key)).ToString());
    }
    [TestMethod, TestCategory("alt1")]
    public void tokens_alt1_f()
    {
      var input = "NOT 55 >  1  AND NOT(2<10)   ";
      var tokens = get_tokens_alt1(input);

      Assert.AreEqual<string>("NOT,55,>,1,AND,NOT,(,2,<,10,),", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Value)).ToString());
      Assert.AreEqual<string>("Text,Text,GreaterThan,Text,Text,Text,LeftParenthesis,Text,LessThan,Text,RightParenthesis,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n.Key)).ToString());
    }
    [TestMethod]
    public void rsub_expr1()
    {
      var tokens = new string[] { "1", "2", "3" };
      Assert.AreEqual<string>("3 2 1 ", reverse_sub_expression(tokens, 0));
      Assert.AreEqual<string>("3 2 ", reverse_sub_expression(tokens, 1, 2));
      Assert.AreEqual<string>("3 2 ", reverse_sub_expression(tokens, 1));
      Assert.AreEqual<string>("3 ", reverse_sub_expression(tokens, 2));
      Assert.AreEqual<string>("1 ", reverse_sub_expression(tokens, 0, 1));
    }
    [TestMethod]
    public void indexof()
    {
      var operators = new string[] { "AND", "OR" };
      var input = "12=A123 OR 55=1 AND 34=123";
      var tokens = get_expression_tokens(input).Reverse().ToArray();
      Assert.AreEqual<string>("34=123,AND,55=1,OR,12=A123,", tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString());

      string operator_token;
      int index = IndexOfLogicalOperator(operators, tokens, out operator_token);
      Assert.AreEqual<int>(1, index);
      Assert.AreEqual<string>("AND", operator_token);
    }
    [TestMethod]
    public void tree0()
    {
      Tree<string> t0 = new Tree<string> { Value = "head" };
      Assert.IsTrue(t0.Add(new Tree<string> { Value = "uno" }));
      Assert.IsTrue(t0.Add(new Tree<string> { Value = "uno" }));

      var sub = new Tree<string> { Value = "dos" };
      Assert.IsTrue(t0.Add(sub));
      Assert.IsFalse(t0.Add(sub));
    }
    [TestMethod]
    public void tree1()
    {
      Tree<string> t1 = tree_parse("35=j");
      Tree<string> t2 = tree_parse("35=8 OR 35=j");
      Tree<string> t3 = tree_parse("35=8 AND 55=AMX");
      Tree<string> t4 = tree_parse("35=8 AND 55=AMX OR 34=123");
      Tree<string> t5 = tree_parse("35=8 AND 55=X OR 34=1 AND 22=4");

      Assert.AreEqual<string>("35=j", t1.Value);

      Assert.AreEqual<string>("OR", t2.Value);
      Assert.AreEqual<string>("35=8", t2.ElementAt(0).Value);
      Assert.AreEqual<string>("35=j", t2.ElementAt(1).Value);

      Assert.AreEqual<string>("AND", t3.Value);
      Assert.AreEqual<string>("35=8", t3.ElementAt(0).Value);
      Assert.AreEqual<string>("55=AMX", t3.ElementAt(1).Value);

      Assert.AreEqual<string>("OR", t4.Value);
      Assert.AreEqual<string>("AND", t4.ElementAt(0).Value);
      Assert.AreEqual<string>("35=8", t4.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("55=AMX", t4.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("34=123", t4.ElementAt(1).Value);

      Assert.AreEqual<string>("AND", t5.Value);
      Assert.AreEqual<string>("OR", t5.ElementAt(0).Value);
      Assert.AreEqual<string>("AND", t5.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("35=8", t5.ElementAt(0).ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("55=X", t5.ElementAt(0).ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("34=1", t5.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("22=4", t5.ElementAt(1).Value);
    }
    [TestMethod]
    public void balanced_parenthesis()
    {
      Assert.IsTrue(are_parenthesis_balanced("abc"));
      Assert.IsTrue(are_parenthesis_balanced("(abc)"));
      Assert.IsTrue(are_parenthesis_balanced("((a))"));
      Assert.IsTrue(are_parenthesis_balanced("a + (b - (c + d))"));
      Assert.IsTrue(are_parenthesis_balanced("(a - 1) + (b - (c + d))"));
      Assert.IsFalse(are_parenthesis_balanced("a + (b - c + d))"));
      Assert.IsFalse(are_parenthesis_balanced("a + (b - (c + d)"));
      Assert.IsFalse(are_parenthesis_balanced("a + b - c + d)"));
      Assert.IsFalse(are_parenthesis_balanced("a + b - c) + d)"));
    }
    [TestMethod]
    public void tree2()
    {
      Tree<int, string> t0 = tree_parse_parenthesis_pass1("x - y + abc");
      Tree<int, string> t1 = tree_parse_parenthesis_pass1("(x) - () + (abc)");

      Assert.AreEqual<string>("x - y + abc", t0[0].Value);
      Trace.WriteLine($"t0=\n{t0}\n");

      var xml = t1.ToString();
      Trace.WriteLine($"t1=\n{xml}");
      Assert.IsNotNull(System.Xml.Linq.XDocument.Parse(xml));

      Assert.AreEqual<int>(5, t1.Count);
      Assert.AreEqual<string>("x", t1[0].Value);
      Assert.AreEqual<int>(0, t1[0].Count);
      Assert.AreEqual<string>(" - ", t1[1].Value);
      Assert.AreEqual<int>(0, t1[1].Count);
      Assert.AreEqual<string>("", t1[2].Value);
      Assert.AreEqual<int>(0, t1[2].Count);
      Assert.AreEqual<string>(" + ", t1[3].Value);
      Assert.AreEqual<int>(0, t1[3].Count);
      Assert.AreEqual<string>("abc", t1[4].Value);
      Assert.AreEqual<int>(0, t1[4].Count);
    }
    [TestMethod]
    public void tree3_pass2_a()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("39=1 OR 39=2"));

      Trace.WriteLine($"result:\n{whole_expr}");
      Assert.AreEqual<string>("OR", whole_expr.Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_b()
    {
      //Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(35=8) AND (39=1 OR 39=2)"));
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("35=8 AND (39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("35=8", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(1).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_b2()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(35=8) AND (39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("35=8", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(1).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_c()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("56=DC1 AND 35=8 AND (39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("AND", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("56=DC1", whole_expr.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("35=8", whole_expr.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(1).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_d()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("56=DC1 AND (35=8 OR 35=9) AND (39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("56=DC1", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("AND", whole_expr.ElementAt(1).Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("35=8", whole_expr.ElementAt(1).ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("35=9", whole_expr.ElementAt(1).ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(1).ElementAt(1).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).ElementAt(1).ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_e()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(35=8 OR 35=9) AND 39=1"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("35=8", whole_expr.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("35=9", whole_expr.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_f()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("OR", whole_expr.Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_g()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(39=1 OR 39=2) AND"));
      bool btree = is_binary_tree(whole_expr);

      Trace.WriteLine(whole_expr.ToString());
      Assert.IsFalse(btree);
    }
    [TestMethod]
    public void tree3_pass2_h()
    {
      string output = null;
      try
      {
        tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("AND (39=1 OR 39=2)"));
      }
      catch (Exception ex)
      {
        output = $"{ex.GetType().Name}: {ex.Message}";
      }

      Assert.IsNotNull(output);
      Assert.IsTrue(output.Contains("ArgumentException: Bad syntax."));
    }
    [TestMethod]
    public void tree3_pass2_i()
    {
      string output = null;
      try
      {
        tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("AND (39=1 OR 39=2) OR"));
      }
      catch (Exception ex)
      {
        output = $"{ex.GetType().Name}: {ex.Message}";
      }

      Assert.IsNotNull(output);
      Assert.IsTrue(output.Contains("ArgumentException: Bad syntax."));
    }

    [TestMethod]
    public void tree3_pass2_j()
    {
      string output = null;
      try
      {
        tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("AND 39=1 OR 39=2 OR"));
      }
      catch (Exception ex)
      {
        output = $"{ex.GetType().Name}: {ex.Message}";
      }

      Assert.IsNotNull(output);
      Assert.IsTrue(output.Contains("ArgumentException: Bad syntax."));
    }
    [TestMethod]
    public void tree3_pass2_k()
    {
      var whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("39=1 OR 39=2 OR"));
      bool btree = is_binary_tree(whole_expr);

      Trace.WriteLine(whole_expr.ToString());
      Assert.IsFalse(btree);
    }
    [TestMethod]
    public void tree3_pass2_l()
    {
      string output = null;
      try
      {
        tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("AND 39=1 OR 39=2"));
      }
      catch (Exception ex)
      {
        output = $"{ex.GetType().Name}: {ex.Message}";
      }

      //56=DC1 AND 35=8 AND (39=1 OR 39=2) OR 23=B

      Assert.IsNotNull(output);
      Assert.IsTrue(output.Contains("ArgumentException: Bad syntax."));
    }
    [TestMethod]
    public void tree3_pass2_m()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(56=DC1 OR 56=DC2) AND (35=8) AND (39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("56=DC1", whole_expr.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("56=DC2", whole_expr.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("AND", whole_expr.ElementAt(1).Value);
      Assert.AreEqual<string>("35=8", whole_expr.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(1).ElementAt(1).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).ElementAt(1).ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_n()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(56=DC1 OR 56=DC2) AND 35=8 AND (39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("56=DC1", whole_expr.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("56=DC2", whole_expr.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("AND", whole_expr.ElementAt(1).Value);
      Assert.AreEqual<string>("35=8", whole_expr.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(1).ElementAt(1).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).ElementAt(1).ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_o()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(56=DC1 OR 56=DC2) AND (39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("56=DC1", whole_expr.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("56=DC2", whole_expr.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("OR", whole_expr.ElementAt(1).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).ElementAt(0).Value);
      Assert.AreEqual<string>("39=2", whole_expr.ElementAt(1).ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_p()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(56=DC1) AND (39=1)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("56=DC1", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).Value);
    }
    [TestMethod]
    public void tree3_pass2_q()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(56=DC1) AND 39=1"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("56=DC1", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("39=1", whole_expr.ElementAt(1).Value);
    }
    [TestMethod, Ignore]
    public void tree3_pass2_nested_parenthesis_a()
    {
      Tree<string> whole_expr = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1("(56=DC1 AND (35=8 OR 35=9)) OR (39=1 OR 39=2)"));

      Trace.WriteLine(whole_expr.ToString());
      Assert.AreEqual<string>("AND", whole_expr.Value);
      Assert.AreEqual<string>("AND", whole_expr.ElementAt(0).Value);
      Assert.AreEqual<string>("56=DC1", whole_expr.ElementAt(0).ElementAt(0).Value);
      Assert.AreEqual<string>("35=8 OR 35=9", whole_expr.ElementAt(0).ElementAt(1).Value);
      Assert.AreEqual<string>("39=1 OR 39=2", whole_expr.ElementAt(1).Value);
    }
    [TestMethod, Ignore]
    public void less_than_greater_than()
    {
      dynamic_filter1(null, "(34>4566 AND 34<5000) AND 35=8 AND (39=1 OR 39=2)");
    }
    [TestMethod]
    public void keyed_tree_data_schema_0()
    {
      Tree<string> x = new Tree<string> { Value = "cero" };
      x.Add(new Tree<string> { Value = "uno" });
      x.Add(new Tree<string> { Value = "subuno" });

      var xml = x.ToString();
      Trace.WriteLine(xml);
      Assert.IsNotNull(System.Xml.Linq.XDocument.Parse(xml));
    }
    [TestMethod]
    public void keyed_tree_data_schema_2()
    {
      Tree<int, string> x = new Tree<int, string> { Value = "cero" };
      x[1] = new Tree<int, string> { Value = "uno" };
      x[2] = new Tree<int, string> { Value = "dos" };
      x[8] = new Tree<int, string> { Value = "ocho" };

      Assert.AreEqual<int>(3, x.Count);

      Assert.AreEqual<string>("cero", x.Value);
      Assert.AreEqual<string>("uno", x[1].Value);
      Assert.AreEqual<string>("dos", x[2].Value);
      Assert.AreEqual<string>("ocho", x[8].Value);
      Assert.AreEqual<int>(1, x.ElementAt(0).Key);
      Assert.AreEqual<int>(2, x.ElementAt(1).Key);
      Assert.AreEqual<int>(8, x.ElementAt(2).Key);
      Assert.AreEqual<int>(0, x.ElementAt(0).Value.Count);
      Assert.AreEqual<int>(0, x.ElementAt(1).Value.Count);
      Assert.AreEqual<int>(0, x.ElementAt(2).Value.Count);

      Assert.AreEqual<Type>(typeof(int), x.ElementAt(0).Key.GetType());
      Assert.AreEqual<Type>(typeof(Tree<int, string>), x.ElementAt(0).Value.GetType());

      var xml = x.ToString();
      Trace.WriteLine(xml);
      Assert.IsNotNull(System.Xml.Linq.XDocument.Parse(xml));
    }
    [TestMethod]
    public void tree_expr1()
    {
      Tree<string> t = tree_parse("35=8");

      var filter_tags = t.Value.Split('=');
      int filter_tag = int.Parse(filter_tags[0]);
      string filter_value = filter_tags[1];

      ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");
      ParameterExpression msg = Expression.Parameter(typeof(IEnumerable<KeyValuePair<int, string>>), "msg");
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(KeyValuePair<int, string>));

      Expression key_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Key"));
      Expression key_right = Expression.Constant(filter_tag, typeof(int));
      Expression key_expr = Expression.Equal(key_left, key_right);
      Expression value_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Value"));
      Expression value_right = Expression.Constant(filter_value, typeof(string));
      Expression value_expr = Expression.Equal(value_left, value_right);
      var pair_filter_and = Expression.And(key_expr, value_expr);
      var pair_filter = Expression.Lambda<Func<KeyValuePair<int, string>, bool>>(pair_filter_and, new ParameterExpression[] { pair });
      MethodCallExpression any = Expression.Call(any_method, msg, pair_filter);

      Assert.AreEqual<string>("msg.Any(pair => ((pair.Key == 35) And (pair.Value == \"8\")))", any.ToString());
    }
    [TestMethod]
    public void tree_expr2()
    {
      Tree<string> t = tree_parse("35=8 OR 35=j");

      var filter_tags = t.ElementAt(0).Value.Split('=');
      int filter_tag = int.Parse(filter_tags[0]);
      string filter_value = filter_tags[1];

      ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");
      ParameterExpression msg = Expression.Parameter(typeof(IEnumerable<KeyValuePair<int, string>>), "msg");
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(KeyValuePair<int, string>));

      Expression key_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Key"));
      Expression key_right = Expression.Constant(filter_tag, typeof(int));
      Expression key_expr = Expression.Equal(key_left, key_right);
      Expression value_left = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Value"));
      Expression value_right = Expression.Constant(filter_value, typeof(string));
      Expression value_expr = Expression.Equal(value_left, value_right);
      var pair_filter_and = Expression.And(key_expr, value_expr);
      var pair_filter = Expression.Lambda<Func<KeyValuePair<int, string>, bool>>(pair_filter_and, new ParameterExpression[] { pair });
      MethodCallExpression any = Expression.Call(any_method, msg, pair_filter);

      var filter_tags_ = t.ElementAt(1).Value.Split('=');
      int filter_tag_ = int.Parse(filter_tags_[0]);
      string filter_value_ = filter_tags_[1];

      Expression key_left_ = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Key"));
      Expression key_right_ = Expression.Constant(filter_tag_, typeof(int));
      Expression key_expr_ = Expression.Equal(key_left_, key_right_);
      Expression value_left_ = Expression.Property(pair, typeof(KeyValuePair<int, string>).GetProperty("Value"));
      Expression value_right_ = Expression.Constant(filter_value_, typeof(string));
      Expression value_expr_ = Expression.Equal(value_left_, value_right_);
      var pair_filter_and_ = Expression.And(key_expr_, value_expr_);
      var pair_filter_ = Expression.Lambda<Func<KeyValuePair<int, string>, bool>>(pair_filter_and_, new ParameterExpression[] { pair });
      MethodCallExpression any_ = Expression.Call(any_method, msg, pair_filter_);

      //t.Value == "OR"
      Expression or = Expression.Or(any, any_);

      Assert.AreEqual<string>("msg.Any(pair => ((pair.Key == 35) And (pair.Value == \"8\")))", any.ToString());
      Assert.AreEqual<string>("msg.Any(pair => ((pair.Key == 35) And (pair.Value == \"j\")))", any_.ToString());
      Assert.AreEqual<string>("(msg.Any(pair => ((pair.Key == 35) And (pair.Value == \"8\"))) Or msg.Any(pair => ((pair.Key == 35) And (pair.Value == \"j\"))))", or.ToString());
    }
    [TestMethod]
    public void tree_expr3()
    {
      ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");
      ParameterExpression msg = Expression.Parameter(typeof(IEnumerable<KeyValuePair<int, string>>), "msg");

      Tree<string> t1 = tree_parse("35=8 OR 35=j");
      Expression expr1 = build_filter_expression(t1, pair, msg);

      Tree<string> t5 = tree_parse("35=8 AND 55=X OR 34=1 AND 22=4");
      Expression expr5 = build_filter_expression(t5, pair, msg);

      Assert.AreEqual<string>("(msg.Any(pair => ((pair.Key == 35) And (pair.Value == \"8\"))) Or msg.Any(pair => ((pair.Key == 35) And (pair.Value == \"j\"))))", expr1.ToString());
      Assert.AreEqual<string>("(((msg.Any(pair => ((pair.Key == 35) And (pair.Value == \"8\"))) And msg.Any(pair => ((pair.Key == 55) And (pair.Value == \"X\")))) Or msg.Any(pair => ((pair.Key == 34) And (pair.Value == \"1\")))) And msg.Any(pair => ((pair.Key == 22) And (pair.Value == \"4\"))))", expr5.ToString());
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
      var result = q.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n.Count()));

      var output = $"{result}";
      Assert.AreEqual<string>("3|3|", output);
    }
    [TestMethod]
    public void call_static_12()
    {
      /*
      Where(LL => LL.Any(pair => pair.Key == 35 && pair.Value == "8") || LL.Any(pair => pair.Key == 35 && pair.Value == "j"))

      Where
      (
        LL => 
          LL.Any
          (
            pair => pair.Key == 35 && pair.Value == "8"
          )
          ||
          LL.Any
          (
            pair => pair.Key == 35 && pair.Value == "j"
          )
      )

      Where(LL => LL.Any(pair => pair.n == 0) || LL.Any(pair => pair.n == 6))

      Where
      (
        LL =>
          LL.Any(pair => pair.n == 0)
          ||
          LL.Any(pair => pair.n == 6)
      )
      */
      var E = new List<List<B>>
      {
        new List<B> { new B(0), new B(5), new B(125) },
        new List<B> { new B(1), new B(6), new B(126) },
        new List<B> { new B(2), new B(5), new B(125) }
      };
      IQueryable<IEnumerable<B>> Q = E.AsQueryable<IEnumerable<B>>();

      ParameterExpression b = Expression.Parameter(typeof(B), "b");
      ParameterExpression bb = Expression.Parameter(typeof(IEnumerable<B>), "bb");
      var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(B));

      Expression left1 = Expression.Field(b, typeof(B).GetField("n"));
      Expression right1 = Expression.Constant(0, typeof(int));
      Expression expr1 = Expression.Equal(left1, right1);
      var b_filter1 = Expression.Lambda<Func<B, bool>>(expr1, new ParameterExpression[] { b });
      MethodCallExpression any1 = Expression.Call(any_method, bb, b_filter1);

      Expression left2 = Expression.Field(b, typeof(B).GetField("n"));
      Expression right2 = Expression.Constant(6, typeof(int));
      Expression expr2 = Expression.Equal(left2, right2);
      var b_filter2 = Expression.Lambda<Func<B, bool>>(expr2, new ParameterExpression[] { b });
      MethodCallExpression any2 = Expression.Call(any_method, bb, b_filter2);

      Expression main_where_filter = Expression.Or(any1, any2);
      var selection = Expression.Lambda<Func<IEnumerable<B>, bool>>(main_where_filter, new ParameterExpression[] { bb });

      MethodCallExpression where_expression = Expression.Call(
        typeof(Queryable),
        "Where",
        new Type[] { Q.ElementType },
        Q.Expression,
        selection);

      var q = Q.Provider.CreateQuery<IEnumerable<B>>(where_expression);
      var filtered_iqueryable_output = q.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0}) ", n.Aggregate(new StringBuilder(), (w2, n2) => w2.AppendFormat("{0},", n2.n))));

      var filtered_ienumerable = E.Where(LL => LL.Any(pair => pair.n == 0) || LL.Any(pair => pair.n == 6));
      var filtered_ienumerable_output = filtered_ienumerable.Aggregate(new StringBuilder(), (w, next) => w.AppendFormat("({0}) ", next.Aggregate(new StringBuilder(), (w2, n2) => w2.AppendFormat("{0},", n2.n))));

      Assert.AreEqual<string>("(0,5,125,) (1,6,126,) ", $"{filtered_iqueryable_output}");
      Assert.AreEqual<string>("(0,5,125,) (1,6,126,) ", $"{filtered_ienumerable_output}");
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