using System;
using System.Diagnostics;
using System.Linq;

namespace StringMatchingAnalysis
{

    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            //first start with randoms

            for (int i = 0; i < 5; i++)
            {
                int n = 0;

                switch (i)
                {
                    case (0):
                        n = 10000;
                        break;
                    case (1):
                        n = 25000;
                        break;
                    case (2):
                        n = 50000;
                        break;
                    case (3):
                        n = 75000;
                        break;
                    case (4):
                        n = 100000;
                        break;
                    default:
                        break;
                }

                //Generate String of Text.
                string text = RandomString(n);

                int m = 1000;
                string randomPattern = "";
                string containedPattern = "";

                Console.WriteLine($"n = {n}, m = {m}_____________________________________");

                //Generate Random Pattern to be found.
                randomPattern = RandomString(m);
                containedPattern = GenerateContainedPattern(text, m);

                BruteMatching(text, randomPattern);
                BruteMatching(text, containedPattern);

                //Generate Contained Pattern
                m = n / 2;
                Console.WriteLine($"n = {n}, m = {m}_____________________________________");
                randomPattern = RandomString(m);
                containedPattern = GenerateContainedPattern(text, m);

                BruteMatching(text, randomPattern);
                BruteMatching(text, containedPattern);
            }
        }

        private static void BruteMatching(string text, string pattern)
        {
            //Run 10 times and get average
            double average = 0;
            double totalSum = 0;
            int indexFound = -1;

            for (int x = 0; x < 100; x++)
            {
                //Start Timer
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                int n = text.Length;
                int m = pattern.Length;

                for (int i = 0; i < n - m; i++)
                {
                    int j = 0;

                    while (j < m && text[i + j] == pattern[j])
                    {
                        j++;
                    }

                    if (j == m)
                    {
                        indexFound = i;
                        break;
                    }
                }

                //End Timer, get time.
                stopwatch.Stop();
                TimeSpan timeTaken = stopwatch.Elapsed;
                var elapsed_time = Math.Round((double)timeTaken.TotalMilliseconds, 3);
                totalSum += elapsed_time;


                Console.WriteLine($"Run {x}, RT = {elapsed_time}");
            }


            average = totalSum / 100;
            average = Math.Round((double)average, 3);

            if (indexFound != -1)
            {
                Console.WriteLine($"Match found, index {indexFound}.");
                Console.WriteLine($"Average Time elapsed: {average}msec\n");
            }
            else
            {

                Console.WriteLine($"Match Not found.");
                Console.WriteLine($"Average Time elapsed: {average}msec\n");
            }
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateContainedPattern(string text, int m)
        {
            int maxIndex = text.Length - m;

            int chosenIndex = random.Next(0, maxIndex);

            string containedPattern = text.Substring(chosenIndex, m);

            return containedPattern;
        }
    }
}
