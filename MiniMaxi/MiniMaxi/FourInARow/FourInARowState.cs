using System;
using System.Text;
using MiniMaxi.Interfaces;

namespace MiniMaxi.FourInARow
{
	public sealed class FourInARowState : IGameState
	{
		internal static readonly Int32 ColumnCount = 7;

		internal static readonly Int32 MiddleColumn = ColumnCount / 2;

		internal static readonly Int32 RowCount = 6;

		private readonly Int32[] _nextMoveIndexes;

		internal Int32[] Indexes
		{
			get
			{
				return _nextMoveIndexes;
			}
		}

        private readonly Char[] _stateDesc;

		public FourInARowState()
		{
			_fields = new FourInARowFieldState[ColumnCount][];

			_nextMoveIndexes = new Int32[ColumnCount];

			for (Int32 q = 0; q < _fields.Length; q++)
			{
				_fields[q] = new FourInARowFieldState[RowCount];
			}

            _stateDesc = new Char[RowCount * ColumnCount];

            for (Int32 q = 0; q < _stateDesc.Length; q++)
            {
                _stateDesc[q] = GetStateDesc(FourInARowFieldState.Empty);
            }
        }

        private static Char GetStateDesc(FourInARowFieldState state)
        {
            switch (state)
            {
                case FourInARowFieldState.Empty: return 'E';
                case FourInARowFieldState.Circle: return 'O';
                case FourInARowFieldState.Cross: return 'X';
                default:
                    throw new NotSupportedException(state.ToString());
            }
        }

        private Int32 _lastEmptyRow = 100;

        internal FourInARowState(FourInARowState source)
		{
			if (null == source)
			{
				throw new ArgumentNullException("source");
			}

			_nextMoveIndexes = (Int32[])source._nextMoveIndexes.Clone();

			_fields = new FourInARowFieldState[ColumnCount][];

            for (Int32 q = 0; q < _fields.Length; q++)
			{
				_fields[q] = (FourInARowFieldState[])source._fields[q].Clone();
			}

            _stateDesc = (Char[])source._stateDesc.Clone();

            _lastEmptyRow = source._lastEmptyRow;
        }

		private FourInARowFieldState[][] _fields;

        internal Int32 LastEmptyRow
        {
            get
            {
                return _lastEmptyRow;
            }
        }

		private Int32[] _crossesInRow = new Int32[RowCount];

		internal Int32[] CrossesInRow
		{
			get
			{
				return _crossesInRow;
			}
		}

		private Int32[] _circlesInRow = new Int32[RowCount];

		internal Int32[] CirclesInRow
		{
			get
			{
				return _circlesInRow;
			}
		}

		private Int32[] _crossesInColumn = new Int32[ColumnCount];

		internal Int32[] CrossesInColumn
		{
			get
			{
				return _crossesInColumn;
			}
		}

		private Int32[] _circlesInColumn = new Int32[ColumnCount];

		internal Int32[] CirclesInColumn
		{
			get
			{
				return _circlesInColumn;
			}
		}

        public String Key
        {
            get
            {
                return new String(_stateDesc);
            }
        }

        internal void Set(Int32 x, Int32 y, FourInARowFieldState state)
		{
			_fields[x][y] = state;

            _stateDesc[x + ColumnCount * y] = GetStateDesc(state);

			_nextMoveIndexes[x]++;

            if (y >= _lastEmptyRow)
            {
                _lastEmptyRow = y + 1;
            }

			if (state == FourInARowFieldState.Cross)
			{
				_crossesInRow[y]++;
				_crossesInColumn[x]++;
			}
			else if (state == FourInARowFieldState.Circle)
			{
				_circlesInRow[y]++;
				_circlesInColumn[x]++;
			}
		}

		internal FourInARowFieldState Get(Int32 x, Int32 y)
		{
			return _fields[x][y];
		}

		public override String ToString()
		{
			StringBuilder sb = new StringBuilder(1024);

			for (Int32 y = FourInARowState.RowCount - 1; y >= 0; y--)
			{
				for (Int32 x = 0, mx = FourInARowState.ColumnCount; x < mx; x++)
				{
					String text;

					switch (Get(x, y))
					{
						case FourInARowFieldState.Cross:
							text = "X";
							break;

						case FourInARowFieldState.Circle:
							text = "O";
							break;

						default:
							text = "_";
							break;
					}

					sb.Append(text);
				}

				sb.Append(Environment.NewLine);
			}

			return sb.ToString();
		}
	}
}


