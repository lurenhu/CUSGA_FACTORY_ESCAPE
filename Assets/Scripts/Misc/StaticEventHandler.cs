using System;

public static class StaticEventHandler
{
    public static event Action<FirstClickNodeArgs> OnFirstClickNode;

    public static void CallFirstClickNode(Node node)
    {
        OnFirstClickNode?.Invoke(new FirstClickNodeArgs() {node = node});
    }

    public static event Action<SecondClickNodeArgs> OnSecondClickNode;

    public static void CallSecondClickNode(Node node)
    {
        OnSecondClickNode?.Invoke(new SecondClickNodeArgs() {node = node});
    }
}

public class FirstClickNodeArgs : EventArgs
{
    public Node node;
}

public class SecondClickNodeArgs : EventArgs
{
    public Node node;
}