using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PPAssingment2
{
    class Program
    {
        private static int savedIndex = int.MaxValue;
        private static object theLock = new object();
        static void Main(string[] args)
        {
            E6();
        }

        private static void E2()
        {
            int[] array = new int[100000000];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i + 1;
            }

            Stopwatch sw = new Stopwatch();

            sw.Start();

            foreach (var number in array)
            {
                Math.Sqrt(number);
            }

            sw.Stop();

            Console.WriteLine("Normal time: " + sw.ElapsedMilliseconds);

            sw.Reset();

            sw.Start();

            Parallel.ForEach(array, number => { Math.Sqrt(number); });

            sw.Stop();

            Console.WriteLine("Paralel time: " + sw.ElapsedMilliseconds);
        }

        private static void E3()
        {
            int[] A = new int[100000000];
            int[] B = new int[100000000];
            int[] C = new int[100000000];



            for (int i = 0; i < A.Length; i++)
            {
                A[i] = i + 1;
                B[i] = i + 1;
            }

            Stopwatch sw = new Stopwatch();

            sw.Start();

            for (int i = 0; i < C.Length; i++)
            {
                C[i] = A[i] + B[i];
            }

            sw.Stop();

            Console.WriteLine("Normal time: " + sw.ElapsedMilliseconds);

            sw.Reset();

            sw.Start();

            Parallel.For(0, C.Length, i => { C[i] = A[i] + B[i]; });

            sw.Stop();

            Console.WriteLine("Paralel time: " + sw.ElapsedMilliseconds);
        }

        private static void E42()
        {
            int[] array = new int[100000000];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i + 1;
            }

            Stopwatch sw = new Stopwatch();

            sw.Start();

            foreach (var number in array)
            {
                Math.Sqrt(number);
            }

            sw.Stop();

            Console.WriteLine("Normal time: " + sw.ElapsedMilliseconds);

            sw.Reset();

            sw.Start();

            Parallel.ForEach(Partitioner.Create(0, array.Length),
                (range) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        Math.Sqrt(array[i]);
                    }
                });

            sw.Stop();

            Console.WriteLine("Paralel time: " + sw.ElapsedMilliseconds);
        }

        private static void E43()
        {
            int[] A = new int[100000000];
            int[] B = new int[100000000];
            int[] C = new int[100000000];



            for (int i = 0; i < A.Length; i++)
            {
                A[i] = i + 1;
                B[i] = i + 1;
            }

            Stopwatch sw = new Stopwatch();

            sw.Start();

            for (int i = 0; i < C.Length; i++)
            {
                C[i] = A[i] + B[i];
            }

            sw.Stop();

            Console.WriteLine("Normal time: " + sw.ElapsedMilliseconds);

            sw.Reset();

            sw.Start();

            Parallel.ForEach(Partitioner.Create(0, C.Length),
                (range) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        C[i] = A[i] + B[i];
                    }
                });


            sw.Stop();

            Console.WriteLine("Paralel time: " + sw.ElapsedMilliseconds);
        }

        private static void E5()
        {
            int[] A = new int[100000000];
            int[] B = new int[100000000];

            Console.WriteLine("Normal:");
            TimeAction(() =>
            {
                Random rng = new Random(6969);
                for (int i = 0; i < B.Length; i++)
                {
                    B[i] = rng.Next();
                }
            });

            Console.WriteLine("Parralel:");
            TimeAction(() =>
                {
                    Parallel.ForEach(
                        Partitioner.Create(0, 100000000),
                        new ParallelOptions(),
                        () =>
                        {
                            return new Random(420 /** This should be a new randon generated number 
                                                       *each time or else it will be the same sequence again and again**/);
                        },
                        (range, loopState, random) =>
                        {
                            for (int i = range.Item1; i < range.Item2; i++)
                                A[i] = random.Next();
                            return random;
                        },
                        _ => { });
                }
            );
        }

        private static void E6()
        {
            int[] array = new int[100000000];
            Parallel.ForEach(
                Partitioner.Create(0, 100000000),
                new ParallelOptions(),
                () => { return new Random(123); },
                (range, loopState, random) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        array[i] = random.Next(100000);
                    return random;
                },
                _ => { });



            TimeAction(() =>
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == 69)
                    {
                        Console.WriteLine("Lowest index is: " + i);
                        break;
                    }
                }
            });



            TimeAction(() =>
            {
                Parallel.ForEach(Partitioner.Create(0, array.Length),
                    (range, loopState) =>
                    {
                        for (int i = range.Item1; i < range.Item2; i++)
                        {
                            if (array[i] == 69)
                            {
                                lock (theLock)
                                {
                                    if (i < savedIndex)
                                    {
                                        savedIndex = i;
                                    }
                                }
                                loopState.Break();
                            }
                        }
                    });
            });

            Console.WriteLine("Lowest index is: " + savedIndex);
        }


        private static void TimeAction(Action action)
        {
            var sw = Stopwatch.StartNew();
            action.Invoke();
            sw.Stop();
            Console.WriteLine("Time: " + sw.ElapsedMilliseconds);
        }
    }
}
