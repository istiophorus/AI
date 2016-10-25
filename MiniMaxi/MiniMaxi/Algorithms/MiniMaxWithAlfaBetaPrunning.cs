using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMaxi.Interfaces;

namespace MiniMaxi.Algorithms
{
	/// <summary>
	/// https://pl.wikipedia.org/wiki/Algorytm_alfa-beta
	/// funkcja minimax(węzeł, głębokość)
	///		zwróć alfabeta(węzeł, głębokość, -∞, +∞)
	///		
	///	funkcja alfabeta(węzeł, głębokość, α, β)
	///		jeżeli węzeł jest końcowy lub głębokość = 0
	///			zwróć wartość heurystyczną węzła
	///		jeżeli przeciwnik ma zagrać w węźle
	///			dla każdego potomka węzła
	///				β := min(β, alfabeta(potomek, głębokość-1, α, β))
	///				jeżeli α≥β
	///					przerwij przeszukiwanie  {odcinamy gałąź Alfa}
	///			zwróć β
	///		w przeciwnym przypadku {my mamy zagrać w węźle}
	///			dla każdego potomka węzła
	///				α := max(α, alfabeta(potomek, głębokość-1, α, β))
	///				jeżeli α≥β
	///					przerwij przeszukiwanie  {odcinamy gałąź Beta}
	///			zwróć α
	/// </summary>
	public sealed class MiniMaxWithAlfaBetaPrunning : IGameAlgorithm
	{
		private readonly Int32 _depth;

		private readonly IGameLogic _gameLogic;

		private readonly IGameFactory _gameFactory;

		private readonly IGameStateEvaluator _stateEvaluator;

		public MiniMaxWithAlfaBetaPrunning(Int32 depth, IGameFactory gameFactory)
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

		private Int32 FindMoveScore(IGameState gameState, GamePlayer currentPlayer, Int32 depth, Int32 alfa, Int32 beta)
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
				for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					Int32 moveScore = FindMoveScore(newState, OtherPlayer(currentPlayer), depth - 1, alfa, beta);

					alfa = Math.Max(alfa, moveScore);

					if (alfa >= beta)
					{
						break;
					}
				}

				return alfa;
			}
			else if (currentPlayer == GamePlayer.PlayerMin)
			{
				for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					Int32 moveScore = FindMoveScore(newState, OtherPlayer(currentPlayer), depth - 1, alfa, beta);

					beta = Math.Min(beta, moveScore);

					if (alfa >= beta)
					{
						break;
					}
				}

				return beta;
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

			Int32 alfa = Int32.MinValue;

			Int32 beta = Int32.MaxValue;

			IGameMove selectedMove = null;

			if (currentPlayer == GamePlayer.PlayerMax)
			{
				for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					Int32 moveScore = FindMoveScore(newState, OtherPlayer(currentPlayer), _depth - 1, alfa, beta);

					if (moveScore > alfa || selectedMove == null)
					{
						alfa = moveScore;
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
				for (Int32 q = 0; q < moves.Length; q++)
				{
					IGameMove nextMove = moves[q];

					IGameState newState = _gameLogic.MakeMove(nextMove, gameState);

					Int32 moveScore = FindMoveScore(newState, OtherPlayer(currentPlayer), _depth - 1, alfa, beta);

					if (moveScore < beta || selectedMove == null)
					{
						beta = moveScore;
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
			return FindBestMoveImpl(gameState, player);
		}

	}
}
