// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.17020
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace APPDroid.Framework.WServiceInstaller {
    
    
    /// <remarks/>
    [System.Web.Services.WebServiceBinding(Name="WServiceInstallerSoap", Namespace="http://servinte.com.co/ClientCenterServices")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class WServiceInstaller : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback AllOkOperationCompleted;
        
        private System.Threading.SendOrPostCallback CanInstallOperationCompleted;
        
        private System.Threading.SendOrPostCallback ValidateClientCredentialsOperationCompleted;
        
        private System.Threading.SendOrPostCallback ActivateLicencesOperationCompleted;
        
        private System.Threading.SendOrPostCallback DeactivateLicencesOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetActiveModulesOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetTrialTimeOperationCompleted;
        
        public WServiceInstaller() {
            this.Url = "http://dmr.servinte.com.co/DMRSite/ClientCenterServices/WServiceInstaller.asmx";
        }
        
        public WServiceInstaller(string url) {
            this.Url = url;
        }
        
        public event AllOkCompletedEventHandler AllOkCompleted;
        
        public event CanInstallCompletedEventHandler CanInstallCompleted;
        
        public event ValidateClientCredentialsCompletedEventHandler ValidateClientCredentialsCompleted;
        
        public event ActivateLicencesCompletedEventHandler ActivateLicencesCompleted;
        
        public event DeactivateLicencesCompletedEventHandler DeactivateLicencesCompleted;
        
        public event GetActiveModulesCompletedEventHandler GetActiveModulesCompleted;
        
        public event GetTrialTimeCompletedEventHandler GetTrialTimeCompleted;
        
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://servinte.com.co/ClientCenterServices/AllOk", RequestNamespace="http://servinte.com.co/ClientCenterServices", ResponseNamespace="http://servinte.com.co/ClientCenterServices", ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped, Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        public bool AllOk() {
            object[] results = this.Invoke("AllOk", new object[0]);
            return ((bool)(results[0]));
        }
        
        public System.IAsyncResult BeginAllOk(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("AllOk", new object[0], callback, asyncState);
        }
        
        public bool EndAllOk(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        public void AllOkAsync() {
            this.AllOkAsync(null);
        }
        
        public void AllOkAsync(object userState) {
            if ((this.AllOkOperationCompleted == null)) {
                this.AllOkOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAllOkCompleted);
            }
            this.InvokeAsync("AllOk", new object[0], this.AllOkOperationCompleted, userState);
        }
        
        private void OnAllOkCompleted(object arg) {
            if ((this.AllOkCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AllOkCompleted(this, new AllOkCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://servinte.com.co/ClientCenterServices/CanInstall", RequestNamespace="http://servinte.com.co/ClientCenterServices", ResponseNamespace="http://servinte.com.co/ClientCenterServices", ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped, Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        public bool CanInstall(string Arg1, int Arg2, string Arg3, string Arg4) {
            object[] results = this.Invoke("CanInstall", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4});
            return ((bool)(results[0]));
        }
        
        public System.IAsyncResult BeginCanInstall(string Arg1, int Arg2, string Arg3, string Arg4, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("CanInstall", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4}, callback, asyncState);
        }
        
        public bool EndCanInstall(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        public void CanInstallAsync(string Arg1, int Arg2, string Arg3, string Arg4) {
            this.CanInstallAsync(Arg1, Arg2, Arg3, Arg4, null);
        }
        
        public void CanInstallAsync(string Arg1, int Arg2, string Arg3, string Arg4, object userState) {
            if ((this.CanInstallOperationCompleted == null)) {
                this.CanInstallOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCanInstallCompleted);
            }
            this.InvokeAsync("CanInstall", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4}, this.CanInstallOperationCompleted, userState);
        }
        
        private void OnCanInstallCompleted(object arg) {
            if ((this.CanInstallCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CanInstallCompleted(this, new CanInstallCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://servinte.com.co/ClientCenterServices/ValidateClientCredentials", RequestNamespace="http://servinte.com.co/ClientCenterServices", ResponseNamespace="http://servinte.com.co/ClientCenterServices", ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped, Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        public bool ValidateClientCredentials(string Arg1, string Arg2) {
            object[] results = this.Invoke("ValidateClientCredentials", new object[] {
                        Arg1,
                        Arg2});
            return ((bool)(results[0]));
        }
        
        public System.IAsyncResult BeginValidateClientCredentials(string Arg1, string Arg2, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ValidateClientCredentials", new object[] {
                        Arg1,
                        Arg2}, callback, asyncState);
        }
        
        public bool EndValidateClientCredentials(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        public void ValidateClientCredentialsAsync(string Arg1, string Arg2) {
            this.ValidateClientCredentialsAsync(Arg1, Arg2, null);
        }
        
        public void ValidateClientCredentialsAsync(string Arg1, string Arg2, object userState) {
            if ((this.ValidateClientCredentialsOperationCompleted == null)) {
                this.ValidateClientCredentialsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnValidateClientCredentialsCompleted);
            }
            this.InvokeAsync("ValidateClientCredentials", new object[] {
                        Arg1,
                        Arg2}, this.ValidateClientCredentialsOperationCompleted, userState);
        }
        
        private void OnValidateClientCredentialsCompleted(object arg) {
            if ((this.ValidateClientCredentialsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ValidateClientCredentialsCompleted(this, new ValidateClientCredentialsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://servinte.com.co/ClientCenterServices/ActivateLicences", RequestNamespace="http://servinte.com.co/ClientCenterServices", ResponseNamespace="http://servinte.com.co/ClientCenterServices", ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped, Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        public bool ActivateLicences(string Arg1, int Arg2, string Arg3, string Arg4, string Arg5, string Arg6, string Arg7, string Arg8, int Arg9, double Arg10, string Arg11) {
            object[] results = this.Invoke("ActivateLicences", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5,
                        Arg6,
                        Arg7,
                        Arg8,
                        Arg9,
                        Arg10,
                        Arg11});
            return ((bool)(results[0]));
        }
        
        public System.IAsyncResult BeginActivateLicences(string Arg1, int Arg2, string Arg3, string Arg4, string Arg5, string Arg6, string Arg7, string Arg8, int Arg9, double Arg10, string Arg11, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ActivateLicences", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5,
                        Arg6,
                        Arg7,
                        Arg8,
                        Arg9,
                        Arg10,
                        Arg11}, callback, asyncState);
        }
        
        public bool EndActivateLicences(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        public void ActivateLicencesAsync(string Arg1, int Arg2, string Arg3, string Arg4, string Arg5, string Arg6, string Arg7, string Arg8, int Arg9, double Arg10, string Arg11) {
            this.ActivateLicencesAsync(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, Arg11, null);
        }
        
        public void ActivateLicencesAsync(string Arg1, int Arg2, string Arg3, string Arg4, string Arg5, string Arg6, string Arg7, string Arg8, int Arg9, double Arg10, string Arg11, object userState) {
            if ((this.ActivateLicencesOperationCompleted == null)) {
                this.ActivateLicencesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnActivateLicencesCompleted);
            }
            this.InvokeAsync("ActivateLicences", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5,
                        Arg6,
                        Arg7,
                        Arg8,
                        Arg9,
                        Arg10,
                        Arg11}, this.ActivateLicencesOperationCompleted, userState);
        }
        
        private void OnActivateLicencesCompleted(object arg) {
            if ((this.ActivateLicencesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ActivateLicencesCompleted(this, new ActivateLicencesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://servinte.com.co/ClientCenterServices/DeactivateLicences", RequestNamespace="http://servinte.com.co/ClientCenterServices", ResponseNamespace="http://servinte.com.co/ClientCenterServices", ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped, Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        public bool DeactivateLicences(string Arg1, int Arg2, string Arg3, string Arg4, string Arg5, string Arg6, string Arg7, int Arg8, double Arg9, string Arg10) {
            object[] results = this.Invoke("DeactivateLicences", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5,
                        Arg6,
                        Arg7,
                        Arg8,
                        Arg9,
                        Arg10});
            return ((bool)(results[0]));
        }
        
        public System.IAsyncResult BeginDeactivateLicences(string Arg1, int Arg2, string Arg3, string Arg4, string Arg5, string Arg6, string Arg7, int Arg8, double Arg9, string Arg10, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("DeactivateLicences", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5,
                        Arg6,
                        Arg7,
                        Arg8,
                        Arg9,
                        Arg10}, callback, asyncState);
        }
        
        public bool EndDeactivateLicences(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        public void DeactivateLicencesAsync(string Arg1, int Arg2, string Arg3, string Arg4, string Arg5, string Arg6, string Arg7, int Arg8, double Arg9, string Arg10) {
            this.DeactivateLicencesAsync(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9, Arg10, null);
        }
        
        public void DeactivateLicencesAsync(string Arg1, int Arg2, string Arg3, string Arg4, string Arg5, string Arg6, string Arg7, int Arg8, double Arg9, string Arg10, object userState) {
            if ((this.DeactivateLicencesOperationCompleted == null)) {
                this.DeactivateLicencesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeactivateLicencesCompleted);
            }
            this.InvokeAsync("DeactivateLicences", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5,
                        Arg6,
                        Arg7,
                        Arg8,
                        Arg9,
                        Arg10}, this.DeactivateLicencesOperationCompleted, userState);
        }
        
        private void OnDeactivateLicencesCompleted(object arg) {
            if ((this.DeactivateLicencesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeactivateLicencesCompleted(this, new DeactivateLicencesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://servinte.com.co/ClientCenterServices/GetActiveModules", RequestNamespace="http://servinte.com.co/ClientCenterServices", ResponseNamespace="http://servinte.com.co/ClientCenterServices", ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped, Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        public string GetActiveModules(string Arg1, int Arg2, bool Arg3, string Arg4, string Arg5) {
            object[] results = this.Invoke("GetActiveModules", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5});
            return ((string)(results[0]));
        }
        
        public System.IAsyncResult BeginGetActiveModules(string Arg1, int Arg2, bool Arg3, string Arg4, string Arg5, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetActiveModules", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5}, callback, asyncState);
        }
        
        public string EndGetActiveModules(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        public void GetActiveModulesAsync(string Arg1, int Arg2, bool Arg3, string Arg4, string Arg5) {
            this.GetActiveModulesAsync(Arg1, Arg2, Arg3, Arg4, Arg5, null);
        }
        
        public void GetActiveModulesAsync(string Arg1, int Arg2, bool Arg3, string Arg4, string Arg5, object userState) {
            if ((this.GetActiveModulesOperationCompleted == null)) {
                this.GetActiveModulesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetActiveModulesCompleted);
            }
            this.InvokeAsync("GetActiveModules", new object[] {
                        Arg1,
                        Arg2,
                        Arg3,
                        Arg4,
                        Arg5}, this.GetActiveModulesOperationCompleted, userState);
        }
        
        private void OnGetActiveModulesCompleted(object arg) {
            if ((this.GetActiveModulesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetActiveModulesCompleted(this, new GetActiveModulesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://servinte.com.co/ClientCenterServices/GetTrialTime", RequestNamespace="http://servinte.com.co/ClientCenterServices", ResponseNamespace="http://servinte.com.co/ClientCenterServices", ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped, Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        [return: System.Xml.Serialization.XmlElementAttribute(IsNullable=true, DataType="int")]
        public System.Nullable<int> GetTrialTime(string Arg1, int Arg2) {
            object[] results = this.Invoke("GetTrialTime", new object[] {
                        Arg1,
                        Arg2});
            return ((System.Nullable<int>)(results[0]));
        }
        
        public System.IAsyncResult BeginGetTrialTime(string Arg1, int Arg2, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetTrialTime", new object[] {
                        Arg1,
                        Arg2}, callback, asyncState);
        }
        
        public System.Nullable<int> EndGetTrialTime(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Nullable<int>)(results[0]));
        }
        
        public void GetTrialTimeAsync(string Arg1, int Arg2) {
            this.GetTrialTimeAsync(Arg1, Arg2, null);
        }
        
        public void GetTrialTimeAsync(string Arg1, int Arg2, object userState) {
            if ((this.GetTrialTimeOperationCompleted == null)) {
                this.GetTrialTimeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetTrialTimeCompleted);
            }
            this.InvokeAsync("GetTrialTime", new object[] {
                        Arg1,
                        Arg2}, this.GetTrialTimeOperationCompleted, userState);
        }
        
        private void OnGetTrialTimeCompleted(object arg) {
            if ((this.GetTrialTimeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetTrialTimeCompleted(this, new GetTrialTimeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
    }
    
    public partial class AllOkCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AllOkCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    public delegate void AllOkCompletedEventHandler(object sender, AllOkCompletedEventArgs args);
    
    public partial class CanInstallCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CanInstallCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    public delegate void CanInstallCompletedEventHandler(object sender, CanInstallCompletedEventArgs args);
    
    public partial class ValidateClientCredentialsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ValidateClientCredentialsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    public delegate void ValidateClientCredentialsCompletedEventHandler(object sender, ValidateClientCredentialsCompletedEventArgs args);
    
    public partial class ActivateLicencesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ActivateLicencesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    public delegate void ActivateLicencesCompletedEventHandler(object sender, ActivateLicencesCompletedEventArgs args);
    
    public partial class DeactivateLicencesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DeactivateLicencesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    public delegate void DeactivateLicencesCompletedEventHandler(object sender, DeactivateLicencesCompletedEventArgs args);
    
    public partial class GetActiveModulesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetActiveModulesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    public delegate void GetActiveModulesCompletedEventHandler(object sender, GetActiveModulesCompletedEventArgs args);
    
    public partial class GetTrialTimeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetTrialTimeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Nullable<int> Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Nullable<int>)(this.results[0]));
            }
        }
    }
    
    public delegate void GetTrialTimeCompletedEventHandler(object sender, GetTrialTimeCompletedEventArgs args);
}
