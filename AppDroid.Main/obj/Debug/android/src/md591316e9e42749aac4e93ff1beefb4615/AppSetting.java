package md591316e9e42749aac4e93ff1beefb4615;


public class AppSetting
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"";
		mono.android.Runtime.register ("AppDroid.Main.Activities.AppSetting, AppDroid.Main, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null", AppSetting.class, __md_methods);
	}


	public AppSetting () throws java.lang.Throwable
	{
		super ();
		if (getClass () == AppSetting.class)
			mono.android.TypeManager.Activate ("AppDroid.Main.Activities.AppSetting, AppDroid.Main, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onBackPressed ()
	{
		n_onBackPressed ();
	}

	private native void n_onBackPressed ();

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
