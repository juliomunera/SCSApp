using System;
using System.Collections.Generic;
using SCSAPP.Services.Messages;
using SCSAPP.Services.Messages.Entities;

namespace APPDroid.Framework.Context
{
	#region Main ActivitiesTypes
	/// <summary>
	/// Activities types.
	/// </summary>
	public class ActivitiesTypes
	{
		/// <summary>
		/// Gets or sets the type of the login.
		/// </summary>
		/// <value>The type of the login.</value>
		public Type LoginType {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type of the login.
		/// </summary>
		/// <value>The type of the login.</value>
		public Type MenuType {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the type of the home.
		/// </summary>
		/// <value>The type of the home.</value>
		public Type HomeType {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the patient.
		/// </summary>
		/// <value>The patient.</value>
		public Patient Patient {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the parameters cross activities
		/// </summary>
		/// <value>The parameters.</value>
		public Dictionary<string, object> Parameters { get; set;}

		public Dictionary<string, object> ParametersOne { get; set;}

		/// <summary>
		/// Gets or sets the position selected on administer.
		/// </summary>
		/// <value>The position selected on administer.</value>
		public int PositionOnAdminister {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the administer data.
		/// </summary>
		/// <value>The administer data.</value>
		public int[] AdministerData {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="APPDroid.Framework.Context.ActivitiesTypes"/> first primary.
		/// </summary>
		/// <value><c>true</c> if first primary; otherwise, <c>false</c>.</value>
		public bool FirstPrimary {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets a value indicating whether this instance is scanning.
		/// </summary>
		/// <value><c>true</c> if this instance is scanning; otherwise, <c>false</c>.</value>
		public bool IsScanning {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the medicament dose.
		/// </summary>
		/// <value>The medicament dose.</value>
		public Medicament medicamentDose {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the diluent selected.
		/// </summary>
		/// <value>The diluent selected.</value>
		public MasterItem DiluentSelecteds {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="APPDroid.Framework.Context.ActivitiesTypes"/> administar medicament.
		/// </summary>
		/// <value><c>true</c> if administar medicament; otherwise, <c>false</c>.</value>
		public bool AdministarMedicament {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="APPDroid.Framework.Context.ActivitiesTypes"/> imagen chect.
		/// </summary>
		/// <value><c>true</c> if imagen chect; otherwise, <c>false</c>.</value>
		public bool ImagenChect {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the patients loaded in patient list.
		/// </summary>
		/// <value>The patients loaded in patient list.</value>
		public List<Patient> PatientsLoadedInPatientList {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the value variable CP
		/// </summary>
		/// <value>The value variable C.</value>
		public MasterItem ValVarCP {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the service charge medicines.
		/// </summary>
		/// <value>The service charge medicines.</value>
		public DrugChargesInitialize ServiceChargeMedicines {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the list patients.
		/// </summary>
		/// <value>The list patients.</value>
		public List<ListOfPatiens> listPatients {
			get;
			set;
		}

		public RefundPatient devPatients 
		{
			get;
			set;
		}

		/// <summary>
        /// Gets or sets the AttachedConcept.
		/// </summary>
        /// <value>The AttachedConcept.</value>
        public string AttachedConcept
        {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>The fuente.</value>
        public string year
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>The fuente.</value>
        public string month
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fuente.
        /// </summary>
		public int PositionPatient
        {
			get;
			set;
		}

        /// <summary>
        /// Gets or sets the Almacen
        /// </summary>
        public Warehouse Almacen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ParametrosActivity
        /// </summary>
		public DrugChargesInitialize ParametrosActivity {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the patient selecte.
		/// </summary>
		/// <value>The patient selecte.</value>
		public PatientSource PatientSelecte {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number hours.
		/// </summary>
		/// <value>The number hours.</value>
		public int? NumberHours 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the listmedicament.
		/// </summary>
		/// <value>The listmedicament.</value>
		public List<Medicament> listmedicament 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ead patient.
		/// </summary>
		/// <value>The ead patient.</value>
		public string EadPatient 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the patien fund.
		/// </summary>
		/// <value>The patien fund.</value>
		public RefundPatient patienFund 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ubicacion car.
		/// </summary>
		/// <value>The ubicacion car.</value>
		public string ubicacionCar
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the services car.
		/// </summary>
		/// <value>The services car.</value>
		public string servicesCar 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the services car.
		/// </summary>
		/// <value>The services car.</value>
		public string NumberUbic 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the numero orden car.
		/// </summary>
		/// <value>The numero orden car.</value>
		public int NumeroOrdenCar 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="APPDroid.Framework.Context.ActivitiesTypes"/> indica cum.
		/// </summary>
		/// <value><c>true</c> if indica cum; otherwise, <c>false</c>.</value>
		public bool indicaCum 
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the initial date.
		/// </summary>
		/// <value>The initial date.</value>
		public DateTime InitialDate 
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the final date.
		/// </summary>
		/// <value>The final date.</value>
		public DateTime FinalDate 
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the patient full.
		/// </summary>
		/// <value>The patient full.</value>
		public List<Medicament> PatientFull {
			get;
			set;
		}

	}
	#endregion

	/// <summary>
	/// Administer type.
	/// </summary>
	public enum AdministerType {
		NoAdmin = 0,
		AdminManual = 1,
		AdminBarCode = 2,
		None = 4,
	}

}

