using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi
{
	public static class Extensions
	{
		public static Tuple<Int32, T> FindFirstMax<T>(this T[] input) where T : IComparable<T>
		{
			if (null == input)
			{
				throw new ArgumentNullException("input");
			}

			Int32 index = -1;

			T maxValue = default(T);

			for (Int32 q = 1, mq = input.Length; q < mq; q++)
			{
				T item = input[q];

				if (index < 0 || item.CompareTo(maxValue) > 0)
				{
					index = q;
					maxValue = item;
				}
			}

			return new Tuple<Int32, T>(index, maxValue);
		}

		public static Tuple<Int32, T> FindFirstMin<T>(this T[] input) where T : IComparable<T>
		{
			if (null == input)
			{
				throw new ArgumentNullException("input");
			}

			Int32 index = -1;

			T minValue = default(T);

			for (Int32 q = 1, mq = input.Length; q < mq; q++)
			{
				T item = input[q];

				if (index < 0 || item.CompareTo(minValue) < 0)
				{
					index = q;
					minValue = item;
				}
			}

			return new Tuple<Int32, T>(index, minValue);
		}
	}
}
