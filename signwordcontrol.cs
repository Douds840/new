using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SIGNWORLDCONTROL : MonoBehaviour
{
    public Text[] blankBoxes;   // The empty boxes where letters are filled
    public Button[] letterButtons; // All the letter buttons
    public Button clearButton;  // Button to clear the filled letters
    public Button enterButton;  // Button to submit the answer
    public Button hintButton;   // Button to use hints
    public Text hintCounterText;  // Text to display how many hints are left
    public Text feedbackText;   // Text to show feedback (Correct/Incorrect or Empty Slot)
    public Text scoreText;      // Text to display the score
    public Text timerText;      // Text to display the timer
    public string correctWord = "USA";  // The correct answer
    public int maxHints = 3;    // Maximum number of hints allowed
    private int currentBlank = 0;  // Index to keep track of which blank to fill
    private int hintsLeft;      // Tracks the number of remaining hints
    private int score = 0;      // Variable to track the player's score
    private List<int> usedHints = new List<int>();  // Keeps track of which letters have already been removed

    private float timeLeft = 20f;  // 20-second timer
    private bool isTimerRunning = false;  // Flag to check if the timer is running
    private bool gameEnded = false;  // To stop the game when time runs out

    void Start()
    {
        hintsLeft = maxHints;  // Initialize the hints left
        UpdateHintCounter();   // Display the initial hint count
        UpdateScore();         // Display the initial score
        UpdateTimerText();     // Display the initial timer

        // Add listeners to letter buttons
        foreach (Button button in letterButtons)
        {
            button.onClick.AddListener(() => SelectLetter(button));
        }

        // Add listener to the clear button
        clearButton.onClick.AddListener(ClearBlanks);

        // Add listener to the enter button to check the answer
        enterButton.onClick.AddListener(CheckAnswer);

        // Add listener to the hint button to use a hint
        hintButton.onClick.AddListener(UseHint);
    }

    void Update()
    {
        // Handle the countdown timer
        if (isTimerRunning && !gameEnded)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                GameOver();
            }

            UpdateTimerText();
        }
    }

    // Function to handle letter button clicks and start the timer
    void SelectLetter(Button clickedButton)
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;  // Start the timer when the first letter is selected
        }

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

        // Re-enable all letter buttons
        foreach (Button button in letterButtons)
        {
            button.interactable = true;
        }

        // Clear the feedback text
        feedbackText.text = "";

        // Reset the timer
        timeLeft = 20f;
        isTimerRunning = false;
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
            feedbackText.text = "Correct! You've guessed the word!";
            feedbackText.color = Color.green;  // Change color to green for correct feedback

            // Award 10 points for a correct answer, but only once
            if (isTimerRunning)  // To ensure the score is added only the first time
            {
                AddScore(10);  // Award 10 points for the correct answer
                isTimerRunning = false;  // Stop the timer since the user has guessed the correct word
            }
        }
        else
        {
            feedbackText.text = "Wrong Answer, Try Again!";
            feedbackText.color = Color.red;  // Change color to red for incorrect feedback
        }
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

    // Function to update the hint counter display
    void UpdateHintCounter()
    {
        hintCounterText.text = "Hints Left: " + hintsLeft;
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
        scoreText.text = "Score: " + score;
    }

    // Function to update the timer display
    void UpdateTimerText()
    {
        timerText.text = "Time Left: " + Mathf.Ceil(timeLeft) + "s";
    }

    // Function to handle the game over state when the timer runs out
    void GameOver()
    {
        isTimerRunning = false;
        gameEnded = true;  // Mark the game as ended
        feedbackText.text = "Time's up! Game over!";
        feedbackText.color = Color.red;

        // Disable all letter buttons
        foreach (Button button in letterButtons)
        {
            button.interactable = false;
        }

        // Disable the enter button
        enterButton.interactable = false;

        // Disable the hint button
        hintButton.interactable = false;
    }
}
