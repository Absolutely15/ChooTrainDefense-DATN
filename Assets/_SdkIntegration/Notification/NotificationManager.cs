using System;
using System.Collections;
using ATSoft;
#if USE_NOTIFICATION
using NotificationSamples;
#if UNITY_IOS
using Unity.Notifications.iOS;
#elif UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#endif
using UnityEngine;

public class NotificationManager : Singleton<NotificationManager>, IService
{
#if USE_NOTIFICATION
    [SerializeField] private NotificationConfig data;
    [SerializeField] private GameNotificationsManager manager;
    //[SerializeField] private string[] contents;

    [HideInInspector] public bool initialized;

    private string channelId = "default_channel";
    private const string keyMonthNotify = "MonthNotify";
    private const string keyYeahNotify = "YeahNotify";
    
    private int monthNotify;
    private int yeahNotify;
    private int target12am = 12;
    private int target9pm = 21; //9pm

    int currentDay = DateTime.Now.Day;
    int currentMonth = DateTime.Now.Month;
    int currentYear = DateTime.Now.Year;
    private DateTime dateValue;
#endif
    public void Initialize()
    {
#if USE_NOTIFICATION
#if UNITY_EDITOR
        Debug.Log("=== Unity Editor Initialize Notification ===");
        return;
#endif
        if (!initialized)
            OnInitChannelNoti();
        OnInitSchedule();
#endif
    }

#if USE_NOTIFICATION
    private void OnInitSchedule()
    {
        monthNotify = PlayerPrefs.GetInt(keyMonthNotify, 0);
        yeahNotify = PlayerPrefs.GetInt(keyYeahNotify, 0);

        if (monthNotify == 0 && yeahNotify == 0)
        {
            ScheduleNotification();
        }
        else
        {
            if (System.DateTime.Now.Year > yeahNotify)
            {
                ScheduleNotification();
            }
            else if (System.DateTime.Now.Year == yeahNotify && System.DateTime.Now.Month > monthNotify)
            {
                ScheduleNotification();
            }
        }

#if UNITY_ANDROID
        var adNotiIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

        if (adNotiIntentData != null)
        {
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
        }
#elif UNITY_IOS
    var adNotiIntentData = iOSNotificationCenter.GetLastRespondedNotification();

        if (adNotiIntentData != null)
        {
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
        }
#endif
    }

    private void OnApplicationPause(bool isPause)
    {
        if (isPause)
        {
            DateTime fireTime = System.DateTime.Now
                .AddDays(3)
                .AddHours(-System.DateTime.Now.Hour)
                .AddMinutes(-System.DateTime.Now.Minute)
                .AddSeconds(-System.DateTime.Now.Second);

            SendNotification(NotiType.OFFLINE_3_DAY, fireTime.AddHours(12));
            SendNotification(NotiType.OFFLINE_3_DAY, fireTime.AddHours(21));
        }
    }

    #region Setup channel and send noti method

    private void OnInitChannelNoti()
    {
        GameNotificationChannel[] listChannels = new GameNotificationChannel[data.dataList.Count];

        for (int i = 0; i < data.dataList.Count; i++)
        {
            var notifyData = data.dataList[i];
            listChannels[i] =
                new GameNotificationChannel(notifyData.ChannelID, notifyData.ChannelName, notifyData.ChannelDes);
        }

        manager.Initialize(listChannels);
        initialized = true;
    }

    private void SendNotification(NotiType notiType, DateTime fireTime)
    {
        try
        {
            var notifyData = data.FindByType(notiType);
            if (notifyData == null)
                return;
            
            string content = notifyData.Content;
            //string content = contents[UnityEngine.Random.Range(0, contents.Length)];
            SendNotification(notifyData.Title, content, fireTime, null, false, notifyData.ChannelID, notifyData.SmallIconName,
                notifyData.LargeIconName);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    #endregion

    #region ScheduleNoti

    private void ScheduleNotification()
    {
        if (!initialized)
            OnInitChannelNoti();

        StartCoroutine(NotificationSchedule());
    }

    private IEnumerator NotificationSchedule()
    {
        manager.CancelAllNotifications();

        int currentHourOfTheDay = System.DateTime.Now.Hour;
        int lastDayOfMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        int offset = lastDayOfMonth - (currentDay - 1);
        int dayOfMonth = currentDay;
        
        yeahNotify = currentYear;
        monthNotify = currentMonth;
        ScheduleNotiSaveData();

        for (int i = 1; i <= offset; i++)
        {
            if (dayOfMonth <= lastDayOfMonth)
            {
                dateValue = new DateTime(currentYear, currentMonth, dayOfMonth);
                if (dayOfMonth == DateTime.Now.Day)
                {
                    SetUpNoti12amAnd9pm(currentHourOfTheDay < 12 ? NotiType.DAILY_12_AM : NotiType.NONE,
                        NotiType.DAILY_9_PM);
                }
                else
                {
                    SetUpNoti12amAnd9pm(NotiType.DAILY_12_AM, NotiType.DAILY_9_PM);
                }

                dayOfMonth++;
            }
        }

        yield return null;
    }

    #endregion

    #region SetupToSendNoti

    private void SetUpNoti12amAnd9pm(NotiType type12, NotiType type9, bool isNexMonth = false)
    {
        if (type12 == NotiType.NONE)
        {
            SendNotification(type9, FireTime(target9pm, isNexMonth));
        }
        else
        {
            SendNotification(type12, FireTime(target12am, isNexMonth));
            SendNotification(type9, FireTime(target9pm, isNexMonth));
        }
    }

    private DateTime FireTime(int hourTarget, bool isNexMonth = false)
    {
        if (isNexMonth)
        {
            return System.DateTime.Now
                .AddMonths(1)
                .AddDays(dateValue.Day - currentDay)
                .AddHours(hourTarget - System.DateTime.Now.Hour)
                .AddMinutes(-System.DateTime.Now.Minute)
                .AddSeconds(-System.DateTime.Now.Second);
        }

        return System.DateTime.Now
            .AddDays(dateValue.Day - currentDay)
            .AddHours(hourTarget - System.DateTime.Now.Hour)
            .AddMinutes(-System.DateTime.Now.Minute)
            .AddSeconds(-System.DateTime.Now.Second);
    }

    #endregion

    #region Save schedule

    private void ScheduleNotiSaveData()
    {
        PlayerPrefs.SetInt(keyMonthNotify, monthNotify);
        PlayerPrefs.SetInt(keyYeahNotify, yeahNotify);
    }

    #endregion

    #region common

    /// <summary>
    /// Queue a notification with the given parameters.
    /// </summary>
    /// <param name="title">The title for the notification.</param>
    /// <param name="body">The body text for the notification.</param>
    /// <param name="deliveryTime">The time to deliver the notification.</param>
    /// <param name="badgeNumber">The optional badge number to display on the application icon.</param>
    /// <param name="reschedule">
    /// Whether to reschedule the notification if foregrounding and the notification hasn't yet been shown.
    /// </param>
    /// <param name="channelId">Channel ID to use. If this is null/empty then it will use the default ID. For Android
    /// the channel must be registered in <see cref="GameNotificationsManager.Initialize"/>.</param>
    /// <param name="smallIcon">Notification small icon.</param>
    /// <param name="largeIcon">Notification large icon.</param>
    private void SendNotification(string title, string body, DateTime deliveryTime, int? badgeNumber = null,
        bool reschedule = false, string channelId = null,
        string smallIcon = null, string largeIcon = null)
    {
        IGameNotification notification = manager.CreateNotification();

        if (notification == null)
        {
            return;
        }

        notification.Title = title;
        notification.Body = body;
        notification.Group = !string.IsNullOrEmpty(channelId) ? channelId : this.channelId;
        notification.DeliveryTime = deliveryTime;
        notification.SmallIcon = smallIcon;
        notification.LargeIcon = largeIcon;
        if (badgeNumber != null)
        {
            notification.BadgeNumber = badgeNumber;
        }

        manager.ScheduleNotification(notification);
    }

    /// <summary>
    /// Cancel a given pending notification
    /// </summary>
    private void CancelPendingNotificationItem(PendingNotification itemToCancel)
    {
        manager.CancelNotification(itemToCancel.Notification.Id.Value);
    }

    #endregion
#endif
}

public class ScheduleNotificationProfile
{
    public int month;
    public int year;

    public ScheduleNotificationProfile(int month, int year)
    {
        this.month = month;
        this.year = year;
    }
}
