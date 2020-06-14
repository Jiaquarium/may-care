using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ProximityFader : MonoBehaviour
{
    public float maxDistance;
    public float minAlpha;
    // this determines the transparency of all sprite renderers
    public SpriteRenderer target;
    public SpriteRenderer[] spriteRenderers;
    
    [SerializeField]
    private float distance;
    [SerializeField]

    void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }    
    }
    
    void Update()
    {
        AdjustFade();
    }

    void AdjustFade()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);
        Color tmpColor = target.color;
        
        if (distance >= maxDistance)
        {
            tmpColor.a = minAlpha;
            SetColor(tmpColor);
            return;
        }

        tmpColor.a = ((1 - (distance / maxDistance)) * (1f - minAlpha)) + minAlpha;
        SetColor(tmpColor);
    }

    void SetColor(Color tmpColor)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = tmpColor;
        }
    }

    public void Setup(
        SpriteRenderer _targetSpriteRenderer,
        SpriteRenderer[] _spriteRenderers
    )
    {
        target = _targetSpriteRenderer;
        spriteRenderers = _spriteRenderers;
    }
}
