using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowMove : IGameMove
	{
		public Int32 Column { get; set; }

		public FourInARowFieldState State { get; set; }
	}
}
