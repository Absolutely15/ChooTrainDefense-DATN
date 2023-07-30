using DG.Tweening;
using TMPro;
using UnityEngine;

public class UITextPopup : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public Vector3 initLocalPosition;
    public float slideUp;
    public float duration;

    private void OnEnable()
    {
        transform.localPosition = initLocalPosition;
        transform.DOLocalMoveY( initLocalPosition.y + slideUp, duration).SetEase(Ease.OutSine).OnComplete(SelfKill);
        tmp.alpha = 1;
        tmp.DOFade(0, duration).SetEase(Ease.InCirc);
    }

    private void SelfKill()
    {
        gameObject.SetActive(false);
    }
}
