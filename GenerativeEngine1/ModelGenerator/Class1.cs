using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGenerator
{
  public class Class1
  {
    private System.Data.SqlClient.SqlConnection con;
    private Exception exception;

    public Class1()
    {
      OK = true;
    }
    public bool OK { get; private set; }

    public void Setup()
    {
      con = new System.Data.SqlClient.SqlConnection("");
      OK = con != null;

      open();
    }

    private void open()
    {
      exception = null;
      try
      {
        OK = false;
        con.Open();
        OK = con.State == System.Data.ConnectionState.Open;
      }
      catch (Exception ex)
      {
        exception = ex;
      }
    }

    public string GetGeneratedSummary() => $"//Generated summary: {exception?.Message ?? "Additional code to be generated here for not OK case."}";
  }
}