using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[DefaultExecutionOrder(-6)]
public class GPExecutor : Singleton<GPExecutor>
{
    #region Properties
    public GPRainbowDefense gameplay;
    public UnityEvent onInitializationDone = new UnityEvent();
    public UnityEvent onPauseGame = new UnityEvent();
    public UnityEvent onResumeGame = new UnityEvent();
    public bool IsPause { private set; get; }
    #endregion

    #region Unity Functions
    private void Start()
    {
        gameplay.OnInitializationGame();
        onInitializationDone.Invoke();
    }
    private void Update()
    {
        gameplay.CheckGameLogic();
    }
    #endregion

    #region Basic Functions
    public void StartGame()
    {
        gameplay.OnStartGame();
    }
    
    [Button]
    public void Replay()
    {
        Transitions.Instance.FadeAllIn(ReloadScene);
    }

    public static void ReloadScene()
    {
        Observer.Instance.RemoveAllObserver();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
    
    #region Pause / Resume Game
    public void PauseGame()
    {
        IsPause = true;
        Time.timeScale = 0;
        onPauseGame.Invoke();
    }
    public void ResumeGame()
    {
        IsPause = false;
        Time.timeScale = 1;
        onResumeGame.Invoke();
    }
    #endregion
}
