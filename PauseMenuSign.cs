using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuSign : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
   
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
       
    }
    public void Home()
    {
        SceneManager.LoadScene("mathlevels");
        Time.timeScale = 1;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale=1;
    }
}
