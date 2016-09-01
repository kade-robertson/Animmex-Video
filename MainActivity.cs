using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using AnimmexAPI;
using System.Net;
using Java.Security.Cert;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;

namespace Animmex_Video
{
    [Activity(Label = "Animmex Video", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        AnimmexClient api;
        List<AnimmexVideo> vids;
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            SearchView sb = FindViewById<SearchView>(Resource.Id.searchView1);
            sb.SetQueryHint("Seach..");
            sb.QueryTextSubmit += QuerySubmitted;
            api = new AnimmexClient(UserAgent.Chrome);
        }

        public async void QuerySubmitted(object sender, EventArgs e)
        {
            try {
                Toast.MakeText(this, "Searching...", ToastLength.Short).Show();
                var videos = await api.Search(((SearchView)sender).Query);
                vids = videos;

                ListView lv = FindViewById<ListView>(Resource.Id.listView1);
                lv.Adapter = new CustomAdapter(this, Resource.Layout.Main, videos);
                lv.ItemClick += ItemClick;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "ERROR: " + ex.ToString(), ToastLength.Long).Show();
            }
        }

        public async void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var links = await api.GetDirectVideoLinks(vids[e.Position]);
            Toast.MakeText(this, links.BestQualityStream, ToastLength.Long).Show();
        }
    }
}

