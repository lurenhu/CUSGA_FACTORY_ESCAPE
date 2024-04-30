using System;
using UnityEngine.UIElements;

public static class StaticEventHandler
{
    public static event Action<CommitArgs> OnCommit;
    public static void CallCommit(int anxiety_change_value)
    {
        OnCommit?.Invoke(new CommitArgs() {anxiety_change_value = anxiety_change_value});
    }

    public static event Action<StopTimingArgs> OnStopTiming;
    public static void CallStopTiming(Node node)
    {
        OnStopTiming?.Invoke(new StopTimingArgs() {node = node});
    }

    public static event Action<StartTimingArgs> OnStartTiming;
    public static void CallStartTiming(Node node)
    {
        OnStartTiming?.Invoke(new StartTimingArgs() {node = node});
    }

    public static event Action<GetNextNodeLevel> OnGetNextNodeLevel;
    public static void CallGetNextNodeLevel()
    {
        OnGetNextNodeLevel?.Invoke(new GetNextNodeLevel() {});
    }

}
public class CommitArgs : EventArgs
{
    public int anxiety_change_value;
}

public class StopTimingArgs : EventArgs
{
    public Node node;
}

public class StartTimingArgs : EventArgs
{
    public Node node;
}

public class GetNextNodeLevel : EventArgs
{
    
}
