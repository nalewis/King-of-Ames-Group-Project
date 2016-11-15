using GamePieces.Cards;
using GamePieces.Session;
using Networking.Actions;

namespace Controllers
{
    public static class GameStateController
    {
        public static void AcceptAction(ActionPacket actionPacket)
        {
            switch (actionPacket.Action)
            {
                case Action.StartTurn:
                    Game.StartTurn();
                    break;
                case Action.Roll:
                    DiceController.Roll();
                    break;
                case Action.EndRolling:
                    DiceController.EndRolling();
                    break;
                case Action.BuyCard:
                    switch ((CardsForSale) actionPacket.Value)
                    {
                        case CardsForSale.One:
                            CardController.BuyCardOne();
                            break;
                        case CardsForSale.Two:
                            CardController.BuyCardTwo();
                            break;
                        case CardsForSale.Three:
                            CardController.BuyCardThree();
                            break;
                        default:
                            return;
                    }
                    break;
                case Action.RemoveCard:
                    Game.RemoveCard((Card) actionPacket.Value);
                    break;
                case Action.EndTurn:
                    Game.EndTurn();
                    break;
                case Action.Yield:
                    MonsterController.GetById(actionPacket.PlayerId).Yield();
                    break;
                case Action.SaveDie:
                    DiceController.SaveDie((int) actionPacket.Value);
                    break;
                case Action.UnSaveDie:
                    DiceController.UnSaveDie((int) actionPacket.Value);
                    break;
                default:
                    return;
            }
        }

        public static ActionPacket StartTurn()
        {
            return new ActionPacket(Action.StartTurn);
        }

        public static ActionPacket Roll()
        {
            return new ActionPacket(Action.Roll);
        }

        public static ActionPacket EndRolling()
        {
            return new ActionPacket(Action.EndRolling);
        }

        public static ActionPacket BuyCard(CardsForSale cardsForSale)
        {
            return new ActionPacket(Action.BuyCard, value: cardsForSale);
        }

        public static ActionPacket RemoveCard()
        {
            return new ActionPacket(Action.RemoveCard);
        }

        public static ActionPacket EndTurn()
        {
            return new ActionPacket(Action.EndTurn);
        }

        public static ActionPacket Yield(int playerId)
        {
            return new ActionPacket(Action.Yield, playerId);
        }

        public static ActionPacket SaveDie(int index)
        {
            return new ActionPacket(Action.SaveDie, value: index);
        }

        public static ActionPacket UnSaveDie(int index)
        {
            return new ActionPacket(Action.UnSaveDie, value: index);
        }
    }
}