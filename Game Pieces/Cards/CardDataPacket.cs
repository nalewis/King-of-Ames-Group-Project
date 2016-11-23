using System;

namespace GamePieces.Cards
{
    [Serializable]
    public struct CardDataPacket
    {
        public Type Type { get; }
        public bool Activated { get; }

        public CardDataPacket(Type type, bool activated)
        {
            Type = type;
            Activated = activated;
        }
    }
}