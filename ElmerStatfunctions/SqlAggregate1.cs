using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedAggregate(Format.Native)]
public struct SqlAggregate1
{
    public void Init()
    {
        // Put your code here
    }

    public void Accumulate(SqlString Value)
    {
        // Put your code here
    }

    public void Merge (SqlAggregate1 Group)
    {
        // Put your code here
    }

    public SqlString Terminate ()
    {
        // Put your code here
        return new SqlString (string.Empty);
    }

    // This is a place-holder member field
    public int _var1;
}
