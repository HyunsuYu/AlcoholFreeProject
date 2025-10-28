using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public sealed class CutSceneItem : MonoBehaviour
{
    [SerializeField] private Image m_image_CutSceneImage;
    [SerializeField] private TMP_Text m_text_CutSceneName;

    private int m_cutSceneIndex;


    public void SelectThis()
    {
        RightToolBarControl.Instance.TrySetCutScene(m_cutSceneIndex);
    }

    internal void Render(in int targetCutSceneIndex)
    {
        m_cutSceneIndex = targetCutSceneIndex;

        m_image_CutSceneImage.sprite = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[m_cutSceneIndex].CurSceneSprite;
        m_text_CutSceneName.text = ResourceDescBuffer.Instance.CutSceneResourceDesc.CutSceneDatas[m_cutSceneIndex].CutSceneName;
    }
}