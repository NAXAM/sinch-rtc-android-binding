using System;
using Android.OS;
using Android.Runtime;
using Java.Lang;

namespace Com.Sinch.Gson
{
    partial class JsonStreamParser
    {
        public unsafe Java.Lang.Object Next()
        {
            return NextElement();
        }
    }
}

namespace Com.Sinch.Android.Rtc.Internal.Service.Http
{
    partial class DefaultHttpService
    {
        partial class AsyncHttpRequest
        {
            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {
                return DoInBackground(@params as Java.Lang.Void[]);
            }
        }
    }
}

namespace Com.Sinch.Gson.Internal.Bind
{
    partial class CollectionTypeAdapterFactory
    {
        partial class Adapter
        {
            public override void Write(global::Com.Sinch.Gson.Stream.JsonWriter p0, global::Java.Lang.Object p1)
            {
                Write(p0, p1 as global::System.Collections.ICollection);
            }

            public override global::Java.Lang.Object Read(global::Com.Sinch.Gson.Stream.JsonReader p0)
            {
                var localRef = JavaCollection.ToLocalJniHandle(ReadCollection(p0));
                try
                {
                    return new Java.Lang.Object(localRef, JniHandleOwnership.TransferLocalRef);
                }
                finally
                {
                    JNIEnv.DeleteLocalRef(localRef);
                }
            }
        }
    }

    partial class DateTypeAdapter
    {
        public override void Write(global::Com.Sinch.Gson.Stream.JsonWriter p0, global::Java.Lang.Object p1)
        {
            Write(p0, p1 as global::Java.Util.Date);
        }

        public override global::Java.Lang.Object Read(global::Com.Sinch.Gson.Stream.JsonReader p0)
        {
            return ReadDate(p0);
        }
    }

    partial class MapTypeAdapterFactory
    {
        partial class Adapter
        {
            public override void Write(global::Com.Sinch.Gson.Stream.JsonWriter p0, global::Java.Lang.Object p1)
            {
                Write(p0, p1 as global::System.Collections.IDictionary);
            }

            public override global::Java.Lang.Object Read(global::Com.Sinch.Gson.Stream.JsonReader p0)
            {
                var localRef = JavaDictionary.ToLocalJniHandle(ReadDictionary(p0));
                try
                {
                    return new Java.Lang.Object(localRef, JniHandleOwnership.TransferLocalRef);
                }
                finally
                {
                    JNIEnv.DeleteLocalRef(localRef);
                }
            }
        }
    }

    partial class SqlDateTypeAdapter
    {
        public override void Write(global::Com.Sinch.Gson.Stream.JsonWriter p0, global::Java.Lang.Object p1)
        {
            Write(p0, p1 as global::Java.Sql.Date);
        }

        public override global::Java.Lang.Object Read(global::Com.Sinch.Gson.Stream.JsonReader p0)
        {
            return ReadDate(p0);
        }
    }

    partial class TimeTypeAdapter
    {
        public override void Write(global::Com.Sinch.Gson.Stream.JsonWriter p0, global::Java.Lang.Object p1)
        {
            Write(p0, p1 as global::Java.Sql.Time);
        }

        public override global::Java.Lang.Object Read(global::Com.Sinch.Gson.Stream.JsonReader p0)
        {
            return ReadTime(p0);
        }
    }
}

namespace Org.Webrtc.Sinch
{
    partial class EglBase14
    {
        public override Org.Webrtc.Sinch.EglBase.Context EglBaseContext
        {
            get
            {
                return EglBase14Context;
            }
        }
    }
}

namespace Com.Sinch.Android.Rtc.Internal.Client.Calling
{
    partial class DefaultCallClient
    {
        public void SetRespectNativeCalls(bool p0)
        {
            RespectNativeCalls = p0;
        }
    }
}

namespace Com.Sinch.Gson.Internal { 
    partial class LinkedHashTreeMap {
		static IntPtr id_entrySet;
		// Metadata.xml XPath method reference: path="/api/package[@name='com.sinch.gson.internal']/class[@name='LinkedHashTreeMap']/method[@name='entrySet' and count(parameter)=0]"
		[Register("entrySet", "()Ljava/util/Set;", "")]
		public override unsafe global::System.Collections.ICollection EntrySet()
		{
			if (id_entrySet == IntPtr.Zero)
				id_entrySet = JNIEnv.GetMethodID(class_ref, "entrySet", "()Ljava/util/Set;");
			try
			{
                return global::Android.Runtime.JavaSet.FromJniHandle(JNIEnv.CallObjectMethod(((global::Java.Lang.Object)this).Handle, id_entrySet), JniHandleOwnership.TransferLocalRef);
			}
			finally
			{
			}
		}
    }

    partial class LinkedTreeMap {
		static IntPtr id_entrySet;
		// Metadata.xml XPath method reference: path="/api/package[@name='com.sinch.gson.internal']/class[@name='LinkedTreeMap']/method[@name='entrySet' and count(parameter)=0]"
		[Register("entrySet", "()Ljava/util/Set;", "")]
		public override unsafe global::System.Collections.ICollection EntrySet()
		{
			if (id_entrySet == IntPtr.Zero)
				id_entrySet = JNIEnv.GetMethodID(class_ref, "entrySet", "()Ljava/util/Set;");
			try
			{
				return global::Android.Runtime.JavaSet.FromJniHandle(JNIEnv.CallObjectMethod(((global::Java.Lang.Object)this).Handle, id_entrySet), JniHandleOwnership.TransferLocalRef);
			}
			finally
			{
			}
		}
    }
}