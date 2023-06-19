using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            InputManager.advanceDay += AdvanceDay;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            InputManager.advanceDay -= AdvanceDay;
    }

    private void AdvanceDay()
    {
        StartCoroutine(GameManager.instance.NewDaySequence());
        AudioManager.instance.PlayDoorCloseSFX();
    }
}
