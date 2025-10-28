using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public sealed class MemoriesContentItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text_ContentIndex;
    [SerializeField] private Image m_image_ContentImage;
    [SerializeField] private TMP_Text m_text_AdditionalInfo;
    [SerializeField] private TMP_Text m_text_CapturedDate;


    internal void Render(in string fileName, in int contentIndex)
    {
        m_text_ContentIndex.text = (contentIndex + 1).ToString();

        #region Content Image
        byte[] imageData = System.IO.File.ReadAllBytes(fileName);

        Texture2D texture = new Texture2D(1920, 1080);
        texture.LoadImage(imageData);
        texture.Apply();

        m_image_ContentImage.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        #endregion

        string[] pureFileNameTokens = System.IO.Path.GetFileNameWithoutExtension(fileName).Split("_");
        m_text_AdditionalInfo.text = pureFileNameTokens[0];
        m_text_CapturedDate.text = $"{pureFileNameTokens[1]}-{pureFileNameTokens[2]}-{pureFileNameTokens[3]}";
    }
}