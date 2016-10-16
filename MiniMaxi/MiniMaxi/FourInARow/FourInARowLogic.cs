using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowLogic : IGameLogic
	{
		public IGameMove[] GetPossibleMoves(IGameState gameState, GamePlayer player)
		{
			if (null == gameState)
			{
				throw new ArgumentNullException("gameState");
			}

			FourInARowState state = (FourInARowState)gameState;

			List<IGameMove> moves = new List<IGameMove>();

			for (Int32 q = 0, mq = state.Fields.Length; q < mq; q++)
			{
				if (state.Fields[q][state.Fields[q].Length - 1] != FourInARowFieldState.Empty)
				{
					moves.Add(new FourInARowMove
						{
							Column = q,
							State = player == GamePlayer.PlayerMax ? FourInARowFieldState.Cross : FourInARowFieldState.Circle
						});
				}
			}

			return moves.ToArray();
		}

		public IGameState MakeMove(IGameMove gameMove, IGameState gameState)
		{
			throw new NotImplementedException();
		}

		public Boolean IsMovePossible(IGameState state, IGameMove move)
		{
			throw new NotImplementedException();
		}

		public Boolean IsTie(IGameState state)
		{
			throw new NotImplementedException();
		}

		public Boolean IsPlayerMaxWinner(IGameState state)
		{
			throw new NotImplementedException();
		}

		public Boolean IsPlayerMinWinner(IGameState state)
		{
			throw new NotImplementedException();
		}

		public Boolean IsFinished(IGameState state)
		{
			throw new NotImplementedException();
		}
	}
}
