using System.Runtime.Serialization;

namespace Domain.Exceptions;

public class PasswordException : Exception
{
    public PasswordException(string message) : base(message) { }
    
    protected PasswordException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
}