using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMaxi.Interfaces;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowMove : IGameMove
	{
		public Int32 Column { get; set; }

		public FourInARowFieldState State { get; set; }

		public override String ToString()
		{
			return String.Format("[{0};{1}]", Column, State);
		}
	}
}
