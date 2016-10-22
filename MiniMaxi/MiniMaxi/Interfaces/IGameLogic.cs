using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.Interfaces
{
	public interface IGameLogic
	{
		IGameMove[] GetPossibleMoves(IGameState gameState, GamePlayer player);

		IGameState MakeMove(IGameMove gameMove, IGameState gameState);

		Boolean IsMovePossible(IGameState state);

		Boolean IsTie(IGameState state);

		Boolean IsPlayerMaxWinner(IGameState state);

		Boolean IsPlayerMinWinner(IGameState state);

		Boolean IsFinished(IGameState state);
	}
}
