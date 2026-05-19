using System;
using System.Collections.Generic;
using System.Linq;

namespace BookLendingApp.FEApplication.Common
{
    public static class ConsoleUi
    {
        public static void WriteTitle(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', Math.Max(20, title.Length + 8)));
            Console.WriteLine($"  {title}");
            Console.WriteLine(new string('=', Math.Max(20, title.Length + 8)));
            Console.ResetColor();
        }

        public static void WriteMenuOptions(IEnumerable<string> options)
        {
            var index = 1;
            foreach (var option in options)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{index}. ");
                Console.ResetColor();
                Console.WriteLine(option);
                index++;
            }
        }

        public static void WriteTable(IEnumerable<string> rows, bool showIndices = false)
        {
            var list = rows?.ToList() ?? new List<string>();
            if (!list.Any())
            {
                return;
            }

            // Split rows by '|' and trim cells
            var splitRows = list.Select(r => r.Split('|').Select(c => c.Trim()).ToArray()).ToList();

            // Detect if rows are in "Key: Value" format to extract headers
            bool hasKeyValue = splitRows.Count > 0 && splitRows.All(r => r.All(cell => cell.Contains(":"))) ;
            string[]? headers = null;
            if (hasKeyValue)
            {
                headers = splitRows[0].Select(cell => cell.Split(new[] { ':' }, 2)[0].Trim()).ToArray();
                // convert splitRows to value-only rows
                for (int i = 0; i < splitRows.Count; i++)
                {
                    splitRows[i] = splitRows[i].Select(cell => cell.Split(new[] { ':' }, 2)[1].Trim()).ToArray();
                }
            }

            // Compute max width per column (consider headers if present)
            var columnCount = splitRows.Max(r => r.Length);
            var widths = new int[columnCount];
            if (headers != null)
            {
                for (int j = 0; j < headers.Length; j++)
                {
                    widths[j] = Math.Max(widths[j], headers[j].Length);
                }
            }
            for (int i = 0; i < splitRows.Count; i++)
            {
                for (int j = 0; j < splitRows[i].Length; j++)
                {
                    widths[j] = Math.Max(widths[j], splitRows[i][j].Length);
                }
            }

            // Print header if available
            if (headers != null)
            {
                if (showIndices)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("#   ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("| ");
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                for (int c = 0; c < columnCount; c++)
                {
                    var head = c < headers.Length ? headers[c] : string.Empty;
                    var padded = head.PadRight(widths[c] + 2);
                    if (c < columnCount - 1)
                    {
                        Console.Write(padded);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("| ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.Write(padded);
                    }
                }
                Console.WriteLine();
                Console.ResetColor();
            }

            // Print rows with padding (data rows in white)
            for (int i = 0; i < splitRows.Count; i++)
            {
                var row = splitRows[i];
                if (showIndices)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{(i + 1).ToString().PadRight(2)}  ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("| ");
                }

                Console.ForegroundColor = ConsoleColor.White;
                for (int c = 0; c < columnCount; c++)
                {
                    var cell = c < row.Length ? row[c] : string.Empty;
                    var padded = cell.PadRight(widths[c] + 2);
                    if (c < columnCount - 1)
                    {
                        Console.Write(padded);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("| ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write(padded);
                    }
                }
                Console.WriteLine();
            }

            Console.ResetColor();
        }

        public static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Pause()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to continue...");
            Console.ResetColor();
            Console.ReadKey(intercept: true);
        }
    }
}
