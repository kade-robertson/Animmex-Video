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
using Square.Picasso;

namespace Animmex_Video
{
    public class CustomAdapter : ArrayAdapter
    {
        private Context context;
        private List<AnimmexVideo> videos;
        private LayoutInflater inflater;
        private int resource;

        public CustomAdapter(Context context, int resource, List<AnimmexVideo> videos) : base(context, resource, videos)
        {
            this.context = context;
            this.resource = resource;
            this.videos = videos;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (inflater == null) inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            if (convertView == null) convertView = inflater.Inflate(resource, parent, false);

            var holder = new CustomHolder(convertView);
            var video = videos[position];
            holder.VideoTitle.Text = $"{video.Title}{System.Environment.NewLine}{video.Duration}";
            Picasso.With(context).Load(video.ThumbnailURL).Into(holder.Thumbnail);

            return convertView;
        }
    }
}