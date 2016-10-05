using System;
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
                game.StartTurn();
                Console.WriteLine("\n\r" + game.Current.Name + ", select Dice to Switch Save State or Exit Rolling");
                while (game.Current.RemainingRolls != 0)
                {
                    game.Roll();
                    game.Dice.ForEach(die => Console.Write(die.Symbol + " "));
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
                        int n;
                        if (!int.TryParse(number, out n)) continue;
                        n -= 1;
                        if(n >= 0 && n < game.Dice.Count)
                            game.Dice[n].Save = !game.Dice[n].Save;
                    }
                }
                game.EndRolling();
                Console.WriteLine(game.Current.Name + ", your turn is over!");
                Console.WriteLine("Victroy Points: " + game.Current.VictroyPoints);
                Console.WriteLine("Health: " + game.Current.Health);
                game.EndTurn();
            }
        }
    }
}