// xcopy /v C:\design\github_spike\spike\Solution1\ConsoleApplication1\bin\Release\ConsoleApplication1.exe \tools\mail\orgmail.exe
// xcopy /v C:\design\github_spike\spike\Solution1\ConsoleApplication1\bin\Release\ConsoleApplication1.pdb \tools\mail\orgmail.pdb
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using static System.Console;

/*
Exchange Online and Exchange development
https://docs.microsoft.com/en-us/exchange/client-developer/exchange-server-development

Overview of Microsoft Graph
https://docs.microsoft.com/en-us/graph/overview
https://graph.microsoft.com
https://graph.microsoft.com/v1.0/me/messages
http://aka.ms/graph
https://developer.microsoft.com/en-us/graph
https://docs.microsoft.com/en-us/graph/api/resources/mail-api-overview?view=graph-rest-1.0
https://docs.microsoft.com/en-us/graph/auth/auth-concepts

https://developer.microsoft.com/en-us/graph/blogs/upcoming-changes-to-exchange-web-services-ews-api-for-office-365/
https://techcommunity.microsoft.com/t5/exchange-team-blog/upcoming-changes-to-exchange-web-services-ews-api-for-office-365/ba-p/608055
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-and-exchange-online-april-2020-update/ba-p/1275508
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-and-exchange-online-july-update/ba-p/1530163
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-and-exchange-online-february-2021-update/ba-p/2111904

https://stackoverflow.com/questions/32735584/o365-ews-managed-api-and-exchangeversion
https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/ews-schema-versions-in-exchange?redirectedfrom=MSDN

Introduction to GraphQL
aka.ms/graphql
https://graphql.org/learn/

EWS Managed API reference
https://docs.microsoft.com/en-us/exchange/client-developer/web-service-reference/ews-managed-api-reference-for-exchange
https://docs.microsoft.com/en-us/dotnet/api/microsoft.exchange.webservices.data?view=exchange-ews-api

Office 365 Management APIs overview
https://docs.microsoft.com/en-us/office/office-365-management-api/office-365-management-apis-overview
*/
static class orgmail
{
  class Input
  {
    private const string To = "to";
    private const string Bcc = "bcc";

    public bool excep_only, needs, html, confirm, expand, restart, soft, deleteditems;
    public int count, pageSize, offset, read;
    public Microsoft.Exchange.WebServices.Data.OffsetBasePoint offsetBasePoint;
    public string subject, body, folder;
    public FileInfo file, subjectfile, pack;
    public List<FileInfo> attachs;
    public bool ascii, utf7, utf8, utf32, unicode, latin1;
    public bool? allPages;

    private Microsoft.Exchange.WebServices.Data.ItemView view;
    private Microsoft.Exchange.WebServices.Data.SearchFilter filter;
    private Microsoft.Exchange.WebServices.Data.DeleteMode delete_mode;
    private string informative_subject;

    public void latest()
    {
      if (pageSize == 0) pageSize = 10;
      var _allpages = allPages ?? false;
      if (string.IsNullOrWhiteSpace(folder)) folder = "Inbox";

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      SetView();
      SetFilter();

      WriteLine($"Folder: [{folder}]");
      WriteLine($"subject: [{subject}]");

      bool moreItems = true;
      int count = 0;
      while (moreItems)
      {
        var found = string.IsNullOrWhiteSpace(subject) ? exchange.FindItems(target_folder.Id, view) : exchange.FindItems(target_folder.Id, filter, view);
        if (found.Any() == false) break;
        moreItems = found.MoreAvailable && _allpages;
        if (moreItems)
        {
          view.Offset += pageSize;
        }
        foreach (Microsoft.Exchange.WebServices.Data.EmailMessage item in found.OrderBy(i => i.DateTimeReceived))
        {
          ++count;
          if (count == read)
          {
            item.Load();
            WriteLine(item.Body.Text);
            return;
          }
          else if (read == 0)
          {
            WriteLine($"\nIsRead:\t{item.IsRead}\nFrom:\t{item.From.Name}\nSubject:\t{item.Subject}\nReceived:\t{item.DateTimeReceived:s}\nCount:\t{count}");
          }
        }
      }
    }
    public void excep()
    {
      if (pageSize == 0) pageSize = 10;
      var _allpages = allPages ?? false;
      if (string.IsNullOrWhiteSpace(folder)) folder = "Clutter";

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      SetView();
      SetFilter();

      WriteLine($"Folder: [{folder}]");
      if (subject != null)
      {
        WriteLine($"subject: [{subject}]");
      }
      if (informative_subject != null)
      {
        WriteLine($"informative subject: [{informative_subject}]");
      }

      int count = 0;
      bool moreItems = true;
      while (moreItems)
      {
        var found = exchange.FindItems(target_folder.Id, filter, view);
        if (found.Any() == false) break;
        moreItems = found.MoreAvailable && _allpages;
        if (moreItems)
        {
          view.Offset += pageSize;
        }
        foreach (Microsoft.Exchange.WebServices.Data.EmailMessage item in found.OrderBy(i => i.DateTimeReceived))
        {
          WriteLine($"\nIsRead:\t{item.IsRead}\nFrom:\t{item.From.Name}\nSubject:\t{item.Subject}\nReceived:\t{item.DateTimeReceived:s}\nCount:\t{++count}");
        }
      }
    }
    public void clean()// -clean [-pageSize=200] [-restart] [-soft|-deleteditems]
    {
      if (pageSize == 0) pageSize = 10;
      var _allpages = allPages ?? true;
      if (string.IsNullOrWhiteSpace(folder)) folder = "Clutter";

      if (soft) delete_mode = Microsoft.Exchange.WebServices.Data.DeleteMode.SoftDelete;
      else if (deleteditems) delete_mode = Microsoft.Exchange.WebServices.Data.DeleteMode.MoveToDeletedItems;
      else delete_mode = Microsoft.Exchange.WebServices.Data.DeleteMode.HardDelete;

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      do
      {
        SetView();
        SetFilter();
        var rand = new Random();

        WriteLine($"Folder: [{folder}]");
        if (subject != null)
        {
          WriteLine($"subject: [{subject}]");
        }
        if (informative_subject != null)
        {
          WriteLine($"informative subject: [{informative_subject}]");
        }
        WriteLine($"DeleteMode: {delete_mode}");
        if (confirm)
        {
          Write("Type 'YES' to DeleteItems: ");
          if (ReadLine() != "YES")
          {
            WriteLine("\nCancelled by the user: DeleteItems was not executed.");
            return;
          }
        }

        bool moreItems = true;
        while (moreItems)
        {
          var found = exchange.FindItems(target_folder.Id, filter, view);
          if (found.Any() == false) break;
          moreItems = found.MoreAvailable && _allpages;
          if (moreItems)
          {
            view.Offset += pageSize;
          }

          var responses = exchange.DeleteItems(found.Select(msg => msg.Id), delete_mode, Microsoft.Exchange.WebServices.Data.SendCancellationsMode.SendToNone, Microsoft.Exchange.WebServices.Data.AffectedTaskOccurrence.AllOccurrences);
          foreach (var g in responses.GroupBy(resp => resp.Result))
          {
            WriteLine($"{g.Key}: {g.Count()}");
          }
          continue;
        }
        if (restart)
        {
          var sleep = rand.Next(30000, 60000);
          Write($"Waiting for {sleep / 1000:N0}s...");
          System.Threading.Thread.Sleep(sleep);
          WriteLine($"\nRestarting...");
        }
      } while (restart);
      WriteLine("Done.");
    }

    //.\tools\mail\orgmail.exe -send -subject="the here4" -file="doc111.htm"
    //.\tools\mail\orgmail.exe -send -subject="the here5" -pack="doc111.htm"
    private Encoding encoding;
    public void send()
    {
      if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[To])) throw new ArgumentException("There is no configured recipients.");
      var msg = new Microsoft.Exchange.WebServices.Data.EmailMessage(GetExchangeService());
      SetEncoding();
      WriteLine($"Encoding: {encoding}");
      msg.Subject = GetSubject();
      WriteLine($"Subject: [{msg.Subject}]");
      msg.Body = GetBody();
      if (expand)
      {
        WriteLine($"Body: [{msg.Body}]");
      }
      SetRecipients(msg);
      SetAttachments(msg);
      if (confirm)
      {
        Write("Type 'YES' to Send: ");
        if (ReadLine() != "YES")
        {
          WriteLine("\nCancelled by the user: Send was not executed.");
          return;
        }
      }
      msg.Send();
      WriteLine($"Send(): Done.");
    }
    public DirectoryInfo outdir;
    public void download()
    {
      if (outdir == null) outdir = new DirectoryInfo(Environment.CurrentDirectory);
      if (pageSize == 0) pageSize = 10;
      var _allpages = allPages ?? false;
      if (string.IsNullOrWhiteSpace(folder)) folder = "Inbox";

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      SetView();
      SetFilter();

      WriteLine($"Folder: [{folder}]");
      WriteLine($"subject: [{subject}]");
      WriteLine($"outdir: [{outdir.FullName}]");

      bool moreItems = true;
      int count = 0;
      while (moreItems)
      {
        var found = string.IsNullOrWhiteSpace(subject) ? exchange.FindItems(target_folder.Id, view) : exchange.FindItems(target_folder.Id, filter, view);
        if (found.Any() == false) break;
        moreItems = found.MoreAvailable && _allpages;
        if (moreItems)
        {
          view.Offset += pageSize;
        }
        foreach (Microsoft.Exchange.WebServices.Data.EmailMessage item in found.OrderBy(i => i.DateTimeReceived))
        {
          ++count;
          item.Load();
          WriteLine($"\nIsRead:\t{item.IsRead}\nFrom:\t{item.From.Name}\nSubject:\t{item.Subject}\nReceived:\t{item.DateTimeReceived:s}\nCount:\t{count}");
          WriteLine($"Attachments:\t{item.Attachments.Count}");
          foreach (var _attach in item.Attachments)
          {
            var attach = _attach as Microsoft.Exchange.WebServices.Data.FileAttachment;
            if (attach == null)
            {
              WriteLine($"Not supported attachment type ({_attach.GetType().FullName}");
              continue;
            }
            var downfile = Path.Combine(outdir.FullName, attach.Name);
            Write($"DownloadTo: {downfile}");
            attach.Load(downfile);
            WriteLine();
          }
        }
      }
    }
    void SetEncoding()
    {
      if(ascii) encoding = Encoding.ASCII;
      else if (utf7) encoding = Encoding.UTF7;
      else if (utf8) encoding = Encoding.UTF8;
      else if (utf32) encoding = Encoding.UTF32;
      else if (unicode) encoding = Encoding.Unicode;
      else if (latin1) encoding = Encoding.GetEncoding("ISO-8859-1");
      else encoding = Encoding.UTF8;
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
      result.BodyType = html ? Microsoft.Exchange.WebServices.Data.BodyType.HTML : Microsoft.Exchange.WebServices.Data.BodyType.Text;
      WriteLine($"BodyType: {result.BodyType}");
      var payload = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(body)) payload.AppendLine(body);
      if (pack?.Exists == true) payload.Append($"{GetBodyFromPackFile()}");
      if (file?.Exists == true) payload.Append(File.ReadAllText(file.FullName, encoding));
      result.Text = $"{payload}";
      WriteLine($"Payload length: {result.Text.Length}");
      return result;
    }
    string GetSubjectFromPackFile()
    {
      if (pack?.Exists == false) return string.Empty;
      var subjectpack = new StringBuilder();
      var after_first_line = false;
      using (var reader = new StreamReader(pack.FullName, encoding))
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
      const string attach_directive = "@attach=";

      if (pack?.Exists == false) return string.Empty;
      var bodypack = new StringBuilder();
      var isBody = false;
      using (var reader = new StreamReader(pack.FullName, encoding))
        do
        {
          var line = reader.ReadLine();
          if (line == null) break;
          if (!(isBody || string.IsNullOrWhiteSpace(line))) continue;
          if (!isBody && string.IsNullOrWhiteSpace(line)) { isBody = true; continue; };
          if (line.StartsWith(attach_directive))
          {
            var attach_file = new FileInfo(line.Substring(attach_directive.Length));
            if (attach_file.Exists)
            {
              if (attachs == null) attachs = new List<FileInfo>();
              attachs.Add(attach_file);
            }
            else bodypack.AppendLine($"[not found] {line}");
          }
          else bodypack.AppendLine(line);
        } while (true);
      return $"{bodypack}";
    }
    void SetRecipients(Microsoft.Exchange.WebServices.Data.EmailMessage msg)
    {
      ConfigurationManager.AppSettings[To]?.Trim().Split('|').ToList().ForEach(t => msg.ToRecipients.Add(t.Trim()));
      ConfigurationManager.AppSettings[Bcc]?.Trim().Split('|').ToList().ForEach(t => msg.BccRecipients.Add(t.Trim()));
    }
    void SetAttachments(Microsoft.Exchange.WebServices.Data.EmailMessage msg)
    {
      if (attachs?.Count > 0) attachs.ForEach(attach =>
      {
        if (attach.Exists)
        {
          Write($"\tAttaching {attach.FullName}...");
          msg.Attachments.AddFileAttachment(attach.FullName);
          WriteLine($"Done.");
        }
        else
        {
          WriteLine($"\tAttach not found: {attach.FullName}");
        }
      });
      WriteLine($"Attachments count: {msg.Attachments.Count}");
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
      //System.Configuration.ConfigurationManager.AppSettings["access"];
      //StringCipher.Decrypt(System.Configuration.ConfigurationManager.AppSettings["access"], System.Configuration.ConfigurationManager.AppSettings["key"]);
      GetSecret();
    }
    string GetSecret()
    {
      var x = new encod.sencod();
      return x.Open(System.Configuration.ConfigurationManager.AppSettings["access"]);
    }
    Microsoft.Exchange.WebServices.Data.Folder GetTargetFolder(Microsoft.Exchange.WebServices.Data.ExchangeService exchange, string foldername)
    {
      if (string.IsNullOrWhiteSpace(foldername))
      {
        throw new ArgumentNullException($"{nameof(foldername)}", "Folder name cannot be null or empty.");
      }
      Microsoft.Exchange.WebServices.Data.Folder target_folder = null;
      var stepline = foldername.Split('\\', '/', '|').Select(n => n.Trim()).Where(fname => string.IsNullOrWhiteSpace(fname) == false);
      Microsoft.Exchange.WebServices.Data.FolderId parentfolder = null;
      foreach (var step in stepline)
      {
        target_folder = listfolders(exchange, step, parentfolder);
        if (target_folder == null) throw new Exception($"Folder '{foldername}' not found.");
        parentfolder = target_folder.Id;
      }
      return target_folder;
    }
    Microsoft.Exchange.WebServices.Data.Folder listfolders(Microsoft.Exchange.WebServices.Data.ExchangeService exchange, string foldername, Microsoft.Exchange.WebServices.Data.FolderId parentfolder = null)
    {
      WriteLine($"\n{nameof(listfolders)}:");

      var view = new Microsoft.Exchange.WebServices.Data.FolderView(50);
      view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
          Microsoft.Exchange.WebServices.Data.FolderSchema.Id,
          Microsoft.Exchange.WebServices.Data.FolderSchema.ChildFolderCount,
          Microsoft.Exchange.WebServices.Data.FolderSchema.DisplayName);

      var filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.IsEqualTo(Microsoft.Exchange.WebServices.Data.FolderSchema.DisplayName, foldername);

      var found = parentfolder == null ? exchange.FindFolders(Microsoft.Exchange.WebServices.Data.WellKnownFolderName.MsgFolderRoot, filter, view) : exchange.FindFolders(parentfolder, filter, view);
      foreach (Microsoft.Exchange.WebServices.Data.Folder f in found.Folders)
      {
        WriteLine($"[{f.DisplayName,-30}]");//\t{f.Id}
        if (f.DisplayName == foldername) return f;
      }
      return null;
    }
    void SetView()
    {
      view = new Microsoft.Exchange.WebServices.Data.ItemView(pageSize, offset, offsetBasePoint);
      view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Id,
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.From,
          Microsoft.Exchange.WebServices.Data.ItemSchema.Subject,
          Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived,
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.IsRead);
      view.OrderBy.Add(Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived, Microsoft.Exchange.WebServices.Data.SortDirection.Descending);
    }
    void SetFilter()
    {
      informative_subject = null;
      if (string.IsNullOrWhiteSpace(subject) == false)
      {
        var subject_filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, subject, Microsoft.Exchange.WebServices.Data.ContainmentMode.Substring, Microsoft.Exchange.WebServices.Data.ComparisonMode.IgnoreCaseAndNonSpacingCharacters);
        filter = subject_filter;
      }
      else
      {
        var exceptionText_in_subject = "Exception";
        var needsText_in_subject = "needs help";
        var exception_filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, exceptionText_in_subject, Microsoft.Exchange.WebServices.Data.ContainmentMode.Substring, Microsoft.Exchange.WebServices.Data.ComparisonMode.IgnoreCaseAndNonSpacingCharacters);
        var needs_help_filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, needsText_in_subject, Microsoft.Exchange.WebServices.Data.ContainmentMode.Substring, Microsoft.Exchange.WebServices.Data.ComparisonMode.IgnoreCaseAndNonSpacingCharacters);
        var both = new Microsoft.Exchange.WebServices.Data.SearchFilter.SearchFilterCollection
        (
          Microsoft.Exchange.WebServices.Data.LogicalOperator.Or,
          exception_filter,
          needs_help_filter
        );
        filter = null;
        var visible_subject_exceptionFilter = $"'{exceptionText_in_subject}'";
        var visible_subject_needsFilter = $"'{needsText_in_subject}'";
        var default_subject_filter_label = "Default subject filter";
        if (excep_only && !needs)
        {
          filter = exception_filter;
          informative_subject = $"({default_subject_filter_label}: {visible_subject_exceptionFilter})";
        }
        else if (!excep_only && needs)
        {
          filter = needs_help_filter;
          informative_subject = $"({default_subject_filter_label}): {visible_subject_needsFilter})";
        }
        else
        {
          filter = both;
          informative_subject = $"({default_subject_filter_label}: {visible_subject_exceptionFilter} OR {visible_subject_needsFilter})";
        }
      }
    }

    #region call StringCipher
    public string plain, secret, key;
    public void close()
    {
      WriteLine(StringCipher.Encrypt(plain, key));
    }
    public void open()
    {
      WriteLine(StringCipher.Decrypt(secret, key));
    }
    #endregion
  }

  /*
https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
https://tekeye.uk/visual_studio/encrypt-decrypt-c-sharp-string
https://docs.microsoft.com/en-us/dotnet/standard/security/walkthrough-creating-a-cryptographic-application
  */
  public static class StringCipher
  {
    // This constant is used to determine the keysize of the encryption algorithm in bits.
    // We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int Keysize = 256;

    // This constant determines the number of iterations for the password bytes generation function.
    private const int DerivationIterations = 1000;

    public static string Encrypt(string plainText, string passPhrase)
    {
      // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
      // so that the same Salt and IV values can be used when decrypting.
      var saltStringBytes = Generate256BitsOfRandomEntropy();
      var ivStringBytes = Generate256BitsOfRandomEntropy();
      var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
      using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
      {
        var keyBytes = password.GetBytes(Keysize / 8);
        using (var symmetricKey = new RijndaelManaged())
        {
          symmetricKey.BlockSize = 256;
          symmetricKey.Mode = CipherMode.CBC;
          symmetricKey.Padding = PaddingMode.PKCS7;
          using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
          {
            using (var memoryStream = new MemoryStream())
            {
              using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
              {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                var cipherTextBytes = saltStringBytes;
                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                return Convert.ToBase64String(cipherTextBytes);
              }
            }
          }
        }
      }
    }

    public static string Decrypt(string cipherText, string passPhrase)
    {
      // Get the complete stream of bytes that represent:
      // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
      var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
      // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
      var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
      // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
      var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
      // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
      var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

      using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
      {
        var keyBytes = password.GetBytes(Keysize / 8);
        using (var symmetricKey = new RijndaelManaged())
        {
          symmetricKey.BlockSize = 256;
          symmetricKey.Mode = CipherMode.CBC;
          symmetricKey.Padding = PaddingMode.PKCS7;
          using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
          {
            using (var memoryStream = new MemoryStream(cipherTextBytes))
            {
              using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
              {
                var plainTextBytes = new byte[cipherTextBytes.Length];
                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
              }
            }
          }
        }
      }
    }

    private static byte[] Generate256BitsOfRandomEntropy()
    {
      var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
      using (var rngCsp = new RNGCryptoServiceProvider())
      {
        // Fill the array with cryptographically secure random bytes.
        rngCsp.GetBytes(randomBytes);
      }
      return randomBytes;
    }
  }

  public static void _Main(string[] args)
  {
    if (args?.Length == 0) nutility.Switch.ShowUsage(typeof(Input));
    else nutility.Switch.AsType<Input>(args);
  }
}