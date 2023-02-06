using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MainPlayerAvatar : BasePlayerAvatar
{
    private GameObject _avatar;
    [SerializeField] private GameObject dronePrefab;
    
    // Networked Avatar Data
    [Networked] public string networkedNickname { get; set; }
    [Networked] private NetworkBool networkedIsMale { get; set; }
    [Networked] private int networkedFaceTypeID { get; set; }
    [Networked] private int networkedHairStyleID { get; set; }
    [Networked] private Color networkedHairColor { get; set; }
    [Networked] private int networkedOutfitID { get; set; }
    [Networked] private int networkedAccessoriesGlassesID { get; set; }
    [Networked] private float networkedSkinColor { get; set; }
    
    // Animation
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int DirectionX = Animator.StringToHash("directionX");
    private static readonly int DirectionY = Animator.StringToHash("directionY");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int GravityForce = Animator.StringToHash("gravityForce");

    private bool _previousIsGroundedState;
    
    // Networked Animation Properties
    [Networked] protected float networkedAnimPropSpeed { get; set; }
    [Networked] protected float networkedAnimPropDirectionX { get; set; }
    [Networked] protected float networkedAnimPropDirectionY { get; set; }
    [Networked] protected NetworkBool networkedAnimPropIsGrounded { get; set; }
    [Networked] protected float networkedAnimPropGravityForce { get; set; }

    protected AvatarExpressionData AvatarExpressionData;

    protected AvatarProperties avatarProperties;
    protected AnimatorOverrideController AnimatorOverrideController;
    
    // Emoji
    [SerializeField] protected AudioSource emojiSfxAudioSource;
    [SerializeField] protected ParticleSystem emojiParticle;
    
    private void Start()
    {
        // UserInteractionEvents.OnVoiceLockStatus += UserMicControl;
        // UserInteractionEvents.OnVoiceChangeDistance += VoiceChangeDistance;
        // ExperienceEvents.OnConfirmAvatarSelection += ChangeAvatar; 
        // ExperienceEvents.OnBackAvatarSelection += ChangeAvatar;

        AvatarExpressionData = RoomManager.Instance.avatarExpressionData;

        if (BaseAnimator.runtimeAnimatorController.GetType() == typeof(AnimatorOverrideController))
        {
            AnimatorOverrideController femaleAnimator =
                BaseAnimator.runtimeAnimatorController as AnimatorOverrideController;
            List<KeyValuePair<AnimationClip, AnimationClip>> animatorOverrides =
                new List<KeyValuePair<AnimationClip, AnimationClip>>();
            femaleAnimator.GetOverrides(animatorOverrides);
            for (int i = 0; i < femaleAnimator.overridesCount; ++i)
            {
                animatorOverrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(animatorOverrides[i].Key,
                    animatorOverrides[i].Value));
            }

            AnimatorOverrideController = new AnimatorOverrideController(femaleAnimator);

            AnimatorOverrideController.ApplyOverrides(animatorOverrides);
        }
        else
        {
            AnimatorOverrideController = new AnimatorOverrideController
            { runtimeAnimatorController = BaseAnimator.runtimeAnimatorController };
        }

        BaseAnimator.runtimeAnimatorController = AnimatorOverrideController;
    }

    // private void OnDestroy()
    // {
        // UserInteractionEvents.OnVoiceLockStatus -= UserMicControl;
        // UserInteractionEvents.OnVoiceChangeDistance -= VoiceChangeDistance;
        // ExperienceEvents.OnConfirmAvatarSelection -= ChangeAvatar;
        // ExperienceEvents.OnBackAvatarSelection -= ChangeAvatar;
    // }

    protected override void Setup()
    {
        SetupAvatar();
    }

    #region Appearance

    protected void SetupAvatar()
    {
        if (Object.HasInputAuthority)
        {
            networkedNickname = UserDataManager.Instance.userDataModel.Nickname;
            networkedIsMale = UserDataManager.Instance.userDataModel.IsMale;
            networkedFaceTypeID = UserDataManager.Instance.userDataModel.FaceTypeID;
            networkedHairStyleID = UserDataManager.Instance.userDataModel.HairStyleID;
            networkedHairColor = new Color(
                UserDataManager.Instance.userDataModel.HairColor[0],
                UserDataManager.Instance.userDataModel.HairColor[1],
                UserDataManager.Instance.userDataModel.HairColor[2]
            );
            networkedOutfitID = UserDataManager.Instance.userDataModel.OutfitID;
            networkedAccessoriesGlassesID = UserDataManager.Instance.userDataModel.AccessoriesGlassesID;
            networkedSkinColor = UserDataManager.Instance.userDataModel.SkinColor;
        }
        
        UserDataModel userDataModel = new UserDataModel(
            networkedNickname,
            networkedIsMale,
            networkedFaceTypeID,
            networkedHairStyleID,
            new[]{networkedHairColor.r, networkedHairColor.g, networkedHairColor.b},
            networkedOutfitID,
            networkedAccessoriesGlassesID,
            networkedSkinColor
        );

        gameObject.name = networkedNickname + " Avatar";
        playerNameTag.Init(networkedNickname, voiceAudio, Object.HasInputAuthority);
        LoadCustomAvatar(userDataModel);
    }

    protected virtual void LoadCustomAvatar(UserDataModel data)
    {
        var loadedObject = Resources.Load<GameObject>(data.IsMale
            ? ResourcePath.CustomableAvatarMalePath
            : ResourcePath.CustomableAvatarFemalePath
        );
        
        //TODO: Reenable this block when build
        if (_avatar != null)
        {
            Destroy(_avatar);
        }
        _avatar = Instantiate(loadedObject, Vector3.zero, Quaternion.identity);

        ConfigAvatar(_avatar);
        
        SkinnedMeshRenderer headSkinnedMeshRenderer =
            _avatar.transform.Find("Head").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer hairSkinnedMeshRenderer =
            _avatar.transform.Find("Hair").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer bodySkinnedMeshRenderer =
            _avatar.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer accSkinnedMeshRenderer =
            _avatar.transform.Find("Accessories").GetComponent<SkinnedMeshRenderer>();
        
        string genderPath = data.IsMale ? "Male/" : "Female/";
        
        // Use default avatar when data is null. Temporary code
        Debug.Log(data.FaceTypeID);
        // if (data.FaceTypeID == null) return;
        
        headSkinnedMeshRenderer.sharedMesh = Resources.Load<Mesh>( ResourcePath.HeadMeshPath + genderPath + data.FaceTypeID);
        headSkinnedMeshRenderer.material = Resources.Load<Material>( ResourcePath.HeadMaterialPath + genderPath + data.FaceTypeID);
        
        Material skinMaterial = headSkinnedMeshRenderer.material;
        skinMaterial.SetFloat("_Tone", data.SkinColor);
        headSkinnedMeshRenderer.material = skinMaterial;
        
        Material[] mats = bodySkinnedMeshRenderer.materials;
        mats[1] = skinMaterial;
        
        bodySkinnedMeshRenderer.sharedMesh = Resources.Load<Mesh>( ResourcePath.CostumesMeshPath + genderPath + data.OutfitID);
        mats[0] = Resources.Load<Material>(ResourcePath.CostumesMaterialPath  + genderPath + data.OutfitID);
        hairSkinnedMeshRenderer.sharedMesh = Resources.Load<Mesh>(ResourcePath.HairstylesMeshPath  + genderPath + data.HairStyleID);
        
        accSkinnedMeshRenderer.sharedMesh = Resources.Load<Mesh>(ResourcePath.GlassesMeshPath  + genderPath + data.AccessoriesGlassesID);
        
        Material[] matsAcc = accSkinnedMeshRenderer.materials;
        matsAcc[0] = Resources.Load<Material>(ResourcePath.GlassesMaterialPath + genderPath + data.AccessoriesGlassesID);
        accSkinnedMeshRenderer.materials = matsAcc;
        
        bodySkinnedMeshRenderer.materials = mats;
        
        Material hairMaterial = hairSkinnedMeshRenderer.material;
        hairMaterial.color = new Color(data.HairColor[0], data.HairColor[1], data.HairColor[2]);
        hairSkinnedMeshRenderer.material = hairMaterial;
    }

    private void ChangeAvatar()
    {
        RPC_ChangeAvatar();
    }

    [Rpc]
    private void RPC_ChangeAvatar()
    {
        SetupAvatar();
    }
    
    protected virtual void ConfigAvatar(GameObject avatar)
    {
        // Config avatar transform
        avatar.transform.SetParent(avatarRoot.transform);
        avatar.transform.localPosition = new Vector3(0, 0, 0);
        avatar.transform.localRotation = Quaternion.identity;
    }

    #endregion Appearance
    
    #region Behaviour

    protected override void LocalUpdate()
    {

    }

    protected override void NetworkUpdate()
    {
        transform.SetPositionAndRotation(networkedPosition, networkedRotation);

        BaseAnimator.SetFloat(Speed, networkedAnimPropSpeed);
        BaseAnimator.SetFloat(DirectionX, networkedAnimPropDirectionX);
        BaseAnimator.SetFloat(DirectionY, networkedAnimPropDirectionY);
        BaseAnimator.SetBool(IsGrounded, networkedAnimPropIsGrounded);
        BaseAnimator.SetFloat(GravityForce, networkedAnimPropGravityForce);
        
        FootstepSoundEffect();
    }

    protected override void FootstepSoundEffect()
    {
        // Make sure audio state is playing
        if (!locomotionAudio.isPlaying)
        {
            locomotionAudio.Play();
        }

        if (networkedAnimPropIsGrounded)
        {
            if (!_previousIsGroundedState)
            {
                // Define fall volume 
                float fallVolume = -(networkedAnimPropGravityForce + playerSettings.fallForceOffset) /
                                   playerSettings.jumpForce;

                locomotionAudio.PlayOneShot(playerSettings.sfxLanding, fallVolume);
                _previousIsGroundedState = true;

                return;
            }

            _previousIsGroundedState = true;

            if (networkedAnimPropSpeed == 0)
            {
                locomotionAudio.clip = null;
            }
            else if (networkedAnimPropSpeed > 0f && networkedAnimPropSpeed < 0.51f)
            {
                locomotionAudio.clip = playerSettings.sfxWalk;
            }
            else if (networkedAnimPropSpeed >= 0.51f)
            {
                locomotionAudio.clip = playerSettings.sfxRun;

            }
        }
        else
        {
            locomotionAudio.clip = null;
            _previousIsGroundedState = false;
        }
    }

    protected void OnEmojiSelected(int emojiIndex)
    {
        if (Object.HasInputAuthority)
        {
            RPC_PlayEmoji(emojiIndex);
        }
    }
    
    [Rpc]
    private void RPC_PlayEmoji(int emojiIndex)
    {
        AvatarExpressionData.EmojiModel emojiModel = AvatarExpressionData.emojiList[emojiIndex];

        emojiParticle.GetComponent<ParticleSystemRenderer>().material = emojiModel.particleMaterial;
        emojiParticle.Play();

        if (emojiModel.audioSfx != null)
        {
            emojiSfxAudioSource.PlayOneShot(emojiModel.audioSfx);
        }
    }
    
    protected void InstantiateDrone()
    {
        if (!Object.HasInputAuthority) return;

        Runner.Spawn(dronePrefab, Vector3.zero, Quaternion.identity, Runner.LocalPlayer);
    }
    
    #endregion Behaviour
}
