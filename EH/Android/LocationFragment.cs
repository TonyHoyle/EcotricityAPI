using Android.Support.V4.App;
using Android.OS;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Support.V7.App;
using TonyHoyle.EH;

namespace ClockworkHighway.Android
{
    public class LocationFragment : Fragment, IOnMapReadyCallback
    {
        private double _locationLat;
        private double _locationLon;
        private string _locationName;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.location, container, false);
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            int locationId = Arguments.GetInt("locationId");
            _locationLat = Arguments.GetDouble("latitude");
            _locationLon = Arguments.GetDouble("longitude");
            _locationName = Arguments.GetString("name");

            var mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var list = View.FindViewById<ListView>(Resource.Id.connectors);
            var progress = View.FindViewById<ProgressBar>(Resource.Id.progressBar);

            ((AppCompatActivity)Activity).SupportActionBar.Title = _locationName;

            list.EmptyView = progress;

            var eh = SharedData.api;
            var location = await eh.getLocationDetailsAsync(locationId);
            list.Adapter = new LocationAdapter(Context, location);

            list.ItemClick += (sender, args) => { PumpClicked(((LocationAdapter)list.Adapter).GetItem(args.Position)); } ;
        }

        private void PumpClicked(EHApi.LocationDetails location)
        {
            var frag = new StartChargeFragment();
            frag.Arguments = new Bundle();
            frag.Arguments.PutString("location", Newtonsoft.Json.JsonConvert.SerializeObject(location));
            frag.Show(FragmentManager, "StartChargeFragment");
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var camera = new CameraPosition.Builder()
                        .Target(new LatLng(_locationLat, _locationLon))
                        .Zoom(15)
                        .Build();
            var marker = new MarkerOptions()
                        .SetPosition(new LatLng(_locationLat, _locationLon))
                        .SetTitle(_locationName);

            googleMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(camera));
            googleMap.AddMarker(marker);
            // Finding route is a pain involving web calls to google directions API
        }
    }
}
