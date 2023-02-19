using System;
using System.Collections.Generic;

namespace RaceTo21
{
	public class Player
	{
		public string name;
		public List<Card> cards = new List<Card>();
		public PlayerStatus status = PlayerStatus.active;
		public int score; // The total score of the player's current hand
		public int bet;

		public Player(string n)
		{
			name = n;
			bet = 75; // The total number of chips each player initially owns
		}

		/* Introduces player by name
		 * Called by CardTable object
		 */
		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum);
		}
	}
}

