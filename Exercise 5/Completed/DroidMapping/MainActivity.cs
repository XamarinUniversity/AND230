using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Threading.Tasks;

namespace DroidMapping
{
    [Activity(Label = "DroidMapping", MainLauncher = true)]
    public class MainActivity : Activity, IOnMapReadyCallback
    {
        MappingPermissionsHelper permissionHelper;
        MapFragment mapFragment;
        GoogleMap map;

        static readonly LatLng Location_Xamarin = new LatLng(37.80, -122.40);
        static readonly LatLng Location_NewYork = new LatLng(40.77, -73.98);
        static readonly LatLng Location_Chicago = new LatLng(41.88, -87.63);
        static readonly LatLng Location_Dallas = new LatLng(32.90, -97.03);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            permissionHelper = new MappingPermissionsHelper(this);
            permissionHelper.CheckAndRequestPermissions();

            permissionHelper.HasLocationPermissionAsync.ContinueWith((getPermissionTask) =>
            {
                if (getPermissionTask.IsCompleted)
                {
                    RunOnUiThread(() =>
                    {
                        // Set our view from the "main" layout resource
                        SetContentView(Resource.Layout.Main);


                        mapFragment = FragmentManager.FindFragmentById(Resource.Id.map) as MapFragment;

                        mapFragment.GetMapAsync(this);
                    });
                }
            });
        }

        public /*async*/ void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            map.MapType = GoogleMap.MapTypeNormal;

            map.AddMarker(new MarkerOptions().SetPosition(Location_NewYork));

            map.AddMarker(new MarkerOptions()
                .SetPosition(Location_Xamarin)
                .SetTitle("Xamarin HQ")
                .SetSnippet("Where the magic happens")
                .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.xamarin_icon)));

            Marker chicagoMarker = map.AddMarker(new MarkerOptions()
                .SetPosition(Location_Chicago)
                .SetTitle("Chicago")
                .Draggable(true)
                .SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow)));

            Marker dallasMarker = map.AddMarker(new MarkerOptions()
                .SetPosition(Location_Dallas)
                .SetTitle("Dallas")
                .SetSnippet("Go Cowboys!")
                .InfoWindowAnchor(1, 0)
                .SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueBlue)));

            map.MarkerClick += delegate (object sender, GoogleMap.MarkerClickEventArgs e) {
                if (e.Marker.Equals(dallasMarker))
                {
                    dallasMarker.Flat = !dallasMarker.Flat;
                    dallasMarker.ShowInfoWindow();
                }
                else
                {
                    // Execute default behavior for other markers.
                    // Could also just execute the following line for every
                    // marker.
                    e.Handled = false;
                }
            };
            map.InfoWindowClick += (sender, e) =>
            {
                if (e.Marker.Id == chicagoMarker.Id)
                {
                    e.Marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRose));
                }
            };
            map.MapClick += (sender, e) =>
            {
                if (!chicagoMarker.IsInfoWindowShown)
                {
                    chicagoMarker.SetIcon(BitmapDescriptorFactory
                        .DefaultMarker(BitmapDescriptorFactory.HueYellow));
                }
            };

            //CameraUpdate update = CameraUpdateFactory.NewLatLngZoom(Location_Xamarin, map.MaxZoomLevel);
            //map.MoveCamera(update);

            map.MapLongClick += (sender, e) => map.AnimateCamera(CameraUpdateFactory.ZoomOut(), 1000, null);

            if (permissionHelper.HasMappingPermissions())
            {
                map.MyLocationEnabled = true;
            }

            //var grantedLocationPermission = await permissionHelper.HasLocationPermissionAsync;
            //map.MyLocationEnabled = grantedLocationPermission;

            //permissionHelper.HasLocationPermissionAsync.ContinueWith((Task<bool> getPermissionTask) =>
            //{
            //    if (getPermissionTask.IsCompleted)
            //    {
            //        this.RunOnUiThread(() =>
            //        {
            //             map.MyLocationEnabled = getPermissionTask.Result;
            //        });
            //    }
            //});
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            permissionHelper.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
