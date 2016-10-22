using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMaxi.Interfaces;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowFactory : IGameFactory
	{
		public IGameMoveEvaluator CreateMoveEvaluator()
		{
			throw new NotSupportedException();
		}

		public IGameLogic CreateLogic()
		{
			return new FourInARowLogic(CreateStateEvaluator());
		}

		public IGameStateEvaluator CreateStateEvaluator()
		{
			return new FourInARowEvaluator();
		}
	}
}
