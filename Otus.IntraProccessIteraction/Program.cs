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
            var text = "Последовательный метод";
            sw.Restart();
            var seqSum = SequentialSum(array1);
            sw.Stop();
            ShowResults(text, array1, sw, seqSum);
            seqSum = 0;

            sw.Restart();
            seqSum = SequentialSum(array2);
            sw.Stop();
            ShowResults(text, array2, sw, seqSum);
            seqSum = 0;

            sw.Restart();
            seqSum = SequentialSum(array3);
            sw.Stop();
            ShowResults(text, array3, sw, seqSum);
            seqSum = 0;

            // Параллельное вычисление
            text = "Параллельный метод с потоками";
            sw.Restart();
            var threadsSum = ParallelThreadsSum(array1);
            sw.Stop();
            ShowResults(text, array1, sw, threadsSum);
            threadsSum = 0;

            sw.Restart();
            threadsSum = ParallelThreadsSum(array2);
            sw.Stop();
            ShowResults(text, array2, sw, threadsSum);
            threadsSum = 0;

            sw.Restart();
            threadsSum = ParallelThreadsSum(array3);
            sw.Stop();
            ShowResults(text, array3, sw, threadsSum);
            threadsSum = 0;

            // Параллельное вычисление с использованием PLINQ
            text = "Параллельный метод с LINQ";
            sw.Restart();
            var linqSum = ParallelLinqSum(array1);
            sw.Stop();
            ShowResults(text, array1, sw, linqSum);
            linqSum = 0;

            sw.Restart();
            linqSum = ParallelLinqSum(array2);
            sw.Stop();
            ShowResults(text, array2, sw, linqSum);
            linqSum = 0;

            sw.Restart();
            linqSum = ParallelLinqSum(array3);
            sw.Stop();
            ShowResults(text, array3, sw, linqSum);
            linqSum = 0;
        }

        private static void ShowResults(string methodType, int[] array, Stopwatch sw, long sum)
        {
            Console.WriteLine($"{methodType}, размер {array.Length}: сумма={sum}, время={sw.ElapsedMilliseconds} мс");
        }


        static long SequentialSum(int[] array)
        {
            var sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            return sum;
        }

        static long ParallelThreadsSum(int[] array)
        {
            var numThreads = Environment.ProcessorCount;
            var partialResults = new long[numThreads];
            var threads = new Thread[numThreads];

            var segmentSize = array.Length / numThreads;
            for (int i = 0; i < numThreads; i++)
            {
                int startIndex = i * segmentSize;
                int endIndex = (i == numThreads - 1) ? array.Length : (startIndex + segmentSize);

                int localI = i;
                threads[i] = new Thread(() => CalculatePartialSum(localI, array, startIndex, endIndex, partialResults));
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            long finalSum = 0;
            foreach (var sum in partialResults)
            {
                finalSum += sum;
            }
            return finalSum;
        }

        static void CalculatePartialSum(int index, int[] array, int startIndex, int endIndex, long[] results)
        {
            var sum = 0;
            for (var i = startIndex; i < endIndex; i++)
            {
                sum += array[i];
            }
            results[index] = sum;
        }

        static long ParallelLinqSum(int[] array)
        {
            return array.AsParallel().Sum();
        }
    }
}
