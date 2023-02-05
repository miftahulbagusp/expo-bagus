using System.Collections;
using Fusion;
using UnityEngine;

public class NonVRPlayerAvatar : MainPlayerAvatar
{
    // Emote
    private Coroutine _emoteRoutine;
    private Coroutine _interactRoutine;
    
    // Animation
    private float _minRandomActTime = 15f;
    private float _maxRandomActTime = 30f;
    
    // Networked Animation Properties
    [Networked] private bool isIdling { get; set; }
    [Networked] private float timeToRandomAct { get; set; }

    protected override void Setup()
    {
        base.Setup();
        
        NonVRPlayerController.Instance.OnEmojiSelected += OnEmojiSelected;
        NonVRPlayerController.Instance.OnEmoteSelected += OnEmoteSelected;
    }
    
    #region Appearance

    protected override void ConfigAvatar(GameObject avatar)
    {
        base.ConfigAvatar(avatar);

        // Config avatar component
        BaseAnimator = avatar.GetComponent<Animator>();
        avatarProperties = avatar.GetComponent<AvatarProperties>();

        avatar.gameObject.SetActive(true);
        BaseAnimator.enabled = true;
    }

    #endregion Appearance

    #region Behaviour

    protected override void LocalUpdate()
    {
        base.LocalUpdate();
        
        networkedPosition = NonVRPlayerController.Instance.transform.position;
        networkedRotation = NonVRPlayerController.Instance.avatarPivot.transform.rotation;
            
        networkedAnimPropSpeed = NonVRPlayerController.Instance.CurrentMovementSpeed;
        networkedAnimPropDirectionX = NonVRPlayerController.Instance.CurrentMovementDirection.x;
        networkedAnimPropDirectionY = NonVRPlayerController.Instance.CurrentMovementDirection.y;
        networkedAnimPropIsGrounded = NonVRPlayerController.Instance.IsGrounded;
        networkedAnimPropGravityForce = NonVRPlayerController.Instance.GravityForce;
    }

    protected override void NetworkUpdate()
    {
        base.NetworkUpdate();
        
        IdleRandomAct();
    }

    #endregion
    
    #region EmojiAndEmote
    
    private IEnumerator PlayEmote(AnimationClip clip, AnimationClip expressionClip = null, float normalizedTime = 0)
    {
        BaseAnimator.CrossFade("Locomotion", 0.05f);

        yield return null;
        yield return new WaitUntil(() => !BaseAnimator.IsInTransition(0));
        yield return null;
        AnimatorOverrideController["Emote"] = clip;
        if (expressionClip != null) AnimatorOverrideController["Expression_Emote"] = expressionClip;
        BaseAnimator.runtimeAnimatorController = AnimatorOverrideController;

        yield return null;
        if (normalizedTime > 0)
        {
            BaseAnimator.Play("Emote", 0, normalizedTime);
            if (expressionClip != null) BaseAnimator.Play("Expression_Emote", 1, normalizedTime);
            yield return new WaitForSeconds(clip.length - clip.length * normalizedTime);
        }
        else
        {
            BaseAnimator.CrossFade("Emote", 0.1f);
            if (expressionClip != null) BaseAnimator.CrossFade("Expression_Emote", 0.1f);
            yield return new WaitForSeconds(clip.length);
        }

        yield return null;
        _emoteRoutine = null;
    }

    private void OnEmoteSelected(int emoteIndex)
    {
        if (Object.HasInputAuthority)
        {
            if (BaseAnimator.GetCurrentAnimatorClipInfo(0).Length > 0 && !BaseAnimator.IsInTransition(0))
            {
                if (networkedAnimPropIsGrounded && networkedAnimPropSpeed == 0)
                {
                    RPC_PlayEmote(emoteIndex);
                }
            }
        }
    }

    private void JoinCollaborativeEmote(NonVRPlayerAvatar target)
    {
        string animationName = target.BaseAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        float animationTime = target.BaseAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        RPC_JoinCollaborativeEmote(animationName, animationTime);
    }

    private void IdleRandomAct()
    {
        // Idle random animation
        if (isIdling)
        {
            timeToRandomAct -= Time.deltaTime;

            if (timeToRandomAct <= 0)
            {
                // photonView.RPC(nameof(RPC_PlayIdleRandomAct), RpcTarget.All, Random.Range(0, 2));
                BaseAnimator.CrossFade("Idle_RandomAct" + Random.Range(0, 2), 0.1f);
                timeToRandomAct = Random.Range(_minRandomActTime, _maxRandomActTime);
            }
        }

        if (BaseAnimator.GetCurrentAnimatorClipInfo(0).Length != 0)
        {
            if (BaseAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Locomotion_Idle"))
            {
                if (!isIdling)
                {
                    isIdling = true;
                    timeToRandomAct = Random.Range(_minRandomActTime, _maxRandomActTime);
                }
            }
            else
            {
                isIdling = false;
            }
        }
    }

    [Rpc]
    private void RPC_PlayEmote(int emoteIndex)
    {
        AvatarExpressionData.EmoteModel emoteModel = AvatarExpressionData.emoteList[emoteIndex];

        if (_emoteRoutine == null)
        {
            _emoteRoutine = StartCoroutine(PlayEmote(emoteModel.clip, emoteModel.expressionClip));
        }
        else
        {
            if (AnimatorOverrideController["Emote"] != emoteModel.clip && _emoteRoutine != null)
            {
                StopCoroutine(_emoteRoutine);
                _emoteRoutine = StartCoroutine(PlayEmote(emoteModel.clip, emoteModel.expressionClip));
            }
        }
    }


    [Rpc]
    private void RPC_JoinCollaborativeEmote(string animationName, float normalizedTime)
    {
        AvatarExpressionData.EmoteModel emoteModel =
            AvatarExpressionData.emoteList.Find(model => model.clip.name == animationName);
        if (_emoteRoutine == null)
        {
            _emoteRoutine = StartCoroutine(PlayEmote(emoteModel.clip, emoteModel.expressionClip, normalizedTime));
        }
        else
        {
            if (AnimatorOverrideController["Emote"] != emoteModel.clip && _emoteRoutine != null)
            {
                StopCoroutine(_emoteRoutine);
                _emoteRoutine = StartCoroutine(PlayEmote(emoteModel.clip, emoteModel.expressionClip, normalizedTime));
            }
        }
    }

    #endregion EmojiAndEmote
}