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
        DirectLinks links;
        bool use_cache = true;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            SearchView sb = FindViewById<SearchView>(Resource.Id.searchView1);
            Switch sw = FindViewById<Switch>(Resource.Id.switch1);
            sw.CheckedChange += UseCacheChanged;
            sb.SetQueryHint("Seach..");
            sb.QueryTextSubmit += QuerySubmitted;
            api = new AnimmexClient(UserAgent.Chrome);
            ListView lv = FindViewById<ListView>(Resource.Id.listView1);
            lv.ItemClick += ItemClick;
        }

        public void UseCacheChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var sw = (Switch)sender;
            use_cache = sw.Checked;
        }

        public async void QuerySubmitted(object sender, EventArgs e)
        {
            try {
                Toast.MakeText(this, "Searching...", ToastLength.Short).Show();
                var videos = await api.Search(((SearchView)sender).Query);
                vids = videos;
                ListView lv = FindViewById<ListView>(Resource.Id.listView1);
                lv.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, videos.Select(vid => vid.Title).ToList());
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "ERROR: " + ex.ToString(), ToastLength.Long).Show();
            }
        }

        public async void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Finding links...");
                progress.SetCancelable(false);
                progress.Show();
                var video = vids[e.Position];
                if (use_cache) {
                    try { links = await api.GetCachedVideoLinks(video); }
                    catch { links = await api.GetDirectVideoLinks(video); }
                } else {
                    try { links = await api.GetDirectVideoLinks(video); }
                    catch { links = await api.GetCachedVideoLinks(video); }
                }
                progress.Dismiss();
                // Toast.MakeText(this, links.BestQualityStream, ToastLength.Long).Show();
                var options = new List<string>();
                if (links.Stream2160p != "") options.Add("2160p");
                if (links.Stream1440p != "") options.Add("1440p");
                if (links.Stream1080p != "") options.Add("1080p");
                if (links.Stream720p != "") options.Add("720p");
                if (links.StreamSD != "") options.Add("SD");

                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Streaming Options");
                builder.SetItems(options.ToArray(), delegate (object xsender, DialogClickEventArgs xe)
                {
                    var intent = new Intent(Intent.ActionView);
                    switch (options[xe.Which])
                    {
                        case "2160p":
                            intent.SetDataAndType(Android.Net.Uri.Parse(links.Stream2160p), "video/*");
                            break;
                        case "1440p":
                            intent.SetDataAndType(Android.Net.Uri.Parse(links.Stream1440p), "video/*");
                            break;
                        case "1080p":
                            intent.SetDataAndType(Android.Net.Uri.Parse(links.Stream1080p), "video/*");
                            break;
                        case "720p":
                            intent.SetDataAndType(Android.Net.Uri.Parse(links.Stream720p), "video/*");
                            break;
                        case "SD":
                            intent.SetDataAndType(Android.Net.Uri.Parse(links.StreamSD), "video/*");
                            break;
                    }

                    StartActivity(Intent.CreateChooser(intent, "Complete action using..."));
                });
                var alert = builder.Create();
                alert.Show();
            } catch (Exception ex) {
                Toast.MakeText(this, "An error occurred, try again.", ToastLength.Short);
            }
        }
    }
}

