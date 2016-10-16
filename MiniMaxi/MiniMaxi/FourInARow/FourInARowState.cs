using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowState : IGameState
	{
		internal static readonly Int32 ColumnCount = 7;

		internal static readonly Int32 RowCount = 6;

		private readonly Int32[] _nextMoveIndexes;

		public FourInARowState()
		{
			_fields = new FourInARowFieldState[ColumnCount][];

			_nextMoveIndexes = new Int32[ColumnCount];

			for (Int32 q = 0; q < _fields.Length; q++)
			{
				_fields[q] = new FourInARowFieldState[RowCount];
			}
		}

		internal FourInARowState(FourInARowState source)
		{
			if (null == source)
			{
				throw new ArgumentNullException("source");
			}

			_nextMoveIndexes = (Int32[])source._nextMoveIndexes.Clone();

			_fields = new FourInARowFieldState[7][];

			for (Int32 q = 0; q < _fields.Length; q++)
			{
				_fields[q] = (FourInARowFieldState[])source._fields[q].Clone();
			}
		}

		private FourInARowFieldState[][] _fields;

		internal FourInARowFieldState[][] Fields { get; private set; }
	}
}
