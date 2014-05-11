using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Checkers99GAME;

namespace Checkers99UI
{
	class TestEngine : IEngine
	{
		static Byte mustJumpFromSquare;
		Engine engine = new Engine();
		Byte[] board;
		Image whiteKing;
		Image redKing;

		static TestEngine()
		{
			mustJumpFromSquare = 0;
		}

		public TestEngine(Byte[] board)
		{
			this.board = board;

			try
			{
				whiteKing = Image.FromFile(@"images/whiteKing.png");
				redKing = Image.FromFile(@"images/redKing.png");
			}
			catch (System.Exception e)
			{
				MessageBox.Show("Failed to load King checker images", "Error");
			}
		}

		public Boolean IsMoveLegal(Int32 startSquare, Int32 endSquare, Player player)
		{
			List<Move> moves = engine.FindMoves(board, player.Color.ToString());

			for (var i = 0; i < moves.Count; i++)
			{
				var move = moves[i];

				// force same piece jumping
				if (player.Type == Player.PlayerType.HUMAN)
					if (mustJumpFromSquare > 0 && move.StartSquare != mustJumpFromSquare)
						continue;

				if (move.StartSquare == (Byte)startSquare && move.EndSquare == (Byte)endSquare)
					return true;
			}

			return false;
		}

		public Move GetMove(Int32 startSquare, Int32 endSquare, Player player)
		{
			List<Move> moves = engine.FindMoves(board, player.Color.ToString());

			for (var i = 0; i < moves.Count; i++)
			{
				var move = moves[i];

				if (move.StartSquare == (Byte)startSquare && move.EndSquare == (Byte)endSquare)
				{
					// same piece must jump rule
					if (player.Type == Player.PlayerType.HUMAN)
					{
						if (move.HasChildMoves)
							mustJumpFromSquare = move.EndSquare;
						else
							mustJumpFromSquare = 0;				
					}					

					return move;
				}
			}

			return null;
		}

		public void UpdateBoardState(Byte startSquare, Byte endSquare, Control.ControlCollection formControls)
		{
			var playerColorNum = board[startSquare];
			var jumpedSquare = engine.GetJumpedSquare(startSquare, endSquare);
			var panelName = "sq" + jumpedSquare;

			// move the piece
			board[endSquare] = board[startSquare];
			board[startSquare] = 0;

			// remove jumped piece
			if (jumpedSquare != 0)
			{
				board[jumpedSquare] = 0;

				if (formControls.ContainsKey(panelName))
					formControls[panelName].BackgroundImage = null;
			}

			// handle kinging
			if (endSquare < 8 && board[endSquare] == 1)
			{
				board[endSquare] = 3;
				formControls["sq" + endSquare].BackgroundImage = whiteKing;
			}

			else if (endSquare > 55 && board[endSquare] == 2)
			{
				board[endSquare] = 4;
				formControls["sq" + endSquare].BackgroundImage = redKing;
			}
		}

		public Boolean CanPlayerMove(String playerColor)
		{
			var moves = engine.FindMoves(board, playerColor);
			return moves.Count > 0;
		}
	}
}
