using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Multi_Task.Kteam;
using WPF_Multi_Task.Model;

namespace WPF_Multi_Task.ViewModels
{
    public class ProfileViewModel : BaseViewModel, IThreadController, IDisposable
    {
        private bool disposedValue;

        #region IThreadController
        public Task Task { get; set; }
        public CancellationTokenSource Cts { get; set; }
        public PauseTokenSource Pts { get; set; }
        #endregion

        #region Property
        private ProfileDetail _ProfileDetail;

        public ProfileDetail ProfileDetail
        {
            get => _ProfileDetail;
            set => SetProperty(ref _ProfileDetail, value);
        }
        public ICollection<ProfileViewModel> Container { get; set; }

        #endregion

        #region Commands
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand DeleteCommand { get; }
        #endregion

        public ProfileViewModel()
        {
            StartCommand = new Command(Start);
            StopCommand = new Command(Stop);
            PauseCommand = new Command(Pause);
            ResumeCommand = new Command(Resume);
            DeleteCommand = new AsyncCommand(DeleteAsync);
        }

        void Start()
        {
            this.Cts = new CancellationTokenSource();

            Task = Task.Run(async () =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    ProfileDetail.Status = $"Working...{DateTime.Now.Second}";
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    //try
                    //{
                    //    sourceToken.Token.ThrowIfCancellationRequested();
                    //}
                    //catch
                    //{
                    //    return;
                    //}
                    //await pauseToken.Token.WaitWhilePausedAsync();
                }
            }, Cts.Token);

            //var sourceToken = new CancellationTokenSource();
            ////var pauseToken = new PauseTokenSource();
            //var ct = this.StartTask(async () =>
            //{
            //    while (true)
            //    {
            //        ProfileDetail.Status = $"Working...{DateTime.Now.Second}";
            //        await Task.Delay(TimeSpan.FromSeconds(1));
            //        try
            //        {
            //            sourceToken.Token.ThrowIfCancellationRequested();
            //        }
            //        catch
            //        {
            //            return;
            //        }
            //        //await pauseToken.Token.WaitWhilePausedAsync();
            //    }
            //}, sourceToken/*, pauseToken*/);
        }

        void Stop()
        {
            ProfileDetail.Status = "Being stop";

            Cts?.Cancel();

            ProfileDetail.Status = "Stop";
        }

        void Pause()
        {
            //if (this.PauseTask())
            //{
            //    ProfileDetail.Status = "Paused";
            //}
        }

        void Resume()
        {
            //if (this.ResumeTask())
            //{
            //    ProfileDetail.Status = "Resumed";
            //}
        }

        async Task DeleteAsync()
        {
            Stop();

            ProfileDetail.Status = $"Deleting Profile";
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ProfileDetail.Status = $"Removing";
                Container.Remove(this);
                Container.UpdateProfileIndex();
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    if (!Task.IsCanceled)
                        Task.Wait();

                    Task.Dispose();
                    Cts.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ProfileViewModel()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
