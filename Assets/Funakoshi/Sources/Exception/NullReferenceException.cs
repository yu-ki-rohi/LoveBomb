using System;

public class NullReferenceException : Exception
{
    public NullReferenceException() : base() { }
    public NullReferenceException(string message) : base(message) { }
}
