using System.Runtime.Serialization;

namespace Domain.Exceptions;

[Serializable]
public class UserException : Exception
{
    public UserException(string message) : base(message) { }
    
    protected UserException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
}