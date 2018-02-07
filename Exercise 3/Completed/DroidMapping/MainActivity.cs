using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.Maps;

namespace DroidMapping
{
    [Activity(Label = "DroidMapping", MainLauncher = true)]
    public class MainActivity : Activity, IOnMapReadyCallback
    {
        MappingPermissionsHelper permissionHelper;
        MapFragment mapFragment;
        GoogleMap map;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            permissionHelper = new MappingPermissionsHelper(this);
            permissionHelper.CheckAndRequestPermissions();

            mapFragment = FragmentManager.FindFragmentById(Resource.Id.map) as MapFragment;

            mapFragment.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            map.MapType = GoogleMap.MapTypeHybrid;
            map.MyLocationEnabled = true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            permissionHelper.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
