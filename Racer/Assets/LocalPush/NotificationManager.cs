using System;
using UnityEngine;
using SeganX;

#if UNITY_ANDROID && !UNITY_EDITOR

using System.Linq;

#endif

namespace LocalPush
{
    public enum NotificationType { FullFuel, FreePackage, LeagueStart, LegendStore }

    public static class NotificationManager
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        private const string FullClassName = "com.hippogames.simpleandroidnotifications.Controller";
        private const string MainActivityClassName = "com.unity3d.player.UnityPlayerActivity";

#endif

        /// <summary>
        /// Schedule simple notification without app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public static int Send(double delay, NotificationType notificationType, NotificationIcon smallIcon = 0)
        {
            if (GetLastNotificationSendId(notificationType) >= 0)
                Cancel(notificationType);

            return SendCustom(new NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = TimeSpan.FromSeconds(delay),
                Title = GetTitle(notificationType),
                Message = GetMessage(notificationType),
                Ticker = GetMessage(notificationType),
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = new Color(.8f, 0, 0),
                LargeIcon = ""
            });
        }

        /// <summary>
        /// Schedule notification with app icon.
        /// </summary>
        /// <param name="smallIcon">List of build-in small icons: notification_icon_bell (default), notification_icon_clock, notification_icon_heart, notification_icon_message, notification_icon_nut, notification_icon_star, notification_icon_warning.</param>
        public static int SendWithAppIcon(double delay, NotificationType notificationType, NotificationIcon smallIcon = 0)
        {
            if (GetLastNotificationSendId(notificationType) >= 0)
                Cancel(notificationType);

            return SendCustom(new NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = TimeSpan.FromSeconds(delay),
                Title = GetTitle(notificationType),
                Message = GetMessage(notificationType),
                Ticker = GetMessage(notificationType),
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = smallIcon,
                SmallIconColor = new Color(.8f, 0, 0),
                LargeIcon = "app_icon"
            });
        }

        /// <summary>
        /// Schedule customizable notification.
        /// </summary>
        public static int SendCustom(NotificationParams notificationParams)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

            var p = notificationParams;
            var delay = (long) p.Delay.TotalMilliseconds;

            new AndroidJavaClass(FullClassName).CallStatic("SetNotification", p.Id, delay, p.Title, p.Message, p.Ticker,
                p.Sound ? 1 : 0, p.Vibrate ? 1 : 0, p.Light ? 1 : 0, p.LargeIcon, GetSmallIconName(p.SmallIcon), ColotToInt(p.SmallIconColor), MainActivityClassName);

#else

            //Debug.LogWarning("Simple Android Notifications are not supported for current platform. Build and play this scene on android device!");

#endif

            return notificationParams.Id;
        }

        /// <summary>
        /// Cancel notification by id.
        /// </summary>
        public static void Cancel(NotificationType notificationType)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

            new AndroidJavaClass(FullClassName).CallStatic("CancelScheduledNotification", GetLastNotificationSendId(notificationType));

#endif
        }

        /// <summary>
        /// Cancel all notifications.
        /// </summary>
        public static void CancelAll()
        {
#if UNITY_ANDROID && !UNITY_EDITOR

            new AndroidJavaClass(FullClassName).CallStatic("CancelAllScheduledNotifications");

#endif
        }

        private static int ColotToInt(Color color)
        {
            var smallIconColor = (Color32)color;

            return smallIconColor.r * 65536 + smallIconColor.g * 256 + smallIconColor.b;
        }

        private static string GetSmallIconName(NotificationIcon icon)
        {
            return "anp_" + icon.ToString().ToLower();
        }

        private static string GetTitle(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.FullFuel:
                    return LocalizationService.Get(111080);
                case NotificationType.FreePackage:
                    return LocalizationService.Get(111081);
                case NotificationType.LeagueStart:
                    return LocalizationService.Get(111082);
                case NotificationType.LegendStore:
                    return LocalizationService.Get(111083);
                default:
                    Debug.LogError("Please add notificaitonType: " + notificationType);
                    return null;
            }
        }

        private static string GetMessage(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.FullFuel:
                    return LocalizationService.Get(111090);
                case NotificationType.FreePackage:
                    return LocalizationService.Get(111091);
                case NotificationType.LeagueStart:
                    return LocalizationService.Get(111092);
                case NotificationType.LegendStore:
                    return LocalizationService.Get(111093);
                default:
                    Debug.LogError("Please add notificaitonType: " + notificationType);
                    return null;
            }
        }

        private static string GetLastNotificationSendIdString(NotificationType notificationType) { return "LastNotification_" + notificationType.ToString(); }
        private static int GetLastNotificationSendId(NotificationType notificationType)
        {
            return PlayerPrefs.GetInt(GetLastNotificationSendIdString(notificationType), -1);
        }
        private static void GetLastNotificationSendId(NotificationType notificationType, int sendId)
        {
            PlayerPrefs.SetInt(GetLastNotificationSendIdString(notificationType), sendId);
        }
    }
}