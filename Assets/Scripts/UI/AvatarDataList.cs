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
    private List<AvatarDataItem> _maleDataItemList = new List<AvatarDataItem>();
    private List<AvatarDataItem> _femaleDataItemList = new List<AvatarDataItem>();
    
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
        foreach (var dataItem in _maleDataItemList)
        {
            dataItem.gameObject.SetActive(isMale);
        }

        foreach (var dataItem in _femaleDataItemList)
        {
            dataItem.gameObject.SetActive(!isMale);
        }
    }

    public void SelectRandomSkin(bool isMale)
    {
        if (isMale)
        {
            int index = Random.Range(0, _maleDataItemList.Count - 1);

            _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(false);

            _currentMaleItemIndex = index;
            _maleDataItemList[_currentMaleItemIndex].imageFrameGameObject.SetActive(true);

            OnSkinItemSelected?.Invoke(index);
        }
        else
        {
            int index = Random.Range(0, _femaleDataItemList.Count - 1);

            _femaleDataItemList[_currentFemaleItemIndex].imageFrameGameObject.SetActive(false);

            _currentFemaleItemIndex = index;
            _femaleDataItemList[_currentFemaleItemIndex].imageFrameGameObject.SetActive(true);

            OnSkinItemSelected?.Invoke(index);
        }
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
        GenerateSkinListItem(true, out _maleDataItemList);
        GenerateSkinListItem(false, out _femaleDataItemList);
    }

    private void GenerateSkinListItem(bool isMale, out List<AvatarDataItem> avatarDataItemList)
    {
        avatarDataItemList = new List<AvatarDataItem>();
        
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
            
            avatarDataItem.button.onClick.AddListener(delegate { OnButtonItemClickSkin(index, isMale); });

            avatarDataItemList.Add(avatarDataItem);

            if (isMale)
            {
                if (_currentMaleItemIndex == index)
                {
                    avatarDataItem.imageFrameGameObject.SetActive(true);
                }
            }
            else
            {
                if (_currentFemaleItemIndex == index)
                {
                    avatarDataItem.imageFrameGameObject.SetActive(true);
                }
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

        GenerateDataListItem(_maleDataList, true, out _maleDataItemList);
        GenerateDataListItem(_femaleDataList, false, out _femaleDataItemList);
    }

    private void GenerateDataListItem(List<AvatarSO> avatarDataList, bool isMale, out List<AvatarDataItem> avatarDataItemList)
    {
        avatarDataItemList = new List<AvatarDataItem>();

        for (int i = 0; i < avatarDataList.Count; i++)
        {
            int index = i;
            AvatarDataItem avatarDataItem = Instantiate(itemPrefab, parentObject.transform);

            avatarDataItem.iconImage.gameObject.SetActive(true);
            avatarDataItem.iconImage.sprite = isMale ? _maleDataList[index].icon : _femaleDataList[index].icon;

            avatarDataItem.button.onClick.AddListener(delegate { OnButtonItemClickData(index, isMale, isMale ? _maleDataList[index] : _femaleDataList[index]); });

            avatarDataItemList.Add(avatarDataItem);

            if (isMale)
            {
                if (_currentMaleItemIndex == index)
                {
                    avatarDataItem.imageFrameGameObject.SetActive(true);
                }
            }
            else
            {
                if (_currentFemaleItemIndex == index)
                {
                    avatarDataItem.imageFrameGameObject.SetActive(true);
                }
            }
        }
    }

    private void OnButtonItemClickSkin(int index, bool isMale)
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
