using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ColorChanger : MonoBehaviour
{
    public UnityEvent<Color> colorChangerEvent;
    
    [SerializeField] private Texture2D paletteTexture;
    [SerializeField] private RectTransform palette;

    [SerializeField] private RectTransform cursor;
    [SerializeField] private Image buttonColor;
    [SerializeField] private Image selectedColor;
    
    private Color _pickedColor;
    private float _paletteHalfWidth, _paletteHalfHeight;
    

    private void Start()
    {
        _paletteHalfWidth = palette.rect.width / 2f;
        _paletteHalfHeight = palette.rect.height / 2f;
        LoadColor();
    }
    
    public void PickColor()
    {
        SaveColor(_pickedColor);
        colorChangerEvent.Invoke(_pickedColor);
    }

    public void ShowPrePickedColor(BaseEventData data)
    {
        PointerEventData pointer = data as PointerEventData;
        
        // Move cursor only within palette
        cursor.position = new Vector2(
            Mathf.Clamp(pointer.position.x,  palette.position.x - _paletteHalfWidth,palette.position.x + _paletteHalfWidth),
            Mathf.Clamp(pointer.position.y, palette.position.y - _paletteHalfHeight, palette.position.y + _paletteHalfHeight));
        
        Vector2 cursorRealPosition = new Vector2(
            _paletteHalfWidth + cursor.localPosition.x,
            _paletteHalfHeight + cursor.localPosition.y);
        
        _pickedColor = paletteTexture.GetPixel(
            (int)(cursorRealPosition.x * (paletteTexture.width / palette.rect.width)),
            (int)(cursorRealPosition.y * (paletteTexture.height / palette.rect.height)));
        
        selectedColor.color = _pickedColor;
    }
    
    void SaveColor(Color color)
    {
        PlayerPrefs.SetFloat("PlayerColorR", color.r);
        PlayerPrefs.SetFloat("PlayerColorG", color.g);
        PlayerPrefs.SetFloat("PlayerColorB", color.b);
        PlayerPrefs.SetFloat("PlayerColorA", color.a);
        PlayerPrefs.Save();
        buttonColor.color = color;
    }
    
    void LoadColor()
    {
        if (PlayerPrefs.HasKey("PlayerColorR"))
        {
            float r = PlayerPrefs.GetFloat("PlayerColorR");
            float g = PlayerPrefs.GetFloat("PlayerColorG");
            float b = PlayerPrefs.GetFloat("PlayerColorB");
            float a = PlayerPrefs.GetFloat("PlayerColorA");
            Color savedColor = new Color(r, g, b, a);
            _pickedColor = savedColor;
            buttonColor.color = _pickedColor;
            colorChangerEvent.Invoke(savedColor);
        }
    }
}
