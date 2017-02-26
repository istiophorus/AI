using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMaxi.Interfaces;

namespace MiniMaxi.Algorithms
{
	/// <summary>
	/// minimax(s)
	/// if terminal(s) utility(s)
	/// if player(s) == MAX max minimax(result(s, a))
	/// if player(s) == MIN min minimax(result(s, a))
	/// </summary>
	public sealed class MiniMaxAlgorithmImproved : IGameAlgorithm
	{
		private readonly Int32 _depth;

		private readonly IGameLogic _gameLogic;

		private readonly IGameFactory _gameFactory;

		private readonly IGameStateEvaluator _stateEvaluator;

		private readonly Boolean _useParallel;

		public MiniMaxAlgorithmImproved(Int32 depth, IGameFactory gameFactory, Boolean useParallel)
		{
			if (depth < 1)
			{
				throw new ArgumentOutOfRangeException("depth");
			}

			if (null == gameFactory)
			{
				throw new ArgumentNullException("gameFactory");
			}

			_useParallel = useParallel;

			_depth = depth;

			_gameFactory = gameFactory;

			_gameLogic = gameFactory.CreateLogic();

			_stateEvaluator = gameFactory.CreateStateEvaluator();
		}

		private static GamePlayer OtherPlayer(GamePlayer player)
		{
			switch (player)
			{
				case GamePlayer.PlayerMax:
					return GamePlayer.PlayerMin;

				case GamePlayer.PlayerMin:
					return GamePlayer.PlayerMax;

				default:
					throw new NotSupportedException(player.ToString());
			}
		}

        private static readonly Double FutureDiscount = 0.9;

		private Int32 FindMoveScore(IGameState gameState, GamePlayer currentPlayer, Int32 depth)
		{
			if (depth <= 0 || _gameLogic.IsFinished(gameState))
			{
				return _stateEvaluator.Evaluate(gameState, currentPlayer);
			}

			IGameMove[] moves = _gameLogic.GetPossibleMoves(gameState, currentPlayer);

			if (moves.Length <= 0)
			{
				//// there are no more possible moves to analyse, so return current state evaluation

				return _stateEvaluator.Evaluate(gameState, currentPlayer);
			}

			Int32[] rates = new Int32[moves.Length];

			Action<Int32> checkSingleMove = moveIndex =>
				{
					IGameMove nextMove = moves[moveIndex];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					rates[moveIndex] = (Int32)(FindMoveScore(newState, OtherPlayer(currentPlayer), depth - 1) * FutureDiscount);
				};

			if (_useParallel)
			{
				Parallel.For(0, moves.Length, checkSingleMove);
			}
			else
			{
				for (Int32 q = 0; q < moves.Length; q++)
				{
					checkSingleMove(q);
				}
			}

			if (currentPlayer == GamePlayer.PlayerMax)
			{
				return rates.FindFirstMax().Item2;
			}
			else if (currentPlayer == GamePlayer.PlayerMin)
			{
				return rates.FindFirstMin().Item2;
			}
			else
			{
				throw new NotSupportedException(currentPlayer.ToString());
			}
		}

		private IGameMove FindBestMoveImpl(IGameState gameState, GamePlayer currentPlayer)
		{
			IGameMove[] moves = _gameLogic.GetPossibleMoves(gameState, currentPlayer);

			if (moves.Length <= 0)
			{
				return null;
			}

			Int32[] rates = new Int32[moves.Length];

			Action<Int32> checkSingleMove = moveIndex =>
				{
					IGameMove nextMove = moves[moveIndex];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					rates[moveIndex] = (Int32)(FindMoveScore(newState, OtherPlayer(currentPlayer), _depth - 1) * FutureDiscount);
				};

			if (_useParallel)
			{
				Parallel.For(0, moves.Length, checkSingleMove);
			}
			else
			{
				for (Int32 q = 0; q < moves.Length; q++)
				{
					checkSingleMove(q);
				}
			}

			return ExtractResult(currentPlayer, moves, rates);
		}

		private static IGameMove ExtractResult(GamePlayer currentPlayer, IGameMove[] moves, Int32[] rates)
		{
			Int32 index = -1;
			Int32 rate = 0;

			List<Int32> equalRate = new List<Int32>(rates.Length);

			if (currentPlayer == GamePlayer.PlayerMax)
			{
				for (Int32 q = 0; q < rates.Length; q++)
				{
					Int32 currentRate = rates[q];

					if (index == -1 || currentRate > rate)
					{
						index = q;
						rate = currentRate;

						equalRate.Clear();

						equalRate.Add(q);
					}
					else
					{
						if (currentRate == rate)
						{
							equalRate.Add(q);
						}
					}
				}
			}
			else if (currentPlayer == GamePlayer.PlayerMin)
			{
				for (Int32 q = 0; q < rates.Length; q++)
				{
					Int32 currentRate = rates[q];

					if (index == -1 || currentRate < rate)
					{
						index = q;
						rate = currentRate;

						equalRate.Clear();

						equalRate.Add(q);
					}
					else
					{
						if (currentRate == rate)
						{
							equalRate.Add(q);
						}
					}
				}
			}
			else
			{
				throw new NotSupportedException(currentPlayer.ToString());
			}

			if (equalRate.Count > 1)
			{
				index = equalRate[Environment.TickCount % equalRate.Count];
			}

			return moves[index];
		}

		private static Int32 AdjustStateRate(Int32 depth, Int32 stateRate)
		{
			return (Int32)(Math.Sign(stateRate) * (Math.Abs(stateRate) + depth));
		}

		public IGameMove FindBestMove(IGameState gameState, GamePlayer player)
		{
			return FindBestMoveImpl(gameState, player);
		}
	}
}


