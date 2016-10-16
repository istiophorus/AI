using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi
{
	public interface IGameMoveEvaluator
	{
		Int32 Evaluate(IGameState gameState, IGameMove gameMove, IGameState newGameState);
	}
}
