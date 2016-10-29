using GamePieces.Cards;
using GamePieces.Session;

namespace Controllers
{
    public static class CardController
    {

        /// <summary>
        /// Gets the card on top of the deck without removing it
        /// </summary>
        /// <returns>Card</returns>
        public static Card TopOfDeck()
        {
            return Game.Deck.Count != 0 ? Game.Deck.Peek() : null;
        }

        /// <summary>
        /// Gets the first card for sale
        /// </summary>
        /// <returns>Card</returns>
        public static Card CardForSaleOne()
        {
            return Game.CardsForSale.Count > 0 ? Game.CardsForSale[0] : null;
        }

        /// <summary>
        /// Gets the second card for sale
        /// </summary>
        /// <returns>Card</returns>
        public static Card CardForSaleTwo()
        {
            return Game.CardsForSale.Count > 1 ? Game.CardsForSale[1] : null;
        }

        /// <summary>
        /// Gets the third card for sale
        /// </summary>
        /// <returns>Card</returns>
        public static Card CardForSaleThree()
        {
            return Game.CardsForSale.Count > 2 ? Game.CardsForSale[2] : null;
        }
    }
}