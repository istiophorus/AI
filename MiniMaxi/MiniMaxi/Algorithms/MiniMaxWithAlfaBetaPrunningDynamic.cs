using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMaxi.Interfaces;

namespace MiniMaxi.Algorithms
{
	/// <summary>
	/// https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning
	////01 function alphabeta(node, depth, α, β, maximizingPlayer)
	////02      if depth = 0 or node is a terminal node
	////03          return the heuristic value of node
	////04      if maximizingPlayer
	////05          v := -∞
	////06          for each child of node
	////07              v := max(v, alphabeta(child, depth - 1, α, β, FALSE))
	////08              α := max(α, v)
	////09              if β ≤ α
	////10                  break (* β cut-off *)
	////11          return v
	////12      else
	////13          v := ∞
	////14          for each child of node
	////15              v := min(v, alphabeta(child, depth - 1, α, β, TRUE))
	////16              β := min(β, v)
	////17              if β ≤ α
	////18                  break (* α cut-off *)
	////19          return v
	/// </summary>
	public sealed class MiniMaxWithAlfaBetaPrunningDynamic : IGameAlgorithm
	{
		private readonly Int32 _depth;

		private readonly IGameLogic _gameLogic;

		private readonly IGameFactory _gameFactory;

		private readonly IGameStateEvaluator _stateEvaluator;

		public MiniMaxWithAlfaBetaPrunningDynamic(Int32 depth, IGameFactory gameFactory)
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

		private Int32 FindMoveScore(IGameState gameState, GamePlayer currentPlayer, Int32 depth, Int32 alfa, Int32 beta, Dictionary<String, Int32> ratesMap)
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

			if (currentPlayer == GamePlayer.PlayerMax)
			{
				Int32 maxMoveScore = Int32.MinValue;

				for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

                    Int32 moveScore;

                    if (!ratesMap.TryGetValue(newState.Key, out moveScore))
                    {
                        moveScore = FindMoveScore(newState, OtherPlayer(currentPlayer), depth - 1, alfa, beta, ratesMap);

                        ratesMap.Add(newState.Key, moveScore);
                    }
                    else
                    {

                    }

					maxMoveScore = Math.Max(maxMoveScore, moveScore);

					alfa = Math.Max(alfa, maxMoveScore);

					if (beta <= alfa)
					{
						break;
					}
				}

				return maxMoveScore;
			}
			else if (currentPlayer == GamePlayer.PlayerMin)
			{
				Int32 minMoveScore = Int32.MaxValue;

				for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

                    Int32 moveScore;

                    if (!ratesMap.TryGetValue(newState.Key, out moveScore))
                    {
                        moveScore = FindMoveScore(newState, OtherPlayer(currentPlayer), depth - 1, alfa, beta, ratesMap);

                        ratesMap.Add(newState.Key, moveScore);
                    }
                    else
                    {

                    }


                    minMoveScore = Math.Min(minMoveScore, moveScore);

					beta = Math.Min(beta, minMoveScore);

					if (beta <= alfa)
					{
						break;
					}
				}

				return minMoveScore;
			}
			else
			{
				throw new NotSupportedException(currentPlayer.ToString());
			}
		}

		private IGameMove FindBestMoveImpl(IGameState gameState, GamePlayer currentPlayer, Dictionary<String, Int32> ratesMap)
		{
			IGameMove[] moves = _gameLogic.GetPossibleMoves(gameState, currentPlayer);

			if (moves.Length <= 0)
			{
				return null;
			}

			Int32 alfa = Int32.MinValue;

			Int32 beta = Int32.MaxValue;

			IGameMove selectedMove = null;

			if (currentPlayer == GamePlayer.PlayerMax)
			{
				Int32 maxMoveScore = Int32.MinValue;

				for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					Int32 moveScore = FindMoveScore(newState, OtherPlayer(currentPlayer), _depth - 1, alfa, beta, ratesMap);

					if (moveScore > alfa || selectedMove == null)
					{
						maxMoveScore = Math.Max(maxMoveScore, moveScore);

						alfa = Math.Max(alfa, maxMoveScore);

						selectedMove = nextMove;
					}

					if (alfa >= beta)
					{
						break;
					}
				}

				return selectedMove;
			}
			else if (currentPlayer == GamePlayer.PlayerMin)
			{
				Int32 minMoveScore = Int32.MaxValue;

				for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					Int32 moveScore = FindMoveScore(newState, OtherPlayer(currentPlayer), _depth - 1, alfa, beta, ratesMap);

					if (moveScore < beta || selectedMove == null)
					{
						minMoveScore = Math.Min(minMoveScore, moveScore);

						beta = Math.Min(beta, minMoveScore);

						selectedMove = nextMove;
					}

					if (alfa >= beta)
					{
						break;
					}
				}

				return selectedMove;
			}
			else
			{
				throw new NotSupportedException(currentPlayer.ToString());
			}
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
            Dictionary<String, Int32> ratesMap = new Dictionary<String, Int32>();

			return FindBestMoveImpl(gameState, player, ratesMap);
		}
	}
}
