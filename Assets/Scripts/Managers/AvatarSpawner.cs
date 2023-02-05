using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AvatarSpawner : SimulationBehaviour, ISpawned
{
    [SerializeField] private NonVRPlayerAvatar nonVRAvatar;

    public void Spawned()
    {
        if (RoomManager.Instance.GameIsRunning)
            InstantiateNetworkAvatar(Runner.LocalPlayer);
    }
    
    public void InstantiateNetworkAvatar(PlayerRef playerRef)
    {
            var avatarObject = Runner.Spawn(nonVRAvatar, Vector3.zero, Quaternion.identity, playerRef);
            Runner.SetPlayerObject(playerRef, avatarObject.Object);
            
            NonVRPlayerController.Instance.OnScaleChanged += avatarObject.ChangeScale;
            NonVRPlayerController.Instance.ChangeScale(RoomManager.Instance.defaultPlayerSize);
    }
}