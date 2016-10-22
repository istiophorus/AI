using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMaxi.Interfaces;

namespace MiniMaxi.TicTacToe
{
	public sealed class TicTacToeFactory : IGameFactory
	{
		public IGameMoveEvaluator CreateMoveEvaluator()
		{
			return new TicTacToeEvaluator();
		}

		public IGameLogic CreateLogic()
		{
			return new TicTacToeLogic(CreateStateEvaluator());
		}

		public IGameStateEvaluator CreateStateEvaluator()
		{
			return new TicTacToeEvaluator();
		}
	}
}
