using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if UNITY_IOS
using Unity.Advertisement.IosSupport;
//#endif

public class AppTrackingTransparencyHelper : MonoBehaviour
{
    [SerializeField] bool initOnStart = true;
    public static Action<int> onTrackingStatusReceived;

    // Start is called before the first frame update
    void Start()
    {
        if (initOnStart) Init(null);
    }

    public static void Init(Action<int> onTrackingStatusReceived)
    {
#if UNITY_IOS
        AppTrackingTransparencyHelper.onTrackingStatusReceived += onTrackingStatusReceived;
        // Check the user's consent status.
        // If the status is undetermined, display the request request:
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED && Application.platform == RuntimePlatform.IPhonePlayer)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking(AuthorizationTrackingReceived);
        }
        else
        {
            AuthorizationTrackingReceived((int)ATTrackingStatusBinding.GetAuthorizationTrackingStatus());
        }
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        Debug.Log($"ATTracking status: {status}");
#else
        onTrackingStatusReceived?.Invoke(0);
#endif
    }

    private static void AuthorizationTrackingReceived(int status)
    {
        Debug.LogFormat("Tracking status received: {0}", status);
        onTrackingStatusReceived?.Invoke(status);
    }
}
