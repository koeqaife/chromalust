using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChangeTextColorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text buttonText;
    private Color originalColor;
    public Color newcolor;

    void Start()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        originalColor = buttonText.color;
        if (newcolor == null)
            newcolor = Color.white;
        else
            newcolor = new Color(newcolor.r, newcolor.g, newcolor.b, 255f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = newcolor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = originalColor;
    }
}
