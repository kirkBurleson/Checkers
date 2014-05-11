using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		private Stack<Byte[]> _undoBoards;
		private Engine _engine;
		private List<Move> _startingMoves;
		private Byte[] _kingSquares;
		private Stack<Byte[]> _history;

		public AI()
		{
			_undoBoards = new Stack<Byte[]>();
			_history = new Stack<Byte[]>();
			_engine = new Engine();
			_startingMoves = new List<Move>();
			_kingSquares = new Byte[] { 1, 3, 5, 7, 56, 58, 60, 62 };
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

		private Move DoAI(Player player)
		{
			return DoAI(player, null);
		}

		private Move DoAI(Player player, List<Move> movePool)
		{
			Move bestMove = null;

			List<Move> potentialMoves = (movePool == null) ? GetPlayerLegalMoves(player) : movePool;

			if (potentialMoves.Count > 0)
				bestMove = potentialMoves[0];

			for (var i = 0; i < potentialMoves.Count(); i++)
			{
				_history.Push((Byte[])_board.Clone());

				MakeMove(potentialMoves[i]);

				ChangeSides(player);
				var moves = GetPlayerLegalMoves(player);
				potentialMoves[i].SetScore(MinMax(moves, 3, player));

				if (potentialMoves[i].Score > bestMove.Score)
					bestMove = potentialMoves[i];

				_board = (Byte[])_history.Pop();
			}


			if (bestMove != null)
				DoMustJumpLogic(bestMove);

			return bestMove;
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

		private static void DrawBoard(Byte[] board)
		{
			Console.Clear();
			for (Byte index = 0; index < board.Length; index++)
			{
				if (index % 8 == 0) Console.WriteLine();

				if (board[index] == 0)
					Console.Write(" .");
				else
					Console.Write(" " + board[index].ToString());

			}
			Console.WriteLine();
		}

		private static void ChangeSides(Player player)
		{
			player.SetColor(GetOppositeColor(player));
		}

		private static Single SumChildScores(List<Move> children)
		{
			Single total = 0.0f;

			foreach (Move m in children)
			{
				total += m.Score;

				if (m.HasChildMoves)
					total += SumChildScores(m.Jumps);
			}

			return total;
		}

		private Single EvaluateBoard()
		{
			Single score = 0.0f;

			score = GetPieceScore();

			score += GetHeuristicScore();

			return score;
		}

		private Single GetPieceScore()
		{
			Single score = 0.0f;

			foreach (Byte square in _board)
			{
				switch (square)
				{
					case 1://white checker
						score -= 1.0f;
						break;

					case 2://red checker
						score += 1.0f;
						break;

					case 3://white king
						score -= 3.0f;
						break;

					case 4://red king
						score += 3.0f;
						break;
				}
			}

			return score;
		}

		private Single GetHeuristicScore()
		{
			Single score = 0.0f;

			score += GetControlSquareScore();

			score += GetKingsRowDefenseScore();

			return score;
		}

		private Single GetKingsRowDefenseScore()
		{
			Single score = 0.0f;

			Byte[] defenseSquares = { 1, 5, 58, 62 };

			foreach (Byte square in defenseSquares)
			{
				switch ((Engine.Piece)_board[square])
				{
					case Engine.Piece.RedChecker:
					case Engine.Piece.RedKing:
						score += .01f;
						break;
					case Engine.Piece.WhiteChecker:
					case Engine.Piece.WhiteKing:
						score -= .01f;
						break;
				}
			}
			return score;
		}

		private Single GetControlSquareScore()
		{
			Single score = 0.0f;

			Byte[] controlSquares = { 35, 37, 26, 28 };

			foreach (Byte square in controlSquares)
			{
				switch ((Engine.Piece)_board[square])
				{
					case Engine.Piece.RedChecker:
					case Engine.Piece.RedKing:
						score += .01f;
						break;

					case Engine.Piece.WhiteChecker:
					case Engine.Piece.WhiteKing:
						score -= .01f;
						break;
				}
			}

			return score;
		}

		private static Player.PlayerColor GetOppositeColor(Player currentPlayer)
		{
			return (currentPlayer.Color == Player.PlayerColor.RED) ? Player.PlayerColor.WHITE : Player.PlayerColor.RED;
		}

		private void RemoveJumpedPiece(Move move)
		{
			Byte js = _engine.GetJumpedSquare(move.StartSquare, move.EndSquare);
			_board[js] = 0;
		}

		private Single MinMax(List<Move> moves, Int32 plies, Player player)
		{
			if (plies == 0)
				return EvaluateBoard();
			
			for (var i = 0; i < moves.Count(); i++)
			{
				_history.Push((Byte[])_board.Clone());

				MakeMove(moves[i]);

				ChangeSides(player);

				var s = MinMax(GetPlayerLegalMoves(player), --plies, player);

				_board = _history.Pop();

				return s;
			}

			return EvaluateBoard();
		}

		private Move GetBestMove()
		{
			Move bestMove = null;
			Single high = Single.MinValue;
			Single total = 0f;

			foreach (Move move in _startingMoves)
			{
				total += move.Score;

				if (move.HasChildMoves)
					total += SumChildScores(move.Jumps);

				if (total > high)
				{
					bestMove = move;
					high = total;
				}

				total = 0.0f;
			}

			return bestMove;
		}

		private List<Move> GetPlayerLegalMoves(Player player)
		{
			String playerColor = (player.Color == Player.PlayerColor.NONE ? "" : player.Color.ToString().ToLower());

			List<Move> moves = _engine.FindMoves(_board, playerColor);

			return moves;
		}

		private void ScoreMoves(List<Move> potentialMoves)
		{
			foreach (Move move in potentialMoves)
			{
				_undoBoards.Push((Byte[])_board.Clone());

				MakeMove(move);

				SetMoveScore(move);

				UndoMove();
			}
		}

		private void SetMoveScore(Move move)
		{
			move.SetScore(EvaluateBoard());
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

		private void UndoMove()
		{
			_undoBoards.Pop().CopyTo(_board, 0);
		}
	}
}
