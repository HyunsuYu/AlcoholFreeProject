using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

using CommonUtilLib.ThreadSafe;


public sealed class MouseEffector : SingleTonForGameObject<MouseEffector>
{
    [SerializeField] private ParticleSystem m_particleSystem;

    private IObjectPool<ParticleSystem> m_pool;

    private Queue<ParticleSystem> m_effectQueue = new Queue<ParticleSystem>();


    public void Awake()
    {
        if(GameObject.Find(gameObject.name) != null && GameObject.Find(gameObject.name) != gameObject)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        SetInstance(this);

        m_pool = new ObjectPool<ParticleSystem>(
            createFunc: () =>
            {
                var effect = Instantiate(m_particleSystem, Vector3.zero, Quaternion.identity);
                DontDestroyOnLoad(effect.gameObject);

                return effect;
            },
            actionOnGet: (obj) =>
            {
                obj.gameObject.SetActive(true);
            },
            actionOnRelease: (obj) =>
            {
                obj.gameObject.SetActive(false);
            },
            actionOnDestroy: (obj) =>
            {
                Destroy(obj.gameObject);
            },
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var effect = m_pool.Get();

            Vector3 targetPos = Vector3.zero;
            if (Camera.main.orthographic)
            {
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = Camera.main.transform.position.z + 10.0f;
            }
            else
            {
                targetPos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;

                targetPos = new Vector3()
                {
                    x = targetPos.x * 33.0f,
                    y = targetPos.y * 33.0f,
                    z = Camera.main.transform.position.z + 10.0f
                };
            }

            effect.transform.position = targetPos;
            effect.Play();

            var audioSource = effect.GetComponent<AudioSource>();
            audioSource.volume = SettingDataBuffer.Instance.SettingData.AudioSetting.MasterVolume * SettingDataBuffer.Instance.SettingData.AudioSetting.SFXVolume;
            audioSource.Play();

            m_effectQueue.Enqueue(effect);

            Invoke(nameof(ReleaseEffect), 1.0f);
        }
    }

    internal void ClearAllEffects()
    {
        while (m_effectQueue.Count > 0)
        {
            m_pool.Release(m_effectQueue.Dequeue());
        }
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private void ReleaseEffect()
    {
        if (m_effectQueue.Count > 0)
        {
            m_pool.Release(m_effectQueue.Dequeue());
        }
    }
}