using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Task_3
{
    public class Game
    {
        public const int TableWidth = 128;
        public string[] Moves { get; }
        public Random Random;

        public Game(string[] moves)
        {
            if (!IsMovesValid(moves))
                throw new Exception(
                    "Invalid moves variation provided. Variation must have odd amount of moves, and not less than 3");
            Moves = moves;
            Random = new Random();
        }

        public bool PlayRound()
        {
            Console.WriteLine(new string('=', 128));
            byte[] key = new byte[16];
            RandomNumberGenerator.Create().GetNonZeroBytes(key);
            HMACSHA256 hmac = new HMACSHA256(key);
            int aiMove = Random.Next(0, Moves.Length);
            byte[] aiMoveHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Moves[aiMove]));

            Console.WriteLine($"HMAC: {BitConverter.ToString(aiMoveHash).Replace("-", "")}");
            PrintValidInput();
            string playerResponse = Console.ReadLine();
            switch (playerResponse)
            {
                case "0":
                {
                    return true;
                }
                case "?":
                {
                    PrintTable();
                    break;
                }
                default:
                {
                    if (int.TryParse(playerResponse, out int playerMove))
                    {
                        if (playerMove - 1 < 0 || playerMove - 1 >= Moves.Length)
                        {
                            PrintInvalidInput();
                        }
                        else
                        {
                            int gameResult = CompareMoves(playerMove - 1, aiMove);
                            Console.WriteLine($"Your move {playerMove} ({Moves[playerMove - 1]})");
                            Console.WriteLine($"AI move: {aiMove + 1} ({Moves[aiMove]})");
                            PrintGameResult(gameResult);
                            Console.WriteLine($"HMAC key: {BitConverter.ToString(hmac.Key).Replace("-", "")}");
                        }
                    }
                    else
                    {
                        PrintInvalidInput();
                    }

                    break;
                }
            }

            return false;
        }

        private void PrintInvalidInput()
        {
            Console.WriteLine("Invalid input. Please enter one of the following moves:");
            PrintValidInput();
        }
        private void PrintGameResult(int gameResult)
        {
            switch (gameResult)
            {
                case 1:
                {
                    Console.WriteLine("You win!");
                    break;
                }
                case -1:
                {
                    Console.WriteLine("You lost!");
                    break;
                }
                default:
                {
                    Console.WriteLine("Draw!");
                    break;
                }
            }
        }

        private void PrintValidInput()
        {
            for (int i = 0; i < Moves.Length; i++)
            {
                Console.WriteLine($"{i + 1} - play {Moves[i]}");
            }

            Console.WriteLine("0 - exit");
            Console.WriteLine("? - help");
        }

        private void PrintTable()
        {
            PrintLine();
            PrintRow(Moves);
            PrintLine();
            string[] winRow = new string[Moves.Length];
            string[] loseRow = new string[Moves.Length];
            string[] drawRow = new string[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                winRow[i] = "Wins:";
                loseRow[i] = "Loses to:";
                drawRow[i] = "Draw if:";
                for (int j = 0; j < Moves.Length; j++)
                {
                    int result = CompareMoves(i, j);
                    switch (result)
                    {
                        case 0:
                        {
                            drawRow[i] += $" {Moves[j]}";
                            break;
                        }
                        case -1:
                        {
                            loseRow[i] += $" {Moves[j]}";
                            break;
                        }
                        case 1:
                        {
                            winRow[i] += $" {Moves[j]}";
                            break;
                        }
                    }
                }
            }

            PrintRow(winRow);
            PrintLine();
            PrintRow(loseRow);
            PrintLine();
            PrintRow(drawRow);
            PrintLine();
        }

        private static void PrintLine()
        {
            Console.WriteLine(new string('-', TableWidth));
        }

        private static void PrintRow(string[] columns)
        {
            var width = (TableWidth - columns.Length) / columns.Length;
            var row = columns.Aggregate("|", (current, column) => current + (Center(column, width) + "|"));

            Console.WriteLine(row);
        }

        private static string Center(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            return string.IsNullOrEmpty(text)
                ? new string(' ', width)
                : text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }

        private int CompareMoves(int moveIndex1, int moveIndex2)
        {
            if (moveIndex1 < moveIndex2)
                moveIndex1 += Moves.Length;
            int delta = moveIndex1 - moveIndex2;
            int halfMoves = Moves.Length / 2;
            if (delta == 0)
                return 0;
            if (delta > halfMoves)
                return -1;
            return 1;
        }

        public static bool CanCreate(string[] moves)
        {
            return IsMovesValid(moves);
        }

        private static bool IsMovesValid(string[] moves)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                for (int j = i + 1; j < moves.Length; j++)
                {
                    if (moves[i].Equals(moves[j]))
                        return false;
                }
            }

            return moves.Length >= 3 && moves.Length % 2 == 1;
        }
    }
}