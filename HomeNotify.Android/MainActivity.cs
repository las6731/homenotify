using System.Collections.Generic;
using System.Net.Http;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Gms.Common;
using Android.Util;
using Firebase.Messaging;
using Firebase.Iid;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace HomeNotify.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const string TAG = "MainActivity";
        internal static readonly string CHANNEL_ID = "general_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;

        internal static readonly string API_URL = "http://10.0.0.24:5000"; // set to your API url

        private TextView msgText;
        private ListView topicList;

        private ListAdapter listAdapter;

        private HttpClient httpClient;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            SetContentView (Resource.Layout.activity_main);
            msgText = FindViewById<TextView> (Resource.Id.msgText);

            IsPlayServicesAvailable();

            CreateNotificationChannel();
            
            if (!Preferences.ContainsKey("topics"))
            {
                Preferences.Set("topics", JsonConvert.SerializeObject(new Dictionary<string, bool>()));
            }
            
            topicList = FindViewById<ListView>(Resource.Id.topicList);

            UpdateAdapter();
            
            httpClient = new HttpClient();

            var getTopicsButton = FindViewById<Button>(Resource.Id.getTopicsButton);
            getTopicsButton.Click += delegate
            {
                var response = httpClient.GetAsync($"{API_URL}/topics").Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var topics = JsonConvert.DeserializeObject<string[]>(content);
                var existingTopics =
                    JsonConvert.DeserializeObject<Dictionary<string, bool>>(Preferences.Get("topics", "{}"));
                var before = existingTopics.Count;
                foreach (var topic in topics)
                {
                    if (!existingTopics.ContainsKey(topic))
                        existingTopics[topic] = false;
                }
                Preferences.Set("topics", JsonConvert.SerializeObject(existingTopics));
                
                RunOnUiThread(UpdateAdapter);
                
                var newTopics = existingTopics.Count - before;
                Toast.MakeText(this, $"Retrieved {newTopics} new topics.", ToastLength.Short).Show();
            };
            
            var subscribeButton = FindViewById<Button>(Resource.Id.subscribeButton);
            subscribeButton.Click += delegate
            {
                FirebaseMessaging.Instance.SubscribeToTopic("topics");
                Log.Debug(TAG, "Subscribed to topic notifications.");
            };

            Log.Debug(TAG, "FCM Token: " + FirebaseInstanceId.Instance.Token);
            
            Platform.Init(this, bundle);
        }

        public void UpdateAdapter()
        {
            if (listAdapter == null)
            {
                listAdapter = new ListAdapter(this, JsonConvert.DeserializeObject<Dictionary<string, bool>>(Preferences.Get("topics", "{}")));
                topicList.Adapter = listAdapter;
            }
            else
            {
                listAdapter.Items = JsonConvert.DeserializeObject<Dictionary<string, bool>>(Preferences.Get("topics", "{}"));
                listAdapter.NotifyDataSetChanged();
            }
        }
        
        public bool IsPlayServicesAvailable ()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError (resultCode))
                    msgText.Text = GoogleApiAvailability.Instance.GetErrorString (resultCode);
                else
                {
                    msgText.Text = "This device is not supported";
                    Finish ();
                }
                return false;
            }
            else
            {
                msgText.Text = "Google Play Services is available.";
                return true;
            }
        }
        
        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                "FCM Notifications",
                NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}