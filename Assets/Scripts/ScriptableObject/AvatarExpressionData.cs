using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarExpressionData", menuName = "ScriptableObjects/AvatarExpressionData")]
public class AvatarExpressionData : ScriptableObject
{

    public List<EmojiModel> emojiList;
    public List<EmoteModel> emoteList;
    
    [Serializable]
    public struct EmojiModel
    {
        public string code;
        public Sprite iconSprite;
        public AudioClip audioSfx;
        public Material particleMaterial;
    }

    [Serializable]
    public struct EmoteModel
    {
        public string name;
        public Sprite iconSprite;
        public bool isCollaborative;
        public AnimationClip clip;
        public AnimationClip expressionClip;

    }
}
