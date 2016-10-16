using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowEvaluator : IGameMoveEvaluator, IGameStateEvaluator
	{
		public int Evaluate(IGameState gameState, IGameMove gameMove, IGameState newGameState)
		{
			throw new NotImplementedException();
		}

		public int Evaluate(IGameState gameState)
		{
			throw new NotImplementedException();
		}
	}
}
