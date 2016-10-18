﻿﻿using System.Collections.Generic;
using System.Linq;
using DataStructures.Observer_Pattern;
using GamePieces.Cards;
using GamePieces.Session;

namespace GamePieces.Monsters
{
    public class Monster : Observable<Monster>
    {
        //Location & Neighbors
        private readonly int _index;

        public Monster Previous =>
            Game.Players == 1 ? this : _index != 0 ? Game.Monsters[_index - 1] : Game.Monsters.Last();

        public Monster Next =>
            Game.Players == 1 ? this : _index != Game.Players - 1 ? Game.Monsters[_index + 1] : Game.Monsters.First();

        //Name
        public string Name { get; }

        //State
        public State State
        {
            get { return Get(); }
            set { Set(value); }
        }

        //Location
        public Location Location
        {
            get { return Get(); }
            set { Set(value); }
        }

        public bool InTokyo => Location != Location.Default;
        public bool CanYield => InTokyo && Game.Attacked.Contains(this);

        //Cards
        public readonly List<Card> Cards = new List<Card>();

        public int NumberOfCards
        {
            get { return Get(); }
            private set { Set(value); }
        }

        public int PreviousNumberOfCards { get; private set; }

        //Energy
        public int Energy
        {
            get { return Get(); }
            set
            {
                if (Get() != null)
                {
                    if (value < 0) value = 0;
                    State = value > Energy ? State.Energizing : State.DeEnergizing;
                    PreviousEnergy = Energy;
                }
                Set(value);
            }
        }

        public int PreviousEnergy { get; private set; }

        //Victory Points
        public int VictroyPoints
        {
            get { return Get(); }
            set
            {
                if (Get() != null)
                {
                    if (value < 0) value = 0;
                    if (value > 20) value = 20;
                    State = value > VictroyPoints ? State.Scoring : State.Losing;
                    PreviousVictroyPoints = VictroyPoints;
                }
                Set(value);
            }
        }

        public int PreviousVictroyPoints { get; private set; }

        //Health
        public int Health
        {
            get { return Get(); }
            set
            {
                if (Get() != null)
                {
                    if (value <= 0)
                    {
                        value = 0;
                        Kill();
                    }
                    if (value > MaximumHealth) value = MaximumHealth;
                    State = value > Health ? State.Healing : State.Attacked;
                    if (State == State.Healing && InTokyo) return;
                    PreviousHealth = Health;
                }
                Set(value);
            }
        }

        public int PreviousHealth { get; private set; }
        public int MaximumHealth { get; set; } = 10;
        public int AttackPoints { get; set; }


        //Dice
        public int Dice { get; set; } = 6;
        public int MaximumRolls { get; set; } = 3;

        public int RemainingRolls
        {
            get { return Get(); }
            set { Set(value); }
        }

        /// <summary>
        /// The monster game piece
        /// </summary>
        /// <param name="name">The name of the monster</param>
        public Monster(string name = "")
        {
            Name = name;
            Energy = 0;
            NumberOfCards = 0;
            VictroyPoints = 0;
            Health = MaximumHealth;
            Location = Location.Default;
            RemainingRolls = 0;
            _index = Game.Players;
            State = State.EndOfTurn;
        }

        /// <summary>
        /// This needs to be called to start the turn of the monster.
        /// All values are set to the correct state for starting a turn.
        /// </summary>
        public void StartTurn()
        {
            State = State.StartOfTurn;
            Cards.ForEach(card => card.Reset());
            if (InTokyo) VictroyPoints += 2;
            RemainingRolls = MaximumRolls;
            DiceRoller.Setup(Dice);
        }

        /// <summary>
        /// Rolls all available dice in 'Game Components'
        /// </summary>
        public void Roll()
        {
            if (RemainingRolls == 0) return;
            State = State.Rolling;
            DiceRoller.Roll();
            RemainingRolls--;
            if (RemainingRolls == 0)
            {
                EndRolling();
            }
        }

        /// <summary>
        /// Tallys the score of the dice and returns them to the dice pool
        /// </summary>
        public void EndRolling()
        {
            State = State.TallyDice;
            DiceRoller.EndRolling(this);
        }

        /// <summary>
        /// Monster performs its attack
        /// </summary>
        public void Attack()
        {
            State = State.Attacking;
            Game.Attacked.Clear();
            if (AttackPoints != 0)
            {
                Game.Attacked.AddRange(Game.Monsters.Where(monster => monster.InTokyo != InTokyo).ToList());
                Game.Attacked.ForEach(monster => monster.Health -= AttackPoints);
                Board.Update();
                Board.MoveIntoTokyo(this);
            }
            State = State.None;
            AttackPoints = 0;
        }

        /// <summary>
        /// If in Tokyo and just attacked, the attacker and this swap locations
        /// </summary>
        public void Yield()
        {
            if (!CanYield) return;
            State = State.Yielding;
            Board.LeaveTokyo(this);
            Board.MoveIntoTokyo(Game.Current);
        }

        /// <summary>
        /// Add a power-up card to the monster by spending saved energy
        /// </summary>
        /// <param name="card">Card to buy</param>
        public void BuyCard(Card card)
        {
            if (Energy < card.Cost) return;
            State = State.BuyingCard;
            Energy -= card.Cost;
            if (card.CardType != CardType.Keep)
            {
                card.Update(this);
                if (card.CardType == CardType.Discard) return;
            }
            PreviousNumberOfCards = NumberOfCards;
            NumberOfCards++;
            Cards.Add(card);
            Subscribe(card);
        }

        /// <summary>
        /// Remove power-up card from the monster
        /// </summary>
        /// <param name="card">Card to remove</param>
        public void RemoveCard(Card card)
        {
            State = State.RemovingCard;
            Cards.Remove(card);
            Unsubscribe(card);
            card.UndoEffect(this);
            PreviousNumberOfCards = NumberOfCards;
            NumberOfCards--;
        }

        /// <summary>
        /// Sells the desired card to the given monster
        /// </summary>
        /// <param name="monster">Monster buying the card</param>
        /// <param name="card">Card being sold</param>
        public void SellCard(Monster monster, Card card)
        {
            RemoveCard(card);
            State = State.SellingCard;
            Energy += card.Cost;
            monster.BuyCard(card);
        }

        /// <summary>
        /// End the monster's turn and resets values
        /// </summary>
        public void EndTurn()
        {
            State = State.EndOfTurn;
        }

        /// <summary>
        /// Removes the monster from the game
        /// </summary>
        private void Kill()
        {
            if (InTokyo) Board.LeaveTokyo(this);
            Cards.Clear();
            Game.Monsters.Remove(this);
            Game.Dead.Add(this);
            State = State.Dead;
        }
    }
}