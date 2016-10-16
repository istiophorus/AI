using System;
using System.Collections.Generic;
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
	public sealed class MiniMaxAlgorithm
	{
		private readonly Int32 _depth;

		private readonly IGameLogic _gameLogic;

		private readonly IGameFactory _gameFactory;

		private readonly IGameMoveEvaluator _moveEvaluator;

		private readonly IGameStateEvaluator _stateEvaluator;

		public MiniMaxAlgorithm(Int32 depth, IGameFactory gameFactory)
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

			_moveEvaluator = gameFactory.CreateMoveEvaluator();

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

		private BestMoveInfo FindBestMoveImpl(IGameState gameState, GamePlayer currentPlayer, Int32 depth)
		{
			IGameMove[] moves = _gameLogic.GetPossibleMoves(gameState, currentPlayer);

			if (moves.Length <= 0)
			{
				//// there are no more possible moves to analyse, so return current state evaluation

				return new BestMoveInfo
					{
						MoveRate = _stateEvaluator.Evaluate(gameState)
					};
			}

			Int32[] rates = new Int32[moves.Length];

			Parallel.For(0, moves.Length, q =>

			//for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					Int32 stateRate = _stateEvaluator.Evaluate(newState);

					if (stateRate != 0 /* */)
					{
						rates[q] = AdjustStateRate(depth, stateRate);
					}
					else
					{
						if (depth > 0)
						{
							BestMoveInfo bestMoveInfo = FindBestMoveImpl(newState, OtherPlayer(currentPlayer), depth - 1);

							bestMoveInfo.Move = nextMove;

							rates[q] = bestMoveInfo.MoveRate;
						}
						else
						{
							stateRate = _moveEvaluator.Evaluate(gameState, nextMove, newState);

							rates[q] = AdjustStateRate(depth, stateRate);
						}
					}
				});

			Int32 index = -1;
			Int32 rate = 0;

			if (currentPlayer == GamePlayer.PlayerMax)
			{
				for (Int32 q = 0; q < rates.Length; q++)
				{
					Int32 currentRate = rates[q];

					if (index == -1 || currentRate > rate)
					{
						index = q;
						rate = currentRate;
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
					}
				}
			}
			else
			{
				throw new NotSupportedException(currentPlayer.ToString());
			}

			return new BestMoveInfo
				{
					Move = moves[index],
					MoveRate = rates[index]
				};
		}

		private static Int32 AdjustStateRate(Int32 depth, Int32 stateRate)
		{
			return (Int32)(Math.Sign(stateRate) * (Math.Abs(stateRate) + depth));
		}

		public IGameMove FindBestMove(IGameState gameState, GamePlayer player)
		{
			BestMoveInfo bestMoveInfo = FindBestMoveImpl(gameState, player, _depth);

			return bestMoveInfo.Move;
		}
	}
}


