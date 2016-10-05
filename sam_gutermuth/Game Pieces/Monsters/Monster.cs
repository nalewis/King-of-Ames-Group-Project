﻿using System.Collections.Generic;
using System.Linq;
using DataStructures.Observer_Pattern;
using GamePieces.Cards;
using GamePieces.Session;

namespace GamePieces.Monsters
{
    public class Monster : Observable<Monster>
    {
        //Game Components
        private GameComponents GameComponents { get; }
        public List<Monster> Monsters => GameComponents.Monsters;
        public Board Board => GameComponents.Board;
        private Combat Combat => GameComponents.Combat;
        public DiceRoller DiceRoller => GameComponents.DiceRoller;

        //Location & Neighbors
        private readonly int Index;
        public Monster Previous => Monsters.Count == 1 ? this : Index != 0 ? Monsters[Index - 1] : Monsters.Last();

        public Monster Next
            => Monsters.Count == 1 ? this : Index != Monsters.Count - 1 ? Monsters[Index + 1] : Monsters.First();


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
                    if (value < 0) value = 0;
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
        /// <param name="gameComponents">The components which the monster belongs to</param>
        /// <param name="name">The name of the monster</param>
        public Monster(GameComponents gameComponents, string name = "")
        {
            GameComponents = gameComponents;
            Name = name;
            Energy = 0;
            NumberOfCards = 0;
            VictroyPoints = 0;
            Health = MaximumHealth;
            Location = Location.Default;
            RemainingRolls = 0;
            Index = GameComponents.Monsters.Count;
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
            if (InTokyo) VictroyPoints+=2;
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
            Combat.Attack(this);
        }

        /// <summary>
        /// If in Tokyo and just attacked, the attacker and this swap locations
        /// </summary>
        public void Yield()
        {
            if (!InTokyo || !Combat.Attacked.Contains(this)) return;
            State = State.Yielding;
            Board.LeaveTokyo(this);
            Board.MoveIntoTokyo(Combat.Attacker);
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

            if (GameComponents.CardsForSale.Contains(card))
            {
                GameComponents.CardsForSale.Remove(card);
                GameComponents.CardsForSale.Add(GameComponents.Deck.Pop());
            }

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
            Combat.Reset();
        }

        /// <summary>
        /// Removes the monster from the game
        /// </summary>
        public void Kill()
        {
            State = State.Dead;
            if (InTokyo) Board.LeaveTokyo(this);
            Cards.Clear();
            GameComponents.Monsters.Remove(this);
            GameComponents.Dead.Add(this);
        }
    }
}