using System;
using Android.App;
using Com.Instabug.Library;
using Com.Instabug.Wrapper.Impl.V10;
using Com.Instabug.Wrapper.Support;
using Com.Instabug.Library.Util;
using Android.Runtime;
using Java.Lang;

namespace InstabugAndroidSample
{
	public class SampleApplication : Application, Thread.IUncaughtExceptionHandler
	{
		public SampleApplication (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		}

		Thread mainThread;

		public override void OnCreate ()
		{
			base.OnCreate ();

			Instabug.Initialize (this)
				.SetCommentRequired (true)
				.SetCrashReportingEnabled (true)
				.SetAnnotationActivityClass (Java.Lang.Class.FromType (typeof(InstabugAnnotationActivity)))
				.SetShowIntroDialog (true)
				.SetEnableOverflowMenuItem (true);

			Thread.DefaultUncaughtExceptionHandler = this;
			mainThread = Thread.CurrentThread ();

			AndroidEnvironment.UnhandledExceptionRaiser += (object sender, RaiseThrowableEventArgs e) => {
				Java.Lang.Thread thread = mainThread;
				UncaughtException (thread, e.Exception.GetJavaException ());
				e.Handled = true;
			};
			System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, args) => {
				UncaughtException (Thread.CurrentThread (), null);
			};
		}

		public void UncaughtException (Thread thread, Throwable ex)
		{
			Instabug.Instance.ReportCrash ("test", thread, ex);
		}
	}
}

