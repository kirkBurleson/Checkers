﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Checkers99GAME;

namespace Checkers99UI
{
	public partial class Form1 : Form
	{
		private AI ai;
		private IEngine engine;
		private String startSquareNum;
		private String endSquareNum;
		private Panel startSquare;
		private Panel endSquare;
		private Player redPlayer;
		private Player whitePlayer;
		private Player currentPlayer;
		private Byte[] board;
		private Timer timer;
		private Boolean gameOver;
		private Image whiteChecker;
		private Image redChecker;
		public Dictionary<String, String> algebraicLookup = new Dictionary<String, String>() {
			{ "1", "A2" },
			{ "3", "A4" },
			{ "5", "A6" },
			{ "7", "A8" },
			{ "8", "B1" },
			{ "10", "B3" },
			{ "12", "B5" },
			{ "14", "B7" },
			{ "17", "C2" },
			{ "19", "C4" },
			{ "21", "C6" },
			{ "23", "C8" },
			{ "24", "D1" },
			{ "26", "D3" },
			{ "28", "D5" },
			{ "30", "D7" },
			{ "33", "E2" },
			{ "35", "E4" },
			{ "37", "E6" },
			{ "39", "E8" },
			{ "40", "F1" },
			{ "42", "F3" },
			{ "44", "F5" },
			{ "46", "F7" },
			{ "49", "G2" },
			{ "51", "G4" },
			{ "53", "G6" },
			{ "55", "G8" },		
			{ "56", "H1" },
			{ "58", "H3" },
			{ "60", "H5" },
			{ "62", "H7" }
		};

		public Form1()
		{
			InitializeComponent();
			StartNewGame();
		}

		private void StartNewGame()
		{
			board = new Byte[] {
                0,2,0,2,0,2,0,2,
                2,0,2,0,2,0,2,0,
                0,2,0,2,0,2,0,2,
                0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,
                1,0,1,0,1,0,1,0,
                0,1,0,1,0,1,0,1,
                1,0,1,0,1,0,1,0};

			ai = new AI();
			engine = new MoveEngine(board);
			redPlayer = new Player(Player.PlayerColor.RED, Player.PlayerType.COMPUTER);
			whitePlayer = new Player(Player.PlayerColor.WHITE, Player.PlayerType.HUMAN);
			currentPlayer = whitePlayer;
			gameOver = false;
			pbTurnSignal.BackColor = Color.Green;
			timer = new Timer();
			timer.Interval = 200;
			timer.Tick += timer_Tick;
			lblScore.Text = "0";

			try
			{
				whiteChecker = Image.FromFile(@"images/whiteChecker.png");
				redChecker = Image.FromFile(@"images/redChecker.png");
			}
			catch (System.Exception e)
			{
				MessageBox.Show("Failed to load King checker images", "Error");
				gameOver = true;
			}
		}

		private void ChangeMessage(String msg)
		{
			lblMsg.Text = msg;
		}

		void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();

			if (currentPlayer.Type == Player.PlayerType.COMPUTER)
			{
				var move = ai.GetMove((Byte[])board.Clone(), currentPlayer);

				if (move == null)
				{
					gameOver = true;
					ChangeMessage("GAME OVER");
					return;
				}

				SimulateMouseEvents(move);				
			}
		}

		private void SimulateMouseEvents(Move move)
		{
			var startSquare = this.Controls["sq" + move.StartSquare];

			startSquare.BackColor = Color.Gray;
			sq_MouseClick(startSquare, null);
			startSquare.BackColor = Color.Green;

			var endSquare = this.Controls["sq" + move.EndSquare];

			endSquare.BackColor = Color.Gray;
			sq_MouseClick(endSquare, null);
			endSquare.BackColor = Color.Green;
		}

		private void sq_MouseEnter(object sender, EventArgs e)
		{
			var panel = sender as Panel;
			panel.BackColor = Color.Gray;
		}

		private void sq_MouseLeave(object sender, EventArgs e)
		{
			var panel = sender as Panel;
			panel.BackColor = Color.Green;
		}

		private void sq_MouseClick(object sender, MouseEventArgs e)
		{	
			if (gameOver)
				return;

			var clickedSquare = sender as Panel;
			var clickedSquareNum = clickedSquare.Name.Substring(2);

			if (startSquare == null)
			{
				if (clickedSquare.BackgroundImage == null)
					ClearMoveState();
				else
				{
					startSquareNum = clickedSquareNum;
					startSquare = clickedSquare;
					ChangeMessage(algebraicLookup[startSquareNum]);
				}

				return;
			}

			if (endSquare == null)
			{
				if (clickedSquare.BackgroundImage == null && clickedSquare.BackColor.Name == "Gray")
				{
					Boolean result = engine.IsMoveLegal(Int32.Parse(startSquareNum), Int32.Parse(clickedSquareNum), currentPlayer);

					if (result)
					{
						var move = engine.GetMove(Int32.Parse(startSquareNum), Int32.Parse(clickedSquareNum), currentPlayer);

						MakeMove(clickedSquare);
						UpdateBoardState();

						if (move.HasChildMoves == false || move.WillKing)
							ChangeCurrentPlayer();

						if (currentPlayer.Type == Player.PlayerType.COMPUTER)
						{
							pbTurnSignal.BackColor = Color.Transparent;
							timer.Start();
						}

						else
						{
							if (engine.CanPlayerMove(currentPlayer.Color.ToString()) == true)
							{
								pbTurnSignal.BackColor = Color.Green;
								lblScore.Text = AI.score.ToString();
							}
							else
							{
								gameOver = true;
								ChangeMessage("GAME OVER");
							}
						}
					}

					else
						Debug.Write("sq_MouseClick: Engine reports illegal move.");
				}
			}

			ClearMoveState();
		}

		private void MakeMove(Panel panel)
		{
			endSquare = panel;
			endSquareNum = endSquare.Name.Substring(2);

			endSquare.BackgroundImage = startSquare.BackgroundImage;
			startSquare.BackgroundImage = null;
		}

		private void ClearMoveState()
		{
			startSquareNum = "";
			startSquare = null;
			endSquare = null;
			endSquareNum = "";

			if (gameOver == false)
				ChangeMessage("");
		}

		private void UpdateBoardState()
		{
			engine.UpdateBoardState(Byte.Parse(startSquareNum), Byte.Parse(endSquareNum), this.Controls);
		}

		private void ChangeCurrentPlayer()
		{
			if (currentPlayer == redPlayer)
				currentPlayer = whitePlayer;
			else
				currentPlayer = redPlayer;
		}

		private void btnPlayAgain_Click(object sender, EventArgs e)
		{
			StartNewGame();
			SetCheckersToStartingPositions();
			ChangeMessage("");
			
		}

		private void SetCheckersToStartingPositions()
		{
			// red checkers
			((Panel)this.Controls["sq1"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq3"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq5"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq7"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq8"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq10"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq12"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq14"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq17"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq19"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq21"]).BackgroundImage = redChecker;
			((Panel)this.Controls["sq23"]).BackgroundImage = redChecker;

			// clear center board
			((Panel)this.Controls["sq24"]).BackgroundImage = null;
			((Panel)this.Controls["sq26"]).BackgroundImage = null;
			((Panel)this.Controls["sq28"]).BackgroundImage = null;
			((Panel)this.Controls["sq30"]).BackgroundImage = null;
			((Panel)this.Controls["sq33"]).BackgroundImage = null;
			((Panel)this.Controls["sq35"]).BackgroundImage = null;
			((Panel)this.Controls["sq37"]).BackgroundImage = null;
			((Panel)this.Controls["sq39"]).BackgroundImage = null;

			// white checkers
			((Panel)this.Controls["sq40"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq42"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq44"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq46"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq49"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq51"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq53"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq55"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq56"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq58"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq60"]).BackgroundImage = whiteChecker;
			((Panel)this.Controls["sq62"]).BackgroundImage = whiteChecker;
		}

	}
}
