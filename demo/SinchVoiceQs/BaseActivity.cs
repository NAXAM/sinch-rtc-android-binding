using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace SinchVoiceQs
{
    public abstract class BaseActivity : AppCompatActivity, IServiceConnection
    {
        SinchService.SinchServiceInterface mSinchServiceInterface;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ApplicationContext.BindService(new Intent(this, typeof(SinchService)), this, Bind.AutoCreate);
        }

        public void OnServiceConnected(ComponentName componentName, IBinder iBinder)
        {
            var className = componentName.ShortClassName.Split('.').Last();
            if (nameof(SinchService).Equals(className))
            {
                mSinchServiceInterface = (SinchService.SinchServiceInterface)iBinder;

                OnServiceConnected();
            }
        }

        public void OnServiceDisconnected(ComponentName componentName)
		{
			var className = componentName.ShortClassName.Split('.').Last();
			if (nameof(SinchService).Equals(className))
            {
                mSinchServiceInterface = null;

                OnServiceDisconnected();
            }
        }

        protected virtual void OnServiceConnected()
        {
            // for subclasses
        }

        protected virtual void OnServiceDisconnected()
        {
            // for subclasses
        }

        protected virtual SinchService.SinchServiceInterface GetSinchServiceInterface()
        {
            return mSinchServiceInterface;
        }

    }
}
