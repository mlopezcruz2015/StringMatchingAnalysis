using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StringMatchingAnalysis
{
    class Program
    {
        //Global variables to save the total sum of the timer and then get average.
        private static double boyerRandomPatternAverage = 0;
        private static double boyerContainedPatternAverage = 0;
        private static double bruteRandomPatternAverage = 0;
        private static double bruteContainedPatternAverage = 0;

        static void Main(string[] args)
        {
            //Reset Logs
            if (File.Exists("BruteRandom.txt"))
                File.Delete("BruteRandom.txt");

            if (File.Exists("BoyerRandom.txt"))
                File.Delete("BoyerRandom.txt");

            if (File.Exists("BruteContained.txt"))
                File.Delete("BruteContained.txt");

            if (File.Exists("BoyerContained.txt"))
                File.Delete("BoyerContained.txt");

            //Use a switch statement to set 'n'
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

                //Generate Random String of Text.
                string text = RandomString(n);


                string randomPattern = "";
                string containedPattern = "";

                int m = 1000;

                //Run tests 10000 times
                for (int k = 0; k < 10000; k++)
                {
                    //Run first tests with m = 1000
                    randomPattern = RandomString(m);
                    containedPattern = GenerateContainedPattern(text, m);

                    BruteMatching(text, randomPattern, false);

                    BruteMatching(text, containedPattern, true);

                    BoyerMoore(text, randomPattern, false);

                    BoyerMoore(text, containedPattern, true);
                }

                LogResults(n, m);

                //Run Second set of tests with m = n / 2
                m = n / 2;

                //Run tests 10000 times
                for (int k = 0; k < 10000; k++)
                {
                    randomPattern = RandomString(m);
                    containedPattern = GenerateContainedPattern(text, m);

                    BruteMatching(text, randomPattern, false);

                    BruteMatching(text, containedPattern, true);

                    BoyerMoore(text, randomPattern, false);

                    BoyerMoore(text, containedPattern, true);
                }

                LogResults(n, m);
            }
        }

        /// <summary>
        /// Uses the global variables that store the average running time for each algorithm. Writes these average RTs to .txt files.
        /// </summary>
        /// <param name="n">Text length</param>
        /// <param name="m">Pattern length</param>
        private static void LogResults(int n, int m)
        {
            //Get Average
            boyerRandomPatternAverage = boyerRandomPatternAverage / 10000;
            boyerContainedPatternAverage = boyerContainedPatternAverage / 10000;
            bruteRandomPatternAverage = bruteRandomPatternAverage / 10000;
            bruteContainedPatternAverage = bruteContainedPatternAverage / 10000;

            File.AppendAllText("BruteRandom.txt", $"n = {n}, m = {m}, with Random String" + Environment.NewLine);
            File.AppendAllText("BruteRandom.txt", $"Average Time elapsed: {bruteRandomPatternAverage}msec" + Environment.NewLine + Environment.NewLine);

            File.AppendAllText("BruteContained.txt", $"n = {n}, m = {m}, with Generated Matching String" + Environment.NewLine);
            File.AppendAllText("BruteContained.txt", $"Average Time elapsed: {bruteContainedPatternAverage}msec" + Environment.NewLine + Environment.NewLine);

            File.AppendAllText("BoyerRandom.txt", $"n = {n}, m = {m}, with Random String" + Environment.NewLine);
            File.AppendAllText("BoyerRandom.txt", $"Average Time elapsed: {boyerRandomPatternAverage}msec" + Environment.NewLine + Environment.NewLine);

            File.AppendAllText("BoyerContained.txt", $"n = {n}, m = {m}, with Generated Matching String" + Environment.NewLine);
            File.AppendAllText("BoyerContained.txt", $"Average Time elapsed: {boyerContainedPatternAverage}msec" + Environment.NewLine + Environment.NewLine);

            //Reset Global Variables
            boyerRandomPatternAverage = 0;
            boyerContainedPatternAverage = 0;
            bruteRandomPatternAverage = 0;
            bruteContainedPatternAverage = 0;
        }

        /// <summary>
        /// Runs the Brute Matching algorithm for string matching
        /// </summary>
        /// <param name="T">String of Text</param>
        /// <param name="P">Pattern to be found</param>
        /// <param name="containedPattern">Marker to determine what global variable to write to.</param>
        private static void BruteMatching(string T, string P, bool containedPattern)
        {
            double totalSum = 0;

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
                    Console.WriteLine($"Brute: index: {i}");
                    break;
                }
            }

            //End Timer, get time.
            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            var elapsed_time = (double)timeTaken.TotalMilliseconds;
            totalSum += elapsed_time;

            //Save elapsed time to appropriate variable.
            if (containedPattern)
            {
                bruteContainedPatternAverage += totalSum;
            }
            else
            {
                bruteRandomPatternAverage += totalSum;
            }
        }

        /// <summary>
        /// Runs the Boyer-Moore algorithm for string matching
        /// </summary>
        /// <param name="T">String of Text</param>
        /// <param name="P">Pattern to be found</param>
        /// <param name="containedPattern">Marker to determine what global variable to write to.</param>
        private static void BoyerMoore(string T, string P, bool containedPattern)
        {
            double totalSum = 0;

            int n = T.Length;
            int m = P.Length;

            int[] shiftIndex = new int[n];
            shift(P.ToCharArray(), m, shiftIndex, n);

            //Start Timer
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i <= n - m;)
            {
                int pos = m - 1;

                while (pos >= 0 && P[pos] == T[i + pos])
                    pos--;

                if (pos < 0)
                {
                    Console.WriteLine($"Boyer index: {i}");

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

            //Save elapsed time to appropriate variable.
            if (containedPattern)
            {
                boyerContainedPatternAverage += totalSum;
            }
            else
            {
                boyerRandomPatternAverage += totalSum;
            }
        }

        //used for booyer moore
        static void shift(char[] str, int m, int[] tempShift, int n)
        {
            int i;

            // Initialize all occurrences as -1
            for (i = 0; i < n; i++)
                tempShift[i] = -1;

            // Fill the actual value of last occurrence 
            // of a character
            for (i = 0; i < m; i++)
                tempShift[(int)str[i]] = i;
        }

        private static Random random = new Random();

        /// <summary>
        /// Creates random string based on the length passed in.
        /// </summary>
        /// <param name="length">Length of string</param>
        /// <returns>Randomized string of length l</returns>
        public static string RandomString(int l)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, l)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Gets max between two numbers.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Max of two numbers</returns>
        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// Generates a pattern that is contained within a string of text.
        /// </summary>
        /// <param name="text">String of text from which pattern is generated</param>
        /// <param name="m">length of pattern to create</param>
        /// <returns>pattern substring from string of text</returns>
        public static string GenerateContainedPattern(string text, int m)
        {
            int maxIndex = text.Length - m;

            int chosenIndex = random.Next(0, maxIndex);

            string containedPattern = text.Substring(chosenIndex, m);

            return containedPattern;
        }
    }
}
