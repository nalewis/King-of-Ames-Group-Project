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

        /// <summary>
        /// Buy the first card
        /// </summary>
        public static void BuyCardOne()
        {
<<<<<<< HEAD
            if(CardForSaleOne() != null) Game.BuyCard(0);
=======
>>>>>>> palmer_develop
        }

        /// <summary>
        /// Buy the second card
        /// </summary>
        public static void BuyCardTwo()
        {
<<<<<<< HEAD
            if(CardForSaleTwo() != null) Game.BuyCard(1);
=======
            if (CardForSaleTwo() != null) Game.BuyCard(1);
>>>>>>> palmer_develop
        }

        /// <summary>
        /// Buy the third card
        /// </summary>
        public static void BuyCardThree()
        {
<<<<<<< HEAD
            if(CardForSaleThree() != null) Game.BuyCard(2);
=======
            if (CardForSaleThree() != null) Game.BuyCard(2);
>>>>>>> palmer_develop
        }
    }
}