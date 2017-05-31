using System;
using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.Util;
using Java.IO;

namespace SinchVoiceQs
{
    public class AudioPlayer
    {
        static readonly String LOG_TAG = nameof(AudioPlayer);

        Context mContext;
        MediaPlayer mPlayer;

        AudioTrack mProgressTone;
        static readonly int SAMPLE_RATE = 16000;

        public AudioPlayer(Context context)
        {
            this.mContext = context.ApplicationContext;
        }

        public void PlayRingtone()
        {
            AudioManager audioManager = (AudioManager)mContext.GetSystemService(Context.AudioService);

            // Honour silent mode
            switch (audioManager.RingerMode)
            {
                case RingerMode.Normal:
                    mPlayer = new MediaPlayer();
                    mPlayer.SetAudioStreamType(Stream.Ring);

                    try
                    {
                        mPlayer.SetDataSource(mContext, Android.Net.Uri.Parse("android.resource://" + mContext.PackageName + "/" + Resource.Raw.phone_loud1));
                        mPlayer.Prepare();
                    }
                    catch (IOException e)
                    {
                        Log.Error(LOG_TAG, "Could not setup media player for ringtone");
                        mPlayer = null;
                        return;
                    }
                    mPlayer.Looping = (true);
                    mPlayer.Start();
                    break;
            }
        }

        public void StopRingtone()
        {
            if (mPlayer != null)
            {
                mPlayer.Stop();
                mPlayer.Release();
                mPlayer = null;
            }
        }

        public void PlayProgressTone()
        {
            StopProgressTone();
            try
            {
                mProgressTone = CreateProgressTone(mContext);
                mProgressTone.Play();
            }
            catch (Exception e)
            {
                Log.Error(LOG_TAG, "Could not play progress tone", e);
            }
        }

        public void StopProgressTone()
        {
            if (mProgressTone != null)
            {
                mProgressTone.Stop();
                mProgressTone.Release();
                mProgressTone = null;
            }
        }

        static AudioTrack CreateProgressTone(Context context)
        {
            AssetFileDescriptor fd = context.Resources.OpenRawResourceFd(Resource.Raw.progress_tone);
            int length = (int)fd.Length;

            AudioTrack audioTrack = new AudioTrack(Stream.VoiceCall, 
                                                   SAMPLE_RATE,
                                                   ChannelOut.Mono, 
                                                   Encoding.Pcm16bit, 
                                                   length, 
                                                   AudioTrackMode.Static);
            byte[] data = new byte[length];

            ReadFileToBytes(fd, data);

            audioTrack.Write(data, 0, data.Length);
            audioTrack.SetLoopPoints(0, data.Length / 2, 30);

            return audioTrack;
        }

        static void ReadFileToBytes(AssetFileDescriptor fd, byte[] data)
        {
            System.IO.Stream inputStream = fd.CreateInputStream();

            int bytesRead = 0;
            while (bytesRead < data.Length)
            {
                int res = inputStream.Read(data, bytesRead, (data.Length - bytesRead));
                if (res == -1)
                {
                    break;
                }
                bytesRead += res;
            }
        }
    }
}
