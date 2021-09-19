using System;

namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game(args);
            while (!game.PlayRound())
            {
            }
        }
        
    }
}