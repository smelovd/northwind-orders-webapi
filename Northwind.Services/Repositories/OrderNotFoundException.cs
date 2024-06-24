using System.Runtime.Serialization;

namespace Northwind.Services.Repositories;

[Serializable]
public class OrderNotFoundException : Exception
{
    public OrderNotFoundException()
    {
    }

    public OrderNotFoundException(string message)
        : base(message)
    {
    }

    public OrderNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected OrderNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
