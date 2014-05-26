using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers99GAME
{
	public sealed class Move
	{
		public Int32 Score { get; private set; }

		public Byte StartSquare { get; private set; }

		public Byte EndSquare { get; private set; }

		public Boolean IsJump { get; private set; }

		public List<Byte> JumpLandingSquares { get; private set; }

		public List<Move> Jumps { get; internal set; }

		public Boolean WillKing { get; internal set; }

		public Move(Byte startSquare, Byte endSquare, Boolean isJump)
		{
			StartSquare = startSquare;
			EndSquare = endSquare;
			IsJump = isJump;
			Score = 0;
			Jumps = new List<Move>();
			JumpLandingSquares = new List<Byte>();
			WillKing = false;
		}

		public void SetScore(Int32 score)
		{
			Score = score;
		}

		public void AddToScore(Int32 score)
		{
			Score += score;
		}

		public Boolean HasChildMoves
		{
			get { return Jumps.Count > 0; }
		}

		public override string ToString()
		{
			String data = "Score: " + Score.ToString() + Environment.NewLine +
										"Start Square: " + StartSquare.ToString() + Environment.NewLine +
										"End Square: " + EndSquare.ToString() + Environment.NewLine +
										"Is a Jump: " + IsJump.ToString() + Environment.NewLine;
			return data;
		}		
	}
}
