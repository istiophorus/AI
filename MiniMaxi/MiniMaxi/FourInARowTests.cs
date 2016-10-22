using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniMaxi.Algorithms;
using MiniMaxi.FourInARow;
using MiniMaxi.Interfaces;

namespace MiniMaxi
{
	[TestClass]
	public sealed class FourInARowTests
	{
		internal static FourInARowState PrepareState(String[] input)
		{
			FourInARowState result = new FourInARowState();

			for (Int32 q = 0; q < FourInARowState.RowCount; q++)
			{
				String line = input[q];

				for (Int32 w = 0; w < FourInARowState.ColumnCount; w++)
				{
					Char ch = line[w];

					if (ch == 'x')
					{
						result.Set(w, q, FourInARowFieldState.Cross);
					}
					else if (ch == 'o')
					{
						result.Set(w, q, FourInARowFieldState.Circle);
					}
				}
			}

			return result;
		}

		private static FourInARowMove Common(String[] input)
		{
			FourInARowState state = PrepareState(input);

			FourInARowFactory factory = new FourInARowFactory();

			IGameLogic logic = factory.CreateLogic();

			MiniMaxAlgorithmImproved alg = new MiniMaxAlgorithmImproved(3, factory);

			Int32 res = factory.CreateStateEvaluator().Evaluate(state, GamePlayer.PlayerMax);

			return (FourInARowMove)alg.FindBestMove(state, GamePlayer.PlayerMax);
		}

		[TestMethod]
		public void TestWin01()
		{
			String[] input = new[] 
				{
					"oo.o...",
					"x......",
					"x......",
					".......",
					".......",
					"......."
				};

			FourInARowMove move = Common(input);

			Assert.AreEqual(2, move.Column);
		}

		[TestMethod]
		public void TestWin02()
		{
			String[] input = new[] 
				{
					"oooxooo",
					"xxxoxxx",
					"oooxooo",
					"xxxoxxx",
					"xoxoxox",
					"oo.o...",
				};

			FourInARowMove move = Common(input);

			Assert.AreEqual(2, move.Column);
		}

		[TestMethod]
		public void TestWin03()
		{
			String[] input = new[] 
				{
					"..xx...",
					"..oo...",
					".......",
					".......",
					".......",
					"......."
				};

			FourInARowMove move = Common(input);

			Assert.IsTrue(move.Column == 1 || move.Column == 4);			
		}

		[TestMethod]
		public void TestWin04()
		{
			String[] input = new[] 
				{
					"..xx...",
					"..o....",
					"..o....",
					"..o....",
					".......",
					"......."
				};

			FourInARowMove move = Common(input);

			Assert.AreEqual(2, move.Column);
		}

		[TestMethod]
		public void TestWin05()
		{
			String[] input = new[] 
				{
					"..xoxo.",
					"..oxoo.",
					".....o.",
					".....x.",
					".......",
					"......."
				};

			FourInARowMove move = Common(input);

			Assert.AreEqual(4, move.Column);
		}
	}
}
