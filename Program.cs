using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StringMatchingAnalysis
{

    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("Brute.txt"))
                File.Delete("Brute.txt");

            if (File.Exists("Boyer.txt"))
                File.Delete("Boyer.txt");

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

                //Run first two tests with m = 1000
                randomPattern = RandomString(m);
                containedPattern = GenerateContainedPattern(text, m);

                File.AppendAllText("Brute.txt", $"n = {n}, m = {m}, with Random String" + Environment.NewLine);
                BruteMatching(text, randomPattern);

                File.AppendAllText("Brute.txt", $"n = {n}, m = {m}, with Generated Matching String" + Environment.NewLine);
                BruteMatching(text, containedPattern);

                File.AppendAllText("Boyer.txt", $"n = {n}, m = {m}, with Random String" + Environment.NewLine);
                BoyerMoore(text, randomPattern);

                File.AppendAllText("Boyer.txt", $"n = {n}, m = {m}, with Generated Matching String" + Environment.NewLine);
                BoyerMoore(text, containedPattern);

                //Run Second two tests with m = n / 2
                m = n / 2;
                randomPattern = RandomString(m);
                containedPattern = GenerateContainedPattern(text, m);

                File.AppendAllText("Brute.txt", $"n = {n}, m = {m}, with Random String" + Environment.NewLine);
                BruteMatching(text, randomPattern);

                File.AppendAllText("Brute.txt", $"n = {n}, m = {m}, with Generated Matching String" + Environment.NewLine);
                BruteMatching(text, containedPattern);

                File.AppendAllText("Boyer.txt", $"n = {n}, m = {m}, with Random String" + Environment.NewLine);
                BoyerMoore(text, randomPattern);

                File.AppendAllText("Boyer.txt", $"n = {n}, m = {m}, with Generated Matching String" + Environment.NewLine);
                BoyerMoore(text, containedPattern);
            }
        }

        private static void BruteMatching(string T, string P)
        {
            //Run 10 times and get average
            double average = 0;
            double totalSum = 0;
            int indexFound = -1;

            for (int x = 0; x < 1000; x++)
            {
                //Start Timer
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                int n = T.Length;
                int m = P.Length;

                for (int i = 0; i < n - m; i++)
                {
                    int j = 0;

                    while (j < m && T[i + j] == P[j])
                    {
                        j++;
                    }

                    if (j == m)
                    {
                        indexFound = i;
                        Console.WriteLine($"Match string found at index: {indexFound}");
                        break;
                    }
                }

                //End Timer, get time.
                stopwatch.Stop();
                TimeSpan timeTaken = stopwatch.Elapsed;
                var elapsed_time = (double)timeTaken.TotalMilliseconds;
                totalSum += elapsed_time;


                //Console.WriteLine($"Run {x}, RT = {elapsed_time}");
            }


            average = totalSum / 1000;
            average = Math.Round((double)average, 3);

            if (indexFound != -1)            
                File.AppendAllText("Brute.txt", $"Match found." + Environment.NewLine);            
            else            
                File.AppendAllText("Brute.txt", $"Match not found." + Environment.NewLine);            

            File.AppendAllText("Brute.txt", $"Average Time elapsed: {average}msec" + Environment.NewLine + Environment.NewLine);
        }

        private static void BoyerMoore(string T, string P)
        {
            //Run 10 times and get average
            double average = 0;
            double totalSum = 0;
            int indexFound = -1;

            for (int x = 0; x < 1000; x++)
            {
                int n = T.Length;
                int m = P.Length;

                int[] shiftIndex = new int[n];
                shift(P.ToCharArray(), m, shiftIndex);

                //Start Timer
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int i = 0; i <= n - m;)
                {
                    int pos = m - 1;

                    while (pos >= 0 && P.Substring(pos, 1) == T.Substring(i + pos, 1))
                        pos--;

                    if (pos < 0)
                    {
                        indexFound = i;

                        Console.WriteLine($"Match string found at index: {indexFound}");

                        i += (i + m < n) ? m - shiftIndex[T[i + m]] : 1;

                        break;
                    }
                    else
                    {
                        i += Max(1, pos - shiftIndex[T[i + pos]]);
                    }
                }

                //End Timer, get time.
                stopwatch.Stop();
                TimeSpan timeTaken = stopwatch.Elapsed;
                var elapsed_time = (double)timeTaken.TotalMilliseconds;
                totalSum += elapsed_time;
            }


            average = totalSum / 1000;
            average = Math.Round((double)average, 3);

            if (indexFound != -1)
                File.AppendAllText("Boyer.txt", $"Match found." + Environment.NewLine);
            else
                File.AppendAllText("Boyer.txt", $"Match not found." + Environment.NewLine);

            File.AppendAllText("Boyer.txt", $"Average Time elapsed: {average}msec" + Environment.NewLine + Environment.NewLine);
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static string GenerateContainedPattern(string text, int m)
        {
            int maxIndex = text.Length - m;

            int chosenIndex = random.Next(0, maxIndex);

            string containedPattern = text.Substring(chosenIndex, m);

            return containedPattern;
        }

        //used for booyer moore
        static void shift(char []str, int m, int []tempShift)
        {
            int i;

            // Initialize all occurrences as -1
            for (i = 0; i < str.Length; i++)
                tempShift[i] = -1;

            // Fill the actual value of last occurrence 
            // of a character
            for (i = 0; i < m; i++)
                tempShift[(int)str[i]] = i;
        }
    }
}
