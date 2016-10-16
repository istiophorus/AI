using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowFactory : IGameFactory
	{
		public IGameMoveEvaluator CreateMoveEvaluator()
		{
			return new FourInARowEvaluator();
		}

		public IGameLogic CreateLogic()
		{
			return new FourInARowLogic();
		}

		public IGameStateEvaluator CreateStateEvaluator()
		{
			return new FourInARowEvaluator();
		}
	}
}
