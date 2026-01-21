using System;

public class ValueAlreadySetException : Exception
{
    public ValueAlreadySetException() { }
    public ValueAlreadySetException(string message) : base(message) { }
}
