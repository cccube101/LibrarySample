using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Helper
{
    public static class Tasks
    {
        // ---------------------------- Property
        public static TweenCancelBehaviour TCB => TweenCancelBehaviour.KillAndCancelAwait;

        // ---------------------------- PublicMethod
        public static async UniTask DelayTime(float time, CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time), true, cancellationToken: ct);
        }

        public static async UniTask Canceled(UniTask task)
        {
            if (await task.SuppressCancellationThrow()) { return; }
        }
    }
}