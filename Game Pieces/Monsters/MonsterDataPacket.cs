using System;
using GamePieces.Cards;

namespace GamePieces.Monsters
{
    [Serializable]
    public struct MonsterDataPacket
    {
        public int PlayerId { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public State State { get; set; }
        public Location Location { get; set; }
        public Card[] Cards { get; set; }
        public int NumberOfCards { get; set; }
        public int PreviousNumberOfCards { get; set; }
        public int Energy { get; set; }
        public int PreviousEnergy { get; set; }
        public int VictoryPoints { get; set; }
        public int PreviousVictoryPoints { get; set; }
        public int Health { get; set; }
        public int PreviousHealth { get; set; }
        public int MaximumHealth { get; set; }
        public int AttackPoints { get; set; }
        public int Dice { get; set; }
        public int MaximumRolls { get; set; }
        public int RemainingRolls { get; set; }

        public MonsterDataPacket(int playerId, int index, string name, State state, Location location, Card[] cards,
            int numberOfCards, int previousNumberOfCards, int energy, int previousEnergy, int victoryPoints,
            int previousVictoryPoints, int health, int previousHealth, int maximumHealth, int attackPoints, int dice,
            int maximumRolls, int remainingRolls)
        {
            PlayerId = playerId;
            Index = index;
            Name = name;
            State = state;
            Location = location;
            Cards = cards;
            NumberOfCards = numberOfCards;
            PreviousNumberOfCards = previousNumberOfCards;
            Energy = energy;
            PreviousEnergy = previousEnergy;
            VictoryPoints = victoryPoints;
            PreviousVictoryPoints = previousVictoryPoints;
            Health = health;
            PreviousHealth = previousHealth;
            MaximumHealth = maximumHealth;
            AttackPoints = attackPoints;
            Dice = dice;
            MaximumRolls = maximumRolls;
            RemainingRolls = remainingRolls;
        }
    }
}