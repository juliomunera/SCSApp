using Android.App;
using Android.Views;
using Android.Widget;
using APPDroid.Framework.Context;

namespace APPDroid.DevMed.Adapters
{
			
	public class AdapterPatients : BaseExpandableListAdapter
	{
		#region Variables and Controls
		readonly Activity Context;
		#endregion

		#region constructor method
		/// <summary>
		/// Initializes a new instance of the <see cref="APPDroid.DevMed.Adapters.AdapterPatients"/> class.
		/// </summary>
		/// <param name="newContext">New context.</param>
		public AdapterPatients(Activity newContext) : base()
		{
			this.Context = newContext;
		}
		#endregion

		#region GetGroupView
		/// <Docs>the position of the group for which the View is
		///  returned</Docs>
		/// <summary>
		/// Gets the group view.
		/// </summary>
		/// <returns>The group view.</returns>
		/// <param name="groupPosition">Group position.</param>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		/// <param name="convertView">Convert view.</param>
		/// <param name="parent">Parent.</param>
		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			View header = convertView ?? Context.LayoutInflater.Inflate (Resource.Layout.header_nombre, null);
				
			TextView nombrePaciente = header.FindViewById<TextView> (Resource.Id.txtDataHeaderdev);
			nombrePaciente.Text = string.Format ("{0} {1} {2}", 
				ActivitiesContext.Context.patienFund.FirstName, 
				ActivitiesContext.Context.patienFund.MiddleName,
				ActivitiesContext.Context.patienFund.LastName);

			return header;			
		}
		#endregion

		#region GetChildView
		/// <Docs>the position of the group that contains the child</Docs>
		/// <param name="isLastChild">Whether the child is the last child within the group</param>
		/// <summary>
		/// Gets the child view.
		/// </summary>
		/// <returns>The child view.</returns>
		/// <param name="groupPosition">Group position.</param>
		/// <param name="childPosition">Child position.</param>
		/// <param name="convertView">Convert view.</param>
		/// <param name="parent">Parent.</param>
		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			View row = convertView;
			if (row == null) {
				row = Context.LayoutInflater.Inflate (Resource.Layout.header_patient, null);

				TextView hitoria = row.FindViewById<TextView> (Resource.Id.txtHistory);
				hitoria.Text = string.Format ("Historia: {0} - {1}", ActivitiesContext.Context.patienFund.History, ActivitiesContext.Context.patienFund.EntryNumber);

				TextView txtServicio = row.FindViewById<TextView> (Resource.Id.txtServiAtancion);
				txtServicio.Text = string.Format ("{0}", ActivitiesContext.Context.patienFund.AtentionService);

				TextView identificacion = row.FindViewById<TextView> (Resource.Id.txtIdentificacion);
				identificacion.Text = string.Format ("{0}. {1}", ActivitiesContext.Context.patienFund.IdentificationType, ActivitiesContext.Context.patienFund.Identification);

				TextView edad = row.FindViewById<TextView> (Resource.Id.txtEdad);
				edad.Text = string.Format ("{0}", ActivitiesContext.Context.patienFund.Age);

				TextView ubicacion = row.FindViewById<TextView> (Resource.Id.txtUbicacion);
				ubicacion.Text = string.Format ("Ubi: {0}", ActivitiesContext.Context.patienFund.Location);


			}

			return row;
		}
		#endregion

		#region GetChildrenCount
		/// <Docs>the position of the group for which the children
		///  count should be returned</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Gets the number of children in a specified group.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Gets the children count.
		/// </summary>
		/// <param name="groupPosition">Group position.</param>
		public override int GetChildrenCount (int groupPosition)
		{
			return 1;
		}
		#endregion

		#region GroupCount
		/// <summary>
		/// Gets the group count.
		/// </summary>
		/// <value>The group count.</value>
		public override int GroupCount {
			get {
				return 1;
			}
		}
		#endregion

		#region implemented abstract members of BaseExpandableListAdapter
		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			return null;	
			//throw new NotImplementedException ();
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return null;
			//throw new NotImplementedException ();
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			//throw new NotImplementedException ();
			return true;
		}

		public override bool HasStableIds {
			get {
				return true;
			}
		}
		#endregion

	}

}

