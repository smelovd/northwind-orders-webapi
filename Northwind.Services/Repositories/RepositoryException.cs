using System.Runtime.Serialization;

namespace Northwind.Services.Repositories;

/// <summary>
/// Represents an exception thrown by a repository method.
/// </summary>
[Serializable]
public class RepositoryException : Exception
{
    public RepositoryException()
    {
    }

    public RepositoryException(string message)
        : base(message)
    {
    }

    public RepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected RepositoryException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
