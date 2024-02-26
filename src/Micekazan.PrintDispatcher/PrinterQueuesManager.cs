using System.Threading.Channels;
using Micekazan.PrintDispatcher.Domain.Contracts;

namespace Micekazan.PrintDispatcher;

public class PrinterQueuesManager
{
    private static readonly object LockObj = new();
    private readonly Dictionary<string, Channel<Document>> _channels = new();

    public Channel<Document> this[string printingToken]
    {
        get
        {
            lock (LockObj)
            {
                if (_channels.TryGetValue(printingToken, out var ch)) return ch;
                ch = Channel.CreateUnbounded<Document>();
                _channels.Add(printingToken, ch);
                return ch;
            }
        }
    }
}