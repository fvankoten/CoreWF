// This file is part of Core WF which is licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Workflow.Models;

namespace System.Activities;

public abstract class AsyncTaskCodeActivity : TaskCodeActivity<object, object>
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public new object Result { get; set; }

    protected abstract Task ExecuteAsync(AsyncCodeActivityContext context, object state, CancellationToken cancellationToken);

    private protected sealed override async Task<object> ExecuteAsyncCore(AsyncCodeActivityContext context, object state, CancellationToken cancellationToken)
    {
        await ExecuteAsync(context, state, cancellationToken);
        return null;
    }
}

public abstract class AsyncTaskCodeActivity<TResult> : TaskCodeActivity<TResult, object>
{
    protected abstract Task<TResult> ExecuteAsync(AsyncCodeActivityContext context, object state, CancellationToken cancellationToken);
    private protected sealed override Task<TResult> ExecuteAsyncCore(AsyncCodeActivityContext context, object state, CancellationToken cancellationToken)
    {
        return ExecuteAsync(context, state, cancellationToken);
    }
}

public abstract class AsyncTaskCodeActivity<TResult, TState> : TaskCodeActivity<TResult, TState> where TState : class
{
    protected abstract Task<TResult> ExecuteAsync(AsyncCodeActivityContext context, TState state, CancellationToken cancellationToken);

    private protected sealed override Task<TResult> ExecuteAsyncCore(AsyncCodeActivityContext context, TState state, CancellationToken cancellationToken)
    {
        return ExecuteAsync(context, state, cancellationToken);
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class TaskCodeActivity<TResult, TState> : AsyncCodeActivity<TResult> where TState : class
{
    protected sealed override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
    {
        var cts = new CancellationTokenSource();

        var userState = PreExecute(context);

        context.UserState = new ActivityState<TState>()
        {
            CancellationTokenSource = cts,   
            State = userState
        };

        var task = ExecuteAsyncCore(context, userState, cts.Token);

        return TaskToAsyncResult.Begin(task, callback, state);
    }

    protected sealed override TResult EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
    {
        var activityState = (ActivityState<TState>)context.UserState;

        using (activityState.CancellationTokenSource)
        {
            var taskResult = TaskToAsyncResult.End<TResult>(result);

            var activityResult = PostExecute(context, activityState.State, taskResult);

            return activityResult;
        }
    }

    protected virtual TState PreExecute(AsyncCodeActivityContext context)
    {
        return null;
    }

    protected virtual TResult PostExecute(AsyncCodeActivityContext context, TState activityState, TResult result)
    {
        return result;
    }

    protected sealed override void Cancel(AsyncCodeActivityContext context)
    {
        var activityState = (ActivityState<TState>)context.UserState;

        activityState.CancellationTokenSource.Cancel();
    }

    private protected abstract Task<TResult> ExecuteAsyncCore(AsyncCodeActivityContext context, TState? state, CancellationToken cancellationToken);
}
