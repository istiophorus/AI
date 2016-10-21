using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowEvaluator : IGameStateEvaluator
	{
		private static readonly Int32 MiddleValue = 10;

		internal static readonly Int32 WinValue = Int32.MaxValue;

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

		public Int32 Evaluate(IGameState gameState, GamePlayer player)
		{
			FourInARowState state = (FourInARowState)gameState;

			PartialResults summary = new PartialResults();

			PartialResults result = CheckColumns(state);

			summary.Add(result);

			result = CheckRows(state);

			summary.Add(result);

			result = CheckDiagonals(state);

			summary.Add(result);

			return CalculateScore(summary, player);
		}

		private static PartialResults CheckDiagonals(FourInARowState state)
		{
			PartialResults summary = new PartialResults();

			//// from left bottom to right top

			for (Int32 startY = 0, mStartY = FourInARowState.RowCount - WinningCount + 1; startY < mStartY; startY++)
			{
				FieldContext context = new FieldContext();

				for (Int32 x = 0, y = startY, my = FourInARowState.RowCount, mx = FourInARowState.ColumnCount; y < my && x < mx; x++, y++)
				{
					ProcessField(state, x, y, context);
				}

				summary.Add(context.PartialResults);
			}

			for (Int32 startX = 1, mStartX = FourInARowState.ColumnCount - WinningCount + 1; startX < mStartX; startX++)
			{
				FieldContext context = new FieldContext();

				for (Int32 x = startX, y = 0, my = FourInARowState.RowCount, mx = FourInARowState.ColumnCount; y < my && x < mx; x++, y++)
				{
					ProcessField(state, x, y, context);
				}

				summary.Add(context.PartialResults);
			}

			//// from right bottom to left top

			for (Int32 startY = 0, mStartY = FourInARowState.RowCount - WinningCount + 1; startY < mStartY; startY++)
			{
				FieldContext context = new FieldContext();

				for (Int32 x = FourInARowState.ColumnCount - 1, y = startY, my = FourInARowState.RowCount; y < my && x >= 0; x--, y++)
				{
					ProcessField(state, x, y, context);
				}

				summary.Add(context.PartialResults);
			}

			for (Int32 sStartX = FourInARowState.ColumnCount - 2, mStartX = WinningCount - 1; sStartX >= mStartX; sStartX--)
			{
				FieldContext context = new FieldContext();

				for (Int32 x = sStartX, y = 0, my = FourInARowState.RowCount; y < my && x >= 0; x--, y++)
				{
					ProcessField(state, x, y, context);
				}

				summary.Add(context.PartialResults);
			}

			return summary;
		}

		private static PartialResults CheckRows(FourInARowState state)
		{
			PartialResults summary = new PartialResults();

			for (Int32 y = 0, my = FourInARowState.RowCount; y < my; y++)
			{
				FieldContext context = new FieldContext();

				for (Int32 x = 0, mx = FourInARowState.ColumnCount; x < mx; x++)
				{
					ProcessField(state, x, y, context);
				}

				summary.Add(context.PartialResults);
			}

			return summary;
		}

		private sealed class FieldContext
		{
			internal Int32 CirclesInRow;

			internal Int32 CrossesInRow;

			internal Int32 EmptyInRow;

			internal PartialResults PartialResults = new PartialResults();

			internal List<FourInARowFieldState> LastFourItems = new List<FourInARowFieldState>(4);
		}

		private static void ProcessField(FourInARowState state, Int32 x, Int32 y, FieldContext context)
		{
			FourInARowFieldState field = state.Get(x, y);

			context.LastFourItems.Add(field);

			if (field == FourInARowFieldState.Empty)
			{
				context.EmptyInRow++;
			}

			if (field == FourInARowFieldState.Circle)
			{
				context.CirclesInRow++;
			}

			if (field == FourInARowFieldState.Cross)
			{
				context.CrossesInRow++;
			}

			if (context.LastFourItems.Count >= 4)
			{
				if (context.LastFourItems.Count > 4)
				{
					FourInARowFieldState first = context.LastFourItems[0];

					context.LastFourItems.RemoveAt(0);

					if (first == FourInARowFieldState.Empty)
					{
						context.EmptyInRow--;
					}

					if (first == FourInARowFieldState.Circle)
					{
						context.CirclesInRow--;
					}

					if (first == FourInARowFieldState.Cross)
					{
						context.CrossesInRow--;
					}
				}

				if (context.CirclesInRow == 0)
				{
					if (context.CrossesInRow == 4)
					{
						context.PartialResults.FourCrossesInRow++;
					}
					else if (context.CrossesInRow == 2 && context.EmptyInRow == 2)
					{
						context.PartialResults.TwoCrossesInRow++;
					}
					else if (context.CrossesInRow == 3 && context.EmptyInRow == 1)
					{
						context.PartialResults.ThreeCrossesInRow++;
					}
					else if (context.CrossesInRow == 1 && context.EmptyInRow == 3)
					{
						context.PartialResults.OneCrossInRow++;
					}
				}

				if (context.CrossesInRow == 0)
				{
					if (context.CirclesInRow == 4)
					{
						context.PartialResults.FourCirclesInRow++;
					}
					else if (context.CirclesInRow == 2 && context.EmptyInRow == 2)
					{
						context.PartialResults.TwoCirclesInRow++;
					}
					else if (context.CirclesInRow == 3 && context.EmptyInRow == 1)
					{
						context.PartialResults.ThreeCirclesInRow++;
					}
					else if (context.CirclesInRow == 1 && context.EmptyInRow == 3)
					{
						context.PartialResults.OneCircleInRow++;
					}
				}
			}
		}

		private static PartialResults CheckColumns(FourInARowState state)
		{
			PartialResults summary = new PartialResults();

			for (Int32 x = 0, mx = FourInARowState.ColumnCount; x < mx; x++)
			{
				FieldContext context = new FieldContext();

				for (Int32 y = 0, my = FourInARowState.RowCount; y < my; y++)
				{
					ProcessField(state, x, y, context);
				}

				summary.Add(context.PartialResults);
			}

			return summary;
		}

		private static Int32 CalculateScore(PartialResults summary, GamePlayer player)
		{
			if (summary.FourCrossesInRow > 0)
			{
				return WinValue * GetSign(FourInARowFieldState.Cross);
			}

			if (summary.FourCirclesInRow > 0)
			{
				return WinValue * GetSign(FourInARowFieldState.Circle);
			}

			Int32 result;

			if (player == GamePlayer.PlayerMax)
			{
				result =
					summary.ThreeCrossesInRow * 1000 * 1 +
					summary.TwoCrossesInRow * 100 * 1 +
					summary.OneCrossInRow * 10 * 1 + 
					summary.ThreeCirclesInRow * 5000 * -1 +
					summary.TwoCirclesInRow * 500 * -1 + 
					summary.OneCircleInRow * 50 * -1;
			}
			else
			{
				result =
					summary.ThreeCrossesInRow * 5000 * 1 +
					summary.TwoCrossesInRow * 500 * 1 +
					summary.OneCrossInRow * 50 * 1 + 
					summary.ThreeCirclesInRow * 1000 * -1 +
					summary.TwoCirclesInRow * 100 * -1 +
					summary.OneCircleInRow * 10 * -1;
			}

			return result;
		}

		private sealed class PartialResults
		{
			internal Int32 OneCrossInRow;

			internal Int32 OneCircleInRow;

			internal Int32 TwoCrossesInRow;

			internal Int32 ThreeCrossesInRow;

			internal Int32 FourCrossesInRow;

			internal Int32 TwoCirclesInRow;

			internal Int32 ThreeCirclesInRow;

			internal Int32 FourCirclesInRow;

			internal void Add(PartialResults other)
			{
				OneCrossInRow += other.OneCrossInRow;

				TwoCrossesInRow += other.TwoCrossesInRow;

				ThreeCrossesInRow += other.ThreeCrossesInRow;

				FourCrossesInRow += other.FourCrossesInRow;

				OneCircleInRow += other.OneCircleInRow;

				TwoCirclesInRow += other.TwoCirclesInRow;

				ThreeCirclesInRow += other.ThreeCirclesInRow;

				FourCirclesInRow += other.FourCirclesInRow;
			}
		}
	}
}
