﻿using System;
using System.Collections.Generic;
using System.Linq;
using GamePieces.Session;

namespace Testing
{
    public class ConsoleGame
    {
        public void Play()
        {
            var game = new Game(new List<string>()
            {
                "King Kong",
                "Godzilla",
            });

            while (!game.Winner)
            {
                var n = 0;
                game.StartTurn();
                Console.WriteLine("\n\r" + game.Current.Name + ", select Dice to Switch Save State or Exit Rolling");
                while (game.Current.RemainingRolls != 0)
                {
                    game.Roll();
                    game.Dice.ForEach(die => Console.Write(die.Symbol + "(" + (die.Save ? "S" : "R") + ") "));
                    Console.WriteLine();
                    var input = Console.ReadLine();
                    if (input == null) continue;
                    if (input.ToUpper().Equals("EXIT"))
                    {
                        break;
                    }
                    var numbers = input.Split(' ').ToList();
                    foreach (var number in numbers)
                    {
                        if (!int.TryParse(number, out n)) continue;
                        n -= 1;
                        if (n >= 0 && n < game.Dice.Count)
                            game.Dice[n].Save = !game.Dice[n].Save;
                    }
                }
                game.EndRolling();
                if (game.Board.TokyoCityIsOccupied && game.Board.TokyoCity.CanYield)
                {
                    Console.WriteLine(game.Board.TokyoCity.Name + ": Yield? Y/N");
                    var yield = Console.ReadLine();
                    if(yield != null && yield.ToUpper().Equals("Y")) game.Board.TokyoCity.Yield();
                }
                if (game.Board.TokyoBayIsOccupied && game.Board.TokyoBay.CanYield)
                {
                    Console.WriteLine(game.Board.TokyoBay.Name + ": Yield? Y/N");
                    var yield = Console.ReadLine();
                    if(yield != null && yield.ToUpper().Equals("Y")) game.Board.TokyoBay.Yield();
                }
                if (game.CardsForSale.Where(card => card.Cost <= game.Current.Energy).ToList().Count != 0)
                {
                    Console.WriteLine("Select Card to Buy or Exit");
                    game.CardsForSale.ForEach(card => Console.Write(card.Name + " " + card.Cost + "   "));
                    Console.WriteLine();
                    var cardSelection = Console.ReadLine();
                    if (cardSelection != null && !cardSelection.ToUpper().Equals("EXIT") &&
                        int.TryParse(cardSelection, out n))
                    {
                        n -= 1;
                        game.BuyCard(n);
                    }
                }

                Console.WriteLine(game.Current.Name + ", your turn is over!");
                Console.WriteLine("Victroy Points: " + game.Current.VictroyPoints);
                Console.WriteLine("Health: " + game.Current.Health);
                Console.WriteLine("Energy: " + game.Current.Energy);
                Console.WriteLine("Location: " + game.Current.Location);
                game.Current.Cards.ForEach(card => Console.Write(card.Name + " "));
                Console.WriteLine();
                game.EndTurn();
            }
            Console.WriteLine(game.Monsters.First().Name + " Wins!!!");
        }
    }
}