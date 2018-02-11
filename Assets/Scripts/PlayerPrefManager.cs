using UnityEngine;
using UnityEngine.SceneManagement;

// include so we can manipulate SceneManager

public static class PlayerPrefManager {

	public static int GetLives() {
		if (PlayerPrefs.HasKey("lives")) {
			return PlayerPrefs.GetInt("lives");

		} else {
			return 0;
		}
	}

	public static void SetLives(int lives) {
		PlayerPrefs.SetInt("lives",lives);
	}

	public static int GetScore() {
		if (PlayerPrefs.HasKey("score")) {
			return PlayerPrefs.GetInt("score");
		} else {
			return 0;
		}
	}

	public static void SetScore(int score) {
		PlayerPrefs.SetInt("score",score);
	}

	public static int GetHighscore() {
		if (PlayerPrefs.HasKey("highScore")) {
			return PlayerPrefs.GetInt("highScore");
		} else {
			return 0;
		}
	}

	public static void SetHighscore(int highscore) {
		PlayerPrefs.SetInt("highScore",highscore);
	}


	// story the current player state info into PlayerPrefs
	public static void SavePlayerState( int highScore, int lives) {
		// save currentscore and lives to PlayerPrefs for moving to next level
		PlayerPrefs.SetInt("lives",lives);
		PlayerPrefs.SetInt("highScore",highScore);
	}
	
	// reset stored player state and variables back to defaults
	public static void ResetPlayerState(int startLives, bool resetHighscore) {
		Debug.Log ("Player State reset.");
		PlayerPrefs.SetInt("lives",startLives);
		PlayerPrefs.SetInt("score", 0);

		if (resetHighscore)
			PlayerPrefs.SetInt("highScore", 0);
	}

	// store a key for the name of the current level to indicate it is unlocked
	public static void UnlockLevel() {
		// get current scene
		Scene scene = SceneManager.GetActiveScene();
		PlayerPrefs.SetInt(scene.name,1);
	}

	// determine if a levelname is currently unlocked (i.e., it has a key set)
	public static bool LevelIsUnlocked(string levelName) {
		return (PlayerPrefs.HasKey(levelName));
	}

	// output the defined Player Prefs to the console
	public static void ShowPlayerPrefs() {
		// store the PlayerPref keys to output to the console
		string[] values = {"score","highScore","lives"};

		// loop over the values and output to the console
		foreach(string value in values) {
			if (PlayerPrefs.HasKey(value)) {
				Debug.Log (value+" = "+PlayerPrefs.GetInt(value));
			} else {
				Debug.Log (value+" is not set.");
			}
		}
	}
}
