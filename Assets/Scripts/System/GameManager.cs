using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public static Action newDay;
    public static Action enablePlayerMovement;
    public static Action disablePlayerMovement;
    
    public GameObject _holdableItemPrefab;

    private void OnEnable()
    {
        InputManager.cancelStarted += CloseGame;
    }

    public IEnumerator NewDaySequence()
    {
        InputManager.instance.DisableGeneralInputs();
        disablePlayerMovement?.Invoke();

        var nightFade = UIManager.instance._nightFade;
        nightFade.enabled = true;
        
        var duration = 1.0f;
        var elapsedTime = 0f;

        Color startAlpha = nightFade.color;
        Color endAlpha = new Color(startAlpha.r, startAlpha.g, startAlpha.b, 0f);
        
        newDay?.Invoke();
        
        while (elapsedTime < duration)
        { 
            float normalizedTime = elapsedTime / duration; 
            nightFade.color = Color.Lerp(startAlpha, endAlpha, normalizedTime); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        nightFade.color = endAlpha;
        nightFade.enabled = false;
        nightFade.color = startAlpha;
        
        InputManager.instance.EnableGeneralInputs();
        enablePlayerMovement?.Invoke();
    }

    private void CloseGame() => Application.Quit();
}
