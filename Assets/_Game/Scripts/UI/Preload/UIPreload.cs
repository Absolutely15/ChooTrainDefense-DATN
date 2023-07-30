using System;
using UnityEngine;

public class UIPreload : MonoBehaviour
{
    [SerializeField] private RectTransform enemy;
    [SerializeField] private Animator anim;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;

    private bool reachDes;
    private static readonly int Attack = Animator.StringToHash("Attack");

    public void ChaseAnim(float progress)
    {
        enemy.anchoredPosition = Vector2.Lerp(startPos, endPos, progress*3);
    }

    private void Update()
    {
        if (enemy.anchoredPosition != endPos || reachDes) return;
        reachDes = true;
        anim.SetBool(Attack, true);
    }
}
