using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.TicTacToe
{
	public sealed class TicTacToeLogic : IGameLogic
	{
		public TicTacToeLogic(IGameStateEvaluator evaluator)
		{
			if (null == evaluator)
			{
				throw new ArgumentNullException("evaluator");
			}

			_evaluator = evaluator;
		}

		private readonly IGameStateEvaluator _evaluator;

		public IGameMove[] GetPossibleMoves(IGameState gameState, GamePlayer player)
		{
			if (null == gameState)
			{
				throw new ArgumentNullException("gameState");
			}

			TicTacToeState state = (TicTacToeState)gameState;

			List<IGameMove> results = new List<IGameMove>(9);

			for (Int32 x = 0, mx = state.Fields.Length; x < mx; x++)
			{
				for (Int32 y = 0, my = state.Fields.Length; y < my; y++)
				{
					if (state.Fields[x][y] == TicTacToeFieldState.Empty)
					{
						results.Add(new TicTacToeMove
						{
							X = x,
							Y = y,
							Symbol = player == GamePlayer.PlayerMax ? TicTacToeFieldState.Cross : TicTacToeFieldState.Circle
						});
					}
				}
			}

			return results.ToArray();
		}

		public IGameState MakeMove(IGameMove gameMove, IGameState gameState)
		{
			TicTacToeMove move = (TicTacToeMove)gameMove;

			TicTacToeState state = (TicTacToeState)gameState;

			if (move.X < 0 || move.X > 2)
			{
				throw new ArgumentOutOfRangeException("X");
			}

			if (move.Y < 0 || move.Y > 2)
			{
				throw new ArgumentOutOfRangeException("Y");
			}

			if (move.Symbol != TicTacToeFieldState.Circle && move.Symbol != TicTacToeFieldState.Cross)
			{
				throw new ArgumentOutOfRangeException("Symbol");
			}

			if (state.Fields[move.X][move.Y] != TicTacToeFieldState.Empty)
			{
				throw new InvalidOperationException(String.Format("Field is not empty {0} {1} {2}", move.X, move.Y, state.Fields[move.X][move.Y]));
			}

			TicTacToeState newState = (TicTacToeState)state.Clone();

			newState.Fields[move.X][move.Y] = move.Symbol;

			return newState;
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
			return _evaluator.Evaluate(state, GamePlayer.PlayerMax) > 0;
		}

		public Boolean IsPlayerMinWinner(IGameState state)
		{
			return _evaluator.Evaluate(state, GamePlayer.PlayerMax) < 0;
		}

		public Boolean IsFinished(IGameState state)
		{
			return IsPlayerMaxWinner(state) ||
				IsPlayerMinWinner(state) ||
				!IsMovePossible(state);
		}
	}
}
