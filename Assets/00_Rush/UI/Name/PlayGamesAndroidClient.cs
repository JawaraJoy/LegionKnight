using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Rush
{
    internal sealed class PlayGamesAndroidClient
    {
        private const string c_Bridge = "com.jawarajoy.aetherjump.auth.PlayGamesLogin";
        private const int c_TimeoutSeconds = 90;
        internal async Task<string> RequestCodeAsync(bool interactive)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            using var bridge = new AndroidJavaClass(c_Bridge);
            var result = new CodeCallback();
            bridge.CallStatic("request", activity, interactive, result);
            var task = result.Completion.Task;
            if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(c_TimeoutSeconds))) != task)
                throw new TimeoutException();
            return await task;
#else
            await Task.CompletedTask;
            throw new PlatformNotSupportedException();
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        [Preserve]
        private sealed class CodeCallback : AndroidJavaProxy
        {
            internal readonly TaskCompletionSource<string> Completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
            internal CodeCallback() : base(c_Bridge + "$Callback") { }
            [Preserve] public void complete(string code)
            {
                if (string.IsNullOrEmpty(code)) Completion.TrySetException(new InvalidOperationException("Play Games authorization unavailable."));
                else Completion.TrySetResult(code);
            }
        }
#endif
    }
}
