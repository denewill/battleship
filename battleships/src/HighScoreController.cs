
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using System.IO;
using SwinGameSDK;

/// <summary>
/// Controls displaying and collecting high score data.
/// </summary>
/// <remarks>
/// Data is saved to a file.
/// </remarks>
static class HighScoreController
{
    private const string FILE_NAME = "HighScores.txt";
	private const int NAME_WIDTH = 3;
    private const int SCORES_TOP = 80;

    private const int SCORES_X_POSITION = 490;
	/// <summary>
	/// The score structure is used to keep the name and
	/// score of the top players together.
	/// </summary>
	private struct Score : IComparable
	{
		public string Name;

		public int Value;
		/// <summary>
		/// Allows scores to be compared to facilitate sorting
		/// </summary>
		/// <param name="obj">the object to compare to</param>
		/// <returns>a value that indicates the sort order</returns>
		public int CompareTo(object obj)
		{
			if (obj is Score) {
				Score other = (Score)obj;

				return other.Value - this.Value;
			} else {
				return 0;
			}
		}
	}


	private static List<Score> _Scores = new List<Score>();
	/// <summary>
	/// Loads the scores from the highscores text file.
	/// </summary>
	/// <remarks>
	/// The format is
	/// # of scores
	/// NNNSSS
	/// 
	/// Where NNN is the name and SSS is the score
	/// </remarks>
	private static void LoadScores()
	{
        StreamReader streamReader;

        try
        {
            streamReader = new StreamReader(SwinGame.PathToResource(FILE_NAME));

            //Read in the number of scores
            int numScores = 0;
            numScores = Convert.ToInt32(streamReader.ReadLine());

            _Scores.Clear();

            for (int i = 0; i < numScores; i++)
            {
                //Creating a new Score structure
                Score score = new Score();
                string line = null;

                //Reading the line containing both name and score
                line = streamReader.ReadLine();

                //Splitting name and score values from the line and saving to each property of the struct
                score.Name = line.Substring(0, NAME_WIDTH);
                score.Value = Convert.ToInt32(line.Substring(NAME_WIDTH));

                _Scores.Add(score);
            }

            streamReader.Close();
        }
        //Catching potential file opening errors streamReader may encounter.
        catch
        {
            SwinGame.DrawText("Error reading scores text file.", Color.White, GameResources.GameFont("Courier"), SCORES_X_POSITION, SCORES_TOP);
            
            return;
        }
	}

	/// <summary>
	/// Saves the scores back to the highscores text file.
	/// </summary>
	/// <remarks>
	/// The format is
	/// # of scores
	/// NNNSSS
	/// 
	/// Where NNN is the name and SSS is the score
	/// </remarks>
	public static void SaveScores()
	{
        StreamWriter streamWriter;
        try
        {
            streamWriter = new StreamWriter(SwinGame.PathToResource(FILE_NAME));

            streamWriter.WriteLine(_Scores.Count);

            foreach (Score score in _Scores)
            {
                streamWriter.WriteLine(score.Name + score.Value);
            }

            streamWriter.Close();
        }
        //Catching potential file opening errors streamReader may encounter.
        catch
        {
            SwinGame.DrawText("Error writing to scores text file.", Color.White, GameResources.GameFont("Courier"), SCORES_X_POSITION, SCORES_TOP);

            return;
        }
	}

	/// <summary>
	/// Draws the high scores to the screen.
	/// </summary>
	public static void DrawHighScores()
	{
		const int SCORES_HEADING = 40;
		const int SCORE_GAP = 30;

		if (_Scores.Count == 0)
			LoadScores();

		SwinGame.DrawText("   High Scores   ", Color.White, GameResources.GameFont("Courier"), SCORES_X_POSITION, SCORES_HEADING);

		//For all of the scores
		int i = 0;
		for (i = 0; i <= _Scores.Count - 1; i++) {
			Score s = default(Score);

			s = _Scores[i];

			//for scores 1 - 9 use 01 - 09
			if (i < 9) {
				SwinGame.DrawText(" " + (i + 1) + ":   " + s.Name + "   " + s.Value, Color.White, GameResources.GameFont("Courier"), SCORES_X_POSITION, SCORES_TOP + (i * SCORE_GAP));
			} else {
				SwinGame.DrawText(i + 1 + ":   " + s.Name + "   " + s.Value, Color.White, GameResources.GameFont("Courier"), SCORES_X_POSITION, SCORES_TOP + (i * SCORE_GAP));
			}
		}
	}

	/// <summary>
	/// Handles the user input during the top score screen.
	/// </summary>
	/// <remarks>
    /// Updated the Keycodes
    /// </remarks>
	public static void HandleHighScoreInput()
	{
        if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.EscapeKey) || SwinGame.KeyTyped(KeyCode.ReturnKey)) {
			GameController.EndCurrentState();
		}
	}

	/// <summary>
	/// Read the user's name for their highsSwinGame.
	/// </summary>
	/// <param name="value">the player's sSwinGame.</param>
	/// <remarks>
	/// This verifies if the score is a highsSwinGame.
	/// </remarks>
	public static void ReadHighScore(int value)
	{
		const int ENTRY_TOP = 500;

		if (_Scores.Count == 0)
			LoadScores();

		//is it a high score
		if (value > _Scores[_Scores.Count - 1].Value) {
			Score s = new Score();
			s.Value = value;

			GameController.AddNewState(GameState.ViewingHighScores);

			int x = 0;
			x = SCORES_X_POSITION + SwinGame.TextWidth(GameResources.GameFont("Courier"), "Name: ");

			SwinGame.StartReadingText(Color.White, NAME_WIDTH, GameResources.GameFont("Courier"), x, ENTRY_TOP);

			//Read the text from the user
			while (SwinGame.ReadingText()) {
				SwinGame.ProcessEvents();

				UtilityFunctions.DrawBackground();
				DrawHighScores();
				SwinGame.DrawText("Name: ", Color.White, GameResources.GameFont("Courier"), SCORES_X_POSITION, ENTRY_TOP);
				SwinGame.RefreshScreen();
			}

			s.Name = SwinGame.TextReadAsASCII();

			if (s.Name.Length < 3) {
				s.Name = s.Name + new string(Convert.ToChar(" "), 3 - s.Name.Length);
			}

			_Scores.RemoveAt(_Scores.Count - 1);
			_Scores.Add(s);
			_Scores.Sort();

			GameController.EndCurrentState();
		}
	}
}