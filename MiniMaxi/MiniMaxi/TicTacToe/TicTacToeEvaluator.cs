using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.TicTacToe
{
	public sealed class TicTacToeEvaluator : IGameMoveEvaluator, IGameStateEvaluator
	{
		private struct Coords
		{
			internal Coords(Int32 x, Int32 y)
			{
				X = x;

				Y = y;
			}

			internal Int32 X;

			internal Int32 Y;
		}

		private static readonly Coords[][] Lines = new Coords[][] 
			{
				new Coords[]
				{
					new Coords(0, 0),
					new Coords(1, 1),
					new Coords(2, 2),
				},

				new Coords[]
				{
					new Coords(2, 0),
					new Coords(1, 1),
					new Coords(0, 2),
				},

				new Coords[]
				{
					new Coords(0, 0),
					new Coords(0, 1),
					new Coords(0, 2),
				},

				new Coords[]
				{
					new Coords(1, 0),
					new Coords(1, 1),
					new Coords(1, 2),
				},

				new Coords[]
				{
					new Coords(2, 0),
					new Coords(2, 1),
					new Coords(2, 2),
				},

				new Coords[]
				{
					new Coords(0, 0),
					new Coords(1, 0),
					new Coords(2, 0),
				},

				new Coords[]
				{
					new Coords(0, 1),
					new Coords(1, 1),
					new Coords(2, 1),
				},

				new Coords[]
				{
					new Coords(0, 2),
					new Coords(1, 2),
					new Coords(2, 2),
				},
			};

		private static Int32 GetSign(TicTacToeFieldState state)
		{
			if (state == TicTacToeFieldState.Cross)
			{
				return 1;
			}
			else if (state == TicTacToeFieldState.Circle)
			{
				return -1;
			}
			else
			{
				throw new ArgumentException(state.ToString());
			}
		}

		private const Int32 WinValue = 1000;

		private const Int32 DefendingBonus = 100;

		private const Int32 MiddleFieldBonus = 5;

		private const Int32 LineWithTwoBonus = 4;

		public Int32 Evaluate(IGameState gameState, IGameMove gameMove, IGameState newGameState)
		{
			TicTacToeState state = (TicTacToeState)newGameState;

			TicTacToeMove move = (TicTacToeMove)gameMove;

			Int32 sign = GetSign(move.Symbol);

			#region horizontal lines

			Int32 verticalCounter = 0;

			Int32 verticalOponentCounter = 0;

			for (Int32 q = 0; q < 3; q++)
			{
				TicTacToeFieldState field = state.Fields[q][move.Y];

				if (field == move.Symbol)
				{
					verticalCounter++;
				}
				else if (field != TicTacToeFieldState.Empty)
				{
					verticalOponentCounter++;
				}
			}

			if (verticalCounter >= 3)
			{
				return WinValue * sign;
			}

			if (verticalOponentCounter >= 3)
			{
				return WinValue * (-sign);
			}

			#endregion horizontal lines

			#region vertical lines

			Int32 horizontalCounter = 0;

			Int32 horizontalOponentCounter = 0;

			for (Int32 q = 0; q < 3; q++)
			{
				TicTacToeFieldState field = state.Fields[move.X][q];

				if (field == move.Symbol)
				{
					horizontalCounter++;
				}
				else if (field != TicTacToeFieldState.Empty)
				{
					horizontalOponentCounter++;
				}
			}

			if (horizontalCounter >= 3)
			{
				return WinValue * sign;
			}

			if (horizontalOponentCounter >= 3)
			{
				return WinValue * -sign;
			}

			#endregion vertical lines

			#region diagonal lines

			Int32 diagonalCounterA = 0;

			Int32 oponentDiagonalCounterA = 0;

			if (move.X == move.Y) //// check diagonal A
			{
				Coords[] diagonal = Lines[0];

				for (Int32 q = 0; q < 3; q++)
				{
					Coords coords = diagonal[q];

					TicTacToeFieldState field = state.Fields[coords.X][coords.Y];

					if (field == move.Symbol)
					{
						diagonalCounterA++;
					}
					else if (field != TicTacToeFieldState.Empty)
					{
						oponentDiagonalCounterA++;
					}
				}
			}

			if (diagonalCounterA >= 3)
			{
				return WinValue * sign;
			}

			if (oponentDiagonalCounterA >= 3)
			{
				return WinValue * -sign;
			}

			Int32 diagonalCounterB = 0;

			Int32 oponentDiagonalCounterB = 0;

			if (move.X + move.Y == 2) //// check diagonal B
			{
				Coords[] diagonal = Lines[1];

				for (Int32 q = 0; q < 3; q++)
				{
					Coords coords = diagonal[q];

					TicTacToeFieldState field = state.Fields[coords.X][coords.Y];

					if (field == move.Symbol)
					{
						diagonalCounterB++;
					}
					else if (field != TicTacToeFieldState.Empty)
					{
						oponentDiagonalCounterB++;
					}
				}
			}

			if (diagonalCounterB >= 3)
			{
				return WinValue * sign;
			}

			if (oponentDiagonalCounterB >= 3)
			{
				return WinValue * -sign;
			}

			#endregion diagonal lines

			if (horizontalOponentCounter == 2 && horizontalCounter == 1)
			{
				return DefendingBonus * sign; ;
			}

			if (verticalOponentCounter == 2 && verticalCounter == 1)
			{
				return DefendingBonus * sign;
			}

			if (oponentDiagonalCounterB == 2 && diagonalCounterB == 1)
			{
				return DefendingBonus * sign;
			}

			if (oponentDiagonalCounterA == 2 && diagonalCounterA == 1)
			{
				return DefendingBonus * sign;
			}

			Int32 linesWithTwo = 0;

			if (horizontalCounter == 2 && horizontalOponentCounter == 0)
			{
				linesWithTwo++;
			}

			if (verticalCounter == 2 && verticalOponentCounter == 0)
			{
				linesWithTwo++;
			}

			if (diagonalCounterB == 2 && oponentDiagonalCounterB == 0)
			{
				linesWithTwo++;
			}

			if (diagonalCounterA == 2 && oponentDiagonalCounterA == 0)
			{
				linesWithTwo++;
			}

			Int32 resultValue = 0;

			if (linesWithTwo > 0)
			{
				resultValue = linesWithTwo * LineWithTwoBonus * sign;
			}

			if (move.X == 1 && move.Y == 1)
			{
				resultValue = MiddleFieldBonus * sign;
			}

			return resultValue;
		}

		public Int32 Evaluate(IGameState gameState, GamePlayer player)
		{
			TicTacToeState state = (TicTacToeState)gameState;

			for (Int32 q = 0; q < Lines.Length; q++)
			{
				Int32 crosses = 0;
				Int32 circles = 0;

				Coords[] line = Lines[q];

				for (Int32 w = 0; w < line.Length; w++)
				{
					Coords coords = line[w];

					TicTacToeFieldState fieldState = state.Fields[coords.X][coords.Y];

					if (fieldState == TicTacToeFieldState.Cross)
					{
						crosses++;
					}
					else if (fieldState == TicTacToeFieldState.Circle)
					{
						circles++;
					}
				}

				if (crosses == 3)
				{
					return WinValue * GetSign(TicTacToeFieldState.Cross);
				}

				if (circles == 3)
				{
					return WinValue * GetSign(TicTacToeFieldState.Circle);
				}
			}

			return 0;
		}
	}
}
