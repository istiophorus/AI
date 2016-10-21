﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi
{
	/// <summary>
	/// minimax(s)
	/// if terminal(s) utility(s)
	/// if player(s) == MAX max minimax(result(s, a))
	/// if player(s) == MIN min minimax(result(s, a))
	/// </summary>
	public sealed class MiniMaxAlgorithmImproved
	{
		private readonly Int32 _depth;

		private readonly IGameLogic _gameLogic;

		private readonly IGameFactory _gameFactory;

		private readonly IGameStateEvaluator _stateEvaluator;

		public MiniMaxAlgorithmImproved(Int32 depth, IGameFactory gameFactory)
		{
			if (depth < 1)
			{
				throw new ArgumentOutOfRangeException("depth");
			}

			if (null == gameFactory)
			{
				throw new ArgumentNullException("gameFactory");
			}

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

		private sealed class BestMoveInfo
		{
			internal IGameMove Move { get; set; }

			internal Int32 MoveRate { get; set; }
		}

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

			//Parallel.For(0, moves.Length, q =>

			for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					rates[q] = FindMoveScore(newState, OtherPlayer(currentPlayer), depth - 1);
				}
			//);

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

			//Parallel.For(0, moves.Length, q =>

			for (Int32 q = 0; q < moves.Length; q++)
			{
				IGameMove nextMove = moves[q];

				IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

				rates[q] = FindMoveScore(newState, OtherPlayer(currentPlayer), _depth - 1);
			}
			//);

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


