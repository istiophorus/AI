using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowEvaluator : IGameMoveEvaluator, IGameStateEvaluator
	{
		public Int32 Evaluate(IGameState gameState, IGameMove gameMove, IGameState newGameState)
		{
			throw new NotImplementedException();
		}

		private static readonly Int32 WinValue = 10000;

		private static readonly Int32 WinningCount = 4;

		private static Int32 GetSign(FourInARowFieldState state)
		{
			if (state == FourInARowFieldState.Cross)
			{
				return 1;
			}
			else if (state == FourInARowFieldState.Circle)
			{
				return -1;
			}
			else
			{
				throw new ArgumentException(state.ToString());
			}
		}

		public Int32 Evaluate(IGameState gameState)
		{
			FourInARowState state = (FourInARowState)gameState;

			Int32 result = CheckColumns(state);

			if (result != 0)
			{
				return result;
			}

			result = CheckRows(state);

			if (result != 0)
			{
				return result;
			}

			return 0;
		}

		private static Int32 CheckRows(FourInARowState state)
		{
			for (Int32 y = 0, my = FourInARowState.RowCount; y < my; y++)
			{
				Int32 crossesInRow = 0;

				Int32 circlesInRow = 0;

				for (Int32 x = 0, mx = FourInARowState.ColumnCount; x < mx; x++)
				{
					FourInARowFieldState field = state.Fields[x][y];

					if (field == FourInARowFieldState.Circle)
					{
						circlesInRow++;
						crossesInRow = 0;
					}
					else
					{
						circlesInRow = 0;
					}

					if (field == FourInARowFieldState.Cross)
					{
						crossesInRow++;
						circlesInRow = 0;
					}
					else
					{
						crossesInRow = 0;
					}

					if (crossesInRow >= WinningCount)
					{
						return WinValue * GetSign(FourInARowFieldState.Cross);
					}

					if (circlesInRow >= WinningCount)
					{
						return WinValue * GetSign(FourInARowFieldState.Circle);
					}
				}
			}

			return 0;
		}

		private static Int32 CheckColumns(FourInARowState state)
		{
			for (Int32 x = 0, mx = state.Fields.Length; x < mx; x++)
			{
				Int32 crossesInRow = 0;

				Int32 circlesInRow = 0;

				for (Int32 y = 0, my = state.Fields[x].Length; y < my; y++)
				{
					FourInARowFieldState field = state.Fields[x][y];

					if (field == FourInARowFieldState.Circle)
					{
						circlesInRow++;
						crossesInRow = 0;
					}
					else
					{
						circlesInRow = 0;
					}

					if (field == FourInARowFieldState.Cross)
					{
						crossesInRow++;
						circlesInRow = 0;
					}
					else
					{
						crossesInRow = 0;
					}

					if (crossesInRow >= WinningCount)
					{
						return WinValue * GetSign(FourInARowFieldState.Cross);
					}

					if (circlesInRow >= WinningCount)
					{
						return WinValue * GetSign(FourInARowFieldState.Circle);
					}
				}
			}

			return 0;
		}
	}
}
