using MD5;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("1. -compare or -nocompare");
        Console.WriteLine("2. path to file/directory");
        Console.WriteLine("3. -parallel or -sequential (if -nocompare is chosen)");
        Console.WriteLine();

        int numberOfTries = 100;
        var sum = new CheckSum();
        if (args.Length != 2 && args.Length != 3)
        {
            Console.WriteLine("Incorrect input.");
        }

        if (args[0] == "-compare")
        {
            var path = args[1];

            Console.WriteLine("Expected Value (Sequential) | Deviation (Sequential) | Expected Value (Parallel) | Deviation (Parallel) |");
            try
            {
                ComputeTimes(sum.CalculateSequential, path, numberOfTries);
                await ComputeTimesParallel(sum.CalculateParallel, path, numberOfTries);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine();
                Console.WriteLine("Access denied.");
            }
            catch (IOException)
            {
                Console.WriteLine();
                Console.WriteLine("Cannot access open file.");
            }
        }
        else if (args[0] == "-nocompare")
        {
            try
            {
                var checkSum = new CheckSum();
                if (args[2] == "-parallel")
                {
                    Console.WriteLine(await checkSum.CalculateParallel(args[1]));
                }
                else if (args[2] == "-sequential")
                {
                    Console.WriteLine(checkSum.CalculateSequential(args[1]));
                }
                else
                {
                    Console.WriteLine("Incorrect input.");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine();
                Console.WriteLine("Access denied.");
            }
            catch (IOException)
            {
                Console.WriteLine();
                Console.WriteLine("Cannot access open file.");
            }
        }
        else
        {
            Console.WriteLine("Incorrect input.");
        }
    }

    private static void ComputeTimes(Func<string, string> calculateSum, string path, int numberOfTries)
    {
        var data = new long[numberOfTries];
        var watch = new System.Diagnostics.Stopwatch();
        for (int i = 0; i < numberOfTries; i++)
        {
            watch.Start();
            calculateSum(path);
            watch.Stop();
            data[i] = watch.ElapsedMilliseconds;
            watch.Reset();
        }

        var expectedValue = CountExpectedValue(data, numberOfTries);
        var standardDeviation = CountStandardDeviation(data, numberOfTries, expectedValue);
        Console.Write($"         {expectedValue:G4} мс          |          {standardDeviation:G4} мс         | ");
    }

    private static async Task ComputeTimesParallel(Func<string, Task<string>> calculateSum, string path, int numberOfTries)
    {
        var data = new long[numberOfTries];
        var watch = new System.Diagnostics.Stopwatch();
        for (int i = 0; i < numberOfTries; i++)
        {
            watch.Start();
            await calculateSum(path);
            watch.Stop();
            data[i] = watch.ElapsedMilliseconds;
            watch.Reset();
        }

        var expectedValue = CountExpectedValue(data, numberOfTries);
        var standardDeviation = CountStandardDeviation(data, numberOfTries, expectedValue);
        Console.Write($"         {expectedValue:G4} мс          |          {standardDeviation:G4} мс         | ");
    }

    private static double CountExpectedValue(long[] data, int numberOfTries)
    {
        long expectation = 0;
        for (int i = 0; i < numberOfTries; i++)
        {
            expectation += data[i];
        }

        return (double)expectation / numberOfTries;
    }

    private static double CountStandardDeviation(long[] data, int numberOfTries, double expectation)
    {
        double variation = 0;
        for (int i = 0; i < numberOfTries; i++)
        {
            variation += (data[i] - expectation) * (data[i] - expectation);
        }

        return Math.Sqrt(variation / numberOfTries);
    }
}