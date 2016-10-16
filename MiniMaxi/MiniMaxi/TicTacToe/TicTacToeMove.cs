using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.TicTacToe
{
	public sealed class TicTacToeMove : IGameMove
	{
		public Int32 X { get; set; }

		public Int32 Y { get; set; }

		public TicTacToeFieldState Symbol { get; set; }
	}
}
