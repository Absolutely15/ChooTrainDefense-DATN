using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIInAppPurchase : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform scrollView;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button contentStaterPackBtn;
        
    private float lastVerticalPos = 1;
    private float curVerticalPos;
    private void OnEnable()
    {
        GPExecutor.Instance.PauseGame();
        DOVirtual.Float(1, lastVerticalPos, 0.3f, t => scrollRect.verticalNormalizedPosition = t).SetUpdate(true);
        bg.DOFade(1, 0.25f).SetUpdate(true);
        scrollView.DOLocalMoveY(-100, 0.2f).SetUpdate(true);
    }

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            if (!UIManager.Instance.inventoryPanel.gameObject.activeInHierarchy)
                GPExecutor.Instance.ResumeGame();
            
            OnClose();
        });
        
        contentStaterPackBtn.onClick.AddListener(() => UIManager.Instance.starterPackPanel.gameObject.SetActive(true));
        scrollRect.onValueChanged.AddListener(t => curVerticalPos = t.y);
    }

    private void OnClose()
    {
        lastVerticalPos = scrollRect.verticalNormalizedPosition;
        var closeSeq = DOTween.Sequence().SetUpdate(true);
        closeSeq.AppendCallback(() => DOVirtual.Float(scrollRect.verticalNormalizedPosition, 1, 0.25f, t => scrollRect.verticalNormalizedPosition = t))
            .Append(scrollView.DOLocalMoveY(-2500, 0.25f))
            .Append(bg.DOFade(0, 0.25f))
            .AppendCallback(() => gameObject.SetActive(false));
    }

    public bool GemsPack()
    {
        if (gameObject.activeInHierarchy)
        {
            DOVirtual.Float(curVerticalPos, 0, 0.5f, t => scrollRect.verticalNormalizedPosition = t).SetUpdate(true);
            return true;
        }
        
        lastVerticalPos = 0;
        return false;
    }
}
