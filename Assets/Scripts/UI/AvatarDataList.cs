using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AvatarDataList : MonoBehaviour
{
    public enum AvatarDataType
    {
        Skin,
        Face,
        Hair,
        Outfit,
        Glasses
    }

    [SerializeField] private Transform parentObject;
    [SerializeField] private AvatarDataItem itemPrefab;

    private AvatarDataType _dataType;
    
    private List<AvatarSO> _maleDataList = new List<AvatarSO>();
    private List<AvatarSO> _femaleDataList = new List<AvatarSO>();
    private readonly List<AvatarDataItem> _maleDataItemList = new List<AvatarDataItem>();
    private readonly List<AvatarDataItem> _femaleDataItemList = new List<AvatarDataItem>();
    
    private int _currentMaleItemIndex;
    private int _currentFemaleItemIndex;

    public Action<int> OnSkinItemSelected;
    public Action<AvatarSO> OnDataItemSelected;

    public void Init(int dataTypeIndex)
    {
        _dataType = (AvatarDataType) dataTypeIndex;

        _currentMaleItemIndex = 0;
        _currentFemaleItemIndex = 0;
        
        GenerateListItem();

    }

    public void SwitchGenderItem(bool isMale)
    {
        if (_dataType == AvatarDataType.Skin) return;
        
        foreach (var dataItem in _maleDataItemList)
        {
            dataItem.gameObject.SetActive(isMale);
        }

        foreach (var dataItem in _femaleDataItemList)
        {
            dataItem.gameObject.SetActive(!isMale);
        }
    }

    public void SelectRandomSkin()
    {
        int index = Random.Range(0, _maleDataItemList.Count - 1);
            
        _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(false);

        _currentMaleItemIndex = index;
        _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(true);  
        
        OnSkinItemSelected?.Invoke(index);
    }
    
    public void SelectRandomData(bool isMale)
    {
        
        if (isMale)
        {
            int index = Random.Range(0, _maleDataItemList.Count - 1);
            
            _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(false);

            _currentMaleItemIndex = index;
            _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(true);
            
            OnDataItemSelected?.Invoke(_maleDataList[index]);
        }
        else
        {
            int index = Random.Range(0, _femaleDataItemList.Count - 1);

            _femaleDataItemList[_currentFemaleItemIndex].imageFrameGameObject.SetActive(false);

            _currentFemaleItemIndex = index;
            _femaleDataItemList[_currentFemaleItemIndex].imageFrameGameObject.SetActive(true);            
        
            OnDataItemSelected?.Invoke(_femaleDataList[index]);
        }
        
    }
        
    private void GenerateListItem()
    {
        if (_dataType == AvatarDataType.Skin)
        {
            GenerateSkinListItem();
        }
        else
        {
            GenerateDataListItem();
        }
    }
    
    private void GenerateSkinListItem()
    {
        for (int i = 0; i < AssetReferenceDatabase.Instance.skinTones.Count; i++)
        {
            int index = i;
            AvatarDataItem avatarDataItem = Instantiate(itemPrefab, parentObject.transform);

            avatarDataItem.skinImage.gameObject.SetActive(true);
            avatarDataItem.skinImage.color = new Color(
                AssetReferenceDatabase.Instance.baseSkinColor.r * AssetReferenceDatabase.Instance.skinTones[i],
                AssetReferenceDatabase.Instance.baseSkinColor.g * AssetReferenceDatabase.Instance.skinTones[i],
                AssetReferenceDatabase.Instance.baseSkinColor.b * AssetReferenceDatabase.Instance.skinTones[i],
                1f
            );

            avatarDataItem.button.onClick.AddListener(delegate { OnButtonItemClickSkin(index); });

            _maleDataItemList.Add(avatarDataItem);

            if (_currentMaleItemIndex == index)
            {
                avatarDataItem.imageFrameGameObject.SetActive(true);
            }
        }
    }
    
    private void GenerateDataListItem()
    {
        switch (_dataType)
        {
            case AvatarDataType.Face:
                _maleDataList = AssetReferenceDatabase.Instance.faceMaleSOs;
                _femaleDataList = AssetReferenceDatabase.Instance.faceFemaleSOs;
                break;
            case AvatarDataType.Hair:
                _maleDataList = AssetReferenceDatabase.Instance.hairMaleSOs;
                _femaleDataList = AssetReferenceDatabase.Instance.hairFemaleSOs;
                break;
            case AvatarDataType.Outfit:
                _maleDataList = AssetReferenceDatabase.Instance.costumeMaleSOs;
                _femaleDataList = AssetReferenceDatabase.Instance.costumeFemaleSOs;
                break;
            case AvatarDataType.Glasses:
                _maleDataList = AssetReferenceDatabase.Instance.glassesMaleSOs;
                _femaleDataList = AssetReferenceDatabase.Instance.glassesFemaleSOs;
                break;
        }

        for (int i = 0; i < _maleDataList.Count; i++)
        {
            int index = i;
            AvatarDataItem avatarDataItem = Instantiate(itemPrefab, parentObject.transform);

            avatarDataItem.iconImage.gameObject.SetActive(true);
            avatarDataItem.iconImage.sprite = _maleDataList[index].icon;

            avatarDataItem.button.onClick.AddListener(delegate { OnButtonItemClickData(index, true, _maleDataList[index]); });

            _maleDataItemList.Add(avatarDataItem);

            if (_currentMaleItemIndex == index)
            {
                avatarDataItem.imageFrameGameObject.SetActive(true);
            }
        }

        for (int i = 0; i < _femaleDataList.Count; i++)
        {
            int index = i;
            AvatarDataItem avatarDataItem = Instantiate(itemPrefab, parentObject.transform);

            avatarDataItem.iconImage.gameObject.SetActive(true);
            avatarDataItem.iconImage.sprite = _femaleDataList[index].icon;

            avatarDataItem.button.onClick.AddListener(delegate { OnButtonItemClickData(index, false, _femaleDataList[index]); });

            _femaleDataItemList.Add(avatarDataItem);
            
            if (_currentFemaleItemIndex == index)
            {
                avatarDataItem.imageFrameGameObject.SetActive(true);
            }
        }
    }

    private void OnButtonItemClickSkin(int index)
    {
        _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(false);

        _currentMaleItemIndex = index;
        _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(true);
        
        OnSkinItemSelected?.Invoke(index);
    }
    
    private void OnButtonItemClickData(int index, bool isMale, AvatarSO data)
    {
        if (isMale)
        {
            _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(false);

            _currentMaleItemIndex = index;
            _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(true);
        }
        else
        {
            _femaleDataItemList[_currentFemaleItemIndex].imageFrameGameObject.SetActive(false);

            _currentFemaleItemIndex = index;
            _femaleDataItemList[_currentFemaleItemIndex].imageFrameGameObject.SetActive(true);            
        }
        
        OnDataItemSelected?.Invoke(data);
    }
}
