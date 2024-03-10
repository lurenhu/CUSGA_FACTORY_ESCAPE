using System;
using UnityEngine.UIElements;

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

    public static event Action<CommitArgs> OnCommit;
    public static void CallCommit(int anxiety_change_value)
    {
        OnCommit?.Invoke(new CommitArgs() {anxiety_change_value = anxiety_change_value});
    }

    public static event Action<PopUpNodeArgs> OnPopUpNode;
    public static void CallPopUpNode(Node node)
    {
        OnPopUpNode?.Invoke(new PopUpNodeArgs() {node = node});
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

public class CommitArgs : EventArgs
{
    public int anxiety_change_value;
}

public class PopUpNodeArgs : EventArgs
{
    public Node node;
}