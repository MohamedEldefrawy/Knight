using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// include so we can manipulate SceneManager


public class GameManger : MonoBehaviour
{
    //getting refrence for the GameManger
    public static GameManger gm;



    //levels to move
    public string levelAfterVictory;
    public string levelAfterGameOver;

    //player info
    public GameObject _player;

    public GameObject mainCanavas;
    //score stuff
    public Text UiScore;
    public Text UiHighScore;
    public Text UiPlayerHealth;
    public Text UiLevel;
    public Text UiLifeRemaining;
    public GameObject UiGamePaused;




    //game performance
    public int score;
    public int highScore;
    public int startLives = 3;
    public int lives ;
    private Scene _scene;


    void Awake()
    {
        gm = this.GetComponent<GameManger>();
        mainCanavas.SetActive(true);
        setupDefaults();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0f)
            {
                UiGamePaused.SetActive(true); // this brings up the pause UI
                Time.timeScale = 0f; // this pauses the game action
            }
            else
            {
                Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
                UiGamePaused.SetActive(false); // remove the pause UI
            }
        }
    }



    //setup defaults

    void setupDefaults()
    {
        // setup reference to player
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");

        }


        // get current scene
        _scene = SceneManager.GetActiveScene();
        PlayerPrefManager.SetLives(startLives);


        // if levels not specified, default to current level
        if (levelAfterVictory == "")
        {
            Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
            levelAfterVictory = _scene.name;
        }

        if (levelAfterGameOver == "")
        {
            Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
            levelAfterGameOver = _scene.name;
        }

        // friendly error messages
        if (UiScore == null)
            Debug.LogError("Need to set UIScore on Game Manager.");

        if (UiHighScore == null)
            Debug.LogError("Need to set UIHighScore on Game Manager.");

        if (UiLevel == null)
            Debug.LogError("Need to set UILevel on Game Manager.");

        if (UiGamePaused == null)
            Debug.LogError("Need to set UIGamePaused on Game Manager.");

        if (UiLifeRemaining == null)
            Debug.LogError("Need to set UiLifeRemaining on Game Manager.");
        if(UiPlayerHealth == null)
            Debug.LogError("Need to set UiPlayerHealth on Game Manager.");


        // get stored player prefs
        refreshPlayerState();

        // get the UI ready for the game
        refreshGUI();
    }

    void refreshPlayerState()
    {
        lives = PlayerPrefManager.GetLives();

        // special case if lives <= 0 then must be testing in editor, so reset the player prefs
        if (lives <= 0)
        {
            PlayerPrefManager.ResetPlayerState(startLives, false);
            lives = PlayerPrefManager.GetLives();

        }
       // score = PlayerPrefManager.GetScore();
        highScore = PlayerPrefManager.GetHighscore();

        // save that this level has been accessed so the MainMenu can enable it
        PlayerPrefManager.UnlockLevel();
    }

    void refreshGUI()
    {
        // set the text elements of the UI
        UiScore.text = score.ToString();
        UiHighScore.text = "Highscore: " + highScore.ToString();
        UiLifeRemaining.text = lives.ToString();
        UiPlayerHealth.text = KnightController.playerHealth.ToString();
        UiLevel.text = _scene.name;


    }

    // Update is called once per frame
    public void AddPoints(int amount)
    {
        // increase score
        score += amount;
        // update UI
        UiScore.text = score.ToString();

        // if score>highscore then update the highscore UI too
        if (score > highScore)
        {
            highScore = score;
            UiHighScore.text = "Highscore: " + score.ToString();
        }
    }

    public void AddHealth()
    {

        KnightController.playerHealth = 100;
        UiPlayerHealth.text = "100";

    }

    public void reduceHealth(float amount)
    {
        if (amount <= 0)
        {
            UiPlayerHealth.text = " 0";
        }

        else
            UiPlayerHealth.text = amount.ToString();

    }


    public void ResetGame()
    {
        // remove life and update GUI
        lives--;
        refreshGUI();

        KnightController.playerHealth = 100;
        UiPlayerHealth.text = " 100";

        if (lives <= 0)
        { // no more lives
          // save the current player prefs before going to GameOver
            PlayerPrefManager.SavePlayerState( highScore, lives);
            // load the gameOver screen
            StartCoroutine(ResetGameDelay());

        }

    }

    // public function for level complete
    public void LevelCompete()
    {
        // save the current player prefs before moving to the next level
        PlayerPrefManager.SavePlayerState( highScore, lives);

        // use a coroutine to allow the player to get fanfare before moving to next level
        StartCoroutine(LoadNextLevel());
    }

    // load the nextLevel after delay
    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(levelAfterVictory);
    }

    IEnumerator ResetGameDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelAfterGameOver);
    }





}
