using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetReferenceDatabase : MonoBehaviour
{
    public static AssetReferenceDatabase Instance;

    public Color baseSkinColor;
    public List<float> skinTones;
    
    [Header("Male Avatar Data")]
    public List<AvatarSO> faceMaleSOs = new List<AvatarSO>();
    public List<AvatarSO> hairMaleSOs = new List<AvatarSO>();
    public List<AvatarSO> costumeMaleSOs = new List<AvatarSO>();
    public List<AvatarSO> glassesMaleSOs = new List<AvatarSO>();

    [Header("Female Avatar Data")]
    public List<AvatarSO> faceFemaleSOs = new List<AvatarSO>();
    public List<AvatarSO> hairFemaleSOs = new List<AvatarSO>();
    public List<AvatarSO> costumeFemaleSOs = new List<AvatarSO>();
    public List<AvatarSO> glassesFemaleSOs = new List<AvatarSO>();

    [Space]
    [SerializeField] internal PopupSpriteReference popupSpriteReference = new PopupSpriteReference();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    internal class PopupSpriteReference
    {
        public Sprite warningSprite = null;
        public Sprite confirmationSprite = null;
    }
}

