using System.Data.Common;
using System.Runtime.Serialization;

namespace Northwind.Orders.WebApi.Services;

[Serializable]
public class SqlScriptException : DbException
{
    public SqlScriptException()
    {
    }

    public SqlScriptException(string message)
        : base(message)
    {
    }

    public SqlScriptException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected SqlScriptException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
