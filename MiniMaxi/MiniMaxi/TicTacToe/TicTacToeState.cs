using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.TicTacToe
{
	public sealed class TicTacToeState : IGameState, ICloneable
	{
		public TicTacToeState()
		{
			Fields = new TicTacToeFieldState[3][];

			Fields[0] = new TicTacToeFieldState[3];
			Fields[1] = new TicTacToeFieldState[3];
			Fields[2] = new TicTacToeFieldState[3];
		}

		public TicTacToeFieldState[][] Fields { get; private set; }

		public Object Clone()
		{
			TicTacToeState result = new TicTacToeState
				{
					Fields = new TicTacToeFieldState[3][]
				};

			result.Fields[0] = (TicTacToeFieldState[])Fields[0].Clone();
			result.Fields[1] = (TicTacToeFieldState[])Fields[1].Clone();
			result.Fields[2] = (TicTacToeFieldState[])Fields[2].Clone();

			return result;
		}
	}
}
