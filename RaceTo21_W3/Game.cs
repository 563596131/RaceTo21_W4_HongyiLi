using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
        private bool cheating = false; // lets you cheat for testing purposes if true
        private int bustNum = 0; // the numbers of player bust

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
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
            }
            else if (nextTask == Task.PlayerTurn)
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    //if (players.Count == 1) // if only remain one player, win derectly
                    //{
                    //    player.status = PlayerStatus.win;
                    //    cardTable.ShowHand(player);
                    //    cardTable.AnnounceWinner(player);
                    //    nextTask = Task.GameOver;
                    //    return;
                    //}
                    if (cardTable.OfferACard(player))
                    {
                        /* If only one player is not bust
                         * this player will win directly
                         */
                        if (bustNum == players.Count - 1)
                        {
                            // win, showhands and game over
                            player.status = PlayerStatus.win;
                            cardTable.ShowHand(player);
                            cardTable.AnnounceWinner(player);
                            nextTask = Task.GameOver;
                            return;
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
                            player.status = PlayerStatus.win;
                            /* If someone get 21 in total 
                             * this player will win directly
                             */
                            cardTable.ShowHand(player);
                            cardTable.AnnounceWinner(player);
                            nextTask = Task.GameOver;
                            return;
                        }
                    }
                    else
                    {
                        player.status = PlayerStatus.stay;
                        /* out component
                         */
                        //Console.WriteLine(player.name + " is out"); // if bust, player will out
                        //players.RemoveAt(currentPlayer);
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
                    /* If no bust, there will be some different situations
                     * 1. no bust, but everyone stay, there will be one winner
                     * 2. nobody draw card at first
                     */
                    if (bustNum == 0)
                    {
                        if (winner !=null)
                        {
                            Console.WriteLine(winner.name + " wins! ");
                        }
                        else
                        {
                            Console.WriteLine("Pleaase replay"); // no one draw card, error!
                        }
                        Console.Write("Press <Enter> to Exit..."); // give a hint
                        while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                        cardTable.ShowPlayers(players);
                        nextTask = Task.PlayerTurn;
                    }
                    else
                    {
                        cardTable.AnnounceWinner(winner);
                    }
                    nextTask = Task.GameOver;
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

        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
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
            }
            return score;
        }

        public bool CheckActivePlayers()
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

        public Player DoFinalScoring()
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
