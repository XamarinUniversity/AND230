using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

namespace DroidMapping
{
    [Activity(Label = "DroidMapping", MainLauncher = true)]
    public class MainActivity : Activity
    {
        MappingPermissionsHelper permissionHelper;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            permissionHelper = new MappingPermissionsHelper(this);
            permissionHelper.CheckAndRequestPermissions();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            permissionHelper.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
