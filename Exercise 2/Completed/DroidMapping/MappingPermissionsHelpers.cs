using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Linq;

namespace DroidMapping
{
    class MappingPermissionsHelpers
    {
        Activity CurrentActivity { get; set; }
        const int MappingPermissionsRequestCode = 1;
        static string[] requiredPermissions = new[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation, Manifest.Permission.WriteExternalStorage };
        
        public MappingPermissionsHelpers(Activity activity)
        {
            CurrentActivity = activity;
        }

        public bool HasMappingPermissions()
        {
            return requiredPermissions.All(permission => CurrentActivity.CheckSelfPermission(permission) == Android.Content.PM.Permission.Granted);
        }
        public bool ShouldShowPermissionRationale()
        {
            return requiredPermissions.Any(permission => CurrentActivity.ShouldShowRequestPermissionRationale(permission));
        }

        public void RequestPermissions()
        {
            CurrentActivity.RequestPermissions(requiredPermissions, MappingPermissionsRequestCode);
        }

        public void CheckAndRequestPermissions()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                // Permission in pre-23 Android is given at install-time.
                return;
            }

            if (HasMappingPermissions())
            {
                // Runtime permissions have already been granted.
                return;
            }

            if (ShouldShowPermissionRationale())
            {
                // User previously rejected a permission we need. Show something to explain, with an option that will ask for permission again.
                ShowPermissionRationale();
            }
            else
            {
                // We could also, if we detected the user blocked us, explain we need to send them to Settings to allow permissions.

                // Have Android ask for permissions we need.
                RequestPermissions();
            }
        }

        void ShowPermissionRationale()
        {
            // Show something to explain, with an option that will ask for permission again.
            var permissionExplanationAlert = new AlertDialog.Builder(CurrentActivity)
                .SetMessage($"nameof(DroidMapping) needs location permission to map your position and storage permission for map data caching.Can we have permission to use your location and cache map data ?")
                .SetPositiveButton("Allow location use", (sender, args) =>
                {
                    // User convinced to let us ask again. Have Android ask for permissions we need.
                    RequestPermissions();
                })
                .SetNegativeButton("No thanks", (sender, args) =>
                {
                    var cannotProceedWithoutPermissionAlert = new AlertDialog.Builder(CurrentActivity)
                            .SetMessage("Without permission to use your location, you won't see your position on the map.")
                            .SetPositiveButton("Okay", (s, a) => { })
                            .Create();
                    cannotProceedWithoutPermissionAlert.Show();
                })
                .Create();
            permissionExplanationAlert.Show();
        }

        public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                // Use a request code to know what to look for in the permissions/grantResult arrays.
                case MappingPermissionsRequestCode:
                    if (permissions.Length == 0)
                    {
                        // Request dialog was cancelled/interrupted in some way.
                        // TODO: How does one cancel? (Doesn't fire for home button and back button doesn't do anything.)
                        // TODO: Once we figure out how to trigger it, figure out what an app might do knowing it was cancelled.
                    }
                    if (permissions.Length == 3
                        && grantResults.All(grantResult => grantResult == Permission.Granted))
                    {
                        // We can use whatever things required the requested permissions.
                    }
                    // We could also, optionally, detect if the user has blocked us from asking by checking for `Permission.Denied` and `!ShouldShowRequestPermissionRationale`.
                    break;
                default:
                    break;
            }
        }
    }
}