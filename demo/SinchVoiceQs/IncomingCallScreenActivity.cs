using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Sinch.Android.Rtc;
using Com.Sinch.Android.Rtc.Calling;

namespace SinchVoiceQs
{
	[Activity(MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class IncomingCallScreenActivity : BaseActivity
    {

        static readonly string TAG = nameof(IncomingCallScreenActivity);
        String mCallId;
        AudioPlayer mAudioPlayer;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.incoming);

            Button answer = FindViewById<Button>(Resource.Id.answerButton);
            answer.Click += (sender, e) => onClick(sender as View);
            Button decline = FindViewById<Button>(Resource.Id.declineButton);
            decline.Click += (sender, e) => onClick(sender as View);

            mAudioPlayer = new AudioPlayer(this);
            mAudioPlayer.PlayRingtone();
            mCallId = Intent.GetStringExtra(SinchService.CALL_ID);
        }

        protected override void OnServiceConnected()
        {
            ICall call = GetSinchServiceInterface().GetCall(mCallId);
            if (call != null)
            {
                call.AddCallListener(new SinchCallListener(this));
                TextView remoteUser = FindViewById<TextView>(Resource.Id.remoteUser);
                remoteUser.Text = (call.RemoteUserId);
            }
            else
            {
                Log.Error(TAG, "Started with invalid callId, aborting");
                Finish();
            }
        }

        void AnswerClicked()
        {
            mAudioPlayer.StopRingtone();
            ICall call = GetSinchServiceInterface().GetCall(mCallId);
            if (call != null)
            {
                try
                {
                    call.Answer();
                    Intent intent = new Intent(this, typeof(CallScreenActivity));
                    intent.PutExtra(SinchService.CALL_ID, mCallId);


                    StartActivity(intent);
                }
                catch (MissingPermissionException e)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { e.RequiredPermission }, 0);
                }
            }
            else
            {

                Finish();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            if (grantResults[0] == Permission.Granted)
            {
                Toast.MakeText(this, "You may now answer the call", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "This application needs permission to use your microphone to function properly.", ToastLength.Long)
                     .Show();
            }
        }

        void DeclineClicked()
        {
            mAudioPlayer.StopRingtone();
            ICall call = GetSinchServiceInterface().GetCall(mCallId);
            if (call != null)
            {
                call.Hangup();
            }
            Finish();
        }

        private class SinchCallListener : Java.Lang.Object, ICallListener
        {
            readonly IncomingCallScreenActivity outer;

            public SinchCallListener(IncomingCallScreenActivity outer)
            {
                this.outer = outer;
            }

            public void OnCallEnded(ICall call)
            {
                CallEndCause cause = call.Details.EndCause;
                Log.Debug(TAG, "Call ended, cause: " + cause);
                outer.mAudioPlayer.StopRingtone();
                outer.Finish();
            }

            public void OnCallEstablished(ICall call)
            {
                Log.Debug(TAG, "Call established");
            }

            public void OnCallProgressing(ICall call)
            {
                Log.Debug(TAG, "Call progressing");
            }

            public void OnShouldSendPushNotification(ICall call, IList<IPushPair> pushPairs)
            {
                // Send a push through your push provider here, e.g. GCM
            }

        }

        public void onClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.answerButton:
                    AnswerClicked();
                    break;
                case Resource.Id.declineButton:
                    DeclineClicked();
                    break;
            }
        }
    }
}