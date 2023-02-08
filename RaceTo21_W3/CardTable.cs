using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class CardTable
    {
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }

        public bool OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        /* Betting component
         * 1.Each players has a number of chips to bet each round
         * 2.winner of each round gets all of the chips in the current pot
         * 3.if nobody win, the pot remain for the next round
         * 4.player with the most chips after 3 round wins the game
         * called by the Game object
         */
        //public bool OfferBet(Player player)
        //{
        //    Console.WriteLine("Please enter your bet");
        //    string responsBet = Console.ReadLine(); // player type the bet
        //    int numberOfBet;
        //    while (int.TryParse(responsBet, out numberOfBet) == false)
        //    {

        //    }
        //    while (numberOfBet < player.upbet)
        //    {

        //    }
        //    if (player.bet < player.upbet)
        //    {
        //        return false;
        //    }
        //    player.upbet = numberOfBet;
        //    player.bet = player.bet - numberOfBet;
        //    return true;
        //}

        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                /* show the full name of cards
                 * drawn everytime
                 */
                string show = player.name + " has: ";
                foreach (Card card in player.cards)
                {
                    string[] split = card.displayName.Split(" ");
                    string cardName = split[0];
                    string cardLongName = split[0];
                    switch (cardName)
                    {
                        case "2":
                            cardLongName = "Two";
                            break;

                        case "3":
                            cardLongName = "Three";
                            break;

                        case "4":
                            cardLongName = "Four";
                            break;

                        case "5":
                            cardLongName = "Five";
                            break;

                        case "6":
                            cardLongName = "Six";
                            break;

                        case "7":
                            cardLongName = "Seven";
                            break;

                        case "8":
                            cardLongName = "Eight";
                            break;

                        case "9":
                            cardLongName = "Nine";
                            break;

                        case "10":
                            cardLongName = "Ten";
                            break;

                        default:

                            break;
                    }
                    show += cardLongName + " " + split[1] + " " + split[2] + ", ";
                }

                show = show.Remove(show.Length - 2);
                Console.WriteLine(show);

                Console.Write("=" + player.score + "/21 ");
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }


        public void AnnounceWinner(Player player)
        {
            if (player != null)
            {
                Console.WriteLine(player.name + " wins!");
            }
            else
            {
                Console.WriteLine("Everyone busted!");
            }
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}