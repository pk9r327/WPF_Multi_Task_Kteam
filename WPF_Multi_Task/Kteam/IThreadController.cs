using System.Threading;
using System.Threading.Tasks;

namespace WPF_Multi_Task.Kteam
{
    public interface IThreadController
    {
        Task Task { get; set; }
        CancellationTokenSource Cts { get; set; }
        PauseTokenSource Pts { get; set; }
    }
}
