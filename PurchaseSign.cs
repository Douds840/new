using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PurchaseSign : MonoBehaviour, IDataPersistence
{
    [SerializeField] private TMP_Text DisplayScore;
    [SerializeField] private int hintcost;
    [SerializeField] private GameObject BuyHint;
    private Sign SignInstance;

    private void Start()
    {
        SignInstance = FindAnyObjectByType<Sign>();

        if (SignInstance != null)
        {
            int currentScore = SignInstance.GetScore();
            UpdateScore(currentScore);
        }
    }

    public void Back()
    {
        BuyHint.SetActive(false);
        Time.timeScale = 1; // Resume the game timer when exiting the hint purchase screen
    }

    public void Buy()
    {
        BuyHint.SetActive(true);
        Time.timeScale = 0; // Pause the game timer while purchasing a hint
    }

    private void UpdateScore(int score)
    {
        if (DisplayScore != null)
        {
            DisplayScore.text = "Score: " + score.ToString();
        }
    }

    public void HintPurchase()
    {
        int currentScore = SignInstance.GetScore();
        if (currentScore >= hintcost)
        {
            currentScore -= hintcost;
            SignInstance.SetScore(currentScore);

            // Increment hints in the mathcontrollers script
            SignInstance.IncrementHints();

            UpdateScore(currentScore);

            if (SignInstance.hintCounterText!= null)
            {
                SignInstance.hintCounterText.text = "Hints Left: " + SignInstance.hintsLeft;
            }
        }
        else
        {
            Debug.Log("Not enough score to purchase a hint.");
        }
    }

    public void LoadGameData(GameData gameData)
    {

    }

    public void SaveGameData(ref GameData gameData)
    {

    }
}
