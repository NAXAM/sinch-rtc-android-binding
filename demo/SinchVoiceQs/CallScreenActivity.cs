using System;
using System.Collections.Generic;
using Android.App;
using Android.Media;
using Android.Util;
using Android.Widget;
using Com.Sinch.Android.Rtc;
using Com.Sinch.Android.Rtc.Calling;
using Java.Util;

namespace SinchVoiceQs
{
	[Activity(MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/AppTheme")]
    public class CallScreenActivity : BaseActivity
    {
        static readonly string TAG = nameof(CallScreenActivity);

        AudioPlayer mAudioPlayer;
        Timer mTimer;
        UpdateCallDurationTask mDurationTask;

        String mCallId;

        TextView mCallDuration;
        TextView mCallState;
        TextView mCallerName;

        class UpdateCallDurationTask : TimerTask
        {
            readonly CallScreenActivity outer;

            public UpdateCallDurationTask(CallScreenActivity outer)
            {
                this.outer = outer;
            }

            public override void Run()
            {
                outer.RunOnUiThread(outer.UpdateCallDuration);
            }
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.callscreen);

            mAudioPlayer = new AudioPlayer(this);
            mCallDuration = FindViewById<TextView>(Resource.Id.callDuration);
            mCallerName = FindViewById<TextView>(Resource.Id.remoteUser);
            mCallState = FindViewById<TextView>(Resource.Id.callState);
            Button endCallButton = FindViewById<Button>(Resource.Id.hangupButton);

            endCallButton.Click += delegate
            {
                EndCall();
            };

            mCallId = Intent.GetStringExtra(SinchService.CALL_ID);
        }

        protected override void OnServiceConnected()
        {
            ICall call = GetSinchServiceInterface().GetCall(mCallId);
            if (call != null)
            {
                call.AddCallListener(new SinchCallListener(this));
                mCallerName.Text = (call.RemoteUserId);
                mCallState.Text = (call.State.ToString());
            }
            else
            {
                Log.Error(TAG, "Started with invalid callId, aborting.");
                Finish();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            mDurationTask.Cancel();
            mTimer.Cancel();
        }

        protected override void OnResume()
        {
            base.OnResume();
            mTimer = new Timer();
            mDurationTask = new UpdateCallDurationTask(this);
            mTimer.Schedule(mDurationTask, 0, 500);
        }

        public override void OnBackPressed()
        {
            // User should exit activity by ending call, not by going back.
        }

        void EndCall()
        {
            mAudioPlayer.StopProgressTone();
            ICall call = GetSinchServiceInterface().GetCall(mCallId);
            if (call != null)
            {
                call.Hangup();
            }
            Finish();
        }

        string FormatTimespan(int totalSeconds)
        {
            long minutes = totalSeconds / 60;
            long seconds = totalSeconds % 60;

            return string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }

        void UpdateCallDuration()
        {
            ICall call = GetSinchServiceInterface().GetCall(mCallId);
            if (call != null)
            {
                mCallDuration.Text = (FormatTimespan(call.Details.Duration));
            }
        }

        class SinchCallListener : Java.Lang.Object, ICallListener
        {
            readonly CallScreenActivity outer;

            public SinchCallListener(CallScreenActivity outer)
            {
                this.outer = outer;
            }

            public void OnCallEnded(ICall call)
            {
                CallEndCause cause = call.Details.EndCause;
                Log.Debug(TAG, "Call ended. Reason: " + cause);
				outer.mAudioPlayer.StopProgressTone();
                outer.VolumeControlStream = (Android.Media.Stream)AudioManager.UseDefaultStreamType;
                string endMsg = "Call ended: " + call.Details;
                Toast.MakeText(outer, endMsg, ToastLength.Long).Show();
                outer.EndCall();
            }

            public void OnCallEstablished(ICall call)
            {
                Log.Debug(TAG, "Call established");
                outer.mAudioPlayer.StopProgressTone();
                outer.mCallState.Text = (call.State.ToString());

                outer.VolumeControlStream = Android.Media.Stream.VoiceCall;
            }

            public void OnCallProgressing(ICall call)
            {
                Log.Debug(TAG, "Call progressing");
                outer.mAudioPlayer.PlayProgressTone();
            }

            public void OnShouldSendPushNotification(ICall call, IList<IPushPair> pushPairs)
            {
                // Send a push through your push provider here, e.g. GCM
            }

        }
    }
}
