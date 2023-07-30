using UnityEngine;
using UnityEngine.Events;

// Gameplay Base
public abstract class GPBase : MonoBehaviour
{
	public UnityEvent onStartGame = new UnityEvent();
	public UnityEvent onEndGame = new UnityEvent();
	public bool IsInGameplay { get; private set; }

	public virtual void OnInitializationGame() { }

	public virtual void OnStartGame()
	{
		IsInGameplay = true;
		onStartGame.Invoke();
	}

	public virtual void OnEndGame()
	{
		IsInGameplay = false;
		onEndGame.Invoke();
	}
	public virtual void CheckGameLogic() { }
}
