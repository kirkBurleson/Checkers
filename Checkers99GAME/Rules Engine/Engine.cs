using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers99GAME
{
	//Validates moves and does calculations
	public sealed class Engine
	{
		#region BIT BOARDS
		//Bitboard of any square
		private static UInt64[] squareTable = {
			18446744073709551615UL, 4611686018427387904UL, 2305843009213693952UL, 1152921504606846976UL, 576460752303423488UL, 288230376151711744UL, 144115188075855872UL, 72057594037927936UL,
			36028797018963968UL, 18014398509481984UL, 9007199254740992UL, 4503599627370496UL, 2251799813685248UL, 1125899906842624UL, 562949953421312UL, 281474976710656UL,
			140737488355328UL, 70368744177664UL, 35184372088832UL, 17592186044416UL, 8796093022208UL, 4398046511104UL, 2199023255552UL, 1099511627776UL,
			549755813888UL, 274877906944UL, 137438953472UL, 68719476736UL, 34359738368UL, 17179869184UL, 8589934592UL, 4294967296UL,
			2147483648UL, 1073741824UL, 536870912UL, 268435456UL, 134217728UL, 67108864UL, 33554432UL, 16777216UL,
			8388608UL, 4194304UL, 2097152UL, 1048576UL, 524288UL, 262144UL, 131072UL, 65536UL,
			32768UL, 16384UL, 8192UL, 4096UL, 2048UL, 1024UL, 512UL, 256UL,
			128UL, 64UL, 32UL, 16UL, 8UL, 4UL, 2UL, 1UL};

		//find a square from a bitboard
		private static Dictionary<UInt64, Byte> bitboard2Square = new Dictionary<UInt64, Byte> {
			{4611686018427387904UL, 1}, {1152921504606846976UL, 3}, {288230376151711744UL, 5}, {72057594037927936UL, 7},
			{36028797018963968UL, 8}, {9007199254740992UL, 10}, {2251799813685248UL, 12}, {562949953421312UL, 14},
			{70368744177664UL, 17}, {17592186044416UL, 19}, {4398046511104UL, 21}, {1099511627776UL, 23},
			{549755813888UL, 24}, {137438953472UL, 26}, {34359738368UL, 28}, {8589934592UL, 30},
			{1073741824UL, 33}, {268435456UL, 35}, {67108864UL, 37}, {16777216UL, 39},
			{8388608UL, 40}, {2097152UL, 42}, {524288UL, 44}, {131072UL, 46},
			{16384UL, 49}, {4096UL, 51}, {1024UL, 53}, {256UL, 55},
			{128UL, 56}, {32UL, 58}, {8UL, 60}, {2UL, 62} };

		//index order: top-left, top-right, bottom-left, bottom-right
		private static Dictionary<Byte, UInt64[]> kingAttackSquares = new Dictionary<Byte, UInt64[]> {
			{ 1, new UInt64[] { 0UL, 0UL, 36028797018963968UL, 9007199254740992UL } },
			{ 3, new UInt64[] { 0UL, 0UL, 9007199254740992UL, 2251799813685248UL } },
			{ 5, new UInt64[] { 0UL, 0UL, 2251799813685248UL, 562949953421312UL } },
			{ 7, new UInt64[] { 0UL, 0UL, 562949953421312UL, 0UL } },
			{ 8, new UInt64[] { 0UL, 4611686018427387904UL, 0UL, 70368744177664UL } },
			{10, new UInt64[] { 4611686018427387904UL, 1152921504606846976UL, 70368744177664UL, 17592186044416UL }},
			{12, new UInt64[] { 1152921504606846976UL, 288230376151711744UL, 17592186044416UL, 4398046511104UL }},
			{14, new UInt64[] { 288230376151711744UL, 72057594037927936UL, 4398046511104UL, 1099511627776UL }},
			{17, new UInt64[] { 36028797018963968UL, 9007199254740992UL, 549755813888UL, 137438953472UL }},
			{19, new UInt64[] { 9007199254740992UL, 2251799813685248UL, 137438953472UL, 34359738368UL }},
			{21, new UInt64[] { 2251799813685248UL, 562949953421312UL, 34359738368UL, 8589934592UL }},
			{23, new UInt64[] { 562949953421312UL, 0UL, 8589934592UL, 0UL }},
			{24, new UInt64[] { 0UL, 70368744177664UL, 0UL, 1073741824UL }},
			{26, new UInt64[] { 70368744177664UL, 17592186044416UL, 1073741824UL, 268435456UL }},
			{28, new UInt64[] { 17592186044416UL, 4398046511104UL, 268435456UL, 67108864UL }},
			{30, new UInt64[] { 4398046511104UL, 1099511627776UL, 67108864UL, 16777216UL }},
			{33, new UInt64[] { 549755813888UL, 137438953472UL, 8388608UL, 2097152UL }},
			{35, new UInt64[] { 137438953472UL, 34359738368UL, 2097152UL, 524288UL }},
			{37, new UInt64[] { 34359738368UL, 8589934592UL, 524288UL, 131072UL }},
			{39, new UInt64[] { 8589934592UL, 0UL, 131072UL, 0UL }},
			{40, new UInt64[] { 0UL, 1073741824UL, 0UL, 16384UL }},
			{42, new UInt64[] { 1073741824UL, 268435456UL, 16384UL, 4096UL }},
			{44, new UInt64[] { 268435456UL, 67108864UL, 4096UL, 1024UL }},
			{46, new UInt64[] { 67108864UL, 16777216UL, 1024UL, 256UL }},
			{49, new UInt64[] { 8388608UL, 2097152UL, 128UL, 32UL }},
			{51, new UInt64[] { 2097152UL, 524288UL, 32UL, 8UL }},
			{53, new UInt64[] { 524288UL, 131072UL, 8UL, 2UL }},
			{55, new UInt64[] { 131072UL, 0UL, 2UL, 0UL }},
			{56, new UInt64[] { 0UL, 16384UL, 0UL, 0UL }},
			{58, new UInt64[] { 16384UL, 4096UL, 0UL, 0UL }},
			{60, new UInt64[] { 4096UL, 1024UL, 0UL, 0UL }},
			{62, new UInt64[] { 1024UL, 256UL, 0UL, 0UL }}};

		//element 0 = left, element 1 = right
		private static Dictionary<Byte, UInt64[]> whiteAttackSquares = new Dictionary<Byte, UInt64[]> {
			{ 8, new UInt64[] { 0UL, 4611686018427387904UL } },
			{10, new UInt64[] { 4611686018427387904UL, 1152921504606846976UL }},
			{12, new UInt64[] { 1152921504606846976UL, 288230376151711744UL }},
			{14, new UInt64[] { 288230376151711744UL, 72057594037927936UL }},
			{17, new UInt64[] { 36028797018963968UL, 9007199254740992UL }},
			{19, new UInt64[] { 9007199254740992UL, 2251799813685248UL }},
			{21, new UInt64[] { 2251799813685248UL, 562949953421312UL }},
			{23, new UInt64[] { 562949953421312UL, 0UL }},
			{24, new UInt64[] { 0UL, 70368744177664UL }},
			{26, new UInt64[] { 70368744177664UL, 17592186044416UL }},
			{28, new UInt64[] { 17592186044416UL, 4398046511104UL }},
			{30, new UInt64[] { 4398046511104UL, 1099511627776UL }},
			{33, new UInt64[] { 549755813888UL, 137438953472UL }},
			{35, new UInt64[] { 137438953472UL, 34359738368UL }},
			{37, new UInt64[] { 34359738368UL, 8589934592UL }},
			{39, new UInt64[] { 8589934592UL, 0UL }},
			{40, new UInt64[] { 0UL, 1073741824UL }},
			{42, new UInt64[] { 1073741824UL, 268435456UL }},
			{44, new UInt64[] { 268435456UL, 67108864UL }},
			{46, new UInt64[] { 67108864UL, 16777216UL }},
			{49, new UInt64[] { 8388608UL, 2097152UL }},
			{51, new UInt64[] { 2097152UL, 524288UL }},
			{53, new UInt64[] { 524288UL, 131072UL }},
			{55, new UInt64[] { 131072UL, 0UL }},
			{56, new UInt64[] { 0UL, 16384UL }},
			{58, new UInt64[] { 16384UL, 4096UL}},
			{60, new UInt64[] { 4096UL, 1024UL }},
			{62, new UInt64[] { 1024UL, 256UL }}};

		private static Dictionary<Byte, UInt64[]> redAttackSquares = new Dictionary<Byte, UInt64[]> {
			{ 1, new UInt64[] { 36028797018963968UL, 9007199254740992UL } },
			{ 3, new UInt64[] { 9007199254740992UL, 2251799813685248UL } },
			{ 5, new UInt64[] { 2251799813685248UL, 562949953421312UL } },
			{ 7, new UInt64[] { 562949953421312UL, 0UL } },
			{ 8, new UInt64[] { 0UL, 70368744177664UL } },
			{10, new UInt64[] { 70368744177664UL, 17592186044416UL }},
			{12, new UInt64[] { 17592186044416UL, 4398046511104UL }},
			{14, new UInt64[] { 4398046511104UL, 1099511627776UL }},
			{17, new UInt64[] { 549755813888UL, 137438953472UL }},
			{19, new UInt64[] { 137438953472UL, 34359738368UL }},
			{21, new UInt64[] { 34359738368UL, 8589934592UL }},
			{23, new UInt64[] { 8589934592UL, 0UL }},
			{24, new UInt64[] { 0UL, 1073741824UL }},
			{26, new UInt64[] { 1073741824UL, 268435456UL }},
			{28, new UInt64[] { 268435456UL, 67108864UL }},
			{30, new UInt64[] { 67108864UL, 16777216UL }},
			{33, new UInt64[] { 8388608UL, 2097152UL }},
			{35, new UInt64[] { 2097152UL, 524288UL }},
			{37, new UInt64[] { 524288UL, 131072UL }},
			{39, new UInt64[] { 131072UL, 0UL }},
			{40, new UInt64[] { 0UL, 16384UL }},
			{42, new UInt64[] { 16384UL, 4096UL }},
			{44, new UInt64[] { 4096UL, 1024UL }},
			{46, new UInt64[] { 1024UL, 256UL }},
			{49, new UInt64[] { 128UL, 32UL }},
			{51, new UInt64[] { 32UL, 8UL }},
			{53, new UInt64[] { 8UL, 2UL }},
			{55, new UInt64[] { 2UL, 0UL }}};



		//General bitboards
		private UInt64 whiteCheckers = 0UL;
		private UInt64 whiteKings = 0UL;
		private UInt64 redCheckers = 0UL;
		private UInt64 redKings = 0UL;
		private UInt64 emptySquares = 0UL;

		#endregion
		#region LOOKUP TABLES
		//concatenate start square with jump direction.
		private static Dictionary<String, Byte> JumpLandingSquareLookup = new Dictionary<String, Byte>
		{
			{"1se", 19},
			{"3sw", 17},
			{"3se", 21},
			{"5sw", 19},
			{"5se", 23},
			{"7sw", 21},
			{"8se", 26},
			{"10sw", 24},
			{"10se", 28},
			{"12sw", 26},
			{"12se", 30},
			{"14sw", 28},
			{"17ne", 3},
			{"17se", 35},
			{"19nw", 1},
			{"19ne", 5},
			{"19sw", 33},
			{"19se", 37},
			{"21nw", 3},
			{"21ne", 7},
			{"21sw", 35},
			{"21se", 39},
			{"23nw", 5},
			{"23sw", 37},
			{"24ne", 10},
			{"24se", 42},
			{"26nw", 8},
			{"26ne", 12},
			{"26sw", 40},
			{"26se", 44},
			{"28nw", 10},
			{"28ne", 14},
			{"28sw", 42},
			{"28se", 46},
			{"30nw", 12},
			{"30sw", 44},
			{"33ne", 19},
			{"33se", 51},
			{"35nw", 17},
			{"35ne", 21},
			{"35sw", 49},
			{"35se", 53},
			{"37nw", 19},
			{"37ne", 23},
			{"37sw", 51},
			{"37se", 55},
			{"39nw", 21},
			{"39sw", 53},
			{"40ne", 26},
			{"40se", 58},
			{"42nw", 24},
			{"42ne", 28},
			{"42sw", 56},
			{"42se", 60},
			{"44nw", 26},
			{"44ne", 30},
			{"44sw", 58},
			{"44se", 62},
			{"46nw", 28},
			{"46sw", 60},
			{"49ne", 35},
			{"51nw", 33},
			{"51ne", 37},
			{"53nw", 35},
			{"53ne", 39},
			{"55nw", 37},
			{"56ne", 42},
			{"58nw", 40},
			{"58ne", 44},
			{"60nw", 42},
			{"60ne", 46},
			{"62nw", 44}
		};

		//concatenate the start square with the end square to get a string index to retrieve the number of the jumped square.
		//start = 1, end = 19, index is "119" to retrieve 10. You just jumped square #10
		private static Dictionary<String, Byte> JumpedSquareLookup = new Dictionary<String, Byte>
		{
				{"119", 10},
		    {"317", 10},
				{"321", 12},
				{"519", 12},
				{"523", 14},
				{"721", 14},
				{"826", 17},
				{"1024", 17},
				{"1028", 19},
				{"1226", 19},
				{"1230", 21},
				{"1428", 21},
		    {"173", 10},
				{"1735", 26},
				{"191", 10},
				{"195", 12},
				{"1933", 26},
				{"1937", 28},
				{"213", 12},
				{"217", 14},
				{"2135", 28},
				{"2139", 30},
				{"235", 14},
		    {"2337", 30},
				{"2410", 17},
				{"2442", 33},
				{"268", 17},
				{"2612", 19},
				{"2640", 33},
				{"2644", 35},
				{"2810", 19},
				{"2814", 21},
				{"2842", 35},
				{"2846", 37},
		    {"3012", 21},
				{"3044", 37},
				{"3319", 26},
				{"3351", 42},
				{"3517", 26},
				{"3521", 28},
				{"3549", 42},
				{"3553", 44},
				{"3719", 28},
				{"3723", 30},
				{"3751", 44},
		    {"3755", 46},
				{"3921", 30},
				{"3953", 46},
				{"4026", 33},
				{"4058", 49},
				{"4224", 33},
				{"4228", 35},
				{"4256", 49},
				{"4260", 51},
				{"4426", 35},
				{"4430", 37},
		    {"4458", 51},
				{"4462", 53},
				{"4628", 37},
				{"4660", 53},
				{"4935", 42},
				{"5133", 42},
				{"5137", 44},
				{"5335", 44},
				{"5339", 46},
				{"5537", 46},
				{"5642", 49},
		    {"5840", 49},
				{"5844", 51},
				{"6042", 51},
				{"6046", 53},
				{"6244", 53}		
		};
		#endregion
		public enum MoveDirection
		{
			NW,
			NE,
			SW,
			SE
		};
		public enum Piece
		{
			Empty = 0,
			WhiteChecker = 1,
			RedChecker = 2,
			WhiteKing = 3,
			RedKing = 4
		};

		private Stack<Byte[]> _boardStack;
		private Byte[] _board;
		private Byte[] NonAttackingSquaresNW = { 1, 3, 5, 7, 8, 10, 12, 14, 17, 24, 33, 40, 49, 56 };
		private Byte[] NonAttackingSquaresNE = { 1, 3, 5, 7, 8, 10, 12, 14, 23, 30, 39, 46, 55, 62 };
		private Byte[] NonAttackingSquaresSW = { 1, 8, 17, 24, 33, 40, 49, 51, 53, 55, 56, 58, 60, 62 };
		private Byte[] NonAttackingSquaresSE = { 7, 14, 23, 30, 39, 46, 49, 51, 53, 55, 56, 58, 60, 62 };
		private Byte[] NonMovingSquaresNW = { 1, 3, 5, 7, 8, 24, 40, 56 };
		private Byte[] NonMovingSquaresNE = { 1, 3, 5, 7, 23, 39, 55 };
		private Byte[] NonMovingSquaresSW = { 8, 24, 40, 56, 58, 60, 62 };
		private Byte[] NonMovingSquaresSE = { 7, 23, 39, 55, 56, 58, 60, 62 };

		public Engine()
		{
			_boardStack = new Stack<Byte[]>();
		}
		public List<Move> FindMoves(Byte[] board, String colorToMove)
		{
			_board = board;

			List<Move> jumps = new List<Move>();

			jumps = GetJumps(colorToMove.ToLower());

			if (jumps.Count == 0)
			{
				if (colorToMove.ToLower() == "red")
					return FindRedMoves();
				else
					return FindWhiteMoves();
			}

			return jumps;
		}
		public Byte GetJumpedSquare(Byte startSquare, Byte endSquare)
		{
			String index = startSquare.ToString() + endSquare.ToString();
			Byte jumpedSquare = 0;

			if (JumpedSquareLookup.ContainsKey(index))
				jumpedSquare = JumpedSquareLookup[index];

			return jumpedSquare;
		}

		private List<Move> FindWhiteMoves()
		{
			return GetMoves(Player.PlayerColor.WHITE);
		}

		private List<Move> FindRedMoves()
		{
			return GetMoves(Player.PlayerColor.RED);
		}

		private List<Move> GetMoves(Player.PlayerColor playerColor)
		{
			PrepareBitBoards();

			var moves = new List<Move>();

			for (Byte index = 0; index < _board.Length; index++)
			{
				Player.PlayerColor currentSquareCheckerColor = Player.GetCheckerColor((Piece)_board[index]);
				if (currentSquareCheckerColor == playerColor)
				{
					switch ((Piece)_board[index])
					{
						case Piece.WhiteKing:
						case Piece.RedKing:
							if (CanMoveNW(index))
							{
								Byte endSquare = GetSquareFromBitboard(whiteAttackSquares[index][0]);
								var move = new Move(index, endSquare, false) { WillKing = IsKingingSquare(endSquare) };
								moves.Add(move);
							}

							if (CanMoveNE(index))
							{
								Byte endSquare = GetSquareFromBitboard(whiteAttackSquares[index][1]);
								var move = new Move(index, endSquare, false) { WillKing = IsKingingSquare(endSquare) };
								moves.Add(move);
							}

							if (CanMoveSW(index))
							{
								Byte endSquare = GetSquareFromBitboard(redAttackSquares[index][0]);
								var move = new Move(index, endSquare, false) { WillKing = IsKingingSquare(endSquare) };
								moves.Add(move);
							}

							if (CanMoveSE(index))
							{
								Byte endSquare = GetSquareFromBitboard(redAttackSquares[index][1]);
								var move = new Move(index, endSquare, false) { WillKing = IsKingingSquare(endSquare) };
								moves.Add(move);
							}

							break;

						case Piece.WhiteChecker:
							if (CanMoveNW(index))
							{
								Byte endSquare = GetSquareFromBitboard(whiteAttackSquares[index][0]);
								var move = new Move(index, endSquare, false) { WillKing = IsKingingSquare(endSquare) };
								moves.Add(move);
							}

							if (CanMoveNE(index))
							{
								Byte endSquare = GetSquareFromBitboard(whiteAttackSquares[index][1]);
								var move = new Move(index, endSquare, false) { WillKing = IsKingingSquare(endSquare) };
								moves.Add(move);
							}

							break;

						case Piece.RedChecker:
							if (CanMoveSW(index))
							{
								Byte endSquare = GetSquareFromBitboard(redAttackSquares[index][0]);
								var move = new Move(index, endSquare, false) { WillKing = IsKingingSquare(endSquare) };
								moves.Add(move);
							}

							if (CanMoveSE(index))
							{
								Byte endSquare = GetSquareFromBitboard(redAttackSquares[index][1]);
								var move = new Move(index, endSquare, false) { WillKing = IsKingingSquare(endSquare) };
								moves.Add(move);
							}

							break;
					}
				}
			}

			return moves;
		}

		private String GetPlayerColorFromPiece(Piece piece)
		{
			if (piece == 0) return "";

			if ((Int32)piece % 2 == 0)
				return "red";
			else
				return "white";
		}

		private void ClearBitBoards()
		{
			whiteCheckers = 0UL;
			whiteKings = 0UL;
			redCheckers = 0UL;
			redKings = 0UL;
			emptySquares = 0UL;
		}

		private void CreateBitBoards()
		{
			for (int index = 0; index < _board.Length; index++)
			{
				switch (_board[index])
				{
					case 0:
						emptySquares += (UInt64)squareTable[index];
						break;

					case 1:
						whiteCheckers += (UInt64)squareTable[index];
						break;

					case 2:
						redCheckers += (UInt64)squareTable[index];
						break;

					case 3:
						whiteKings += (UInt64)squareTable[index];
						break;

					case 4:
						redKings += (UInt64)squareTable[index];
						break;

					default:
						break;
				}
			}
		}

		private Boolean CanMoveNW(Byte startSquare)
		{
			return (NonMovingSquaresNW.Contains(startSquare) == false) && ((emptySquares & whiteAttackSquares[startSquare][0]) != 0UL);
		}

		private Boolean CanMoveNE(Byte startSquare)
		{
			return (NonMovingSquaresNE.Contains(startSquare) == false) && ((emptySquares & whiteAttackSquares[startSquare][1]) != 0UL);
		}

		private Boolean CanMoveSW(Byte startSquare)
		{
			return (NonMovingSquaresSW.Contains(startSquare) == false) && ((emptySquares & redAttackSquares[startSquare][0]) != 0UL);
		}

		private Boolean CanMoveSE(Byte startSquare)
		{
			return (NonMovingSquaresSE.Contains(startSquare) == false) && ((emptySquares & redAttackSquares[startSquare][1]) != 0UL);
		}

		private Boolean IsLandingSquareEmpty(Byte startSquare, MoveDirection jumpDirection)
		{
			Boolean answer = false;
			String start = startSquare.ToString() + jumpDirection.ToString().ToLower();
			Byte landingSquare = JumpLandingSquareLookup[start];
			UInt64 landingBitBoard = squareTable[landingSquare];

			if ((emptySquares & landingBitBoard) != 0UL)
				answer = true;

			return answer;
		}

		private Boolean WhiteCanAttackNW(Byte startSquare)
		{
			if (startSquare < 18 || NonAttackingSquaresNW.Contains(startSquare))
				return false;
			
			return (redCheckers & whiteAttackSquares[startSquare][0]) != 0UL || (redKings & whiteAttackSquares[startSquare][0]) != 0UL;
		}

		private Boolean WhiteCanAttackNE(Byte startSquare)
		{
			if (startSquare < 16 || NonAttackingSquaresNE.Contains(startSquare))
				return false;

			return (redCheckers & whiteAttackSquares[startSquare][1]) != 0UL || (redKings & whiteAttackSquares[startSquare][1]) != 0UL;
		}

		private Boolean WhiteCanAttackSW(Byte startSquare)
		{
			if (startSquare > 46 || NonAttackingSquaresSW.Contains(startSquare))
				return false;

			return (redCheckers & redAttackSquares[startSquare][0]) != 0UL || (redKings & redAttackSquares[startSquare][0]) != 0UL;
		}

		private Boolean WhiteCanAttackSE(Byte startSquare)
		{
			if (startSquare > 44 || NonAttackingSquaresSE.Contains(startSquare))
				return false;

			return (redCheckers & redAttackSquares[startSquare][1]) != 0UL || (redKings & redAttackSquares[startSquare][1]) != 0UL;
		}

		private Boolean RedCanAttackNW(Byte startSquare)
		{
			if (startSquare < 18 || NonAttackingSquaresNW.Contains(startSquare))
				return false;

			return (whiteCheckers & whiteAttackSquares[startSquare][0]) != 0UL || (whiteKings & whiteAttackSquares[startSquare][0]) != 0UL;
		}

		private Boolean RedCanAttackNE(Byte startSquare)
		{
			if (startSquare < 16 || NonAttackingSquaresNE.Contains(startSquare))
				return false;

			return (whiteCheckers & whiteAttackSquares[startSquare][1]) != 0UL || (whiteKings & whiteAttackSquares[startSquare][1]) != 0UL;
		}

		private Boolean RedCanAttackSW(Byte startSquare)
		{
			if (startSquare > 46 || NonAttackingSquaresSW.Contains(startSquare))
				return false;

			return (whiteCheckers & redAttackSquares[startSquare][0]) != 0UL || (whiteKings & redAttackSquares[startSquare][0]) != 0UL;
		}

		private Boolean RedCanAttackSE(Byte startSquare)
		{
			if (startSquare > 44 || NonAttackingSquaresSE.Contains(startSquare))
				return false;

			return (whiteCheckers & redAttackSquares[startSquare][1]) != 0UL || (whiteKings & redAttackSquares[startSquare][1]) != 0UL;
		}

		private Boolean CanJumpNW(Piece piece, Byte startSquare)
		{
			Boolean answer = false;

			switch (piece)
			{
				case Piece.WhiteKing:
				case Piece.WhiteChecker:
					if (WhiteCanAttackNW(startSquare))
						answer = IsLandingSquareEmpty(startSquare, MoveDirection.NW);
					break;

				case Piece.RedKing:
					if (RedCanAttackNW(startSquare))
						answer = IsLandingSquareEmpty(startSquare, MoveDirection.NW);
					break;
			}

			return answer;
		}

		private Boolean CanJumpNE(Piece piece, Byte startSquare)
		{
			Boolean answer = false;

			switch (piece)
			{
				case Piece.WhiteKing:
				case Piece.WhiteChecker:
					if (WhiteCanAttackNE(startSquare))
						answer = IsLandingSquareEmpty(startSquare, MoveDirection.NE);
					break;

				case Piece.RedKing:
					if (RedCanAttackNE(startSquare))
						answer = IsLandingSquareEmpty(startSquare, MoveDirection.NE);
					break;
			}

			return answer;
		}

		private Boolean CanJumpSW(Piece piece, Byte startSquare)
		{
			Boolean answer = false;

			switch (piece)
			{
				case Piece.WhiteKing:
					if (WhiteCanAttackSW(startSquare))
						answer = IsLandingSquareEmpty(startSquare, MoveDirection.SW);
					break;

				case Piece.RedKing:
				case Piece.RedChecker:
					if (RedCanAttackSW(startSquare))
						answer = IsLandingSquareEmpty(startSquare, MoveDirection.SW);
					break;
			}

			return answer;
		}

		private Boolean CanJumpSE(Piece piece, Byte startSquare)
		{
			Boolean answer = false;

			switch (piece)
			{
				case Piece.WhiteKing:
					if (WhiteCanAttackSE(startSquare))
						answer = IsLandingSquareEmpty(startSquare, MoveDirection.SE);
					break;

				case Piece.RedKing:
				case Piece.RedChecker:
					if (RedCanAttackSE(startSquare))
						answer = IsLandingSquareEmpty(startSquare, MoveDirection.SE);
					break;
			}

			return answer;
		}

		private void PrepareBitBoards()
		{
			ClearBitBoards();
			CreateBitBoards();
		}

		private void FindJumpsFromSquare(Byte startSquare, Move move)
		{
			//NW
			if (CanJumpNW((Piece)_board[startSquare], startSquare))
			{
				Move m = CreateMoveFromJump(startSquare, MoveDirection.NW);
				move.Jumps.Add(m);
				MakeMove(m);
				FindJumpsFromSquare(m.EndSquare, m);
				UndoMove();
			}
			//NE
			if (CanJumpNE((Piece)_board[startSquare], startSquare))
			{
				Move m = CreateMoveFromJump(startSquare, MoveDirection.NE);
				move.Jumps.Add(m);
				MakeMove(m);
				FindJumpsFromSquare(m.EndSquare, m);
				UndoMove();
			}
			//SW
			if (CanJumpSW((Piece)_board[startSquare], startSquare))
			{
				Move m = CreateMoveFromJump(startSquare, MoveDirection.SW);
				move.Jumps.Add(m);
				MakeMove(m);
				FindJumpsFromSquare(m.EndSquare, m);
				UndoMove();
			}
			//SE
			if (CanJumpSE((Piece)_board[startSquare], startSquare))
			{
				Move m = CreateMoveFromJump(startSquare, MoveDirection.SE);
				move.Jumps.Add(m);
				MakeMove(m);
				FindJumpsFromSquare(m.EndSquare, m);
				UndoMove();
			}
		}

		private List<Move> GetJumps(String playerColor)
		{
			PrepareBitBoards();

			var jumps = new List<Move>();

			for (Byte index = 0; index < _board.Length; index++)
			{
				var piece = (Piece)_board[index];

				if (GetPlayerColorFromPiece(piece) == playerColor)
				{
					switch (piece)
					{
						case Piece.WhiteKing:
						case Piece.RedKing:
							if (CanJumpNW(piece, index))
							{
								Move m = CreateMoveFromJump(index, MoveDirection.NW);
								jumps.Add(m);
								MakeMove(m);
								if (!m.WillKing)
									FindJumpsFromSquare(m.EndSquare, m);
								UndoMove();
							}
							if (CanJumpNE(piece, index))
							{
								Move m = CreateMoveFromJump(index, MoveDirection.NE);
								jumps.Add(m);
								MakeMove(m);
								if (!m.WillKing)
									FindJumpsFromSquare(m.EndSquare, m);
								UndoMove();
							}
							if (CanJumpSW(piece, index))
							{
								Move m = CreateMoveFromJump(index, MoveDirection.SW);
								jumps.Add(m);
								MakeMove(m);
								if (!m.WillKing)
									FindJumpsFromSquare(m.EndSquare, m);
								UndoMove();
							}
							if (CanJumpSE(piece, index))
							{
								Move m = CreateMoveFromJump(index, MoveDirection.SE);
								jumps.Add(m);
								MakeMove(m);
								if (!m.WillKing)
									FindJumpsFromSquare(m.EndSquare, m);
								UndoMove();
							}
							break;
						case Piece.WhiteChecker:
							if (CanJumpNW(piece, index))
							{
								Move m = CreateMoveFromJump(index, MoveDirection.NW);
								jumps.Add(m);
								MakeMove(m);
								if (!m.WillKing)
									FindJumpsFromSquare(m.EndSquare, m);
								UndoMove();
							}
							if (CanJumpNE(piece, index))
							{
								Move m = CreateMoveFromJump(index, MoveDirection.NE);
								jumps.Add(m);
								MakeMove(m);
								if (!m.WillKing)
									FindJumpsFromSquare(m.EndSquare, m);
								UndoMove();
							}
							break;
						case Piece.RedChecker:
							if (CanJumpSW(piece, index))
							{
								Move m = CreateMoveFromJump(index, MoveDirection.SW);
								jumps.Add(m);
								MakeMove(m);
								if (!m.WillKing)
									FindJumpsFromSquare(m.EndSquare, m);
								UndoMove();
							}
							if (CanJumpSE(piece, index))
							{
								Move m = CreateMoveFromJump(index, MoveDirection.SE);
								jumps.Add(m);
								MakeMove(m);
								if (!m.WillKing)
									FindJumpsFromSquare(m.EndSquare, m);
								UndoMove();
							}
							break;
					}
				}
			}

			return jumps;
		}

		private Move CreateMoveFromJump(Byte index, MoveDirection direction)
		{
			String key = index.ToString() + direction.ToString().ToLower();
			Byte landingSquare = JumpLandingSquareLookup[key];
			Move move = new Move(index, landingSquare, true);

			return move;
		}

		private Boolean IsKingingSquare(Byte square)
		{
			Byte[] _kingSquares = { 1, 3, 5, 7, 56, 58, 60, 62 };
			return _kingSquares.Contains(square);
		}

		private Byte GetSquareFromBitboard(UInt64 bitBoard)
		{
			return bitboard2Square[bitBoard];
		}

		private void MakeMove(Move move)
		{
			_boardStack.Push((Byte[])_board.Clone());

			_board[move.EndSquare] = _board[move.StartSquare];
			_board[move.StartSquare] = 0;

			// King the checker
			if (_board[move.EndSquare] < 3 && IsKingingSquare(move.EndSquare))
			{
				_board[move.EndSquare] += 2;
				move.WillKing = true;
			}

			if (move.IsJump)
			{
				Byte js = GetJumpedSquare(move.StartSquare, move.EndSquare);
				_board[js] = 0;
			}

			PrepareBitBoards();
		}

		private void UndoMove()
		{
			_boardStack.Pop().CopyTo(_board, 0);

			PrepareBitBoards();
		}
	}
}
