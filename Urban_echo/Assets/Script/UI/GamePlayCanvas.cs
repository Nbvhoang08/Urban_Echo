using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GamePlayCanvas : CanvasUI
{
    [Header("References")]
    public GameManager gameManager;
    public Slider progressBar;
    public Text coins;
    public Text _currentLV;
    public Text _nextLV;
   
    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }


    private void Start()
    {
        
        UpdateProgressBar(0);
    }

    private void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
        if (coins != null)
        {
            coins.text = CoinManager.Instance.GetCoins().ToString();
        }
        if (_currentLV != null)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex +1;
            _currentLV.text = currentSceneIndex.ToString();
        }
        if (_nextLV != null)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex + 2;
            _nextLV.text = currentSceneIndex.ToString();
        }
        // Cập nhật progress bar dựa trên tiến trình xa nhất
        UpdateProgressBar(gameManager.maxProgress);
    }

    private void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress; // Gán giá trị cho Slider (0-1)
        }
    }
}
