using Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

namespace DroidMapping
{
    [Activity(Label = "DroidMapping", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        MappingPermissionsHelpers permissionHelper;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            permissionHelper = new MappingPermissionsHelpers(this);
            permissionHelper.CheckAndRequestPermissions();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            permissionHelper.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
