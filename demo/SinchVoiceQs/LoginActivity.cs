using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Com.Sinch.Android.Rtc;

namespace SinchVoiceQs
{
    [Activity(MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class LoginActivity : BaseActivity, SinchService.StartFailedListener
    {
        Button mLoginButton;
        EditText mLoginName;
        ProgressDialog mSpinner;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);

            mLoginName = FindViewById<EditText>(Resource.Id.loginName);
            mLoginButton = FindViewById<Button>(Resource.Id.loginButton);
            mLoginButton.Enabled = (false);
            mLoginButton.Click += delegate
            {
                LoginClicked();
            };
        }

        protected override void OnServiceConnected()
        {
            mLoginButton.Enabled = (true);
            GetSinchServiceInterface().SetStartListener(this);
        }

        protected override void OnPause()
        {
            if (mSpinner != null)
            {
                mSpinner.Dismiss();
            }
            base.OnPause();
        }

        public void OnStartFailed(ISinchError error)
        {
            Toast.MakeText(this, error.ToString(), ToastLength.Long).Show();
            if (mSpinner != null)
            {
                mSpinner.Dismiss();
            }
        }

        public void OnStarted()
        {
            OpenPlaceCallActivity();
        }

        void LoginClicked()
        {
            var userName = mLoginName.Text;

            if (string.IsNullOrWhiteSpace(userName))
            {
                Toast.MakeText(this, "Please enter a name", ToastLength.Long).Show();
                return;
            }

            if (!GetSinchServiceInterface().IsStarted())
            {
                GetSinchServiceInterface().StartClient(userName);
                ShowSpinner();
            }
            else
            {
                OpenPlaceCallActivity();
            }
        }

        void OpenPlaceCallActivity()
        {
            Intent mainActivity = new Intent(this, typeof(PlaceCallActivity));
            StartActivity(mainActivity);
        }

        void ShowSpinner()
        {
            mSpinner = new ProgressDialog(this);
            mSpinner.SetTitle("Logging in");
            mSpinner.SetMessage("Please wait...");
            mSpinner.Show();
        }
    }
}
