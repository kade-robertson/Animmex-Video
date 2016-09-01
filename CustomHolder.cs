using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using AnimmexAPI;

namespace Animmex_Video
{
    public class CustomHolder
    {
        public TextView VideoTitle;
        public ImageView Thumbnail;

        public CustomHolder(View v)
        {
            VideoTitle = v.FindViewById<TextView>(Resource.Id.videoTitle);
            Thumbnail = v.FindViewById<ImageView>(Resource.Id.videoImg);
        }
    }
}