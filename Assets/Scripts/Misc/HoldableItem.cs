using System;
using UnityEngine;

public class HoldableItem : MonoBehaviour
{
    public Item itemData;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        if (itemData == null)
            Destroy(gameObject);
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (itemData != null)
            _spriteRenderer.sprite = itemData.itemIcon;
    }
}
