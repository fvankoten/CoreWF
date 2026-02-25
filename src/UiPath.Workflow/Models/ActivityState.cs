using System.Threading;

namespace UiPath.Workflow.Models;

public sealed class ActivityState<TState> where TState : class
{
    public CancellationTokenSource CancellationTokenSource { get; init; }
    public TState State { get; set; }
}
