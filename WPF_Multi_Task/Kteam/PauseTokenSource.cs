﻿using System.Threading;
using System.Threading.Tasks;
using WPF_Multi_Task.ViewModel;

namespace WPF_Multi_Task.Kteam
{
    public class PauseTokenSource
    {
        bool _paused = false;
        bool _pauseRequested = false;

        TaskCompletionSource<bool> _resumeRequestTcs;
        TaskCompletionSource<bool> _pauseConfirmationTcs;

        readonly SemaphoreSlim _stateAsyncLock = new SemaphoreSlim(1);
        readonly SemaphoreSlim _pauseRequestAsyncLock = new SemaphoreSlim(1);

        public PauseToken Token { get { return new PauseToken(this); } }

        public async Task<bool> IsPaused(CancellationToken token = default(CancellationToken))
        {
            await _stateAsyncLock.WaitAsync(token);
            try
            {
                return _paused;
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        public async Task ResumeAsync(CancellationToken token = default(CancellationToken))
        {
            await _stateAsyncLock.WaitAsync(token);
            try
            {
                if (!_paused)
                {
                    return;
                }

                await _pauseRequestAsyncLock.WaitAsync(token);
                try
                {
                    var resumeRequestTcs = _resumeRequestTcs;
                    _paused = false;
                    _pauseRequested = false;
                    _resumeRequestTcs = null;
                    _pauseConfirmationTcs = null;
                    resumeRequestTcs.TrySetResult(true);
                }
                finally
                {
                    _pauseRequestAsyncLock.Release();
                }
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        public async Task PauseAsync(CancellationToken token = default(CancellationToken))
        {
            await _stateAsyncLock.WaitAsync(token);
            try
            {
                if (_paused)
                {
                    return;
                }

                Task pauseConfirmationTask = null;

                await _pauseRequestAsyncLock.WaitAsync(token);
                try
                {
                    _pauseRequested = true;
                    _resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _pauseConfirmationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    pauseConfirmationTask = WaitForPauseConfirmationAsync(token);
                }
                finally
                {
                    _pauseRequestAsyncLock.Release();
                }

                await pauseConfirmationTask;

                _paused = true;
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        private async Task WaitForResumeRequestAsync(CancellationToken token)
        {
            using (token.Register(() => _resumeRequestTcs.TrySetCanceled(), useSynchronizationContext: false))
            {
                await _resumeRequestTcs.Task;
            }
        }

        private async Task WaitForPauseConfirmationAsync(CancellationToken token)
        {
            using (token.Register(() => _pauseConfirmationTcs.TrySetCanceled(), useSynchronizationContext: false))
            {
                await _pauseConfirmationTcs.Task;
            }
        }

        internal async Task PauseIfRequestedAsync(CancellationToken token = default(CancellationToken))
        {
            Task resumeRequestTask = null;

            await _pauseRequestAsyncLock.WaitAsync(token);
            try
            {
                if (!_pauseRequested)
                {
                    return;
                }
                resumeRequestTask = WaitForResumeRequestAsync(token);
                _pauseConfirmationTcs.TrySetResult(true);
            }
            finally
            {
                _pauseRequestAsyncLock.Release();
            }

            await resumeRequestTask;
        }
    }
}
