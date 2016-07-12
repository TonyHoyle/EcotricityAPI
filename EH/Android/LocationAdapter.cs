using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using EH.Common;
using Android.Graphics;

namespace EH.Android
{
    class LocationAdapter : ArrayAdapter<EHApi.LocationDetails>
    {
        private class LocationTags : Java.Lang.Object
        {
            public TextView status;
            public TextView pumpId;
            public LinearLayout pumpList;
        }

        public LocationAdapter(Context context, List<EHApi.LocationDetails> locations) : base(context, 0, locations.ToArray())
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            LocationTags tags;

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.LocationLine, parent, false);
                tags = new LocationTags();
                tags.pumpId = view.FindViewById<TextView>(Resource.Id.pumpid);
                tags.status = view.FindViewById<TextView>(Resource.Id.status);
                tags.pumpList = view.FindViewById<LinearLayout>(Resource.Id.pumplist);
                view.Tag = tags;
            }

            tags = (LocationTags)view.Tag;

            var item = GetItem(position);

            tags.pumpId.Text = "Pump "+item.pumpId;
            tags.status.Text = item.status;

            tags.pumpList.RemoveAllViews();
            foreach(var c in item.connector)
                AddConnector(tags.pumpList, c);

            return view;
        }

        private void AddConnector(LinearLayout list, EHApi.Connector connector)
        {
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.LocationPumpLine, list, false);

            var name = view.FindViewById<TextView>(Resource.Id.name);
            var status = view.FindViewById<TextView>(Resource.Id.status);

            Color colour;

            if (connector.compatible.Length > 0) colour = Color.Black;
            else colour = Color.Gray;

            name.SetTextColor(colour);
            status.SetTextColor(colour);

            name.Text = connector.name;
            status.Text = connector.status;

            list.AddView(view);
        }
    }
}