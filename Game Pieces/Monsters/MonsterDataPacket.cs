using System;
using GamePieces.Cards;

namespace GamePieces.Monsters
{
    [Serializable]
    public struct MonsterDataPacket
    {
        public int PlayerId { get; }
        public int Index { get; }
        public string Name { get; }
        public State State { get; }
        public Location Location { get; }
        public Card[] Cards { get; }
        public int NumberOfCards { get; }
        public int PreviousNumberOfCards { get; }
        public int Energy { get; }
        public int PreviousEnergy { get; }
        public int VictoryPoints { get; }
        public int PreviousVictroyPoints { get; }
        public int Health { get; }
        public int PreviousHealth { get; }
        public int MaximumHealth { get; }
        public int AttackPoints { get; }
        public int Dice { get; }
        public int MaximumRolls { get; }
        public int RemainingRolls { get; }

        public MonsterDataPacket(int playerId, int index, string name, State state, Location location, Card[] cards,
            int numberOfCards, int previousNumberOfCards, int energy, int previousEnergy, int victoryPoints,
            int previousVictroyPoints, int health, int previousHealth, int maximumHealth, int attackPoints, int dice,
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
            PreviousVictroyPoints = previousVictroyPoints;
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