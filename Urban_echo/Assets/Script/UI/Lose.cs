using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lose : CanvasUI
{
    public Text currentLV;
    public Text yourCoins;
    private void OnEnable()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        currentLV.text = nextSceneIndex.ToString();
        yourCoins.text = CoinManager.Instance.GetCoins().ToString();
    }

    /// <summary>
    /// Tải lại scene hiện tại.
    /// </summary>
    /// 



    public void ReloadCurrentScene()
    {
        if(CoinManager.Instance.GetCoins() > 100)
        {
            // Lấy scene hiện tại
            Time.timeScale = 1;
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            // Tải lại scene
            UIManager.Instance.CloseUI<Lose>(0.3f);
            SceneManager.LoadScene(currentSceneIndex);
            CoinManager.Instance.RemoveCoins(100);
            
        }
        else
        {
            return;
        }
       
    }

    /// <summary>
    /// Tải scene 1.
    /// </summary>
    public void LoadScene1()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0); // Load scene có index 1
        UIManager.Instance.CloseUI<Lose>(0.3f);
        UIManager.Instance.CloseUI<GamePlayCanvas>(0.2f);
        UIManager.Instance.OpenUI<StartCanvas>();
    }


}
