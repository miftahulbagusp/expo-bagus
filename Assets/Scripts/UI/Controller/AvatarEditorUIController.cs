using UI.View;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Controller
{
    public class AvatarEditorUIController : BaseUIController
    {
        [SerializeField] private AvatarEditorUIView view;

        [SerializeField] private AvatarProperties maleAvatarProperties;
        [SerializeField] private AvatarProperties femaleAvatarProperties;
        [SerializeField] private Camera avatarPreviewCamera;
        [SerializeField] private Transform avatarPreviewTransform;

        private bool _isRotateLeftButtonPressed;
        private bool _isRotateRightButtonPressed;

        private bool _isMale;
        private UserDataModel _maleDataModel;
        private UserDataModel _femaleDataModel;
        
        private void Update()
        {
            if (_isRotateLeftButtonPressed)
            {
                RotateAvatarPreview(Vector3.up);
            } else if (_isRotateRightButtonPressed)
            {
                RotateAvatarPreview(Vector3.down);
            }
            else
            {
                RotateAvatarPreview(new Vector3(0, -view.touchArea.Axis.x, 0));
            }
        }
        
        public override void InitScreen()
        {
            // Adding event to UI
            view.scaleSlider.onValueChanged.AddListener(ZoomAvatarPreview);
            
            if (view.rotateLeftEventTrigger != null && view.rotateRightEventTrigger)
            {
                EventTrigger trigger;
                EventTrigger.Entry entry;
            
                // Adding continuous event trigger 
                trigger = view.rotateLeftEventTrigger;
                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((eventData) => { _isRotateLeftButtonPressed = true; });
                trigger.triggers.Add(entry);
                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((eventData) => { _isRotateLeftButtonPressed = false; });
                trigger.triggers.Add(entry);

                trigger = view.rotateRightEventTrigger;
                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((eventData) => { _isRotateRightButtonPressed = true; });
                trigger.triggers.Add(entry);
                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((eventData) => { _isRotateRightButtonPressed = false; });
                trigger.triggers.Add(entry);
            }
            
            view.nicknameInputField.onValueChanged.AddListener(delegate(string arg0) { view.nicknameText.text = arg0; });
            
            view.confirmButton.onClick.AddListener(ConfirmAvatarEditing);
            view.randomButton.onClick.AddListener(RandomAvatar);
            
            view.hairColorPicker.onValueChanged.AddListener(SelectHairColor);

            // Initialize user data
            _maleDataModel = new UserDataModel
            {
                Nickname = view.nicknameInputField.text,
                IsMale = true,
                SkinColor = 1,
                FaceTypeID = 0,
                HairStyleID = 0,
                HairColor = new []{view.hairColorPicker.CurrentColor.r,view.hairColorPicker.CurrentColor.g,view.hairColorPicker.CurrentColor.b},
                OutfitID = 0,
                AccessoriesGlassesID = 0
            };
            
            _femaleDataModel = new UserDataModel
            {
                Nickname = view.nicknameInputField.text,
                IsMale = false,
                SkinColor = 1,
                FaceTypeID = 0,
                HairStyleID = 0,
                HairColor = new []{view.hairColorPicker.CurrentColor.r,view.hairColorPicker.CurrentColor.g,view.hairColorPicker.CurrentColor.b},
                OutfitID = 0,
                AccessoriesGlassesID = 0
            };
            
            // Adding event to item list
            view.maleGenderButton.onClick.AddListener(() => ChangeGender(true));
            view.femaleGenderButton.onClick.AddListener(() => ChangeGender(false));
            
            view.skinToneDataList.Init(0);
            view.skinToneDataList.OnSkinItemSelected += i => { UpdateAvatar(AvatarDataList.AvatarDataType.Skin, i); };
            
            view.faceDataList.Init(1);
            view.faceDataList.OnDataItemSelected += so =>
            {
                UpdateAvatar(AvatarDataList.AvatarDataType.Face, 0, so);
            };

            view.nicknameInputField.text = $"user#{Random.Range(0000, 9999)}";

            view.hairDataList.Init(2);
            view.hairDataList.OnDataItemSelected += so =>
            {
                UpdateAvatar(AvatarDataList.AvatarDataType.Hair, 0, so);
            };
            
            view.outfitDataList.Init(3);
            view.outfitDataList.OnDataItemSelected += so =>
            {
                UpdateAvatar(AvatarDataList.AvatarDataType.Outfit, 0, so);
            };
            
            view.glassesDataList.Init(4);
            view.glassesDataList.OnDataItemSelected += so =>
            {
                UpdateAvatar(AvatarDataList.AvatarDataType.Glasses, 0, so);
            };
            
            // 
            ChangeGender(true);
        }

        public override void OpenScreen()
        {
            // throw new System.NotImplementedException();
        }

        public override void CloseScreen()
        {
            // throw new System.NotImplementedException();
        }

        private void ZoomAvatarPreview(float value)
        {
            avatarPreviewCamera.orthographicSize = 1 - 0.7f * value;
            Vector3 previewCameraPosition = avatarPreviewCamera.transform.localPosition;
            avatarPreviewCamera.transform.localPosition = new Vector3(previewCameraPosition.x, 0.7f * value, previewCameraPosition.z);
        }

        private void RotateAvatarPreview(Vector3 _rotation)
        {
            float rotationSpeed = 100f;
            avatarPreviewTransform.Rotate( _rotation * rotationSpeed * Time.deltaTime);
        }

        private void ConfirmAvatarEditing()
        {
            if (_isMale)
            {
                _maleDataModel.Nickname = view.nicknameInputField.text;
                UserDataManager.Instance.userDataModel = _maleDataModel;
            }
            else
            {
                _femaleDataModel.Nickname = view.nicknameInputField.text;
                UserDataManager.Instance.userDataModel = _femaleDataModel;                
            }
            
            NetworkManager.Instance.JoinRoom();
        }

        private void RandomAvatar()
        {
            view.skinToneDataList.SelectRandomSkin(_isMale);
            view.faceDataList.SelectRandomData(_isMale);
            view.hairDataList.SelectRandomData(_isMale);
            view.outfitDataList.SelectRandomData(_isMale);
            view.glassesDataList.SelectRandomData(_isMale);

            view.hairColorPicker.R = Random.Range(0, 100) / 100f;
            view.hairColorPicker.G = Random.Range(0, 100) / 100f;
            view.hairColorPicker.B = Random.Range(0, 100) / 100f;
        }

        private void ChangeGender(bool isMale)
        {
            view.skinToneDataList.SwitchGenderItem(isMale);
            view.faceDataList.SwitchGenderItem(isMale);
            view.hairDataList.SwitchGenderItem(isMale);
            view.outfitDataList.SwitchGenderItem(isMale);
            view.glassesDataList.SwitchGenderItem(isMale);

            maleAvatarProperties.gameObject.SetActive(isMale);
            femaleAvatarProperties.gameObject.SetActive(!isMale);

            _isMale = isMale;
        }

        private void UpdateAvatar(AvatarDataList.AvatarDataType dataType, int skinIndex = 0, AvatarSO data = null)
        {
            string genderPath = _isMale ? "Male/" : "Female/";

            Material skinMaterial;
            Material[] mats;
            
            switch (dataType)
            {
                case AvatarDataList.AvatarDataType.Skin:
                    if (_isMale)
                    {
                        skinMaterial = maleAvatarProperties.headMeshRendered.material;
                        skinMaterial.SetFloat("_Tone", AssetReferenceDatabase.Instance.skinTones[skinIndex]);

                        maleAvatarProperties.headMeshRendered.material = skinMaterial;
                    
                        mats = maleAvatarProperties.bodyMeshRendered.materials;
                        mats[1] = skinMaterial;
                        maleAvatarProperties.bodyMeshRendered.materials = mats;
                        
                        _maleDataModel.SkinColor = AssetReferenceDatabase.Instance.skinTones[skinIndex];
                    }
                    else
                    {
                        skinMaterial = femaleAvatarProperties.headMeshRendered.material;
                        skinMaterial.SetFloat("_Tone", AssetReferenceDatabase.Instance.skinTones[skinIndex]);

                        femaleAvatarProperties.headMeshRendered.material = skinMaterial;

                        mats = femaleAvatarProperties.bodyMeshRendered.materials;
                        mats[1] = skinMaterial;
                        femaleAvatarProperties.bodyMeshRendered.materials = mats;

                        _femaleDataModel.SkinColor = AssetReferenceDatabase.Instance.skinTones[skinIndex];
                    }
                    break;
                case AvatarDataList.AvatarDataType.Face:
                    float skinTone = maleAvatarProperties.headMeshRendered.material.GetFloat("_Tone");
                    
                    if (_isMale)
                    {
                        maleAvatarProperties.headMeshRendered.sharedMesh = Resources.Load<Mesh>(ResourcePath.HeadMeshPath + genderPath + data.meshFileName);
                        maleAvatarProperties.headMeshRendered.material = Resources.Load<Material>(ResourcePath.HeadMaterialPath + genderPath + data.meshFileName);
                        
                        skinMaterial = maleAvatarProperties.headMeshRendered.material;
                        skinMaterial.SetFloat("_Tone", skinTone);
                        maleAvatarProperties.headMeshRendered.material = skinMaterial;

                        _maleDataModel.FaceTypeID = data.id;
                    }
                    else
                    {
                        femaleAvatarProperties.headMeshRendered.sharedMesh = Resources.Load<Mesh>(ResourcePath.HeadMeshPath + genderPath + data.meshFileName);
                        femaleAvatarProperties.headMeshRendered.material = Resources.Load<Material>(ResourcePath.HeadMaterialPath + genderPath + data.meshFileName);
                        
                        skinMaterial = femaleAvatarProperties.headMeshRendered.material;
                        skinMaterial.SetFloat("_Tone", skinTone);
                        femaleAvatarProperties.headMeshRendered.material = skinMaterial;
                        
                        _femaleDataModel.FaceTypeID = data.id;
                    }
                    break;
                case AvatarDataList.AvatarDataType.Hair:
                    if (_isMale)
                    {
                        maleAvatarProperties.hairMeshRendered.sharedMesh = Resources.Load<Mesh>(ResourcePath.HairstylesMeshPath + genderPath + data.meshFileName);
                        Material hairMaterial = maleAvatarProperties.hairMeshRendered.material;
                        hairMaterial.color = view.hairColorPicker.CurrentColor;
                        maleAvatarProperties.hairMeshRendered.material = hairMaterial;

                        _maleDataModel.HairStyleID = data.id;
                    }
                    else
                    {
                        femaleAvatarProperties.hairMeshRendered.sharedMesh = Resources.Load<Mesh>(ResourcePath.HairstylesMeshPath + genderPath + data.meshFileName);
                        Material hairMaterial = femaleAvatarProperties.hairMeshRendered.material;
                        hairMaterial.color = view.hairColorPicker.CurrentColor;
                        femaleAvatarProperties.hairMeshRendered.material = hairMaterial;

                        _femaleDataModel.HairStyleID = data.id;
                    }
                    break;
                case AvatarDataList.AvatarDataType.Outfit:
                    if (_isMale)
                    {
                        skinMaterial = maleAvatarProperties.headMeshRendered.material;
                        
                        mats = maleAvatarProperties.bodyMeshRendered.materials;
                        mats[0] = Resources.Load<Material>(ResourcePath.CostumesMaterialPath + genderPath + data.meshFileName);                        
                        mats[1] = skinMaterial;
                        
                        maleAvatarProperties.bodyMeshRendered.sharedMesh = Resources.Load<Mesh>(ResourcePath.CostumesMeshPath + genderPath + data.meshFileName);
                        maleAvatarProperties.bodyMeshRendered.materials = mats;

                        _maleDataModel.OutfitID = data.id;
                    }
                    else
                    {
                        skinMaterial = femaleAvatarProperties.headMeshRendered.material;
                        
                        mats = femaleAvatarProperties.bodyMeshRendered.materials;
                        mats[0] = Resources.Load<Material>(ResourcePath.CostumesMaterialPath + genderPath + data.meshFileName);                        
                        mats[1] = skinMaterial;
                        
                        femaleAvatarProperties.bodyMeshRendered.sharedMesh = Resources.Load<Mesh>(ResourcePath.CostumesMeshPath + genderPath + data.meshFileName);
                        femaleAvatarProperties.bodyMeshRendered.materials = mats;

                        _femaleDataModel.OutfitID = data.id;
                    }
                    break;
                case AvatarDataList.AvatarDataType.Glasses:
                    if (_isMale)
                    {
                        maleAvatarProperties.glassesMeshRendered.sharedMesh = Resources.Load<Mesh>(ResourcePath.GlassesMeshPath + genderPath + data.meshFileName);
                        
                        Material[] matsAcc = maleAvatarProperties.glassesMeshRendered.materials;
                        matsAcc[0] = Resources.Load<Material>(ResourcePath.GlassesMaterialPath + genderPath + data.meshFileName);
                        maleAvatarProperties.glassesMeshRendered.materials = matsAcc;

                        _maleDataModel.AccessoriesGlassesID = data.id;
                    }
                    else
                    {
                        femaleAvatarProperties.glassesMeshRendered.sharedMesh = Resources.Load<Mesh>(ResourcePath.GlassesMeshPath + genderPath + data.meshFileName);
                        
                        Material[] matsAcc = femaleAvatarProperties.glassesMeshRendered.materials;
                        matsAcc[0] = Resources.Load<Material>(ResourcePath.GlassesMaterialPath + genderPath + data.meshFileName);
                        femaleAvatarProperties.glassesMeshRendered.materials = matsAcc;

                        _femaleDataModel.AccessoriesGlassesID = data.id;
                    }
                    break;
            }
        }
        
        private void SelectHairColor(Color color)
        {
            maleAvatarProperties.hairMeshRendered.materials[0].color = color;
            _maleDataModel.HairColor = new []{color.r, color.g, color.b};
            
            femaleAvatarProperties.hairMeshRendered.materials[0].color = color;
            _femaleDataModel.HairColor = new []{color.r, color.g, color.b};
        }
    }
}
