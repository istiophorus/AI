using System;
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

        private static FourInARowMove Common(String[] input, IGameAlgorithm algorithm, IGameFactory gameFactory)
        {
            FourInARowState state = PrepareState(input);

            IGameFactory factory = gameFactory;

            IGameLogic logic = factory.CreateLogic();

            IGameAlgorithm alg = algorithm;

            Int32 res = factory.CreateStateEvaluator().Evaluate(state, GamePlayer.PlayerMax);

            return (FourInARowMove)alg.FindBestMove(state, GamePlayer.PlayerMax);
        }

        private static FourInARowMove Common(String[] input)
		{
            return Common(
                input, 
                new MiniMaxAlgorithmImproved(3, new FourInARowFactory(), true), 
                new FourInARowFactory());
		}

        [TestMethod]
        public void TestMoveEvaluation()
        {
            String[] inputA = new[]
                {
                    ".x.o...",
                    ".x.....",
                    ".......",
                    ".......",
                    ".......",
                    "......."
                };

            String[] inputB = new[]
                {
                    "...o.x.",
                    ".....x.",
                    ".......",
                    ".......",
                    ".......",
                    "......."
                };

            FourInARowState stateA = PrepareState(inputA);

            FourInARowState stateB = PrepareState(inputB);

            FourInARowFactory factory = new FourInARowFactory();

            IGameStateEvaluator evaluator = factory.CreateStateEvaluator();

            Int32 rateA = evaluator.Evaluate(stateA, GamePlayer.PlayerMax);

            Int32 rateB = evaluator.Evaluate(stateB, GamePlayer.PlayerMax);

            Assert.AreEqual(rateA, rateB);

            rateA = evaluator.Evaluate(stateA, GamePlayer.PlayerMin);

            rateB = evaluator.Evaluate(stateB, GamePlayer.PlayerMin);

            Assert.AreEqual(rateA, rateB);
        }

        private void CompareAlgorithms(String[] input, Int32 index, Int32 depth)
        {
            IGameFactory gameFactory = new FourInARowFactory();

            FourInARowMove moveA = Common(
                input,
                new MiniMaxAlgorithmImproved(depth, gameFactory, false),
                gameFactory);

            FourInARowMove moveB = Common(
                input,
                new MiniMaxWithAlfaBetaPrunningDynamic(depth, gameFactory),
                gameFactory);

            FourInARowMove moveC = Common(
                input,
                new MiniMaxWithAlfaBetaPrunningB(depth, gameFactory),
                gameFactory);

            if (moveA.Column != moveC.Column)
            {
            }

            Assert.AreEqual(moveA.Column, moveB.Column, "Scenario failed [A] " + index + " " + depth);

            Assert.AreEqual(moveA.Column, moveC.Column, "Scenario failed [B] " + index + " " + depth);
        }

        private static readonly String[][] ScenariosToCompare = new String[][]
            {
                new[]
                {
                    "oo.o...",
                    "x......",
                    "x......",
                    ".......",
                    ".......",
                    "......."
                },

                new[]
                {
                    "..xxo..",
                    "..oo...",
                    "..xo...",
                    "...x...",
                    ".......",
                    "......."
                },

                new[]
                {
                    ".oo.oxx",
                    ".....x.",
                    ".......",
                    ".......",
                    ".......",
                    "......."
                },

                new[]
                {
                    ".oox.xx",
                    ".......",
                    ".......",
                    ".......",
                    ".......",
                    "......."
                },

                new[]
                {
                    "...x.x.",
                    "...o...",
                    ".......",
                    ".......",
                    ".......",
                    "......."
                },

                new[]
                {
                    "x.xx...",
                    "..oo...",
                    "...o...",
                    ".......",
                    ".......",
                    "......."
                },

                new[]
                {
                    ".x.x...",
                    "...o...",
                    "...o...",
                    ".......",
                    ".......",
                    "......."
                }
            };

        [TestMethod]
        public void TestAlgoResults()
        {
            for (Int32 d = 3; d < 6; d++)
            {
                for (Int32 q = 0; q < ScenariosToCompare.Length; q++)
                {
                    String[] scenario = ScenariosToCompare[q];

                    CompareAlgorithms(scenario, q, d);
                }
            }
        }

        [TestMethod]
        public void TestWin06()
        {
            String[] input = new[]
                {
                    "x.xx...",
                    "..oo...",
                    "...o...",
                    ".......",
                    ".......",
                    "......."
                };

            IGameFactory gameFactory = new FourInARowFactory();

            FourInARowMove move = Common(
                input,
                new MiniMaxWithAlfaBetaPrunningDynamic(3, gameFactory),
                gameFactory);

            Assert.AreEqual(1, move.Column);
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
        public void TestAnotherWin()
        {
            String[] input = new[]
                {
                    "oooxooo",
                    "xxxoxxx",
                    "oooxooo",
                    "xxxoxxx",
                    "xoxoxox",
                    "oooooo.",
                };

            FourInARowState state = PrepareState(input);

            FourInARowFactory factory = new FourInARowFactory();

            IGameLogic logic = factory.CreateLogic();

            Boolean result = logic.IsFinished(state);

            Assert.IsTrue(result);
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
