
using System;
using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using SCSAPP.Services.Messages;

namespace APPDroid.AdmMed.Adapters
{
			
	public class AdaExpaPaciente : BaseExpandableListAdapter
	{
		#region Variables and Controls
		readonly Activity Context;
		Patient patient;
		#endregion

		#region constructor method
		public AdaExpaPaciente(Activity newContext, Patient patient) : base(){
			Context = newContext;
			this.patient = patient;
		}
		#endregion

		#region DataList
		/// <summary>
		/// Gets or sets the data list.
		/// </summary>
		/// <value>The data list.</value>
		protected List<String> DataList { get; set; }
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
		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent){

			convertView = Context.LayoutInflater.Inflate (Resource.Layout.app_nombre_paciente, null);

			convertView.FindViewById<TextView> (Resource.Id.txtDataHeader).Text = string.Format ("{0} {1} {2} - {3}  ", patient.FirstName, patient.MiddleName, patient.LastName, patient.History);

			return convertView;

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
		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent){
			
			convertView = Context.LayoutInflater.Inflate (Resource.Layout.app_detalle_paciente, null);			

			convertView.FindViewById<TextView> (Resource.Id.txtEdadSex).Text =  string.Format("{0}, {1}", patient.Age, patient.Gender);
			convertView.FindViewById<TextView> (Resource.Id.txtCedula).Text =  string.Format("{0} {1}", patient.IdentificationType, patient.Identification);
			if(patient.Bed != null){
				TextView bed = convertView.FindViewById<TextView> (Resource.Id.txtBed);
				bed.Visibility = ViewStates.Visible;
				bed.Text =  string.Format("Cama: {0}", patient.Bed);
			}
			convertView.FindViewById<TextView> (Resource.Id.txtUbicacion).Text =  string.Format("{0}", patient.Location.Value);

			return convertView;
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
		public override int GetChildrenCount (int groupPosition){
			return 1;
		}
		#endregion

		#region GroupCount
		/// <summary>
		/// Gets the group count.
		/// </summary>
		/// <value>The group count.</value>
		public override int GroupCount {
			get {return 1; }
		}
		#endregion

		#region implemented abstract members of BaseExpandableListAdapter
		/// <exception cref="T:System.NotImplementedException"></exception>
		public override Java.Lang.Object GetChild (int groupPosition, int childPosition){
			throw new NotImplementedException ();
		}

		/// <Docs>the position of the group that contains the child</Docs>
		/// <summary>
		/// Gets the ID for the given child within the given group.
		/// </summary>
		/// <returns>The child identifier.</returns>
		/// <param name="groupPosition">Group position.</param>
		/// <param name="childPosition">Child position.</param>
		public override long GetChildId (int groupPosition, int childPosition){
			return childPosition;
		}

		/// <Docs>the position of the group</Docs>
		/// <returns>To be added.</returns>
		/// <para tool="javadoc-to-mdoc">Gets the data associated with the given group.</para>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <summary>
		/// Gets the group.
		/// </summary>
		/// <param name="groupPosition">Group position.</param>
		/// <exception cref="T:System.NotImplementedException"></exception>
		public override Java.Lang.Object GetGroup (int groupPosition){
			throw new NotImplementedException ();
		}

		/// <Docs>the position of the group for which the ID is wanted</Docs>
		/// <returns>To be added.</returns>
		/// <summary>
		/// Gets the group identifier.
		/// </summary>
		/// <param name="groupPosition">Group position.</param>
		public override long GetGroupId (int groupPosition)	{
			return groupPosition;
		}

		/// <Docs>the position of the group that contains the child</Docs>
		/// <summary>
		/// Whether the child at the specified position is selectable.
		/// </summary>
		/// <remarks>Whether the child at the specified position is selectable.</remarks>
		/// <format type="text/html">[Android Documentation]</format>
		/// <since version="Added in API level 1"></since>
		/// <returns><c>true</c> if this instance is child selectable the specified groupPosition childPosition; otherwise, <c>false</c>.</returns>
		/// <param name="groupPosition">Group position.</param>
		/// <param name="childPosition">Child position.</param>
		public override bool IsChildSelectable (int groupPosition, int childPosition){
			//throw new NotImplementedException ();
			return true;
		}

		/// <summary>
		/// Gets a value indicating whether this instance has stable identifiers.
		/// </summary>
		/// <value><c>true</c> if this instance has stable identifiers; otherwise, <c>false</c>.</value>
		public override bool HasStableIds {
			get {return true; }
		}
		#endregion

	}

}

