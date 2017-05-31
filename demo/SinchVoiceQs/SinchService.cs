using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Com.Sinch.Android.Rtc;
using Com.Sinch.Android.Rtc.Calling;

namespace SinchVoiceQs
{
    [Service]
    public class SinchService : Service
    {
        static readonly string APP_KEY = "{YOUR-API-KEY}";
        static readonly string APP_SECRET = "{YOUR-API-SECRET}";
        static readonly string ENVIRONMENT = "{SINCH-ENV}";

        public static readonly string CALL_ID = "CALL_ID";
        static readonly string TAG = nameof(SinchService);

        public SinchService()
        {
            mSinchServiceInterface = new SinchServiceInterface(this);
        }

        SinchServiceInterface mSinchServiceInterface;
        ISinchClient mSinchClient;
        string mUserId;

        StartFailedListener mListener;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            if (mSinchClient != null && mSinchClient.IsStarted)
            {
                mSinchClient.Terminate();
            }
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent)
        {
            return mSinchServiceInterface;
        }

        void Start(String userName)
        {
            if (mSinchClient == null)
            {
                mUserId = userName;
                mSinchClient = Sinch.SinchClientBuilder.Context(ApplicationContext).UserId(userName)
                        .ApplicationKey(APP_KEY)
                        .ApplicationSecret(APP_SECRET)
                        .EnvironmentHost(ENVIRONMENT).Build();

                mSinchClient.SetSupportCalling(true);
                mSinchClient.StartListeningOnActiveConnection();

                mSinchClient.AddSinchClientListener(new MySinchClientListener(this));
                // Permission READ_PHONE_STATE is needed to respect native calls.
                mSinchClient.CallClient.SetRespectNativeCalls(false);
                mSinchClient.CallClient.AddCallClientListener(new SinchCallClientListener(this));
                mSinchClient.Start();
            }
        }

        void Stop()
        {
            if (mSinchClient != null)
            {
                mSinchClient.Terminate();
                mSinchClient = null;
            }
        }

        bool IsStarted()
        {
            return (mSinchClient != null && mSinchClient.IsStarted);
        }

        public interface StartFailedListener
        {
            void OnStartFailed(ISinchError error);

            void OnStarted();
        }

        class MySinchClientListener : Java.Lang.Object, ISinchClientListener
        {
            readonly SinchService outer;

            public MySinchClientListener(SinchService outer)
            {
                this.outer = outer;
            }

            public void OnClientFailed(ISinchClient client, ISinchError error)
            {
                if (outer.mListener != null)
                {
                    outer.mListener.OnStartFailed(error);
                }
                outer.mSinchClient.Terminate();
                outer.mSinchClient = null;
            }

            public void OnClientStarted(ISinchClient client)
            {
                Log.Debug(TAG, "SinchClient started");
                if (outer.mListener != null)
                {
                    outer.mListener.OnStarted();
                }
            }

            public void OnClientStopped(ISinchClient client)
            {
                Log.Debug(TAG, "SinchClient stopped");
            }

            public void OnLogMessage(int level, String area, String message)
            {
                switch (level)
                {
                    case 3://Log.DEBUG:
                        Log.Debug(area, message);
                        break;
                    case 6://Log.ERROR:
                        Log.Error(area, message);
                        break;
                    case 4://Log.INFO:
                        Log.Info(area, message);
                        break;
                    case 2://Log.VERBOSE:
                        Log.Verbose(area, message);
                        break;
                    case 5://Log.WARN:
                        Log.Warn(area, message);
                        break;
                }
            }

            public void OnRegistrationCredentialsRequired(ISinchClient client, IClientRegistration clientRegistration)
            {
            }
        }

        class SinchCallClientListener : Java.Lang.Object, ICallClientListener
        {
            readonly SinchService outer;
            public SinchCallClientListener(SinchService outer)
            {
                this.outer = outer;
            }

            public void OnIncomingCall(ICallClient callClient, ICall call)
            {
                Log.Debug(TAG, "Incoming call");
                Intent intent = new Intent(outer, typeof(IncomingCallScreenActivity));
                intent.PutExtra(CALL_ID, call.CallId);
                intent.AddFlags(ActivityFlags.NewTask);
                outer.StartActivity(intent);
            }
        }

        public class SinchServiceInterface : Binder
        {
            readonly SinchService outer;

            public SinchServiceInterface(SinchService outer)
            {
                this.outer = outer;
            }

            public ICall CallPhoneNumber(String phoneNumber)
            {
                return outer.mSinchClient.CallClient.CallPhoneNumber(phoneNumber);
            }

            public ICall CallUser(String userId)
            {
                if (outer.mSinchClient == null)
                {
                    return null;
                }
                return outer.mSinchClient.CallClient.CallUser(userId);
            }

            public string GetUserName()
            {
                return outer.mUserId;
            }

            public bool IsStarted()
            {
                return outer.IsStarted();
            }

            public void StartClient(String userName)
            {
                outer.Start(userName);
            }

            public void StopClient()
            {
                outer.Stop();
            }

            public void SetStartListener(StartFailedListener listener)
            {
                outer.mListener = listener;
            }

            public ICall GetCall(String callId)
            {
                return outer.mSinchClient.CallClient.GetCall(callId);
            }
        }
    }
}
