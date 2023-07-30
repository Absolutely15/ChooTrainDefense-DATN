using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class StarDisplay : MonoBehaviour
{
    [SerializeField] private Image[] star;
    [SerializeField] private Sprite[] img;
    
    [Button]
    public void GetActiveStar(int amount)
    {
        for (var i = 0; i < star.Length; i++)
        {
            star[i].sprite = i < amount ? img[0] : img[1];
        }
    }
}
