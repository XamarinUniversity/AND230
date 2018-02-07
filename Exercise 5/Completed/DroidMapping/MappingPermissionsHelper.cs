using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Linq;

namespace DroidMapping
{
    public class MappingPermissionsHelper
    {
        readonly Activity currentActivity;
        const int MappingPermissionsRequestCode = 1;
        static string[] requiredPermissions = new[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        
        public MappingPermissionsHelper(Activity activity)
        {
            currentActivity = activity;
        }

        public bool HasMappingPermissions()
        {
            return requiredPermissions.All(permission => currentActivity.CheckSelfPermission(permission) == Permission.Granted);
        }
        public bool ShouldShowPermissionRationale()
        {
            // Has user previously rejected a permission we need. We should show something to explain, with an option that will ask for permission again.
            return requiredPermissions.Any(permission => currentActivity.ShouldShowRequestPermissionRationale(permission));
        }

        public void RequestPermissions()
        {
            currentActivity.RequestPermissions(requiredPermissions, MappingPermissionsRequestCode);
        }

        public void CheckAndRequestPermissions()
        {
            // Only required for Android Nougat and newer.
            if ((int)Build.VERSION.SdkInt < 23) { return; }

            if (HasMappingPermissions()) { return; }

            if (ShouldShowPermissionRationale())
            {
                ShowPermissionRationale();
            }
            else
            {
                RequestPermissions();
            }
        }

        void ShowPermissionRationale()
        {
            // Show something to explain, with an option that will ask for permission again.
            var permissionExplanationAlert = new AlertDialog.Builder(currentActivity)
                .SetMessage($"{nameof(DroidMapping)} needs location permission to map your position. Can we have permission to use your location?")
                .SetPositiveButton("Allow location use", (sender, args) =>
                {
                    // User convinced to let us ask again. Have Android ask for permissions we need.
                    RequestPermissions();
                })
                .SetNegativeButton("No thanks", (sender, args) =>
                {
                    var cannotProceedWithoutPermissionAlert = new AlertDialog.Builder(currentActivity)
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
            if (requestCode != MappingPermissionsRequestCode) { return; }

            if (permissions.Length != requiredPermissions.Length
                || grantResults.Any(grantResult => grantResult == Permission.Denied))
            {
                var cannotProceedWithoutPermissionAlert = new AlertDialog.Builder(currentActivity)
                    .SetMessage("Without permission to use your location, you won't see your position on the map.")
                    .SetPositiveButton("Okay", (s, a) => { })
                    .Create();
                cannotProceedWithoutPermissionAlert.Show();
            }
        }
    }
}