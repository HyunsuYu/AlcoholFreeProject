using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;
using UnityEngine.SceneManagement;


public sealed class ChiefMemoriesManager : SingleTonForGameObject<ChiefMemoriesManager>
{
    [SerializeField] private RectTransform m_rectTransform_Contents;

    [SerializeField] private MemoriesContentPresenter m_storyMemoriesControl;
    [SerializeField] private MemoriesContentPresenter m_cutSceneMemoriesControl;
    [SerializeField] private MemoriesContentPresenter m_miniGameMemoriesControl;

    private static ChiefSettingManager.FromPageType m_fromPageType = ChiefSettingManager.FromPageType.Title;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        RenderAll();

        GameObject bgm = GameObject.Find("BGM");
        if (bgm != null)
        {
            Destroy(bgm);
        }
    }

    internal MemoriesContentPresenter StoryMemoriesControl
    {
        get
        {
            return m_storyMemoriesControl;
        }
    }
    internal MemoriesContentPresenter CutSceneMemoriesControl
    {
        get
        {
            return m_cutSceneMemoriesControl;
        }
    }
    internal MemoriesContentPresenter MiniGameMemoriesControl
    {
        get
        {
            return m_miniGameMemoriesControl;
        }
    }

    internal void RenderAll()
    {
        m_storyMemoriesControl.Render();
        m_cutSceneMemoriesControl.Render();
        m_miniGameMemoriesControl.Render();
    }

    #region Unity Callbacks
    public void Back2Page()
    {
        SceneManager.LoadScene("Title");
    }
    #endregion

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}