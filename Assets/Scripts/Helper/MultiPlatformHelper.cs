using UnityEngine;

public class MultiPlatformHelper : MonoBehaviour
{
    public static MultiPlatformHelper Instance;

    public enum EDeviceType
    {
        Desktop,
        Android,
    }
    
    public EDeviceType DeviceType { get; set; }

#if UNITY_EDITOR
    [Header("Debug Mode")]
    public bool simulateDevice;
    public EDeviceType simulatedDeviceType;
#endif

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        SetDeviceTypeFromSystem();
        
#if UNITY_EDITOR
        if (simulateDevice)
        {
            DeviceType = simulatedDeviceType;
        }
#endif
    }

    private void SetDeviceTypeFromSystem()
    {
        Debug.Log($"Model: {SystemInfo.deviceModel}. " +
                  $"Name: {SystemInfo.deviceName}. " +
                  $"Type: {SystemInfo.deviceType.ToString()}. " +
                  $"Unique Identifier: {SystemInfo.deviceUniqueIdentifier}"
        );

        switch (SystemInfo.deviceType)
        {
            case UnityEngine.DeviceType.Handheld:
                DeviceType = EDeviceType.Android;
                break;
            default:
                DeviceType = EDeviceType.Desktop;
                break;
        }
    }

    public bool IsPlayingOnDesktop()
    {
        return DeviceType == EDeviceType.Desktop;
    }
    
    public bool IsPlayingOnAndroid()
    {
        return DeviceType == EDeviceType.Android;
    }
}