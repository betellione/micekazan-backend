using System.Runtime.CompilerServices;

namespace WebApp1.Extensions;

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<IEnumerable<T>> BatchAsync<T>(this IAsyncEnumerable<T> source, int batchSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var batch = new List<T>(batchSize);

        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            batch.Add(item);
            if (batch.Count < batchSize) continue;
            yield return batch;
            batch = new List<T>(batchSize);
        }

        if (batch.Count > 0) yield return batch;
    }
}