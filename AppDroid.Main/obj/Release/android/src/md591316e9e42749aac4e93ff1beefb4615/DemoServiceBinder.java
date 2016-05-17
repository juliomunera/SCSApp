package md591316e9e42749aac4e93ff1beefb4615;


public class DemoServiceBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("AppDroid.Main.Activities.DemoServiceBinder, AppDroid.Main, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null", DemoServiceBinder.class, __md_methods);
	}


	public DemoServiceBinder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DemoServiceBinder.class)
			mono.android.TypeManager.Activate ("AppDroid.Main.Activities.DemoServiceBinder, AppDroid.Main, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public DemoServiceBinder (md591316e9e42749aac4e93ff1beefb4615.InconsistencyService p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == DemoServiceBinder.class)
			mono.android.TypeManager.Activate ("AppDroid.Main.Activities.DemoServiceBinder, AppDroid.Main, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null", "AppDroid.Main.Activities.InconsistencyService, AppDroid.Main, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
