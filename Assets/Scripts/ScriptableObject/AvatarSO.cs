using UnityEngine;

[CreateAssetMenu(fileName = "Avatar Data", menuName = "ScriptableObjects/Avatar Data")]
public class AvatarSO : ScriptableObject
{
    public int id;
    public Sprite icon;
    public string meshFileName;
    public string materialFileName;
}
