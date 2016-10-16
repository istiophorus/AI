using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowState : IGameState
	{
		public FourInARowState()
		{
			_fields = new FourInARowFieldState[7][];

			for (Int32 q = 0; q < _fields.Length; q++)
			{
				_fields[q] = new FourInARowFieldState[6];
			}
		}

		public FourInARowState(FourInARowState source)
		{
			if (null == source)
			{
				throw new ArgumentNullException("source");
			}

			_fields = new FourInARowFieldState[7][];

			for (Int32 q = 0; q < _fields.Length; q++)
			{
				_fields[q] = (FourInARowFieldState[])source._fields[q].Clone();
			}
		}

		private FourInARowFieldState[][] _fields;

		public FourInARowFieldState[][] Fields { get; private set; }
	}
}
