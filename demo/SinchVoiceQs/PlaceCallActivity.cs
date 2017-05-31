using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Widget;
using Com.Sinch.Android.Rtc;
using Com.Sinch.Android.Rtc.Calling;

namespace SinchVoiceQs
{
	[Activity(MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class PlaceCallActivity : BaseActivity
    {
        Button mCallButton;
        EditText mCallName;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main);

            mCallName = FindViewById<EditText>(Resource.Id.callName);
            mCallButton = FindViewById<Button>(Resource.Id.callButton);
            mCallButton.Enabled = (false);
            mCallButton.Click += delegate
            {
                CallButtonClicked();
            };

            Button stopButton = FindViewById<Button>(Resource.Id.stopButton);
            stopButton.Click += delegate
            {
                StopButtonClicked();
            };
        }

        protected override void OnServiceConnected()
        {
            TextView userName = FindViewById<TextView>(Resource.Id.loggedInName);
            userName.Text = (GetSinchServiceInterface().GetUserName());
            mCallButton.Enabled = (true);
        }

        void StopButtonClicked()
        {
            if (GetSinchServiceInterface() != null)
            {
                GetSinchServiceInterface().StopClient();
            }
            Finish();
        }

        void CallButtonClicked()
        {
            var userName = mCallName.Text;
            if (string.IsNullOrWhiteSpace(userName))
            {
                Toast.MakeText(this, "Please enter a user to call", ToastLength.Long).Show();
                return;
            }

            try
            {
                ICall call = GetSinchServiceInterface().CallUser(userName);
                if (call == null)
                {
                    // Service failed for some reason, show a Toast and abort
                    Toast.MakeText(this, "Service is not started. Try stopping the service and starting it again before placing a call.", ToastLength.Long).Show();
                    return;
                }
                var callId = call.CallId;
                Intent callScreen = new Intent(this, typeof(CallScreenActivity));
                callScreen.PutExtra(SinchService.CALL_ID, callId);

                StartActivity(callScreen);
            }
            catch (MissingPermissionException e)
            {
                ActivityCompat.RequestPermissions(this, new String[] { e.RequiredPermission }, 0);
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
            {
                Toast.MakeText(this, "You may now place a call", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, 
                               "This application needs permission to use your microphone to function properly.",
                               ToastLength.Long)
                     .Show();
            }
        }
    }
}
