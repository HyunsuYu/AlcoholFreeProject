using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MiniGameItemResourceDesc.RecipeData;


public sealed class GlassItem : IBottle
{
    public enum GlassType
    {
        AbsintheGlass = InventoryControl.ItemType.AbsintheGlass,
        OnTheRocksGlass = InventoryControl.ItemType.OnTheRocksGlass,
        CocktailGlass = InventoryControl.ItemType.CocktailGlass,
        WineGlass = InventoryControl.ItemType.WineGlass,
        HighballGlass = InventoryControl.ItemType.HighballGlass,
        OldFashionedGlass = InventoryControl.ItemType.OldFashionedGlass
    }
    public enum IceType
    {
        Ice = InventoryControl.ItemType.Ice,
        BarIce = InventoryControl.ItemType.BarIce,
        SphereIce = InventoryControl.ItemType.SphereIce,
        CubeIce = InventoryControl.ItemType.CubeIce
    }

    [Serializable] public struct GlassMatData
    {
        public float LiqureTopYPos;
        public float LiqureBottomYPos;

        public float buoyancyTopValue;
        public float buoyancyBottomValue;
    }
    [Serializable] public struct GarnishData
    {
        public GameObject[] Garnishs;
    }


    [Header("Glass Item Data")]
    [SerializeField] private float[] m_uiYPosPreset;
    [SerializeField] private Transform m_transform_UILayout;

    [Header("Glass Material Data")]
    [SerializeField] private GlassMatData[] m_glassMatData;

    [SerializeField] private Vector2 m_waveSpeedRange;
    [SerializeField] private Vector2 m_waveAmplitudeRange;
    [SerializeField] private Vector2 m_waveFrequencyRange;

    private GlassType m_curGlassType;
    private SpriteRenderer m_spriteRenderer;

    private Material m_waveMaterial;

    [Header("Wave Data")]
    [SerializeField] private float m_maxWaveSpeed = 15.0f;
    [SerializeField] private float m_maxWaveAmplitude = 0.1f;
    [SerializeField] private float m_maxWaveFrequency = 0.5f;

    [SerializeField] private float m_waveChangeRate = 0.1f;
    [SerializeField] private float m_waveAddScale = 0.2f;

    private float m_curWaveSpeed;
    private float m_curWaveAmplitude;
    private float m_curWaveFrequency;

    [Header("BuoyancyEffector2D")]
    [SerializeField] private BuoyancyEffector2D m_buoyancyEffector;
    [SerializeField] private GameObject[] m_ices;
    [SerializeField] private GameObject[] m_boundaries;
    [SerializeField] private GarnishData[] m_garnishData;
    [SerializeField] private Animator[] m_spoonAnimators;


    public void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_waveMaterial = m_spriteRenderer.material;
    }
    public void FixedUpdate()
    {
        m_waveMaterial.SetFloat("_WaveSpeed", m_curWaveSpeed);
        m_waveMaterial.SetFloat("_WaveAmplitude", m_curWaveAmplitude);
        m_waveMaterial.SetFloat("_WaveFrequency", m_curWaveFrequency);

        float changeRate = 1.0f - Time.fixedDeltaTime * m_waveChangeRate;
        m_curWaveSpeed *= changeRate;
        m_curWaveAmplitude *= changeRate;
        m_curWaveFrequency *= changeRate;
    }

    internal GlassType CurGlassType
    {
        get
        {
            return m_curGlassType;
        }
    }

    internal void Init(in GlassType targetGlassType)
    {
        m_curGlassType = targetGlassType;
        switch (m_curGlassType)
        {
            case GlassType.OldFashionedGlass:
                m_boundaries[1].SetActive(true);
                break;

            default:
                m_boundaries[(int)(m_curGlassType - GlassType.AbsintheGlass)].SetActive(true);
                break;
        }

        Render();
    }
    internal override void Render()
    {
        m_spriteRenderer.sprite = MiniGameItemResourceInterface.Instance.GetItemTitleSprite((InventoryControl.ItemType)m_curGlassType);

        Sprite[] matSprites = MiniGameItemResourceInterface.Instance.GetItemSprites((InventoryControl.ItemType)m_curGlassType);
        switch (m_curGlassType)
        {
            case GlassType.AbsintheGlass:
                m_transform_UILayout.localPosition = new Vector3(m_transform_UILayout.localPosition.x, m_uiYPosPreset[0], m_transform_UILayout.localPosition.z);
                m_waveMaterial.SetFloat("_LiqureTopYPos", m_glassMatData[0].LiqureTopYPos);
                m_waveMaterial.SetFloat("_LiqureBottomYPos", m_glassMatData[0].LiqureBottomYPos);
                m_waveMaterial.SetTexture("_MainTex", matSprites[0].texture);
                m_waveMaterial.SetTexture("_MaskTex", matSprites[1].texture);
                break;

            case GlassType.OnTheRocksGlass:
                m_transform_UILayout.localPosition = new Vector3(m_transform_UILayout.localPosition.x, m_uiYPosPreset[1], m_transform_UILayout.localPosition.z);
                m_waveMaterial.SetFloat("_LiqureTopYPos", m_glassMatData[1].LiqureTopYPos);
                m_waveMaterial.SetFloat("_LiqureBottomYPos", m_glassMatData[1].LiqureBottomYPos);
                m_waveMaterial.SetTexture("_MainTex", matSprites[0].texture);
                m_waveMaterial.SetTexture("_MaskTex", matSprites[1].texture);
                break;

            case GlassType.CocktailGlass:
                m_transform_UILayout.localPosition = new Vector3(m_transform_UILayout.localPosition.x, m_uiYPosPreset[2], m_transform_UILayout.localPosition.z);
                m_waveMaterial.SetFloat("_LiqureTopYPos", m_glassMatData[2].LiqureTopYPos);
                m_waveMaterial.SetFloat("_LiqureBottomYPos", m_glassMatData[2].LiqureBottomYPos);
                m_waveMaterial.SetTexture("_MainTex", matSprites[0].texture);
                m_waveMaterial.SetTexture("_MaskTex", matSprites[1].texture);
                break;

            case GlassType.WineGlass:
                m_transform_UILayout.localPosition = new Vector3(m_transform_UILayout.localPosition.x, m_uiYPosPreset[3], m_transform_UILayout.localPosition.z);
                m_waveMaterial.SetFloat("_LiqureTopYPos", m_glassMatData[3].LiqureTopYPos);
                m_waveMaterial.SetFloat("_LiqureBottomYPos", m_glassMatData[3].LiqureBottomYPos);
                m_waveMaterial.SetTexture("_MainTex", matSprites[0].texture);
                m_waveMaterial.SetTexture("_MaskTex", matSprites[1].texture);
                break;

            case GlassType.HighballGlass:
                m_transform_UILayout.localPosition = new Vector3(m_transform_UILayout.localPosition.x, m_uiYPosPreset[4], m_transform_UILayout.localPosition.z);
                m_waveMaterial.SetFloat("_LiqureTopYPos", m_glassMatData[4].LiqureTopYPos);
                m_waveMaterial.SetFloat("_LiqureBottomYPos", m_glassMatData[4].LiqureBottomYPos);
                m_waveMaterial.SetTexture("_MainTex", matSprites[0].texture);
                m_waveMaterial.SetTexture("_MaskTex", matSprites[1].texture);
                break;

            case GlassType.OldFashionedGlass:
                m_transform_UILayout.localPosition = new Vector3(m_transform_UILayout.localPosition.x, m_uiYPosPreset[5], m_transform_UILayout.localPosition.z);
                m_waveMaterial.SetFloat("_LiqureTopYPos", m_glassMatData[5].LiqureTopYPos);
                m_waveMaterial.SetFloat("_LiqureBottomYPos", m_glassMatData[5].LiqureBottomYPos);
                m_waveMaterial.SetTexture("_MainTex", matSprites[0].texture);
                m_waveMaterial.SetTexture("_MaskTex", matSprites[1].texture);
                break;
        }

        int fillmentCount = GetFillmentCount();
        MiniGameItemResourceDesc.RecipeData recipeData = MiniGameItemResourceInterface.Instance.GetRecipeData(ChiefMiniGameManager.CurRecipe);

        InventoryControl.ItemType curIngredient = InventoryControl.ItemType.None;

        List<InventoryControl.ItemType> addedIngredientOrder = new List<InventoryControl.ItemType>();
        List<int> addedIngredientCounts = new List<int>();
        for(int index = m_invisibleIngredientIndex; index < m_addedIngredients.Count; index++)
        {
            InventoryControl.ItemType ingredient = m_addedIngredients[index];
            if (ingredient != curIngredient)
            {
                addedIngredientOrder.Add(ingredient);
                addedIngredientCounts.Add(1);
                curIngredient = ingredient;
            }
            else
            {
                addedIngredientCounts[addedIngredientCounts.Count - 1]++;
            }
        }
        //foreach (var ingredient in m_addedIngredients)
        //{
        //    if (ingredient != curIngredient)
        //    {
        //        addedIngredientOrder.Add(ingredient);
        //        addedIngredientCounts.Add(1);
        //        curIngredient = ingredient;
        //    }
        //    else
        //    {
        //        addedIngredientCounts[addedIngredientCounts.Count - 1]++;
        //    }
        //}

        curIngredient = InventoryControl.ItemType.None;
        List<InventoryControl.ItemType> recipeIngredientOrder = new List<InventoryControl.ItemType>();
        List<int> recipeIngredientCounts = new List<int>();
        foreach (var recipeStepData in recipeData.RecipeStepDatas)
        {
            if (recipeStepData.Ingredient != curIngredient)
            {
                recipeIngredientOrder.Add(recipeStepData.Ingredient);
                recipeIngredientCounts.Add(1);
                curIngredient = recipeStepData.Ingredient;
            }
            else
            {
                recipeIngredientCounts[recipeIngredientCounts.Count - 1]++;
            }
        }

        bool bisFailed = false;
        float[] waveYPoses = new float[recipeData.RecipeColorDatas.Length];
        List<Vector4> frontTopColors = new List<Vector4>();
        List<Vector4> frontBottomColors = new List<Vector4>();
        List<Vector4> backTopColors = new List<Vector4>();
        List<Vector4> backBottomColors = new List<Vector4>();
        int maxLayerOrder = -1;
        if(m_curGlassType == recipeData.GlassType)
        {
            for (int index = 0; index < m_addedIngredients.Count; index++)
            {
                if (index >= recipeData.RecipeStepDatas.Length)
                {
                    bisFailed = true;
                    break;
                }
                else if (m_addedIngredients[index] != recipeData.RecipeStepDatas[index].Ingredient)
                {
                    bisFailed = true;
                    break;
                }

                if (recipeData.RecipeStepDatas[index].LayerOrder == -1)
                {
                    continue;
                }

                waveYPoses[recipeData.RecipeStepDatas[index].LayerOrder]++;

                if (recipeData.RecipeStepDatas[index].LayerOrder > maxLayerOrder)
                {
                    maxLayerOrder = recipeData.RecipeStepDatas[index].LayerOrder;
                }
            }
            for (int index = 0; index < recipeData.RecipeColorDatas.Length; index++)
            {
                waveYPoses[index] /= 10.0f;
                if (index > 0)
                {
                    waveYPoses[index] += waveYPoses[index - 1];
                }

                frontTopColors.Add(recipeData.RecipeColorDatas[index].FrontTopColor);
                frontBottomColors.Add(recipeData.RecipeColorDatas[index].FrontBottomColor);
                backTopColors.Add(recipeData.RecipeColorDatas[index].BackTopColor);
                backBottomColors.Add(recipeData.RecipeColorDatas[index].BackBottomColor);
            }
        }
        else
        {
            bisFailed = true;
        }

        if (!bisFailed)
        {
            m_waveMaterial.SetFloatArray("_WaveYPoses", waveYPoses.Reverse().ToArray());

            //string waveYPosesLog = string.Empty;
            //foreach (float val in waveYPoses)
            //{
            //    waveYPosesLog += val + " : ";
            //}
            //Debug.Log(waveYPosesLog);

            m_waveMaterial.SetVectorArray("_FrontTopColors", frontTopColors);
            m_waveMaterial.SetVectorArray("_FrontBottomColors", frontBottomColors);
            m_waveMaterial.SetVectorArray("_BackTopColors", backTopColors);
            m_waveMaterial.SetVectorArray("_BackBottomColors", backBottomColors);
            m_waveMaterial.SetInt("_WaveLayerCount", maxLayerOrder + 1);
        }
        else
        {
            //Debug.Log("B");
            Vector4 frontTopColor = Vector4.zero;
            Vector4 frontBottomColor = Vector4.zero;
            Vector4 backTopColor = Vector4.zero;
            Vector4 backBottomColor = Vector4.zero;

            foreach (var ingredient in m_addedIngredients)
            {
                switch (ingredient)
                {
                    case InventoryControl.ItemType.LemonSlice:
                    case InventoryControl.ItemType.OliveSkewers:
                    case InventoryControl.ItemType.LimeSlice:
                    case InventoryControl.ItemType.SilverSpoonWithSugarCube:
                    case InventoryControl.ItemType.SpoonShaking:
                    case InventoryControl.ItemType.HandShaking:
                        break;
                }

                if(ingredient == InventoryControl.ItemType.Shaker)
                {
                    Vector4 shakerFrontTopColor = Vector4.zero;
                    Vector4 shakerFrontBottomColor = Vector4.zero;
                    Vector4 shakerBackTopColor = Vector4.zero;
                    Vector4 shakerBackBottomColor = Vector4.zero;
                    ChiefMiniGameManager.Instance.CurShakerItem.GetShakerColor(out shakerFrontTopColor, out shakerFrontBottomColor, out shakerBackTopColor, out shakerBackBottomColor);

                    frontTopColor += shakerFrontTopColor;
                    frontBottomColor += shakerFrontBottomColor;
                    backTopColor += shakerBackTopColor;
                    backBottomColor += shakerBackBottomColor;
                }
                else
                {
                    Color[] waveColor = MiniGameItemResourceInterface.Instance.GetItemWaveColor(ingredient);
                    frontTopColor += new Vector4(waveColor[0].r, waveColor[0].g, waveColor[0].b, waveColor[0].a);
                    frontBottomColor += new Vector4(waveColor[1].r, waveColor[1].g, waveColor[1].b, waveColor[1].a);
                    backTopColor += new Vector4(waveColor[2].r, waveColor[2].g, waveColor[2].b, waveColor[2].a);
                    backBottomColor += new Vector4(waveColor[3].r, waveColor[3].g, waveColor[3].b, waveColor[3].a);
                }
            }
            m_waveMaterial.SetFloat("_WaveYPos", fillmentCount / 10.0f);

            frontTopColor /= fillmentCount;
            frontBottomColor /= fillmentCount;
            backTopColor /= fillmentCount;
            backBottomColor /= fillmentCount;

            m_waveMaterial.SetFloatArray("_WaveYPoses", new float[] { fillmentCount / 10.0f });
            m_waveMaterial.SetVectorArray("_FrontTopColors", new Vector4[] { frontTopColor });
            m_waveMaterial.SetVectorArray("_FrontBottomColors", new Vector4[] { frontBottomColor });
            m_waveMaterial.SetVectorArray("_BackTopColors", new Vector4[] { backTopColor });
            m_waveMaterial.SetVectorArray("_BackBottomColors", new Vector4[] { backBottomColor });
            m_waveMaterial.SetInt("_WaveLayerCount", 1);
        }

        int glassIndex = (int)(m_curGlassType - GlassType.AbsintheGlass);
        m_buoyancyEffector.surfaceLevel = Mathf.Lerp(m_glassMatData[glassIndex].buoyancyBottomValue, m_glassMatData[glassIndex].buoyancyTopValue, fillmentCount / 10.0f);

        for(int index = m_invisibleIngredientIndex; index < m_addedIngredients.Count; index++)
        {
            switch (m_addedIngredients[index])
            {
                case InventoryControl.ItemType.LimeSlice:
                    m_garnishData[glassIndex].Garnishs[0].SetActive(true);
                    break;

                case InventoryControl.ItemType.LemonSlice:
                    m_garnishData[glassIndex].Garnishs[1].SetActive(true);
                    break;

                case InventoryControl.ItemType.OliveSkewers:
                    m_garnishData[glassIndex].Garnishs[2].SetActive(true);
                    break;

                case InventoryControl.ItemType.SilverSpoonWithSugarCube:
                    m_garnishData[glassIndex].Garnishs[3].SetActive(true);
                    break;
            }
        }
        //foreach (InventoryControl.ItemType ingredient in m_addedIngredients)
        //{
        //    switch (ingredient)
        //    {
        //        case InventoryControl.ItemType.LimeSlice:
        //            m_garnishData[glassIndex].Garnishs[0].SetActive(true);
        //            break;

        //        case InventoryControl.ItemType.LemonSlice:
        //            m_garnishData[glassIndex].Garnishs[1].SetActive(true);
        //            break;

        //        case InventoryControl.ItemType.OliveSkewers:
        //            m_garnishData[glassIndex].Garnishs[2].SetActive(true);
        //            break;

        //        case InventoryControl.ItemType.SilverSpoonWithSugarCube:
        //            m_garnishData[glassIndex].Garnishs[3].SetActive(true);
        //            break;
        //    }
        //}

        RenderState(false, m_curGlassType);
    }

    internal void AddIngredientWaveScale()
    {
        if (GetFillmentCount() == 0)
        {
            return;
        }

        m_curWaveSpeed += m_maxWaveSpeed * m_waveAddScale;
        if (m_curWaveSpeed > m_maxWaveSpeed)
        {
            m_curWaveSpeed = m_maxWaveSpeed;
        }

        m_curWaveAmplitude += m_maxWaveAmplitude * m_waveAddScale;
        if (m_curWaveAmplitude > m_maxWaveAmplitude)
        {
            m_curWaveAmplitude = m_maxWaveAmplitude;
        }

        m_curWaveFrequency += m_maxWaveFrequency * m_waveAddScale;
        if (m_curWaveFrequency > m_maxWaveFrequency)
        {
            m_curWaveFrequency = m_maxWaveFrequency;
        }
    }
    internal void AddDragWaveScale(in Vector3 moveVector)
    {
        if (GetFillmentCount() == 0)
        {
            return;
        }

        float dragPower = (0.05f * moveVector.magnitude);
        m_curWaveSpeed += (m_maxWaveSpeed * m_waveAddScale) * dragPower;
        if (m_curWaveSpeed > m_maxWaveSpeed)
        {
            m_curWaveSpeed = m_maxWaveSpeed;
        }

        m_curWaveAmplitude += (m_maxWaveAmplitude * m_waveAddScale) * dragPower;
        if (m_curWaveAmplitude > m_maxWaveAmplitude)
        {
            m_curWaveAmplitude = m_maxWaveAmplitude;
        }

        m_curWaveFrequency += (m_maxWaveFrequency * m_waveAddScale) * dragPower;
        if (m_curWaveFrequency > m_maxWaveFrequency)
        {
            m_curWaveFrequency = m_maxWaveFrequency;
        }
    }

    internal void AddIce(in IceType targetIceType)
    {
        switch (targetIceType)
        {
            case IceType.Ice:
                m_ices[0].SetActive(true);
                break;

            default:
                m_ices[(int)(targetIceType - IceType.BarIce) + 1].SetActive(true);
                break;
        }
    }
    internal void ShakeWithSpoon()
    {
        switch (m_curGlassType)
        {
            case GlassType.AbsintheGlass:
                m_spoonAnimators[0].gameObject.SetActive(true);
                m_spoonAnimators[0].Play("SpoonShaking_Absinthe");
                break;

            case GlassType.OnTheRocksGlass:
            case GlassType.OldFashionedGlass:
                m_spoonAnimators[1].gameObject.SetActive(true);
                m_spoonAnimators[1].Play("SpoonShaking_OnTheRocks");
                break;

            case GlassType.CocktailGlass:
                m_spoonAnimators[2].gameObject.SetActive(true);
                m_spoonAnimators[2].Play("SpoonShaking_Cocktail");
                break;

            case GlassType.WineGlass:
                m_spoonAnimators[3].gameObject.SetActive(true);
                m_spoonAnimators[3].Play("SpoonShaking_Wine");
                break;

            case GlassType.HighballGlass:
                m_spoonAnimators[4].gameObject.SetActive(true);
                m_spoonAnimators[4].Play("SpoonShaking_Highball");
                break;
        }

        Invoke(nameof(DisableShakerWithSpoon), m_animationDuration);
    }

    private int GetFillmentCount()
    {
        int fillmentCount = 0;
        for (int index = m_invisibleIngredientIndex; index < m_addedIngredients.Count; index++)
        {
            switch (m_addedIngredients[index])
            {
                case InventoryControl.ItemType.LumpSugar:
                case InventoryControl.ItemType.LemonSlice:
                case InventoryControl.ItemType.LimeSlice:
                case InventoryControl.ItemType.OliveSkewers:
                case InventoryControl.ItemType.SilverSpoonWithSugarCube:
                case InventoryControl.ItemType.SpoonShaking:
                case InventoryControl.ItemType.HandShaking:
                case InventoryControl.ItemType.Ice:
                case InventoryControl.ItemType.BarIce:
                case InventoryControl.ItemType.CubeIce:
                case InventoryControl.ItemType.SphereIce:
                    continue;
            }

            fillmentCount++;
        }    
        //foreach (var ingredient in m_addedIngredients)
        //{
        //    switch (ingredient)
        //    {
        //        case InventoryControl.ItemType.LumpSugar:
        //        case InventoryControl.ItemType.LemonSlice:
        //        case InventoryControl.ItemType.LimeSlice:
        //        case InventoryControl.ItemType.OliveSkewers:
        //        case InventoryControl.ItemType.SilverSpoonWithSugarCube:
        //        case InventoryControl.ItemType.SpoonShaking:
        //        case InventoryControl.ItemType.HandShaking:
        //        case InventoryControl.ItemType.Ice:
        //        case InventoryControl.ItemType.BarIce:
        //        case InventoryControl.ItemType.CubeIce:
        //        case InventoryControl.ItemType.SphereIce:
        //            continue;
        //    }

        //    fillmentCount++;
        //}

        return fillmentCount;
    }

    private void DisableShakerWithSpoon()
    {
        switch (m_curGlassType)
        {
            case GlassType.AbsintheGlass:
                m_spoonAnimators[0].gameObject.SetActive(false);
                break;

            case GlassType.OnTheRocksGlass:
            case GlassType.OldFashionedGlass:
                m_spoonAnimators[1].gameObject.SetActive(false);
                break;

            case GlassType.CocktailGlass:
                m_spoonAnimators[2].gameObject.SetActive(false);
                break;

            case GlassType.WineGlass:
                m_spoonAnimators[3].gameObject.SetActive(false);
                break;

            case GlassType.HighballGlass:
                m_spoonAnimators[4].gameObject.SetActive(false);
                break;
        }
    }
}