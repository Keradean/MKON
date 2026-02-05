using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HoverEffectRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera effectCamera;               // Camera used to render the hover effect
    [SerializeField] Material hoverShaderMaterial;      // Shader material that receives the rendered mask
    [SerializeField] Canvas mainCanvas;                 // Main UI canvas where the RawImage is placed

    private Canvas effectCanvas;                        // World-space canvas used to render the TMP mask
    private TMP_Text effectTMP;                         // Copy of the target TMP text for rendering
    private RawImage effectImage;                       // UI element that displays the shader effect
    private RenderTexture renderTexture;                // Texture that holds the rendered TMP mask
    private bool update = false;

    public void OnHoverEnter(TMP_Text targetTMP)
    {
        RectTransform targetRT = targetTMP.rectTransform;
        Canvas targetCanvas = targetTMP.canvas;

        // 1. Calculate layout-corrected size of the target text
        float width = LayoutUtility.GetPreferredWidth(targetRT);
        float height = LayoutUtility.GetPreferredHeight(targetRT);
        Vector2 pixelSize = new Vector2(width, height);

        int texWidth = Mathf.CeilToInt(width);
        int texHeight = Mathf.CeilToInt(height);

        // 2. Create a world-space canvas for rendering the TMP mask
        if (effectCanvas == null)
        {
            GameObject canvasGO = new GameObject("HoverEffectCanvas");
            effectCanvas = canvasGO.AddComponent<Canvas>();
            effectCanvas.renderMode = RenderMode.WorldSpace;
            effectCanvas.worldCamera = effectCamera;
            canvasGO.layer = LayerMask.NameToLayer("TMPMask");

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = targetCanvas.referencePixelsPerUnit;
        }

        // 3. Create a copy of the TMP text inside the effect canvas
        if (effectTMP == null)
        {
            GameObject tmpGO = new GameObject("EffectTMP");
            tmpGO.transform.SetParent(effectCanvas.transform, false);
            tmpGO.layer = LayerMask.NameToLayer("TMPMask");
            effectTMP = tmpGO.AddComponent<TextMeshProUGUI>();
        }

        // Copy all relevant properties from the target TMP
        effectTMP.text = targetTMP.text;
        effectTMP.font = targetTMP.font;
        effectTMP.fontSize = targetTMP.fontSize;
        effectTMP.alignment = targetTMP.alignment;
        effectTMP.fontStyle = targetTMP.fontStyle;
        effectTMP.enableAutoSizing = targetTMP.enableAutoSizing;
        effectTMP.characterSpacing = targetTMP.characterSpacing;
        effectTMP.color = Color.white;
        effectTMP.enableWordWrapping = false;
        effectTMP.overflowMode = TextOverflowModes.Overflow;

        RectTransform effectRT = effectTMP.rectTransform;
        effectRT.sizeDelta = pixelSize;
        effectRT.localPosition = Vector3.zero;

        // 4. Position the effect canvas and camera centered over the target text
        Vector3 worldPos = targetRT.TransformPoint(targetRT.rect.center);
        effectCanvas.transform.position = worldPos;
        effectCamera.transform.position = worldPos + new Vector3(0, 0, -10f);

        // 5. Create a RawImage on the main canvas to display the shader effect
        if (effectImage == null)
        {
            GameObject imageGO = new GameObject("EffectImage");
            imageGO.transform.SetParent(mainCanvas.transform, false);
            effectImage = imageGO.AddComponent<RawImage>();
            effectImage.material = hoverShaderMaterial;
            effectImage.color = Color.white;
            effectImage.raycastTarget = false;
        }

        // Position and size the RawImage to match the target text
        RectTransform imageRT = effectImage.rectTransform;
        imageRT.anchorMin = imageRT.anchorMax = new Vector2(0.5f, 0.5f); // Centered anchor
        imageRT.pivot = new Vector2(0.5f, 0.5f);
        imageRT.sizeDelta = new Vector2(texWidth, texHeight);

        // Align the RawImage to the text's alignment point
        Vector3 worldAlignedPos = GetAlignedWorldPosition(targetTMP);
        imageRT.anchoredPosition = worldAlignedPos - mainCanvas.transform.position;

        // Offset by half the width to account for centered pivot
        Vector2 halfSize = imageRT.sizeDelta * 0.5f;
        float alignmentX = 0f;
        switch (targetTMP.alignment)
        {
            case TextAlignmentOptions.Center:
                alignmentX = 0f;
                break;
            case TextAlignmentOptions.Right:
                alignmentX = -1f;
                break;
            case TextAlignmentOptions.Left:
                alignmentX = 1;
                break;
            default:
                break;
        }
        imageRT.anchoredPosition += new Vector2(halfSize.x * alignmentX, 0f);

        imageRT.localScale = Vector3.one;
        imageRT.rotation = targetRT.rotation;

        // 6. Configure the RenderTexture and camera settings
        renderTexture = new RenderTexture(texWidth, texHeight, 16, RenderTextureFormat.ARGB32);
        effectCamera.targetTexture = renderTexture;
        effectCamera.orthographic = true;
        effectCamera.aspect = (float)texWidth / texHeight;
        effectCamera.orthographicSize = texHeight / 2f;
        effectCamera.cullingMask = LayerMask.GetMask("TMPMask");

        // Pass aspect ratio to shader for distortion correction
        hoverShaderMaterial.SetFloat("_AspectRatio", (float)texWidth / texHeight -2f);

        // Wait one frame before rendering the TMP mask
        StartCoroutine(RenderAfterFrame());
    }

    // Calculates the world position based on text alignment (Left, Center, Right)
    private Vector3 GetAlignedWorldPosition(TMP_Text tmp)
    {
        RectTransform rt = tmp.rectTransform;

        float alignmentX = tmp.alignment switch
        {
            TextAlignmentOptions.Left => 0f,
            TextAlignmentOptions.Center => 0.5f,
            TextAlignmentOptions.Right => 1f,
            _ => rt.pivot.x
        };

        Vector2 alignmentOffset = new Vector2(
            Mathf.Lerp(rt.rect.xMin, rt.rect.xMax, alignmentX),
            rt.rect.center.y
        );

        Vector2 offset = alignmentOffset - rt.rect.center;
        Vector3 worldOffset = rt.TransformVector(offset);

        return rt.TransformPoint(rt.rect.center) + worldOffset;
    }

    // Renders the TMP mask after layout is finalized
    private IEnumerator RenderAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        effectTMP.ForceMeshUpdate();
        effectCamera.enabled = true;
        effectCamera.Render();
        effectCamera.enabled = false;
        update = true;
        // Assign the rendered texture to the shader
        hoverShaderMaterial.SetTexture("_TMPMaskTexture", renderTexture);

        // Show the effect image
        effectImage.enabled = true;
    }

    // Cleans up the effect when hover ends
    public void OnHoverExit()
    {
        update = false;
        if (effectImage != null)
            effectImage.enabled = false;

        if (effectTMP != null)
            effectTMP.text = "";

        if (effectCamera != null)
        {
            effectCamera.targetTexture = null;
            effectCamera.enabled = false;
        }

        if (hoverShaderMaterial != null)
            hoverShaderMaterial.SetTexture("_TMPMaskTexture", null);
        hoverShaderMaterial.SetFloat("_AspectRatio", 0f);
    }
    private void Update()
    {
        if (!update) return;
        hoverShaderMaterial.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}