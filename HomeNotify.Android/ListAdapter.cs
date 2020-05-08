using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace HomeNotify.Android
{
    public class ListAdapter : BaseAdapter<string>
    {
        public  Dictionary<string, bool> Items;
        private Activity Context;

        public ListAdapter(Activity context, Dictionary<string, bool> items) : base()
        {
            this.Context = context;
            this.Items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override string this[int position]
        {
            get { return Items.Keys.ElementAt(position); }
        }

        public override int Count
        {
            get { return Items.Count;  }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            
            var view = Context.LayoutInflater.Inflate(Android.Resource.Layout.list_item, null);
            var button = view.FindViewById<ToggleButton>(Resource.Id.listItem);
            var topic = Items.Keys.ElementAt(position);
            button.Checked = Items[topic];
            button.Text = topic;
            button.Click += delegate
            {
                Dictionary<string, bool> topics = JsonConvert.DeserializeObject<Dictionary<string, bool>>(Preferences.Get("topics", "{}"));
                if (button.Checked)
                {
                    FirebaseMessaging.Instance.SubscribeToTopic(topic);
                    topics[topic] = true;
                    button.Text = topic;
                    Toast.MakeText(Context, $"Subscribed to topic: {topic}", ToastLength.Short).Show();
                }
                else
                {
                    FirebaseMessaging.Instance.UnsubscribeFromTopic(topic);
                    topics[topic] = false;
                    button.Text = topic;
                    Toast.MakeText(Context, $"Unsubscribed from topic: {topic}", ToastLength.Short).Show();
                }
                Preferences.Set("topics", JsonConvert.SerializeObject(topics));
            };
            button.LongClick += delegate
            {
                var alertBuilder = new AlertDialog.Builder(Context);
                var alert = alertBuilder.Create();
                alert.SetTitle("Delete Topic");
                alert.SetMessage($"Are you sure you want to delete the {topic} topic?");
                alert.SetButton("Delete", (o, ev) =>
                {
                    Dictionary<string, bool> topics = JsonConvert.DeserializeObject<Dictionary<string, bool>>(Preferences.Get("topics", "{}"));
                    FirebaseMessaging.Instance.UnsubscribeFromTopic(topic);
                    topics.Remove(topic);
                    Preferences.Set("topics", JsonConvert.SerializeObject(topics));
                    Toast.MakeText(Context, $"Deleted topic: {topic}", ToastLength.Short).Show();
                    if (Context is MainActivity activity)
                    {
                        activity.RunOnUiThread(activity.UpdateAdapter);
                    }
                });
                alert.SetButton2("Cancel", (o, ev) => {});
                alert.Show();
            };
            return view;
        }
    }
}