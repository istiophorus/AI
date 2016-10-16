using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMaxi.TicTacToe;

namespace MiniMaxi
{
	public static class Program
	{
		private static void PrintState(TicTacToeState state)
		{
			for (Int32 y = 0, my = 3; y < my; y++)
			{
				for (Int32 x = 0, mx = 3; x < mx; x++)
				{
					String text;

					switch (state.Fields[x][y])
					{
						case TicTacToeFieldState.Cross: 
							text = "X"; 
							break;

						case TicTacToeFieldState.Circle: 
							text = "O"; 
							break;

						default: 
							text = "_"; 
							break;
					}

					Console.Write(text);
				}

				Console.WriteLine();
			}

			Console.WriteLine();
		}

		static void Main(String[] args)
		{
			TicTacToeFactory factory = new TicTacToeFactory();

			IGameLogic logic = factory.CreateLogic();

			MiniMaxAlgorithm alg = new MiniMaxAlgorithm(3, factory);

			IGameState state = new TicTacToeState();

			while (true)
			{
				IGameMove move = alg.FindBestMove(state, GamePlayer.PlayerMax);

				if (null != move)
				{
					state = logic.MakeMove(move, state);
				}
				else
				{
					break;
				}

				PrintState((TicTacToeState)state);

				if (logic.IsFinished(state))
				{
					break;
				}

				Int32 x = Int32.Parse(Console.ReadLine());
				Int32 y = Int32.Parse(Console.ReadLine());

				state = logic.MakeMove(new TicTacToeMove
					{
						X = x,
						Y = y,
						Symbol = TicTacToeFieldState.Circle
					}, state);

				if (logic.IsFinished(state))
				{
					break;
				}
			}

			PrintState((TicTacToeState)state);
		}
	}
}
