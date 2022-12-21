namespace PriorityQueue;

/// <summary>
/// Thread-safe implementation of a max priority queue.
/// </summary>
/// <typeparam name="TValue">Value type.</typeparam>
public class PriorityQueue<TValue>
{
    private readonly object lockObject = new();

    private readonly PriorityQueue<TValue, int> queue = new(new MaxComparer());
    private int size;

    /// <summary>
    /// Adds an element to the priority queue. Thread-safe.
    /// </summary>
    /// <param name="value">Value of the element.</param>
    /// <param name="priority">Priority of the element.</param>
    public void Enqueue(TValue value, int priority)
    {
        lock (this.lockObject)
        {
            this.queue.Enqueue(value, priority);
            Interlocked.Increment(ref this.size);
            Monitor.PulseAll(this.lockObject);
        }
    }

    /// <summary>
    /// Removes an element with the highest priority from the queue and returns it.
    /// If the queue is empty, waits until an element appears.
    /// Thread-safe.
    /// </summary>
    /// <returns>Element with the highest priority.</returns>
    public TValue Dequeue()
    {
        lock (this.lockObject)
        {
            if (this.queue.Count == 0)
            {
                Monitor.Wait(this.lockObject);
            }

            var element = this.queue.Dequeue();
            Interlocked.Decrement(ref this.size);
            return element;
        }
    }

    /// <summary>
    /// Returns size of the queue at some moment in the past.
    /// </summary>
    /// <returns>Size of the queue at some moment in the past.</returns>
    public int Size()
    {
        lock (this.lockObject)
        {
            return this.size;
        }
    }

    private class MaxComparer : IComparer<int>
    {
        /// <summary>
        /// Compares two objects. Returns a value less than zero if x is less than y,
        /// zero if x is equal to y,
        /// or a value greater than zero if x is greater than y.
        /// </summary>
        /// <param name="x">First number.</param>
        /// <param name="y">Second number.</param>
        public int Compare(int x, int y)
            => y - x;
    }
}