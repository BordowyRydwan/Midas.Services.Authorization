using System.Runtime.Serialization;

namespace Domain.Exceptions;

[Serializable]
public class FamilyException : Exception
{
    public FamilyException(string message) : base(message) { }
    
    protected FamilyException(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
}