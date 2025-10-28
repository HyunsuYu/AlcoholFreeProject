using CommonUtilLib.ThreadSafe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.XR;

using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;


public sealed class MusicDataBuffer : SingleTonForGameObject<MusicDataBuffer>
{
    public enum SFXMusicType : byte
    {
        None,
        Walking,
        Bell
    }

    public struct MusicBinaryData
    {
        public byte[] Data;
        public int Channel;
        public int Frequency;
    }


    private static Dictionary<BackgroundResourceDesc.BackgroundType, AudioClip> m_backgroundMusicClipTable = new Dictionary<BackgroundResourceDesc.BackgroundType, AudioClip>();

    private static Dictionary<CharacterResourceDesc.CharacterListType, AudioClip> m_characterMusicTable_Level_0 = new Dictionary<CharacterResourceDesc.CharacterListType, AudioClip>();
    private static Dictionary<CharacterResourceDesc.CharacterListType, AudioClip> m_characterMusicTable_Level_1 = new Dictionary<CharacterResourceDesc.CharacterListType, AudioClip>();
    private static Dictionary<CharacterResourceDesc.CharacterListType, AudioClip> m_characterMusicTable_Level_2 = new Dictionary<CharacterResourceDesc.CharacterListType, AudioClip>();

    private static Dictionary<SFXMusicType, AudioClip> m_sfxMusicClips = new Dictionary<SFXMusicType, AudioClip>();


    public void Awake()
    {
        SetInstance(this);
    }

    private byte[] Key
    {
        get
        {
            return new byte[]
            {
                110,147,146,162,151,11,139,12,89,201,11,49,46,109,42,168,63,22,41,252,180,67,84,38,2,134,31,154,42,29,80,221
            };
        }
    }
    private byte[] IV
    {
        get
        {
            return new byte[]
            {
                22,35,114,156,173,178,30,223,42,128,12,37,74,162,153,248
            };
        }
    }

    private string BackgroundMusicBasePath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "Music/BGM");
        }
    }
    private string CharacterMusicBasePath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "Music/Character");
        }
    }
    private string SFXMusicBasePath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "Music/SFX");
        }
    }

    [Obsolete]
    internal void PreloadMusicDatas()
    {
        //Dictionary<BackgroundResourceDesc.BackgroundType, Task<MusicBinaryData>> backgroundMusicTable = new Dictionary<BackgroundResourceDesc.BackgroundType, Task<MusicBinaryData>>();
        //Dictionary<CharacterResourceDesc.CharacterListType, Task<MusicBinaryData>> characterMusicTable_Level_0 = new Dictionary<CharacterResourceDesc.CharacterListType, Task<MusicBinaryData>>();
        //Dictionary<SFXMusicType, Task<MusicBinaryData>> sfxMusicTable = new Dictionary<SFXMusicType, Task<MusicBinaryData>>();
        //ThreadPool.QueueUserWorkItem(() =>
        //{

        //});
        //await Task.Run(() =>
        //{
        //    // Preload Background Music
        //    foreach (BackgroundResourceDesc.BackgroundType bgm in Enum.GetValues(typeof(BackgroundResourceDesc.BackgroundType)))
        //    {
        //        //TryGetBackgroundMusic(bgm);
        //        backgroundMusicTable.Add(bgm, LoadAndDecryptBackgroundMusic(bgm));
        //        Debug.Log($"MusicDataBuffer: Preloaded background music for {bgm}");
        //    }
        //    // Preload Character Music
        //    foreach (CharacterResourceDesc.CharacterListType character in Enum.GetValues(typeof(CharacterResourceDesc.CharacterListType)))
        //    {
        //        if (character != CharacterResourceDesc.CharacterListType.MainCharacter && character != CharacterResourceDesc.CharacterListType.Description)
        //        {
        //            //for (int level = 0; level <= 2; level++)
        //            //{
        //            //    TryGetCharacterMusic(character, level);
        //            //}
        //            characterMusicTable_Level_0.Add(character, LoadAndDecryptCharacterMusic(character, 0));
        //            Debug.Log($"MusicDataBuffer: Preloaded character music for {character} at level 0");
        //        }
        //    }
        //    // Preload SFX Music
        //    foreach (SFXMusicType sfx in Enum.GetValues(typeof(SFXMusicType)))
        //    {
        //        if (sfx != SFXMusicType.None)
        //        {
        //            //TryGetSFXMusic(sfx);
        //            sfxMusicTable.Add(sfx, LoadAndDecryptSFXMusic(sfx));
        //            Debug.Log($"MusicDataBuffer: Preloaded SFX music for {sfx}");
        //        }
        //    }
        //});
        //Task.WaitAll(backgroundMusicTable.Values.ToArray());
        //Task.WaitAll(characterMusicTable_Level_0.Values.ToArray());
        //Task.WaitAll(sfxMusicTable.Values.ToArray());
        Debug.Log("MusicDataBuffer: PreloadMusicDatas completed");

        //foreach (var kvp in backgroundMusicTable)
        //{
        //    m_backgroundMusicClipTable.Add(kvp.Key, Convert(kvp.Value.Result.Data, kvp.Value.Result));
        //}
        //Debug.Log("MusicDataBuffer: Background music preload completed");

        //foreach (var kvp in characterMusicTable_Level_0)
        //{
        //    m_characterMusicTable_Level_0.Add(kvp.Key, Convert(kvp.Value.Result.Data, kvp.Value.Result));
        //}
        //Debug.Log("MusicDataBuffer: Character music preload completed");

        //foreach (var kvp in sfxMusicTable)
        //{
        //    m_sfxMusicClips.Add(kvp.Key, Convert(kvp.Value.Result.Data, kvp.Value.Result));
        //}
        //Debug.Log("MusicDataBuffer: SFX music preload completed");
    }

    #region Background Music
    internal AudioClip TryGetBackgroundMusic(in BackgroundResourceDesc.BackgroundType targetBGM)
    {
        if(m_backgroundMusicClipTable.ContainsKey(targetBGM))
        {
            return m_backgroundMusicClipTable[targetBGM];
        }

        string filePath = Path.Combine(BackgroundMusicBasePath, targetBGM.ToString() + ".bin");

        MusicBinaryData musicBinaryData = JsonConvert.DeserializeObject<MusicBinaryData>(File.ReadAllText(filePath));

        //byte[] encrypted = File.ReadAllBytes(filePath);
        byte[] decrypted = Decrypt(musicBinaryData.Data, Key, IV);

        m_backgroundMusicClipTable.Add(targetBGM, Convert(decrypted, musicBinaryData));
        return m_backgroundMusicClipTable[targetBGM];
    }
    internal void TrySaveBackgroundMusic(in AudioClip targetAudioClip, in BackgroundResourceDesc.BackgroundType targetBGM)
    {
        byte[] data = Convert(targetAudioClip);
        byte[] encrypted = Encrypt(data, Key, IV);

        MusicBinaryData musicBinaryData = new MusicBinaryData
        {
            Data = encrypted,
            Channel = targetAudioClip.channels,
            Frequency = targetAudioClip.frequency
        };

        string filePath = Path.Combine(BackgroundMusicBasePath, targetBGM.ToString() + ".bin");

        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //File.WriteAllBytes(filePath, encrypted);
        string json = JsonConvert.SerializeObject(musicBinaryData);
        File.WriteAllText(filePath, json);
    }

    private MusicBinaryData LoadAndDecryptBackgroundMusic(BackgroundResourceDesc.BackgroundType targetBGM)
    {
        string filePath = Path.Combine(BackgroundMusicBasePath, targetBGM.ToString() + ".bin");

        MusicBinaryData musicBinaryData = JsonConvert.DeserializeObject<MusicBinaryData>(File.ReadAllText(filePath));

        //byte[] encrypted = File.ReadAllBytes(filePath);
        byte[] decrypted = Decrypt(musicBinaryData.Data, Key, IV);

        return new MusicBinaryData
        {
            Data = decrypted,
            Channel = musicBinaryData.Channel,
            Frequency = musicBinaryData.Frequency
        };
    }
    #endregion

    #region Character Music
    internal AudioClip TryGetCharacterMusic(in CharacterResourceDesc.CharacterListType targetCharacter, in int level)
    {
        Dictionary<CharacterResourceDesc.CharacterListType, AudioClip> targetTable = null;
        switch (level)
        {
            case 0:
                targetTable = m_characterMusicTable_Level_0;
                break;
            case 1:
                targetTable = m_characterMusicTable_Level_1;
                break;
            case 2:
                targetTable = m_characterMusicTable_Level_2;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), "Invalid character level");
        }
        if (targetTable.ContainsKey(targetCharacter))
        {
            return targetTable[targetCharacter];
        }

        string filePath = Path.Combine(CharacterMusicBasePath, $"{targetCharacter.ToString()}_{level}.bin");

        MusicBinaryData musicBinaryData = JsonConvert.DeserializeObject<MusicBinaryData>(File.ReadAllText(filePath));

        //byte[] encrypted = File.ReadAllBytes(filePath);
        byte[] decrypted = Decrypt(musicBinaryData.Data, Key, IV);

        targetTable.Add(targetCharacter, Convert(decrypted, musicBinaryData));
        return targetTable[targetCharacter];
    }
    internal void TrySaveCharacterMusic(in AudioClip targetAudioClip, in CharacterResourceDesc.CharacterListType targetCharacter, in int level)
    {
        byte[] data = Convert(targetAudioClip);
        byte[] encrypted = Encrypt(data, Key, IV);

        MusicBinaryData musicBinaryData = new MusicBinaryData
        {
            Data = encrypted,
            Channel = targetAudioClip.channels,
            Frequency = targetAudioClip.frequency
        };

        string filePath = Path.Combine(CharacterMusicBasePath, $"{targetCharacter.ToString()}_{level}.bin");

        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //File.WriteAllBytes(filePath, encrypted);
        string json = JsonConvert.SerializeObject(musicBinaryData);
        File.WriteAllText(filePath, json);
    }

    private MusicBinaryData LoadAndDecryptCharacterMusic(CharacterResourceDesc.CharacterListType targetCharacter, int level)
    {
        string filePath = Path.Combine(CharacterMusicBasePath, $"{targetCharacter.ToString()}_{level}.bin");
        MusicBinaryData musicBinaryData = JsonConvert.DeserializeObject<MusicBinaryData>(File.ReadAllText(filePath));
        //byte[] encrypted = File.ReadAllBytes(filePath);
        byte[] decrypted = Decrypt(musicBinaryData.Data, Key, IV);
        return new MusicBinaryData
        {
            Data = decrypted,
            Channel = musicBinaryData.Channel,
            Frequency = musicBinaryData.Frequency
        };
    }
    #endregion

    #region SFX Music
    internal AudioClip TryGetSFXMusic(in SFXMusicType targetSFX)
    {
        if (m_sfxMusicClips.ContainsKey(targetSFX))
        {
            return m_sfxMusicClips[targetSFX];
        }

        string filePath = Path.Combine(SFXMusicBasePath, targetSFX.ToString() + ".bin");

        MusicBinaryData musicBinaryData = JsonConvert.DeserializeObject<MusicBinaryData>(File.ReadAllText(filePath));

        //byte[] encrypted = File.ReadAllBytes(filePath);
        byte[] decrypted = Decrypt(musicBinaryData.Data, Key, IV);

        m_sfxMusicClips.Add(targetSFX, Convert(decrypted, musicBinaryData));
        return m_sfxMusicClips[targetSFX];
    }
    internal void TrySaveSFXMusic(in AudioClip targetAudioClip, in SFXMusicType targetSFX)
    {
        byte[] data = Convert(targetAudioClip);
        byte[] encrypted = Encrypt(data, Key, IV);

        MusicBinaryData musicBinaryData = new MusicBinaryData
        {
            Data = encrypted,
            Channel = targetAudioClip.channels,
            Frequency = targetAudioClip.frequency
        };

        string filePath = Path.Combine(SFXMusicBasePath, targetSFX.ToString() + ".bin");

        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        //File.WriteAllBytes(filePath, encrypted);
        string json = JsonConvert.SerializeObject(musicBinaryData);
        File.WriteAllText(filePath, json);
    }

    private MusicBinaryData LoadAndDecryptSFXMusic(SFXMusicType targetSFX)
    {
        string filePath = Path.Combine(SFXMusicBasePath, targetSFX.ToString() + ".bin");
        MusicBinaryData musicBinaryData = JsonConvert.DeserializeObject<MusicBinaryData>(File.ReadAllText(filePath));
        //byte[] encrypted = File.ReadAllBytes(filePath);
        byte[] decrypted = Decrypt(musicBinaryData.Data, Key, IV);
        return new MusicBinaryData
        {
            Data = decrypted,
            Channel = musicBinaryData.Channel,
            Frequency = musicBinaryData.Frequency
        };
    }
    #endregion

    protected override void Dispose(bool bisDisposing)
    {
        throw new NotImplementedException();
    }

    private static byte[] Convert(in AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        int length = samples.Length;
        writer.Write(length);

        foreach (var sample in samples)
        {
            writer.Write(sample);
        }

        return stream.ToArray();
    }
    private static AudioClip Convert(in byte[] bytes, in MusicBinaryData musicBinaryData)
    {
        MemoryStream stream = new MemoryStream(bytes);
        BinaryReader reader = new BinaryReader(stream);
        int length = reader.ReadInt32();
        float[] samples = new float[length];
        for (int i = 0; i < length; i++)
        {
            samples[i] = reader.ReadSingle();
        }
        AudioClip audioClip = AudioClip.Create("SongName", length, musicBinaryData.Channel, musicBinaryData.Frequency, false);
        audioClip.SetData(samples, 0);
        return audioClip;
    }

    private static byte[] Encrypt(byte[] plaintextBytes, byte[] key, byte[] iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(plaintextBytes, 0, plaintextBytes.Length);
                    csEncrypt.FlushFinalBlock();
                }
                return msEncrypt.ToArray();
            }
        }
    }
    private static byte[] Decrypt(byte[] ciphertextBytes, byte[] key, byte[] iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(ciphertextBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream msPlain = new MemoryStream())
                    {
                        csDecrypt.CopyTo(msPlain);
                        return msPlain.ToArray();
                    }
                }
            }
        }
    }
}