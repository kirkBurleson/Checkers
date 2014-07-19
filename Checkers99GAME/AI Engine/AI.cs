using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Checkers99GAME
{
	public sealed class AI
	{
		public static Int32 score;
		static Byte mustJumpFromSquare;
		static List<Move> jumpChoices;
		static AI()
		{
			score = 0;
			mustJumpFromSquare = 0;
			jumpChoices = new List<Move>();
		}

		private Byte[] _board;
		private Engine _engine;
		private List<Move> _startingMoves;
		private Byte[] _kingSquares;
		private Stack<Byte[]> _history;
		private Int32 _evaluated;

		public AI()
		{
			_history = new Stack<Byte[]>();
			_engine = new Engine();
			_startingMoves = new List<Move>();
			_kingSquares = new Byte[] { 1, 3, 5, 7, 56, 58, 60, 62 };
			_evaluated = 0;
		}

		public Move GetMove(Byte[] board, Player player)
		{
			_board = board;

			var savedPlayerColor = player.Color;
		
			var move = (mustJumpFromSquare == 0) ? DoAI(player) : GetJumpFromChoices(player);

			player.SetColor(savedPlayerColor);

			return move;
		}

		private Move GetJumpFromChoices(Player player)
		{
			Move move = null;

			if (jumpChoices.Count() == 1)
			{
				move = jumpChoices[0];

				DoMustJumpLogic(move);
			}
			else if (jumpChoices.Count() > 1)
			{
				move = DoAI(player, jumpChoices);
			}

			return move;
		}

		private Boolean DoMustJumpLogic(Move move)
		{
			if (move.HasChildMoves && move.WillKing == false)
			{
				mustJumpFromSquare = move.EndSquare;
				jumpChoices = move.Jumps;
				return true;
			}
			else
			{
				mustJumpFromSquare = 0;
				jumpChoices.Clear();
				return false;
			}
		}	

		private Move DoAI(Player player)
		{
			return DoAI(player, null);
		}

		private Move DoAI(Player player, List<Move> movePool)
		{
			List<Move> potentialMoves = (movePool == null) ? GetPlayerLegalMoves(player) : movePool;
			
			// no moves, game's over
			if (potentialMoves.Count == 0)
				return null;

			// one move, must take it
			if (potentialMoves.Count == 1)
			{
				DoMustJumpLogic(potentialMoves[0]);
				return potentialMoves[0];
			}

			Move bestMove = potentialMoves[0];
			//_evaluated = 0; // Statistics only

			for (var i = 0; i < potentialMoves.Count(); i++)
			{
				_history.Push((Byte[])_board.Clone());

				MakeMove(potentialMoves[i]);

				player.SetColor((player.Color == Player.PlayerColor.RED) ? Player.PlayerColor.WHITE : Player.PlayerColor.RED);

				var moves = GetPlayerLegalMoves(player);

				//potentialMoves[i].SetScore(negamax(moves, 6, player));
				
				//potentialMoves[i].SetScore(minimax(moves, 6, player));

				potentialMoves[i].SetScore(alphabeta(moves, 6, player, Int32.MinValue, Int32.MaxValue));

				player.SetColor((player.Color == Player.PlayerColor.RED) ? Player.PlayerColor.WHITE : Player.PlayerColor.RED);

				if (potentialMoves[i].Score > bestMove.Score)
					bestMove = potentialMoves[i];

				_board = (Byte[])_history.Pop();
			}


			if (bestMove != null)
				DoMustJumpLogic(bestMove);

			score = bestMove.Score;

			return bestMove;
		}

		private List<Move> GetPlayerLegalMoves(Player player)
		{
			String playerColor = player.Color.ToString().ToLower();

			List<Move> moves = _engine.FindMoves(_board, playerColor);

			return moves;
		}

		private Int32 minimax(List<Move> moves, Int32 plies, Player player)
		{
			if (plies == 0 || moves.Count == 0)
				return EvaluateBoard(null);

			Int32 best = (player.Color == Player.PlayerColor.RED) ? Int32.MinValue : Int32.MaxValue;
			Int32 current;

			foreach (var m in moves)
			{
				_history.Push((Byte[])_board.Clone());

				MakeMove(m);

				ChangeSides(player);

				var nextMoves = GetPlayerLegalMoves(player);

				current = minimax(nextMoves, plies - 1, player);

				ChangeSides(player);

				if (player.Color == Player.PlayerColor.RED)
				{
					if (current > best)
						best = current;
				}
				else
				{
					if (current < best)
						best = current;
				}

				_board = _history.Pop();
			}

			return best;
		}

		private Int32 negamax(List<Move> moves, Int32 plies, Player player)
		{
			if (plies == 0 || moves.Count == 0)
				return EvaluateBoard(null);

			Int32 best = Int32.MinValue;
			Int32 current;

			foreach (var m in moves)
			{
				_history.Push((Byte[])_board.Clone());

				MakeMove(m);

				ChangeSides(player);

				var nextMoves = GetPlayerLegalMoves(player);

				current = -(negamax(nextMoves, plies - 1, player));

				ChangeSides(player);

				if (current > best)
					best = current;

				_board = _history.Pop();
			}

			return best;
		}

		private Int32 alphabeta(List<Move> moves, Int32 plies, Player player, Int32 alpha, Int32 beta)
		{
			if (moves.Count == 0)
			{
				if (player.Color == Player.PlayerColor.WHITE)
					return Int32.MaxValue;
				else
					return Int32.MinValue;
			}

			if (plies == 0)				
				return EvaluateBoard(null);

			Int32 best = Int32.MinValue;
			Int32 current;
			Int32 localAlpha = alpha;

			foreach (var m in moves)
			{
				_history.Push((Byte[])_board.Clone());

				MakeMove(m);

				player.SetColor((player.Color == Player.PlayerColor.RED) ? Player.PlayerColor.WHITE : Player.PlayerColor.RED);

				var nextMoves = GetPlayerLegalMoves(player);

				current = -(alphabeta(nextMoves, plies - 1, player, -beta, -localAlpha));

				player.SetColor((player.Color == Player.PlayerColor.RED) ? Player.PlayerColor.WHITE : Player.PlayerColor.RED);

				_board = _history.Pop();

				if (current > best)
					best = current;

				if (best >= beta)
					break;

				if (best > localAlpha)
					localAlpha = best;
			}

			return best;
		}

		private void MakeMove(Move move)
		{
			_board[move.EndSquare] = _board[move.StartSquare];
			_board[move.StartSquare] = 0;

			// King the checker
			if ((_kingSquares.Contains(move.EndSquare)) && (_board[move.EndSquare] < 3))
				_board[move.EndSquare] += 2;

			if (move.IsJump)
			{
				RemoveJumpedPiece(move);
				foreach (Move m in move.Jumps)
					MakeMove(m);
			}
		}

		private void RemoveJumpedPiece(Move move)
		{
			Byte js = _engine.GetJumpedSquare(move.StartSquare, move.EndSquare);
			_board[js] = 0;
		}

		private static void ChangeSides(Player player)
		{
			player.SetColor((player.Color == Player.PlayerColor.RED) ? Player.PlayerColor.WHITE : Player.PlayerColor.RED);
		}

		public Int32 EvaluateBoard(Byte[] board)
		{
			Byte[] tmpBoard = (board == null) ? _board : board;
			Int32 score = 0;

			// score pieces
			for (var i = 0; i < 64; i++)
			{
				if (tmpBoard[i] == 0) continue;

				if (tmpBoard[i] == 1) score -= 100;
				else if (tmpBoard[i] == 2) score += 100;
				else if (tmpBoard[i] == 3) score -= 300;
				else if (tmpBoard[i] == 4) score += 300;
			}

			// kings row defense
			if (tmpBoard[1] == 2) score += 20;
			if (tmpBoard[3] == 2) score += 20;
			if (tmpBoard[5] == 2) score += 20;
			if (tmpBoard[7] == 2) score += 20;

			if (tmpBoard[56] == 1) score -= 20;
			if (tmpBoard[58] == 1) score -= 20;
			if (tmpBoard[60] == 1) score -= 20;
			if (tmpBoard[62] == 1) score -= 20;

			// center control
			if (tmpBoard[26] == 2 || tmpBoard[26] == 4) score += 10;
			if (tmpBoard[28] == 2 || tmpBoard[28] == 4) score += 10;
			if (tmpBoard[35] == 2 || tmpBoard[35] == 4) score += 10;
			if (tmpBoard[37] == 2 || tmpBoard[37] == 4) score += 10;

			if (tmpBoard[26] == 1 || tmpBoard[26] == 3) score -= 10;
			if (tmpBoard[28] == 1 || tmpBoard[28] == 3) score -= 10;
			if (tmpBoard[35] == 1 || tmpBoard[35] == 3) score -= 10;
			if (tmpBoard[37] == 1 || tmpBoard[37] == 3) score -= 10;

			return score;
		}
	}
}
