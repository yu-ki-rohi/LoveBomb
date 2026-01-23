using System;

public class GameObjectNotPlacedException : Exception
{
    public GameObjectNotPlacedException() { }
    public GameObjectNotPlacedException(string message) : base(message) { }
}
