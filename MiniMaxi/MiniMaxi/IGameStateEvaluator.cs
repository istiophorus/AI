using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi
{
	public interface IGameStateEvaluator
	{
		Int32 Evaluate(IGameState gameState, GamePlayer player);
	}
}
