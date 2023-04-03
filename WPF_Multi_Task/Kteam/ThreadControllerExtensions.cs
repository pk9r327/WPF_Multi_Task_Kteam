using System;
using System.Threading;
using System.Threading.Tasks;
using WPF_Multi_Task.Kteam;

namespace WPF_Multi_Task.ViewModel
{
    public static class ThreadControllerExtensions
    {
        public static bool StopTask(this IThreadController controller)
        {
            if (controller.Task == null)
                return true;
            try
            {
                if (controller.Cts == null)
                    return true;
                controller.Cts.Cancel();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void TaskWait(this IThreadController controller,
            CancellationTokenSource TokenSrouce/*, PauseTokenSource PauseSource*/)
        {
            var ct = controller.StartTask(async () =>
            {

                while (true)
                {
                    try
                    {
                        TokenSrouce.Token.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        return;
                    }
                    //await PauseSource.Token.WaitWhilePausedAsync();

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }, TokenSrouce/*, PauseSource*/);
        }

        public static bool StartTask(this IThreadController controller,
            Action action, CancellationTokenSource TokenSrouce/*, PauseTokenSource PauseSource*/)
        {
            if (controller.Task != null)
            {
                controller.StopTask();
                controller.Task = null;
            }
            try
            {
                controller.Cts = TokenSrouce;
                //controller.pauseSource = PauseSource;

                if (TokenSrouce == null)
                {
                    controller.Task = Task.Run(() => { action(); });
                }
                else
                {
                    controller.Task = Task.Run(() => { action(); }, TokenSrouce.Token);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> PauseTaskAsync(this IThreadController controller)
        {
            if (controller.Task == null)
                return true;

            try
            {
                await controller.Pts.PauseAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> ResumeTaskAsync(this IThreadController controller)
        {
            if (controller.Task == null)
                return true;

            try
            {
                await controller.Pts.ResumeAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
