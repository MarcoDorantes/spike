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

    public bool excep_only, needs, html;
    public int count;
    public string subject, body;
    public FileInfo file;

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

    public void send()
    {
      if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[To])) throw new ArgumentException("There is no configured recipients.");
      var msg = new Microsoft.Exchange.WebServices.Data.EmailMessage(GetExchangeService());
      msg.Subject = subject;
      msg.Body = GetBody();
      SetRecipients(msg);
      msg.Send();
    }
    Microsoft.Exchange.WebServices.Data.MessageBody GetBody()
    {
      var result = new Microsoft.Exchange.WebServices.Data.MessageBody();
      result.BodyType = html ? Microsoft.Exchange.WebServices.Data.BodyType.HTML : Microsoft.Exchange.WebServices.Data.BodyType.Text;
      var payload = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(body)) payload.Append(body);
      if (file != null) payload.Append(File.ReadAllText(file.FullName));
      result.Text = $"{payload}";
      return result;
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