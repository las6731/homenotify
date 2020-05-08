using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Service.Autofill;
using Android.Support.V4.App;
using Android.Util;
using Firebase.Messaging;
using Newtonsoft.Json;
using Xamarin.Essentials;

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
            SendNotification(message.GetNotification() == null ? null : message.GetNotification().Body, message.Data);
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

            NotificationCompat.Builder notificationBuilder;
            var notificationManager = NotificationManagerCompat.From(this);

            if (data.ContainsKey("topics"))
            {
                var topics = JsonConvert.DeserializeObject<IList<string>>(data["topics"]);
                Dictionary<string, bool> existingTopics = JsonConvert.DeserializeObject<Dictionary<string, bool>>(Preferences.Get("topics", "{}"));
                
                foreach (string topic in topics)
                {
                    FirebaseMessaging.Instance.SubscribeToTopic(topic);
                    
                    existingTopics[topic] = true;

                    Log.Debug(TAG, $"Subscribed to topic: {topic}.");
                }
                
                Preferences.Set("topics", JsonConvert.SerializeObject(existingTopics));
                
                if (Platform.CurrentActivity is MainActivity activity)
                {
                    activity.RunOnUiThread(activity.UpdateAdapter);
                }
                
                notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                    .SetSmallIcon(Resource.Color.transparent)
                    .SetContentTitle("HomeNotify")
                    .SetContentText($"Subscribed to {topics.Count} topics.")
                    .SetAutoCancel(true)
                    .SetContentIntent(pendingIntent);
            }
            else if (data.ContainsKey("unsubscribeTopic"))
            {
                var topic = data["unsubscribeTopic"];
                Dictionary<string, bool> existingTopics = JsonConvert.DeserializeObject<Dictionary<string, bool>>(Preferences.Get("topics", "{}"));
                
                FirebaseMessaging.Instance.UnsubscribeFromTopic(topic);
                existingTopics[topic] = false;
                
                Preferences.Set("topics", JsonConvert.SerializeObject(existingTopics));
                
                if (Platform.CurrentActivity is MainActivity activity)
                {
                    activity.RunOnUiThread(activity.UpdateAdapter);
                }
                
                Log.Debug(TAG, $"Unsubscribed from topic {topic}.");
                notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                    .SetSmallIcon(Resource.Color.transparent)
                    .SetContentTitle("HomeNotify")
                    .SetContentText($"Unsubscribed from topic: {topic}.")
                    .SetAutoCancel(true)
                    .SetContentIntent(pendingIntent);
            }
            else
            {
                notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
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
            }
            
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }
    }
}