using System.Diagnostics;

namespace Otus.IntraProccessIteraction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] array1 = [.. Enumerable.Range(1, 100000).Select(x => x % 10 + 1)];
            int[] array2 = [.. Enumerable.Range(1, 1000000).Select(x => x % 10 + 1)];
            int[] array3 = [.. Enumerable.Range(1, 10000000).Select(x => x % 10 + 1)];

            var sw = new Stopwatch();

            // Обычное вычисление
            sw.Restart();
            var seqSum = SequentialSum(array1);
            sw.Stop();
            ShowResults("Последовательный метод", array1, sw, seqSum);
            seqSum = 0;

            sw.Restart();
            seqSum = SequentialSum(array2);
            sw.Stop();
            ShowResults("Последовательный метод", array2, sw, seqSum);
            seqSum = 0;

            sw.Restart();
            seqSum = SequentialSum(array3);
            sw.Stop();
            ShowResults("Последовательный метод", array3, sw, seqSum);
            seqSum = 0;

            // Параллельное вычисление
            sw.Restart();
            var threadsSum = ParallelThreadsSum(array1);
            sw.Stop();
            ShowResults("Параллельный метод с потоками", array1, sw, threadsSum);
            threadsSum = 0;

            sw.Restart();
            threadsSum = ParallelThreadsSum(array2);
            sw.Stop();
            ShowResults("Параллельный метод с потоками", array2, sw, threadsSum);
            threadsSum = 0;

            sw.Restart();
            threadsSum = ParallelThreadsSum(array3);
            sw.Stop();
            ShowResults("Параллельный метод с потоками", array3, sw, threadsSum);
            threadsSum = 0;

            // Параллельное вычисление с использованием PLINQ
            sw.Restart();
            var linqSum = ParallelLinqSum(array1);
            sw.Stop();
            ShowResults("Параллельный метод с LINQ", array1, sw, linqSum);
            linqSum = 0;

            sw.Restart();
            linqSum = ParallelLinqSum(array2);
            sw.Stop();
            ShowResults("Параллельный метод с LINQ", array2, sw, linqSum);
            linqSum = 0;

            sw.Restart();
            linqSum = ParallelLinqSum(array3);
            sw.Stop();
            ShowResults("Параллельный метод с LINQ", array3, sw, linqSum);
            linqSum = 0;
        }

        private static void ShowResults(string methodType, int[] array, Stopwatch sw, long sum)
        {
            Console.WriteLine($"{methodType}, размер {array.Length}: сумма={sum}, время={sw.ElapsedMilliseconds} мс");
        }


        // Последовательный метод
        static long SequentialSum(int[] arr)
        {
            long sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                sum += arr[i];
            }
            return sum;
        }

        static long ParallelThreadsSum(int[] arr)
        {
            long sum = 0;
            var syncLock = new object();

            var thread = new Thread(() =>
            {

                for (var i = 0; i < arr.Length / 2; i++)
                {
                    lock (syncLock)
                    {
                        sum += arr[i];
                    }
                }
            });
            thread.Start();

            for (var i = arr.Length / 2; i < arr.Length; i++)
            {
                lock (syncLock)
                {
                    sum += arr[i];
                }
            }

            thread.Join();
            return sum;
        }

        static long ParallelLinqSum(int[] arr)
        {
            return arr.AsParallel().Sum();
        }
    }
}
