using System.Collections.Generic;
#if UNITY_IOS
using Unity.Notifications.iOS;
using NotificationSamples.iOS;
#elif UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

public enum NotiType
{
    NONE,
    DAILY_12_AM,
    DAILY_9_PM,
    DAILY_RW_AVAILABLE_12,
    DAILY_RW_AVAILABLE_9,
    OFFLINE_3_DAY,
    STAGE_REQUIRE_12,
    STAGE_REQUIRE_9,
}
[CreateAssetMenu]
public class NotificationConfig : ScriptableObject
{

    [System.Serializable]
    public class NotifyData
    {
        [Header("-----CHANNEL----------------------------------------------")]
        [SerializeField]
        private string channelID;
        [SerializeField]
        private string channelName;
        [SerializeField]
        private string channelDes;
        [SerializeField]
        private bool canShowBadge;
        [SerializeField]
        private bool canBypassDnd;
        [SerializeField]
        private bool enableLights;
        [SerializeField]
        private bool enableVibration;
        [SerializeField]
        private long[] vibrationPattern;
        [SerializeField]
        private int explicitID;

        [Header("NOTIFICATION")]
        [SerializeField]
        private string title;
        [SerializeField]
        private string content;
        [SerializeField]
        private string group;
        [SerializeField]
        private string smallIconName;
        [SerializeField]
        private string largeIconName;
        [SerializeField]
        private NotiType notiType;

        public string ChannelID { get => channelID; set => channelID = value; }
        public string ChannelName { get => channelName; set => channelName = value; }
        public string ChannelDes { get => channelDes; set => channelDes = value; }
        public string Title { get => title; set => title = value; }
        public string Content { get => content; set => content = value; }
        public string Group { get => group; set => group = value; }
        public string SmallIconName { get => smallIconName; set => smallIconName = value; }
        public string LargeIconName { get => largeIconName; set => largeIconName = value; }
        public NotiType NotiType { get => notiType; set => notiType = value; }
    }
    

    #region PARAMS
    [SerializeField]
    public List<NotifyData> dataList = new List<NotifyData>(0);
    #endregion

    #region METHODS

    public NotifyData FindByType(NotiType type)
    {
        if (dataList.Count > 0)
        {
            return dataList.Find(x => x.NotiType == type);
        }
        return null;
    }
    #endregion

}
