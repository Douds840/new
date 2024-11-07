using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Sign : MonoBehaviour, IDataPersistence
{
    public Text[] blankBoxes;   // The empty boxes where letters are filled
    public Button[] letterButtons; // All the letter buttons
    public Button clearButton;  // Button to clear the filled letters
    public Button enterButton;  // Button to submit the answer
    public Button hintButton;   // Button to use hints
    public TMP_Text hintCounterText;  // Text to display how many hints are left
    public TMP_Text feedbackText;   // Text to show feedback (Correct/Incorrect or Empty Slot)
    public TMP_Text scoreText;      // Text to display the score
    public TMP_Text timerText;      // Text to display the timer
    public Text attemptsText;   // **New Text to display the number of attempts**
    public string correctWord = "USA";  // The correct answer
    public int maxHints = 3;    // Maximum number of hints allowed
    public int maxAttempts = 3; // **Maximum number of attempts allowed**
    private int currentBlank = 0;  // Index to keep track of which blank to fill
    public int hintsLeft;      // Tracks the number of remaining hints
    private int score = 0;      // Variable to track the player's score
    private int attempts = 0;   // Tracks incorrect attempts
    private List<int> usedHints = new List<int>();  // Keeps track of which letters have already been removed

    private float timeLeft = 10f;  // 10-second timer
    private bool isTimerRunning = false;  // Flag to check if the timer is running
    private bool gameEnded = false;  // To stop the game when time runs out
    public GameObject gameCompletePanel;  // Panel to show when the game is complete
    public GameObject gameOverPanel;      // Panel to show when the game is over

    // New UI elements for completion data
    public Text completionScoreText;
    public Text completionBestTimeText;
    public Text completionCurrentTimeText;

    public int game_index; // Example initialization, change accordingly
    public int currentlevel; // Example initialization, change accordingly
    private float bestTime = 0f; // Variable to store best time
    private float currentCompletionTime = 0f; // Variable to track current completion time
    private bool isScored = false; // Variable to track scoring
    private bool isNextlevelUnlocked = false; // Variable to track level unlock status
    private bool isAnswered;

    void Start()
    {
        hintsLeft = maxHints;  // Initialize the hints left
        UpdateHintCounter();   // Display the initial hint count
        UpdateScore();         // Display the initial score
        UpdateTimerText();     // Display the initial timer
        UpdateAttemptsText();  // Display the initial attempts count
        isTimerRunning = true;
        // Add listeners to letter buttons
        foreach (Button button in letterButtons)
        {
            button.onClick.AddListener(() => OnAnyButtonClick(() => SelectLetter(button)));
        }

        // Add listener to the clear button
        clearButton.onClick.AddListener(() => OnAnyButtonClick(ClearBlanks));

        // Add listener to the enter button to check the answer
        enterButton.onClick.AddListener(() => OnAnyButtonClick(CheckAnswer));

        // Add listener to the hint button to use hints
        hintButton.onClick.AddListener(() => OnAnyButtonClick(UseHint));
    }

    void UpdateAttemptsText()
    {
        attemptsText.text = "Attempts:" + (maxAttempts - attempts);
    }
    void Update()
    {
        // Handle the countdown timer
        if (isTimerRunning && !gameEnded)
        {
            timeLeft -= Time.deltaTime;
            currentCompletionTime += Time.deltaTime; // Track the current time for this game

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                GameOver();
            }

            UpdateTimerText();
        }
    }

    // Function to handle letter button clicks
    void SelectLetter(Button clickedButton)
    {
        if (currentBlank < blankBoxes.Length)
        {
            // Get the letter from the button text and place it in the next blank box
            blankBoxes[currentBlank].text = clickedButton.GetComponentInChildren<Text>().text;
            clickedButton.interactable = false;  // Disable the button once used
            currentBlank++;
        }
    }

    // Function to clear all the blanks and reset buttons
    void ClearBlanks()
    {
        currentBlank = 0;

        // Clear the text in all blank boxes
        foreach (Text blank in blankBoxes)
        {
            blank.text = "";
        }

        // Re-enable all letter buttons, except the ones that were used as hints
        for (int i = 0; i < letterButtons.Length; i++)
        {
            if (!usedHints.Contains(i))  // Only re-enable buttons not used as hints
            {
                letterButtons[i].interactable = true;
            }
        }

        // Clear the feedback text
        feedbackText.text = "";

        // Reset the timer
        timeLeft = 10f;
        isTimerRunning = false;
        gameEnded = false;  // Reset gameEnded to allow playing again
        UpdateTimerText();
    }

    // Function to check if the player has filled the blanks with the correct word
    void CheckAnswer()
    {
        if (gameEnded)
        {
            return;  // If the game is over, don't check the answer
        }

        // Check if any blank box is still empty
        foreach (Text blank in blankBoxes)
        {
            if (string.IsNullOrEmpty(blank.text))
            {
                feedbackText.text = "Please fill all the slots!";
                feedbackText.color = Color.red;  // Show error message in red
                return;  // Exit the function so it doesn't check the answer
            }
        }
        string playerWord = "";

        // Concatenate the letters from all the blanks
        foreach (Text blank in blankBoxes)
        {
            playerWord += blank.text;
        }

        // Check if the player's word matches the correct word
        if (playerWord == correctWord)
        {
            isAnswered = true;
          

            // Award 10 points for a correct answer, but only once
            if (isTimerRunning)  // To ensure the score is added only the first time
            {
                AddScore(10);  // Award 10 points for the correct answer
                isTimerRunning = false;  // Stop the timer since the user has guessed the correct word

                // Check if the current completion time is the best time
                if (bestTime == 0 || currentCompletionTime < bestTime)
                {
                    bestTime = currentCompletionTime;
                }

                // Display completion details
                ShowCompletionPanel();

                // Stop further interactions
                DisableAllInteractions();
            }
        }
        else
        {
            feedbackText.text = "Wrong Answer, Try Again!";
            feedbackText.color = Color.red;  // Change color to red for incorrect feedback
            attempts++;  // Increment attempts on wrong answer
            UpdateAttemptsText();  // Update the attempts UI

            // If maximum attempts are reached, trigger Game Over
            if (attempts >= maxAttempts)
            {
                GameOver();
            }
        }
    }
    public void IncrementHints()
    {
        hintsLeft++; // Simply increase remaining hints with no max limit
        UpdateHintCounter();
    }


    // Function to handle the game over state when the timer runs out
    void GameOver()
    {
        isTimerRunning = false;
        gameEnded = true;  // Mark the game as ended
       

        // Disable all letter buttons
        foreach (Button button in letterButtons)
        {
            button.interactable = false;
        }

        // Disable the enter button
        enterButton.interactable = false;

        // Disable the hint button
        hintButton.interactable = false;

        // Show the Game Over Panel
        gameOverPanel.SetActive(true);
    }

    // Rest of the code remains unchanged...


    // Function to show the game completion details
    void ShowCompletionPanel()
    {
        gameCompletePanel.SetActive(true);
        completionScoreText.text = "Score: " + score;
        completionBestTimeText.text = "Best Time: " + bestTime.ToString("F2");
        completionCurrentTimeText.text = "Current Time: " + currentCompletionTime.ToString("F2");
    }

    // Function to use a hint (removes a wrong letter)
    void UseHint()
    {
        if (gameEnded)
        {
            return;  // If the game is over, don't allow hints
        }

        // Check if there are hints left
        if (hintsLeft > 0)
        {
            // Find a wrong letter that hasn't been used as a hint
            for (int i = 0; i < letterButtons.Length; i++)
            {
                Button button = letterButtons[i];
                string buttonLetter = button.GetComponentInChildren<Text>().text;

                // If the letter is wrong and the button is still active
                if (!correctWord.Contains(buttonLetter) && button.interactable && !usedHints.Contains(i))
                {
                    button.interactable = false;  // Disable the wrong letter button
                    usedHints.Add(i);  // Track that this letter was removed as a hint
                    hintsLeft--;  // Decrease the number of hints left
                    UpdateHintCounter();  // Update the hint counter UI
                    return;  // Exit after removing one letter
                }
            }
        }
        else
        {
            feedbackText.text = "No hints left!";
            feedbackText.color = Color.red;
        }
    }
    public void SetScore(int newScore)
    {
        score = newScore;
        UpdateScore();
    }
    public int GetScore()
    {
        return score;
    }
    // Function to update the hint counter display
    void UpdateHintCounter()
    {
        hintCounterText.text = "Hints:" + hintsLeft;
    }

    // Function to add points to the score
    void AddScore(int points)
    {
        score += points;
        UpdateScore();  // Update the score UI
    }

    // Function to update the score display
    void UpdateScore()
    {
        scoreText.text = "Score:" + score;
    }

    // Function to update the timer display
    void UpdateTimerText()
    {
        timerText.text = "Time:" + Mathf.Ceil(timeLeft) + "s";
    }

    // Function to disable all interactions after game ends
    void DisableAllInteractions()
    {
        foreach (Button button in letterButtons)
        {
            button.interactable = false;
        }

        // Disable the enter button
        enterButton.interactable = false;

        // Disable the hint button
        hintButton.interactable = false;

        // Disable the clear button
        clearButton.interactable = false;
    }

    // Wrapper to start the timer when any button is clicked
    void OnAnyButtonClick(System.Action action)
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;  // Start the timer if it's not already running
        }
        action.Invoke();  // Execute the original button action
    }

    public void LoadGameData(GameData gamedata)
    {

        if (game_index == 3)
        {
            var sign = gamedata.SignWord;

            this.score = sign.score;
            this.bestTime = sign.levels[currentlevel - 1].bestTime;
            this.isScored = sign.levels[currentlevel - 1].isScored;
            this.isNextlevelUnlocked = sign.levels[currentlevel].isUnlocked;
        }
    }

    public void SaveGameData(ref GameData gameData)
    {
        if (game_index == 3)
        {
            var s = gameData.SignWord;

            s.score = this.score;
            s.levels[currentlevel - 1].bestTime = this.bestTime;
            s.levels[currentlevel - 1].isScored = this.isScored;

            if (!isNextlevelUnlocked)
            {
                s.levels[currentlevel].isUnlocked = true;

                if (s.unlockedlevels < currentlevel + 1 && isAnswered)
                {
                    s.unlockedlevels = currentlevel + 1;
                }


            }
        }
    }
}
