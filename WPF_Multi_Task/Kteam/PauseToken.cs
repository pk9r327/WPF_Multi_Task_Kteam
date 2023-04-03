using System.Threading;
using System.Threading.Tasks;
using WPF_Multi_Task.Kteam;

namespace WPF_Multi_Task.ViewModel
{
    // PauseToken - consumer side
    public struct PauseToken
    {
        readonly PauseTokenSource _source;

        public PauseToken(PauseTokenSource source) { _source = source; }

        public Task<bool> IsPaused() { return _source.IsPaused(); }

        public Task PauseIfRequestedAsync(CancellationToken token = default(CancellationToken))
        {
            return _source.PauseIfRequestedAsync(token);
        }
    }
}
