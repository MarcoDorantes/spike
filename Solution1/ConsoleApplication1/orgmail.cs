using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;
/*
Exchange Online and Exchange development
https://docs.microsoft.com/en-us/exchange/client-developer/exchange-server-development
 */
static class orgmail
{
  class Input
  {
    const string To = "to";
    const string Bcc = "bcc";

    public bool excep_only, needs, text;
    public int count;
    public string subject, body;
    public FileInfo file, subjectfile, pack;

    public void latest()
    {
      var exchange = GetExchangeService();

      var view = new Microsoft.Exchange.WebServices.Data.ItemView(10);
      view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Id,
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.From,
          Microsoft.Exchange.WebServices.Data.ItemSchema.Subject,
          Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived,
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.IsRead);
      view.OrderBy.Add(Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived, Microsoft.Exchange.WebServices.Data.SortDirection.Descending);

      int pageSize = 10;
      bool allPages = false;
      bool moreItems = true;
      int count = 0;
      while (moreItems)
      {
        var found = exchange.FindItems(Microsoft.Exchange.WebServices.Data.WellKnownFolderName.Inbox, view);
        moreItems = found.MoreAvailable && allPages;
        if (moreItems)
        {
          view.Offset += pageSize;
        }
        foreach (Microsoft.Exchange.WebServices.Data.EmailMessage item in found.OrderBy(i => i.DateTimeReceived))
        {
          //item.Load(new Microsoft.Exchange.WebServices.Data.PropertySet(Microsoft.Exchange.WebServices.Data.BasePropertySet.FirstClassProperties));
          var body = "";//item.Body.Text;
          WriteLine($"\nIsRead:\t{item.IsRead}\nFrom:\t{item.From.Name}\nSubject:\t{item.Subject}\nReceived:\t{item.DateTimeReceived.ToString("MMMdd-HHmmss-fff")}\nBody:\t{body}\nCount:\t{++count}");
        }
      }
    }
    public void excep()
    {
      var exchange = GetExchangeService();

      var folder = listfolders(exchange, "Clutter");
      WriteLine($"Located folder: {folder.DisplayName}");

      var view = new Microsoft.Exchange.WebServices.Data.ItemView(10);
      view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Id,
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.From,
          Microsoft.Exchange.WebServices.Data.ItemSchema.Subject,
          Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived,
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.IsRead);

      view.OrderBy.Add(Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived, Microsoft.Exchange.WebServices.Data.SortDirection.Descending);
      var exception_filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, "Exception", Microsoft.Exchange.WebServices.Data.ContainmentMode.Substring, Microsoft.Exchange.WebServices.Data.ComparisonMode.IgnoreCaseAndNonSpacingCharacters);
      var needs_help_filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, "needs help", Microsoft.Exchange.WebServices.Data.ContainmentMode.Substring, Microsoft.Exchange.WebServices.Data.ComparisonMode.IgnoreCaseAndNonSpacingCharacters);
      var both = new Microsoft.Exchange.WebServices.Data.SearchFilter.SearchFilterCollection
      (
              Microsoft.Exchange.WebServices.Data.LogicalOperator.Or,
              exception_filter,
              needs_help_filter
      );
      Microsoft.Exchange.WebServices.Data.SearchFilter filter = null;
      if (excep_only && !needs) filter = exception_filter;
      else if (!excep_only && needs) filter = needs_help_filter;
      else filter = both;

      int pageSize = 10;
      bool allPages = false;
      bool moreItems = true;
      int count = 0;
      while (moreItems)
      {
        var found = exchange.FindItems(folder.Id, filter, view);
        moreItems = found.MoreAvailable && allPages;
        if (moreItems)
        {
          view.Offset += pageSize;
        }
        foreach (Microsoft.Exchange.WebServices.Data.EmailMessage item in found.OrderBy(i => i.DateTimeReceived))
        {
          string body = "";
          WriteLine($"\nIsRead:\t{item.IsRead}\nFrom:\t{item.From.Name}\nSubject:\t{item.Subject}\nReceived:\t{item.DateTimeReceived.ToString("MMMdd-HHmmss-fff")}\nBody:\t{body}\nCount:\t{++count}");
        }
      }
    }
    public void clean()
    {
      var exchange = GetExchangeService();

      string subject = "needs help";
      int pageSize = count <= 0 ? 10 : count;
      bool allPages = true;
      bool expand = false;
      count = 20;//00;

      int offset = 0;
      var view = new Microsoft.Exchange.WebServices.Data.ItemView(pageSize, offset);
      view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
      Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Id,
      Microsoft.Exchange.WebServices.Data.EmailMessageSchema.From,
      Microsoft.Exchange.WebServices.Data.ItemSchema.Subject,
      Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived,
      Microsoft.Exchange.WebServices.Data.EmailMessageSchema.IsRead);
      view.OrderBy.Add(Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived, Microsoft.Exchange.WebServices.Data.SortDirection.Descending);
      var filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, subject);

      var foldername = "Clutter";
      var target_folder = expand == false ? listfolders(exchange, foldername) : null;
      WriteLine($"Target folder: {(target_folder != null ? target_folder.DisplayName : "Deleted Items")}");
      do
      {
        bool moreItems = true;
        while (moreItems)
        {
          var found = expand ? exchange.FindItems(Microsoft.Exchange.WebServices.Data.WellKnownFolderName.DeletedItems, filter, view) : exchange.FindItems(target_folder.Id, filter, view);
          if (found.Any() == false) break;
          moreItems = found.MoreAvailable && allPages;
          if (moreItems)
          {
            view.Offset += pageSize;
          }

          var responses = exchange.DeleteItems(found.Select(msg => msg.Id), Microsoft.Exchange.WebServices.Data.DeleteMode.HardDelete, Microsoft.Exchange.WebServices.Data.SendCancellationsMode.SendToNone, Microsoft.Exchange.WebServices.Data.AffectedTaskOccurrence.AllOccurrences);
          foreach (var g in responses.GroupBy(resp => resp.Result))
          {
            WriteLine($"{g.Key}: {g.Count()}");
          }
          continue;
        }
      } while (true);
      WriteLine("Done.");
    }

    //.\tools\mail\orgmail.exe -send -subject="the here4" -file="doc111.htm"
    //.\tools\mail\orgmail.exe -send -subject="the here5" -pack="doc111.htm"
    public void send()
    {
      if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[To])) throw new ArgumentException("There is no configured recipients.");
      var msg = new Microsoft.Exchange.WebServices.Data.EmailMessage(GetExchangeService());
      msg.Subject = GetSubject();
      WriteLine($"Subject: [{msg.Subject}]");
      msg.Body = GetBody();
      SetRecipients(msg);
      msg.Send();
      WriteLine($"Send(): Done.");
    }
    string GetSubject()
    {
      var subjectline = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(subject)) subjectline.Append($"{subject}");
      if (pack?.Exists == true)
      {
        if (subjectline.Length > 0) subjectline.Append(" ");
        subjectline.Append($"{GetSubjectFromPackFile()}");
      }
      if (subjectfile?.Exists == true)
      {
        if (subjectline.Length > 0) subjectline.Append(" ");
        subjectline.Append($"{File.ReadAllText(subjectfile.FullName)}");
      }
      return $"{subjectline}";
    }
    Microsoft.Exchange.WebServices.Data.MessageBody GetBody()
    {
      var result = new Microsoft.Exchange.WebServices.Data.MessageBody();
      result.BodyType = text ? Microsoft.Exchange.WebServices.Data.BodyType.Text : Microsoft.Exchange.WebServices.Data.BodyType.HTML;
      WriteLine($"BodyType: {result.BodyType}");
      var payload = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(body)) payload.AppendLine(body);
      if (pack?.Exists == true) payload.Append($"{GetBodyFromPackFile()}");
      if (file?.Exists == true) payload.Append(File.ReadAllText(file.FullName));
      result.Text = $"{payload}";
      WriteLine($"Payload length: {result.Text.Length}");
      return result;
    }
    string GetSubjectFromPackFile()
    {
      if (pack?.Exists == false) return string.Empty;
      var subjectpack = new StringBuilder();
      var after_first_line = false;
      using (var reader = pack.OpenText())
        do
        {
          var line = reader.ReadLine();
          if (line == null) break;
          line = line.Trim();
          if (string.IsNullOrWhiteSpace(line)) break;
          if (after_first_line) subjectpack.AppendLine();
          subjectpack.Append(line);
          after_first_line = true;
        } while (true);
      return $"{subjectpack}";
    }
    string GetBodyFromPackFile()
    {
      if (pack?.Exists == false) return string.Empty;
      var bodypack = new StringBuilder();
      var isBody = false;
      using (var reader = pack.OpenText())
        do
        {
          var line = reader.ReadLine();
          if (line == null) break;
          line = line.Trim();
          if (!(isBody || string.IsNullOrWhiteSpace(line))) continue;
          if (!isBody && string.IsNullOrWhiteSpace(line)) { isBody = true; continue; };
          bodypack.AppendLine(line);
        } while (true);
      return $"{bodypack}";
    }
    void SetRecipients(Microsoft.Exchange.WebServices.Data.EmailMessage msg)
    {
      ConfigurationManager.AppSettings[To]?.Trim().Split('|').ToList().ForEach(t => msg.ToRecipients.Add(t.Trim()));
      ConfigurationManager.AppSettings[Bcc]?.Trim().Split('|').ToList().ForEach(t => msg.BccRecipients.Add(t.Trim()));
    }
    Microsoft.Exchange.WebServices.Data.ExchangeService GetExchangeService()
    {
      var ews_url = ConfigurationManager.AppSettings["EWS"];
      var address = ConfigurationManager.AppSettings["emailaddress"];
      var access = GetAccess();
      var exchange = new Microsoft.Exchange.WebServices.Data.ExchangeService(Microsoft.Exchange.WebServices.Data.ExchangeVersion.Exchange2013);
      exchange.Credentials = new Microsoft.Exchange.WebServices.Data.WebCredentials(address, access);
      exchange.Url = new Uri(ews_url);
      return exchange;
    }
    string GetAccess()
    {
      return
      //ReadLine();
      System.Configuration.ConfigurationManager.AppSettings["access"];
    }

    Microsoft.Exchange.WebServices.Data.Folder listfolders(Microsoft.Exchange.WebServices.Data.ExchangeService exchange, string target_folder)
    {
      WriteLine($"\n{nameof(listfolders)}:");

      var view = new Microsoft.Exchange.WebServices.Data.FolderView(50);
      view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
          Microsoft.Exchange.WebServices.Data.FolderSchema.Id,
          Microsoft.Exchange.WebServices.Data.FolderSchema.DisplayName);

      var filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.IsEqualTo(Microsoft.Exchange.WebServices.Data.FolderSchema.DisplayName, target_folder);

      var found = exchange.FindFolders(Microsoft.Exchange.WebServices.Data.WellKnownFolderName.MsgFolderRoot, filter, view);
      foreach (Microsoft.Exchange.WebServices.Data.Folder f in found.Folders)
      {
        WriteLine($"[{f.DisplayName,-30}]");//\t{f.Id}
        if (f.DisplayName == target_folder) return f;
      }
      throw new Exception($"{target_folder} not found.");
    }

  }
  public static void _Main(string[] args)
  {
    nutility.Switch.AsType<Input>(args);
  }
}