using UnityEngine;

public class FarmTile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _cursor;

    public void ShowCursor() => _cursor.SetActive(true);
    public void HideCursor() => _cursor.SetActive(false);
}
