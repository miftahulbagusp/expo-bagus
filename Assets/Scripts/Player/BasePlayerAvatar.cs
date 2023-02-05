using Fusion;
using Settings;
using UnityEngine;

public abstract class BasePlayerAvatar : NetworkBehaviour
{
    [Header("Base player properties")]
    [SerializeField] protected PlayerSettings playerSettings;
    [SerializeField] protected AudioSource voiceAudio;
    [SerializeField] protected AudioSource locomotionAudio;
    [SerializeField] protected Transform avatarRoot;
    [SerializeField] protected NameTag playerNameTag;
    protected Animator BaseAnimator;
    
    // Networked Transform
    [Networked] protected Vector3 networkedPosition { get; set; }
    [Networked] protected Quaternion networkedRotation { get; set; }
    [Networked(OnChanged = nameof(OnScaleChanged))] private float networkedScale { get; set; }
    
    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            LocalUpdate();
        }
        
        NetworkUpdate();
    }

    public override void Spawned()
    {
        Setup();
    }
    
    protected abstract void Setup();
    
    #region Appearance

    public void ChangeScale(float scaleModifier)
    {
        if (Object.HasInputAuthority)
        {
            networkedScale = scaleModifier;
        }
    }

    private static void OnScaleChanged(Changed<BasePlayerAvatar> changed)
    {
        changed.Behaviour.transform.localScale = Vector3.one * changed.Behaviour.networkedScale;
    }

    #endregion Appearance

    #region Behaviour

    protected abstract void LocalUpdate();
    
    protected abstract void NetworkUpdate();

    protected abstract void FootstepSoundEffect();

    #endregion Behaviour
}