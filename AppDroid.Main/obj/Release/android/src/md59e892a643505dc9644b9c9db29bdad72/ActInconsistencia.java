package md59e892a643505dc9644b9c9db29bdad72;


public class ActInconsistencia
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("APPDroid.CarMed.Activities.ActInconsistencia, APPDroid.CarMed, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ActInconsistencia.class, __md_methods);
	}


	public ActInconsistencia () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ActInconsistencia.class)
			mono.android.TypeManager.Activate ("APPDroid.CarMed.Activities.ActInconsistencia, APPDroid.CarMed, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
