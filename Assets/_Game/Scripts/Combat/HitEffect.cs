using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [Title("VFX")]
    [SerializeField] private List<Renderer> preRends;
    [SerializeField] private GameObject sufferDamageGO;
    [SerializeField] private float duration;
    [Range(0,1)] [SerializeField] private float intensity;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int Lumin = Shader.PropertyToID("_Lumin");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        if (sufferDamageGO == null) return;
        var t = sufferDamageGO.GetComponent<Health>();
        if (t == null) return;
        t.onSufferDamage.AddListener((a, b) => Play());
    }


    [Button]
    public void Play() => Play(preRends);
    private void Play(List<Renderer> renderers)
    {
        foreach (var t in renderers)
        {
            PlaySingle(t);
        }
    }
    private void PlaySingle(Renderer rend)
    {
        if (rend.gameObject.activeInHierarchy)
        {
            DOVirtual.Float(intensity, 0, duration, t =>
            {
                rend.GetPropertyBlock(propertyBlock);

                propertyBlock.SetFloat(Lumin, t);

                rend.SetPropertyBlock(propertyBlock);

            }).SetEase(Ease.OutQuad);
        }
    }
}
