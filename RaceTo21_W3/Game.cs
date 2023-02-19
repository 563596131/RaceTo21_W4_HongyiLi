using System;
using System.Collections.Generic;
using System.Linq;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public Task nextTask; // keeps track of game state
        //private bool cheating = false; // lets you cheat for testing purposes if true
        private int bustNum = 0; // the numbers of player bust
        private int Rounds = 0; // define the number of round
        private int sumBet = 0; // Define an intermediate value of winning and losing for easy calculation

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            //deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        private void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */
        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == Task.GetNumberOfPlayers)
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Task.GetNames;
            }
            else if (nextTask == Task.GetNames)
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                }
                nextTask = Task.IntroducePlayers;
            }
            else if (nextTask == Task.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Task.PlayerTurn;
                foreach (Player item in players)
                {
                    if (Rounds == 0)
                    {
                        Console.WriteLine(item.name + ", ante = 10");
                    }
                }
            }
            else if (nextTask == Task.PlayerTurn)
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    if (cardTable.OfferACard(player))
                    {
                        /* If only one player is not bust
                         * this player will win this round directly
                         */
                        if (bustNum == players.Count - 1)
                        {
                            cardTable.ShowHand(player);
                            Rounds++;
                            if (Rounds == 3) // third round
                            {
                                foreach (Player item in players)
                                {
                                    if (item.status != PlayerStatus.leave) // If the player does not leave, will pay the ante bet of this round
                                    {
                                        item.bet = item.bet - 50; // round 3 ante is 50
                                        sumBet = sumBet + 50;
                                    }
                                }
                                player.bet = player.bet + sumBet;
                                foreach (Player item in players)
                                {
                                    Console.WriteLine(item.name + " remains " + item.bet);
                                }
                                Console.WriteLine(player.name + " wins!");
                                /* Sorts the array data according to descending order and returns 
                                 * the first element in the sequence, or the default value if none
                                 * for this part is for finding the final winner with most chips
                                 */
                                var win = players.OrderByDescending(x => x.bet).FirstOrDefault();

                                win.status = PlayerStatus.win;
                                cardTable.AnnounceWinner(win); // show the final winner
                                nextTask = Task.GameOver;

                                return;
                            }
                            else
                            {
                                foreach (Player item in players)
                                {
                                    if (Rounds == 1) // first round
                                    {
                                        item.bet = item.bet - 10; // round 1 ante is 10
                                        sumBet = sumBet + 10;
                                    }
                                    else // second round
                                    {
                                        if (item.status != PlayerStatus.leave)
                                        {
                                            item.bet = item.bet - 15; // round 2 ante is 15
                                            sumBet = sumBet + 15;
                                        }
                                    }
                                    item.cards = new List<Card>();
                                    item.score = 0;
                                    item.status = PlayerStatus.active;
                                }
                                player.bet = player.bet + sumBet;
                                sumBet = 0;
                                bustNum = 0;
                                currentPlayer = 0;
                                player.status = PlayerStatus.active;
                                nextTask = Task.PlayerTurn;
                                Console.WriteLine("Round " + Rounds + " is over");
                                Console.WriteLine(player.name + " wins!");
                                foreach (Player item in players)
                                {
                                    Console.WriteLine(item.name + " remains " + item.bet);
                                }
                                foreach (Player item in players)
                                {
                                    if (!cardTable.LeaveGame(item))
                                    {
                                        Console.WriteLine(item.name + " quit this round!"); // 
                                        item.status = PlayerStatus.leave;
                                        bustNum++;
                                    }
                                    else
                                    {
                                        switch (Rounds + 1)
                                        {
                                            /* Show the player that the number of rounds is increasing
                                             * and the ante will gradually increase
                                             * round 1 ante = 10
                                             * round 2 ante = 15
                                             * round 3 ante = 50
                                             */
                                            case 1:
                                                Console.WriteLine(item.name + ", ante = 10");
                                                break;
                                            case 2:
                                                Console.WriteLine(item.name + ", ante = 15");
                                                break;
                                            case 3:
                                                Console.WriteLine(item.name + ", ante = 50");
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                return;
                            }
                        }
                        Card card = deck.DealTopCard();
                        player.cards.Add(card);
                        player.score = ScoreHand(player);
                        if (player.score > 21)
                        {
                            player.status = PlayerStatus.bust;
                            bustNum++; // the number of bust +1
                        }
                        else if (player.score == 21)
                        {
                            /* If someone get 21 in total 
                             * this player will win directly
                             */
                            cardTable.ShowHand(player);
                            Rounds++;
                            if (Rounds == 3)
                            {
                                foreach (Player item in players)
                                {
                                    if (item.status != PlayerStatus.leave)
                                    {
                                        item.bet = item.bet - 50;
                                        sumBet = sumBet + 50;
                                    }
                                }
                                player.bet = player.bet + sumBet;
                                foreach (Player item in players)
                                {
                                    Console.WriteLine(item.name + " remains " + item.bet);
                                }
                                Console.WriteLine(player.name + " wins!");
                                var win = players.OrderByDescending(x => x.bet).FirstOrDefault();

                                win.status = PlayerStatus.win;
                                cardTable.AnnounceWinner(win);
                                nextTask = Task.GameOver;
                                return;
                            }
                            else
                            {
                                foreach (Player item in players)
                                {
                                    if (Rounds == 1)
                                    {
                                        item.bet = item.bet - 10;
                                        sumBet = sumBet + 10;
                                    }
                                    else
                                    {
                                        if (item.status != PlayerStatus.leave)
                                        {
                                            item.bet = item.bet - 15;
                                            sumBet = sumBet + 15;
                                        }
                                    }
                                    item.cards = new List<Card>();
                                    item.score = 0;
                                    item.status = PlayerStatus.active;
                                }
                                player.bet = player.bet + sumBet;
                                sumBet = 0;
                                bustNum = 0;
                                currentPlayer = 0;
                                nextTask = Task.PlayerTurn;
                                Console.WriteLine("Round " + Rounds + " is over");
                                Console.WriteLine(player.name + " wins!");
                                foreach (Player item in players)
                                {
                                    Console.WriteLine(item.name + " remains " + item.bet);
                                }
                                foreach (Player item in players)
                                {
                                    if (!cardTable.LeaveGame(item))
                                    {
                                        Console.WriteLine(item.name + " quit this round!");
                                        item.status = PlayerStatus.leave;
                                        bustNum++;
                                    }
                                    else
                                    {
                                        switch (Rounds + 1)
                                        {
                                            case 1:
                                                Console.WriteLine(item.name + ", ante = 10");
                                                break;
                                            case 2:
                                                Console.WriteLine(item.name + ", ante = 15");
                                                break;
                                            case 3:
                                                Console.WriteLine(item.name + ", ante = 50");
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                return;
                            }
                        }
                    }
                    else
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);
                nextTask = Task.CheckForEnd;
            }
            else if (nextTask == Task.CheckForEnd)
            {
                if (!CheckActivePlayers())
                {
                    Player winner = DoFinalScoring();
                    if (winner == null)
                    {
                        winner = players.Last();
                    }
                    Rounds++;
                    if (Rounds == 3)
                    {
                        foreach (Player item in players)
                        {
                            if (item.status != PlayerStatus.leave)
                            {
                                item.bet = item.bet - 50;
                                sumBet = sumBet + 50;
                            }
                        }
                        winner.bet = winner.bet + sumBet;
                        foreach (Player item in players)
                        {
                            Console.WriteLine(item.name + " remains " + item.bet);
                        }
                        Console.WriteLine(winner.name + " wins!");
                        var win = players.OrderByDescending(x => x.bet).FirstOrDefault();

                        win.status = PlayerStatus.win;
                        cardTable.AnnounceWinner(win);
                        nextTask = Task.GameOver;
                        return;
                    }
                    else
                    {
                        foreach (Player item in players)
                        {
                            if (Rounds == 1)
                            {
                                item.bet = item.bet - 10;
                                sumBet = sumBet + 10;
                            }
                            else
                            {
                                if (item.status != PlayerStatus.leave)
                                {
                                    item.bet = item.bet - 15;
                                    sumBet = sumBet + 15;
                                }
                            }
                            item.cards = new List<Card>();
                            item.score = 0;
                            item.status = PlayerStatus.active;
                        }
                        winner.bet = winner.bet + sumBet;
                        sumBet = 0;
                        bustNum = 0;
                        currentPlayer = 0;
                        nextTask = Task.PlayerTurn;
                        Console.WriteLine("Round " + Rounds + " is over");
                        Console.WriteLine(winner.name + " wins!");
                        foreach (Player item in players)
                        {
                            Console.WriteLine(item.name + " remains " + item.bet);
                        }
                        foreach (Player item in players)
                        {
                            if (!cardTable.LeaveGame(item))
                            {
                                Console.WriteLine(item.name + " quit this round!");
                                item.status = PlayerStatus.leave;
                                bustNum++;
                            }
                            else
                            {
                                switch (Rounds + 1)
                                {
                                    case 1:
                                        Console.WriteLine(item.name + ", ante = 10");
                                        break;
                                    case 2:
                                        Console.WriteLine(item.name + ", ante = 15");
                                        break;
                                    case 3:
                                        Console.WriteLine(item.name + ", ante = 50");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        return;
                    }
                }
                else
                {
                    currentPlayer++;
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = Task.PlayerTurn;
                }
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Task.GameOver;
            }
        }

        private int ScoreHand(Player player)
        {
            int score = 0;
            //if (cheating == true && player.status == PlayerStatus.active)
            //{
            //    string response = null;
            //    while (int.TryParse(response, out score) == false)
            //    {
            //        Console.Write("OK, what should player " + player.name + "'s score be?");
            //        response = Console.ReadLine();
            //    }
            //    return score;
            //}
            //else
            //{
                foreach (Card card in player.cards)
                {
                    string cardId = card.id;
                    string faceValue = cardId.Remove(cardId.Length - 1);
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            //}
            return score;
        }

        /* Iterate over all player states and check if they still have at least one player
         * Is called by Game object
         * Game object provides bool value
         * Returns bool value to Game object
         */
        private bool CheckActivePlayers()
        {
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        private Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                // if busted don't bother checking!
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            return null; // everyone must have busted because nobody won!
        }
    }
}
