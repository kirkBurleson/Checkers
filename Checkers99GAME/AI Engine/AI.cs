using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Checkers99GAME
{
	public sealed class AI
	{
		static Byte mustJumpFromSquare;
		static List<Move> jumpChoices;
		static AI()
		{
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
			_evaluated = 0; // Statistics only

			for (var i = 0; i < potentialMoves.Count(); i++)
			{
				_history.Push((Byte[])_board.Clone());

				MakeMove(potentialMoves[i]);

				ChangeSides(player);

				var moves = GetPlayerLegalMoves(player);

				//potentialMoves[i].SetScore(negamax(moves, 6, player));
				
				//potentialMoves[i].SetScore(minimax(moves, 6, player));

				potentialMoves[i].SetScore(alphabeta(moves, 8, player, Int32.MinValue, Int32.MaxValue));

				ChangeSides(player);

				if (potentialMoves[i].Score > bestMove.Score)
					bestMove = potentialMoves[i];

				_board = (Byte[])_history.Pop();
			}


			if (bestMove != null)
				DoMustJumpLogic(bestMove);

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
				return EvaluateBoard();

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
				return EvaluateBoard();

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
			// detect winning position
			if (moves.Count == 0)
				return Int32.MinValue;

			if (plies == 0)				
				return EvaluateBoard();

			Int32 best = Int32.MinValue;
			Int32 current;
			Int32 localAlpha = alpha;
			Move bestMove = null;

			foreach (var m in moves)
			{
				_history.Push((Byte[])_board.Clone());

				MakeMove(m);

				ChangeSides(player);

				var nextMoves = GetPlayerLegalMoves(player);

				current = -(alphabeta(nextMoves, plies - 1, player, -beta, -localAlpha));

				ChangeSides(player);

				_board = _history.Pop();

				if (current > best)
				{
					best = current;
					bestMove = m;
				}

				if (best >= beta)
				{
					Debug.WriteLine("Pruning from " + m.StartSquare);
					break;
				}

				if (best > localAlpha)
					localAlpha = best;
			}

			Debug.WriteLine("Best: " + bestMove.StartSquare + " - " + bestMove.EndSquare + " S: " + best + " E: " + _evaluated);
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
			player.SetColor(GetOppositeColor(player));
		}

		private Int32 EvaluateBoard()
		{
			Int32 score = 0;

			score = GetPieceScore();

			score += GetHeuristicScore();

			_evaluated++;

			return score;
		}

		private Int32 GetPieceScore()
		{
			Int32 score = 0;

			foreach (Byte square in _board)
			{
				switch (square)
				{
					case 1://white checker
						score -= 10;
						break;

					case 2://red checker
						score += 10;
						break;

					case 3://white king
						score -= 30;
						break;

					case 4://red king
						score += 30;
						break;
				}
			}

			return score;
		}

		private Int32 GetHeuristicScore()
		{
			Int32 score = 0;

			score += GetControlSquareScore();

			score += GetKingsRowDefenseScore();

			return score;
		}

		private Int32 GetKingsRowDefenseScore()
		{
			Int32 score = 0;

			if ((Engine.Piece)_board[1] == Engine.Piece.RedChecker)
				score += 50;

			if ((Engine.Piece)_board[5] == Engine.Piece.RedChecker)
				score += 50;

			if ((Engine.Piece)_board[58] == Engine.Piece.WhiteChecker)
				score -= 50;

			if ((Engine.Piece)_board[62] == Engine.Piece.WhiteChecker)
				score -= 50;

			return score;
		}

		private Int32 GetControlSquareScore()
		{
			Int32 score = 0;

			Byte[] controlSquares = { 35, 37, 26, 28 };

			foreach (Byte square in controlSquares)
			{
				switch ((Engine.Piece)_board[square])
				{
					case Engine.Piece.RedChecker:
					case Engine.Piece.RedKing:
						score += 20;
						break;

					case Engine.Piece.WhiteChecker:
					case Engine.Piece.WhiteKing:
						score -= 20;
						break;
				}
			}

			return score;
		}

		private static Player.PlayerColor GetOppositeColor(Player currentPlayer)
		{
			return (currentPlayer.Color == Player.PlayerColor.RED) ? Player.PlayerColor.WHITE : Player.PlayerColor.RED;
		}			
	}
}
