package md5277f6bda02cfc7ac27321105c252a6e4;


public class ActSelectPatient
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
		mono.android.Runtime.register ("APPDroid.AdmMed.Activities.ActSelectPatient, APPDroid.AdmMed, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ActSelectPatient.class, __md_methods);
	}


	public ActSelectPatient () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ActSelectPatient.class)
			mono.android.TypeManager.Activate ("APPDroid.AdmMed.Activities.ActSelectPatient, APPDroid.AdmMed, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
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
