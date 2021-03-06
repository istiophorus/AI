﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMaxi.Interfaces
{
	public interface IGameFactory
	{
		IGameMoveEvaluator CreateMoveEvaluator();

		IGameLogic CreateLogic();

		IGameStateEvaluator CreateStateEvaluator();
	}
}
