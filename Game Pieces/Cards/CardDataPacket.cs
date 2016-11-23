using System;

namespace GamePieces.Cards
{
    [Serializable]
    public struct CardDataPacket
    {
        public Type Type { get; }

        public CardDataPacket(Type type)
        {
            Type = type;
        }
    }
}