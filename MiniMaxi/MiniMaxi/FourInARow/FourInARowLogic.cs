using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniMaxi.Interfaces;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowLogic : IGameLogic
	{
		private readonly IGameStateEvaluator _evaluator;

		public FourInARowLogic(IGameStateEvaluator evaluator)
		{
			if (null == evaluator)
			{
				throw new ArgumentNullException("evaluator");
			}

			_evaluator = evaluator;
		}

		public IGameMove[] GetPossibleMoves(IGameState gameState, GamePlayer player)
		{
			if (null == gameState)
			{
				throw new ArgumentNullException("gameState");
			}

			FourInARowState state = (FourInARowState)gameState;

			List<IGameMove> moves = new List<IGameMove>();

			for (Int32 q = 0, mq = FourInARowState.ColumnCount ; q < mq; q++)
			{
				if (state.Get(q, FourInARowState.RowCount - 1) == FourInARowFieldState.Empty)
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
			if (null == gameState)
			{
				throw new ArgumentNullException("gameState");
			}

			if (null == gameMove)
			{
				throw new ArgumentNullException("gameMove");
			}

			FourInARowMove move = (FourInARowMove)gameMove;

			FourInARowState state = (FourInARowState)gameState;

			if (move.Column < 0 || move.Column >= FourInARowState.ColumnCount)
			{
				throw new ArgumentOutOfRangeException("column " + move.Column);
			}

			Int32 nextField = state.Indexes[move.Column];

			if (nextField < FourInARowState.RowCount && 
				state.Get(move.Column, nextField) == FourInARowFieldState.Empty)
			{
				FourInARowState newState = new FourInARowState(state);

				newState.Set(move.Column, nextField, move.State);

				return newState;
			}
			else
			{
				throw new InvalidOperationException(String.Format("Move not allowed {0} {1}", move.Column, nextField));
			}
		}

		public Boolean IsMovePossible(IGameState state)
		{
			IGameMove[] moves = GetPossibleMoves(state, GamePlayer.PlayerMax);

			return moves.Length > 0;
		}

		public Boolean IsTie(IGameState state)
		{
			return
				!IsMovePossible(state) &&
				!IsPlayerMaxWinner(state) &&
				!IsPlayerMinWinner(state);
		}

		public Boolean IsPlayerMaxWinner(IGameState state)
		{
			return _evaluator.Evaluate(state, GamePlayer.PlayerMax) >= FourInARowEvaluator.WinValue;
		}

		public Boolean IsPlayerMinWinner(IGameState state)
		{
			return _evaluator.Evaluate(state, GamePlayer.PlayerMin) <= -FourInARowEvaluator.WinValue;
		}

		public Boolean IsFinished(IGameState state)
		{
			return IsPlayerMaxWinner(state) ||
				IsPlayerMinWinner(state) ||
				!IsMovePossible(state);
		}
	}
}
