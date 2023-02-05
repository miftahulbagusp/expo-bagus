using UnityEngine;


public class AvatarProperties : MonoBehaviour
{
    public Transform grabLocation;

    public SkinnedMeshRenderer headMeshRendered;
    public SkinnedMeshRenderer hairMeshRendered;
    public SkinnedMeshRenderer bodyMeshRendered;
    public SkinnedMeshRenderer glassesMeshRendered;

    public void SetAvatarToController(ref SkinnedMeshRenderer head, ref SkinnedMeshRenderer hair, ref SkinnedMeshRenderer body, ref SkinnedMeshRenderer glasses)
    {
        head = headMeshRendered;
        hair = hairMeshRendered;
        body = bodyMeshRendered;
        glasses = glassesMeshRendered;
    }
}
