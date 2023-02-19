using System;

namespace RaceTo21
{
    class Program
    {
        public static int rounds = 0; // define the number of each round
        static void Main(string[] args)
        {
            CardTable cardTable = new CardTable();
            Game game = new Game(cardTable);
            while (game.nextTask != Task.GameOver)
            {
                game.DoNextTask();
            }
        }
    }
}

