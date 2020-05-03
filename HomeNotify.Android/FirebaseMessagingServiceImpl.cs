using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Util;
using Firebase.Messaging;

namespace HomeNotify.Android
{
    [Service(Name = "com.lshort.homenotify.FirebaseMessagingServiceImpl")]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseMessagingServiceImpl : FirebaseMessagingService
    {
        private const string TAG = "FirebaseMessagingServiceImpl";
        public override void OnNewToken(string token)
        {
            Log.Debug(TAG, "Refreshed token: " + token);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            SendNotification(message.GetNotification().Body, message.Data);
        }
        
        void SendNotification(string messageBody, IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            var pendingIntent = PendingIntent.GetActivity(this,
                MainActivity.NOTIFICATION_ID,
                intent,
                PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                .SetSmallIcon(Resource.Color.transparent)
                .SetContentTitle("HomeNotify")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);
            
            /*
                TODO: use message data to make actions, etc
                intention is to use ONLY data payloads so all notifications are built manually
                (notification payloads don't pass through OnMessageReceived when app is in background)
            */

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }
    }
}