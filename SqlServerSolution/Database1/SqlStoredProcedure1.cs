using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

/*
Getting Started with CLR Integration
https://msdn.microsoft.com/en-us/library/ms131052(v=sql.110).aspx
*/
public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void SqlStoredProcedure1()
    {
      //SqlContext.Pipe.ExecuteAndSend(cmd);
    }
}