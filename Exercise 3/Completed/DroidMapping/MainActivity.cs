using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Gms.Maps;
using System.Threading.Tasks;

namespace DroidMapping
{
    [Activity(Label = "DroidMapping", MainLauncher = true)]
    public class MainActivity : Activity, IOnMapReadyCallback
    {
        MappingPermissionsHelper permissionHelper;
        MapFragment mapFragment;
        GoogleMap map;
        Task<bool> getLocationPermissionsAsync;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            permissionHelper = new MappingPermissionsHelper(this);
            getLocationPermissionsAsync = permissionHelper.CheckAndRequestPermissions();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            mapFragment = FragmentManager.FindFragmentById(Resource.Id.map) as MapFragment;

            mapFragment.GetMapAsync(this);
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;

            map.MapType = GoogleMap.MapTypeHybrid;

            var hasLocationPermissions = await permissionHelper.CheckAndRequestPermissions();
            map.MyLocationEnabled = hasLocationPermissions;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            permissionHelper.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
