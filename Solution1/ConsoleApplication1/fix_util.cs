using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using static System.Console;

namespace SqlWriterAgent
{
  public static class fix_util
  {
    public static IEnumerable<IEnumerable<KeyValuePair<int, string>>> read(System.IO.FileInfo file, bool appendRawFixMessage = false)
    {
      var encoding = Encoding.GetEncoding("ISO-8859-1");//http://www.fixtradingcommunity.org/pg/discussions/topicpost/167593/character-encoding-type
      foreach (var line in read(new System.IO.StreamReader(file.OpenRead(), encoding), appendRawFixMessage))
      {
        yield return line;
      }
    }

    public static IEnumerable<IEnumerable<KeyValuePair<int, string>>> read(System.IO.TextReader reader, bool appendRawFixMessage = false)
    {
      using (reader)
      {
        do
        {
          var line = reader.ReadLine();
          if (line == null) break;
          if (string.IsNullOrWhiteSpace(line)) continue;
          int start = line.IndexOf("8=");
          if (start < 0) continue;
          int out_position = line.IndexOf("\tOUT ", 0, start);
          int in_position = line.IndexOf("\tIN ", 0, start);
          int first_tab_position = line.IndexOf("\t", 0, start);
          if (first_tab_position <= 0) continue;

          var tags_fragment = line.Substring(start);
          var tag_labels = tags_fragment.Split('\x01');
          var msg = new List<KeyValuePair<int, string>>();

          string inout;
          string timestamp = line.Substring(0, first_tab_position);
          string sender_target;
          if (out_position >= 0)
          {
            inout = "OUT";
            sender_target = line.Substring(first_tab_position + 1, out_position - first_tab_position - 1);
          }
          else if (in_position >= 0)
          {
            inout = "IN";
            sender_target = line.Substring(first_tab_position + 1, in_position - first_tab_position - 1);
          }
          else
          {
            inout = "UNKNOWN";
            sender_target = "";
          }
          msg.Add(new KeyValuePair<int, string>(-3, timestamp));
          msg.Add(new KeyValuePair<int, string>(-2, sender_target));
          msg.Add(new KeyValuePair<int, string>(-1, inout));
          foreach (var tag_label in tag_labels)
          {
            var pair = tag_label.Split('=');
            //int tag_number;
            if (pair.Length == 2)//&& int.TryParse(pair[0], out tag_number) == true
            {
              msg.Add(new KeyValuePair<int, string>(int.Parse(pair[0]), pair[1])); //A visible exception alerting about a FIX protocol problem is better than a harmful silence.
                                                                                   //msg.Add(new KeyValuePair<int, string>(tag_number, pair[1]));
            }
          }
          if (appendRawFixMessage)
          {
            msg.Add(new KeyValuePair<int, string>(int.MaxValue, line));
          }
          yield return msg;
        } while (true);
      }
    }

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
          return whole;
        });
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
      if (input == null)
      {
        throw new ArgumentNullException(nameof(input));
      }
      var regex = new System.Text.RegularExpressions.Regex(@"(?<pair>(?<tag>\d+)=(?<val>\w+))|(?<op>AND|OR)");
      foreach (System.Text.RegularExpressions.Match match in regex.Matches(input))
      {
        var token = match.Value?.Trim();
        if (string.IsNullOrWhiteSpace(token) == false)
        {
          yield return token;
        }
      }
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
    static Tree<string> tree_parse_parenthesis_pass2(Tree<int, string> pass1, IEnumerable<string> operators = null)
    {
      if (pass1 == null)
      {
        throw new ArgumentNullException(nameof(pass1));
      }
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
        string[] expr_tokens = get_expression_tokens(expr).ToArray();
        if (expr_tokens.Length == 0)
        {
          throw new Exception("Invalid syntax: No expression.");
        }

        Tree<string> current_node = null;
        parse_pass1_node(expr_tokens, operators, ref result_head, ref current_node, ref pending_node);
        if (current_node != null)
        {
          result_head = current_node;
        }
      }
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
          node.Add(tree_parse(expr_tokens.Where((token, index) => index > 0 && index < expr_tokens.Length - 1).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim()));
          if (node.Add(result_head) == false)
          {
            throw new Exception($"Node {result_head} has been already added to the tree: {node}");
          }
          result_head = null;
          pending_node.Add(node);
        }
        else
        {
          current_node = new Tree<string> { Value = expr_tokens.Last() };
          current_node.Add(tree_parse(expr_tokens.Take(taken).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim()));
          //current_node.Add(tree_parse_parenthesis_pass2(expr_tokens.Take(taken).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim())));
          if (current_node.Add(result_head) == false)
          {
            throw new Exception($"Node {result_head} has been already added to the tree: {current_node}");
          }
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
      }
    }
    static void parse_pass1_node_with_prefix_operator(string[] expr_tokens, IEnumerable<string> operators, ref Tree<string> result_head, ref Tree<string> current_node, ref Tree<string> pending_node)
    {
      int taken = expr_tokens.Count() - 1;
      if (taken > 0)
      {
        pending_node = new Tree<string> { Value = expr_tokens.First() };
        pending_node.Add(new Tree<string> { Value = "<pending>" });
        pending_node.Add(tree_parse(expr_tokens.SkipWhile((_, index) => index == 0).Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim()));
      }
      else
      {
        pending_node = new Tree<string> { Value = expr_tokens.First() };
        pending_node.Add(new Tree<string> { Value = "<pending>" });
        if (pending_node.Add(result_head) == false)
        {
          throw new Exception($"Node {result_head} has been already added to the tree: {pending_node}");
        }
      }
    }
    static void parse_pass1_standalone_node(string[] expr_tokens, IEnumerable<string> operators, ref Tree<string> result_head, ref Tree<string> current_node, ref Tree<string> pending_node)
    {
      var current_expression_node = tree_parse(expr_tokens.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat(" {0}", n)).ToString().Trim());
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
        }
        else
        {
          current_node = new Tree<string> { Value = pending_node.Value };
          current_node.Add(current_expression_node);
          if (current_node.Add(pending_node.ElementAt(1)) == false)
          {
            throw new Exception($"Node {pending_node.ElementAt(1)} has been already added to the tree: {current_node}");
          }
        }
        pending_node = null;
      }
      else
      {
        current_node = current_expression_node;
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
    static Tree<string> tree_parse(string input)
    {
      input = input?.Trim();
      if (string.IsNullOrWhiteSpace(input))
      {
        throw new ArgumentNullException(nameof(input));
      }

      var operators = new string[] { "AND", "OR" };
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
        result.Add(tree_parse(reverse_sub_expression(rexpr_tokens, operator_index + 1)));
        result.Add(tree_parse(reverse_sub_expression(rexpr_tokens, 0, operator_index)));
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
        Tree<string> tree = tree_parse_parenthesis_pass2(tree_parse_parenthesis_pass1(filter_config));

        if (is_binary_tree(tree) == false)
        {
          throw new ArgumentException("Bad syntax.", $"{filter_config}");
        }
        ParameterExpression pair = Expression.Parameter(typeof(KeyValuePair<int, string>), "pair");

        var pair_filter_subtree = build_filter_expression(tree, pair, msg);
        var pair_filter = Expression.Lambda<Func<KeyValuePair<int, string>, bool>>(pair_filter_subtree, new ParameterExpression[] { pair });
        var any_method = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(KeyValuePair<int, string>));
        filter_expression = Expression.Call(any_method, msg, pair_filter);
      }

      var where_selection = Expression.Lambda<Func<IEnumerable<KeyValuePair<int, string>>, bool>>(filter_expression, new ParameterExpression[] { msg });
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
  }

  public static class json_util
  {
    public static IEnumerable<IDictionary<string, object>> read(System.IO.FileInfo file)
    {
      using (var reader = file.OpenText()) do
        {
          string line = reader.ReadLine();
          if (line == null) break;
          if (string.IsNullOrWhiteSpace(line)) continue;

          var start_of_JSON = line.IndexOf("{");
          if (start_of_JSON < 0)
          {
            System.Diagnostics.Trace.WriteLine($"WARNING: no start of JSON message found ({line})");
            continue;
          }
          var payload = line.Substring(start_of_JSON);
          var destination = start_of_JSON > 0 ? line.Substring(0, start_of_JSON) : "no-destination";
          IDictionary<string, object> message = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
          //message[Constant.SolaceDestinationNameKey] = destination.Trim();
          yield return message;
        } while (true);
    }
    public static IEnumerable<T> read_as<T>(System.IO.FileInfo file, bool single_stored_object = false)
    {
      if (single_stored_object)
      {
        var payload = System.IO.File.ReadAllText(file.FullName);
        yield return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(payload);
      }
      else
        using (var reader = file.OpenText()) do
          {
            string line = reader.ReadLine();
            if (line == null) break;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var start_of_JSON = line.IndexOf("{");
            if (start_of_JSON < 0)
            {
              System.Diagnostics.Trace.WriteLine($"WARNING: no start of JSON message found ({line})");
              continue;
            }
            var payload = line.Substring(start_of_JSON);
            //var destination = start_of_JSON > 0 ? line.Substring(0, start_of_JSON) : "no-destination";
            T message = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(payload);
            //message[Constant.SolaceDestinationNameKey] = destination.Trim();
            yield return message;
          } while (true);
    }
    public static IEnumerable<IDictionary<string, object>> scan(System.IO.FileInfo file, string payloadkey = null)
    {
      using (var reader = file.OpenText()) do
        {
          string line = reader.ReadLine();
          if (line == null) break;
          if (string.IsNullOrWhiteSpace(line)) continue;

          var start_of_JSON = line.IndexOf("{");
          if (start_of_JSON < 0) continue;
          var end_of_JSON = line.IndexOf("}");
          if (end_of_JSON < 0) continue;
          var payload = line.Substring(start_of_JSON, end_of_JSON - start_of_JSON + 1);
          if (string.IsNullOrWhiteSpace(payload)) continue;
          //WriteLine($"{payload}\n");
          IDictionary<string, object> message = null;
          try
          {
            message = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
            if (string.IsNullOrWhiteSpace(payloadkey) == false)
            {
              message[payloadkey] = payload;
            }
          }
          catch (Newtonsoft.Json.JsonReaderException ex)
          {
            System.Diagnostics.Trace.WriteLine($"Payload: {payload} JSON Exception: {ex.Message}");
          }
          if (message != null) yield return message;
        } while (true);
    }
  }
}

class json
{
  public static void OrderTypeID(string[] args)
  {
    var OrderTypeID = new Func<IDictionary<string, object>, string>(J => J.ContainsKey("OrderTypeID") ? $"{J["OrderTypeID"]}" : "none");
    //var file = new System.IO.FileInfo(@"C:\Users\41477\Documents\Topic_EER_CAPITALES__VIRTU___sep.19_0911.txt");
    /*
none: 521
61: 230
60: 312
23: 67
22: 13
     */
    //var file = new System.IO.FileInfo(@"C:\Users\41477\Documents\Topic_EER_CAPITALES__VIRTU___sep.19_0947.txt");
    /*
60: 501
22: 49
none: 966
61: 578
23: 138
     */
    //const string payloadkey = "$<payload>$";
    //var json_log = SqlWriterAgent.json_util.scan(file);//, payloadkey);
    //foreach (var group in json_log.GroupBy(J => OrderTypeID(J)))
    //{
    //  WriteLine($"{group.Key}: {group.Count()}");
    //}

    /*
    var folder = @"C:\Users\41477\Documents";
    WriteLine($"{folder}");
    var all_json = System.IO.Directory.EnumerateFiles(folder, "Topic_EER_CAPITALES__VIRTU*.txt").Aggregate(new List<IDictionary<string, object>>(), (whole, next) =>
    {
      WriteLine($"\t{next}");
      foreach (var msg in SqlWriterAgent.json_util.read(new System.IO.FileInfo(next)))
      {
        whole.Add(msg);
      }
      return whole;
    });
    WriteLine($"FixEngine all_json.Count: {all_json.Count}");
    foreach (var group in all_json.GroupBy(J => OrderTypeID(J)))
    {
      WriteLine($"{group.Key}: {group.Count()}");
    }
    */

    var folder = @"C:\Users\41477\Documents";
    WriteLine($"{folder}");
    ulong all_json_Count = 0UL;
    var map = new Dictionary<string, int>();
    foreach (var next in System.IO.Directory.EnumerateFiles(folder, "Topic_EER_CAPITALES__VIRTU*.txt"))
    {
      WriteLine($"\n{next}");
      foreach (var J in SqlWriterAgent.json_util.read(new System.IO.FileInfo(next)))
      {
        Write($"\r\t{++all_json_Count,-10}");
        var id = OrderTypeID(J);
        if (map.ContainsKey(id) == false)
        {
          map[id] = 0;
        }
        ++map[id];
      }
    }
    WriteLine($"\nFixEngine all_json.Count: {all_json_Count}");
    foreach (var id in map.Keys)
    {
      WriteLine($"{id}: {map[id]}");
    }
  }
  static IEnumerable<IDictionary<string,object>> getmsgs(IEnumerable<IEnumerable<Dictionary<string, object>>> json_log_array)
  {
    foreach (var maps in json_log_array)
    {
      WriteLine($"Array elements: {maps.Count()}");
      foreach (var map in maps)
      {
        if (!map.ContainsKey("SeqNum"))
        {
          if (!(map.ContainsKey("Counts Update") || map.ContainsKey("Service Name")))
            WriteLine($"\tExcluded: {map.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))}");
          continue;
        }
        var seqnum = map["SeqNum"];
        map.Remove("SeqNum");
        map.Remove("Hexadecimal");
        var msgtypename = map.First().Key;
        var Jsubmap = map.First().Value as Newtonsoft.Json.Linq.JObject;
        if (Jsubmap?.Type == Newtonsoft.Json.Linq.JTokenType.Object)
        {
          IDictionary<string, object> submap = Jsubmap.ToObject<Dictionary<string, object>>();
          var msg = submap.Aggregate(new Dictionary<string, object>(), (w, n) => { w[n.Key] = n.Value; return w; });
          msg.Add("SeqNum", seqnum);
          msg.Add("Name", map.First().Key);
          yield return msg;
        }
        else
        {
          WriteLine($"[Unsupported] {map.First().Key}: {map.First().Value.GetType().FullName}");
        }
      }
    }
  }
  static IEnumerable<IDictionary<string, object>> getmsgs(IEnumerable<Dictionary<string, object>> json_log)
  {
    foreach (var map in json_log)
    {
      if (!map.ContainsKey("SeqNum"))
      {
        if (!map.ContainsKey("Service Name"))
          WriteLine($"\tExcluded: {map.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))}");
        continue;
      }
      var seqnum = map["SeqNum"];
      map.Remove("SeqNum");
      var msgtypename = map.First().Key;
      var Jsubmap = map.First().Value as Newtonsoft.Json.Linq.JObject;
      if (Jsubmap?.Type == Newtonsoft.Json.Linq.JTokenType.Object)
      {
        IDictionary<string, object> submap = Jsubmap.ToObject<Dictionary<string, object>>();
        var msg = submap.Aggregate(new Dictionary<string, object>(), (w, n) => { w[n.Key] = n.Value; return w; });
        msg.Add("SeqNum", seqnum);
        yield return msg;
      }
      else
      {
        WriteLine($"[Unsupported] {map.First().Key}: {map.First().Value.GetType().FullName}");
      }
    }
  }

  static void as_json_array(IEnumerable<IEnumerable<Dictionary<string, object>>> json_log_array)
  {
    WriteLine($"{json_log_array.Count()}");
    //json_log_array.First().Aggregate(Out,(w,n)=> { w.WriteLine($"{n.GetType().FullName}"); return w; });
    //foreach (var t in json_log_array.First().SelectMany(j => j.Values.Select(v => v.GetType().FullName)).Distinct()) WriteLine(t);
    //foreach (var t in json_log_array.First().SelectMany(j => j.Keys.Select(v => v.GetType().FullName)).Distinct()) WriteLine(t);
    //WriteLine($"{json_log_array.First().First().GetType().FullName}");
    //json_log_array.First().Aggregate(Out, (w, n) => { w.WriteLine($"{n.Aggregate(new StringBuilder("\n"), (w2, n2) => w2.AppendFormat("\t({0},{1})\n", n2.Key, n2.Value == null ? "<null>" : n2.Value))}"); return w; });
    //foreach (var t in json_log_array.First().SelectMany(j => j.Keys).Distinct()) WriteLine(t);

    //foreach (var t in json_log_array.First().Where(J => J.ContainsKey("Orderbook Directory")).Select(j => $"{(((Newtonsoft.Json.Linq.JObject)j["Orderbook Directory"]).ToObject<Dictionary<string, object>>())["Delisting or Maturity Date"]}").Distinct()) WriteLine(t);
    //foreach (var t in json_log_array.First().Where(J => J.ContainsKey("Orderbook Directory")).Select(j => $"{(((Newtonsoft.Json.Linq.JObject)j["Orderbook Directory"]).ToObject<Dictionary<string, object>>())["Delisting Time"]}").Distinct()) WriteLine(t);

    //getmsgs(json_log_array).Where(m => m.ContainsKey("Type") == false).ToList().ForEach(t => WriteLine($"{t.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1}) ", n.Key, n.Value))}"));
    getmsgs(json_log_array).Where(m => m.ContainsKey("Type") == true).Select(m => $"{m["Name"]}[{m["Type"]}]").Distinct().ToList().ForEach(t => WriteLine(t));
    WriteLine($"Array msgs: {getmsgs(json_log_array).Count()}");

    //foreach (var msg in getmsgs(json_log_array))
    //{
    //}

    return;
    foreach (var maps in json_log_array)
    {
      WriteLine($"Array elements: {maps.Count()}");
      foreach (var map in maps)
      {
        foreach (var pair in map)
        {
          if (pair.Value == null || "System.String|System.Int64".Contains(pair.Value.GetType().Name)) continue;
          var Jsubmap = pair.Value as Newtonsoft.Json.Linq.JObject;
          if (Jsubmap?.Type == Newtonsoft.Json.Linq.JTokenType.Object)
          {
            IDictionary<string, object> submap = Jsubmap.ToObject<Dictionary<string, object>>();
            WriteLine($"{pair.Key}: {submap.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1},{2})", n.Key, n.Value, n.Value.GetType().Name))}");
          }
          else
          {
            WriteLine($"[Unsupported] {pair.Key}: {pair.Value.GetType().FullName}");
          }
        }
      }
    }
  }
  static void as_json_log(IEnumerable<Dictionary<string, object>> json_log)
  {
    WriteLine($"{json_log.Count()}");

    getmsgs(json_log).Where(m => m.ContainsKey("Type") == true).Select(m => $"{m["Name"]}[{m["Type"]}]").Distinct().ToList().ForEach(t => WriteLine(t));
    WriteLine($"msgs: {getmsgs(json_log).Count()}");

    return;
    foreach (var map in json_log)
    {
      foreach (var pair in map)
      {
        if (pair.Value == null || "System.String|System.Int64".Contains(pair.Value.GetType().Name)) continue;
        var Jsubmap = pair.Value as Newtonsoft.Json.Linq.JObject;
        if (Jsubmap?.Type == Newtonsoft.Json.Linq.JTokenType.Object)
        {
          IDictionary<string, object> submap = Jsubmap.ToObject<Dictionary<string, object>>();
          WriteLine($"{pair.Key}: {submap.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("({0},{1},{2})", n.Key, n.Value, n.Value.GetType().Name))}");
        }
        else
        {
          WriteLine($"[Unsupported] {pair.Key}: {pair.Value.GetType().FullName}");
        }
      }
    }
  }
  public static void _Main(string[] args)
  {
    try
    {
      /*
[ {"Service Name":"BBO", "Session":1497438007, "Next SeqNum":1}
, {"SeqNum":1,"Hexadecimal":"54 00 00 54 86 ","Time Stamp":{"Type":"T","Seconds":21638}}
]
Newtonsoft.Json.Linq.JObject
       */

      var folder = @"C:\Users\41477\Documents\MarketData\DataCapture\BIVA";
      WriteLine($"{folder}");
      foreach(var ext in new string[] { "*.txt", "*.log" })
      foreach (var filename in System.IO.Directory.EnumerateFiles(folder, ext))
      {
        try
        {
          var file = new System.IO.FileInfo(filename);
          Write($"\n{file.Name} ");
          if (file.Name.EndsWith(".txt") && System.IO.File.ReadLines(file.FullName).First().TrimStart().StartsWith("["))
          {
            Write($"(Single JSON Array) ");
            as_json_array(SqlWriterAgent.json_util.read_as<IEnumerable<Dictionary<string, object>>>(file, single_stored_object: true));
          }
          else
          {
            Write($"(JSON objects) ");
            as_json_log(SqlWriterAgent.json_util.read_as<Dictionary<string, object>>(file));
          }
        }
        catch (Exception ex) { WriteLine($"\n{filename}\n{ex.GetType().FullName}: {ex.Message} {ex.StackTrace}"); }
      }
    }
    catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
  }
}