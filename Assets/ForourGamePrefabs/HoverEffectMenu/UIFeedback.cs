using TMPro;
using UnityEngine;

public class UIFeedback : MonoBehaviour
{
    [SerializeField] HoverEffectRenderer HoverEffectRenderer;
    public void OnClickFeedback()
    {
        //MenueManager.instance.PlayClickSound();
    }

    public void OnPointerEnterFeedback(TMP_Text targetTMP)
    {
        HoverEffectRenderer.OnHoverEnter(targetTMP);
    }

    public void OnPointerExitFeedback()
    {
        HoverEffectRenderer.OnHoverExit();
    }
}