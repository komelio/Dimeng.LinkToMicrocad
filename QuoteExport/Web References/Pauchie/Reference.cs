﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34209
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.34209 版自动生成。
// 
#pragma warning disable 1591

namespace QuoteExport.Pauchie {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="PauchieWebServicePortBinding", Namespace="http://www.Pauchie.com/")]
    public partial class PauchieWebServiceService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback doDASSMOperationCompleted;
        
        private System.Threading.SendOrPostCallback writeExcelOperationCompleted;
        
        private System.Threading.SendOrPostCallback readExcelOperationCompleted;
        
        private System.Threading.SendOrPostCallback writeCsvOperationCompleted;
        
        private System.Threading.SendOrPostCallback getAllBaseInfoOperationCompleted;
        
        private System.Threading.SendOrPostCallback getIncrementBaseInfoOperationCompleted;
        
        private System.Threading.SendOrPostCallback notifyOptimizationToBeFinishedOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public PauchieWebServiceService() {
            this.Url = global::QuoteExport.Properties.Settings.Default.QuoteExport_Pauchie_PauchieWebServiceService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event doDASSMCompletedEventHandler doDASSMCompleted;
        
        /// <remarks/>
        public event writeExcelCompletedEventHandler writeExcelCompleted;
        
        /// <remarks/>
        public event readExcelCompletedEventHandler readExcelCompleted;
        
        /// <remarks/>
        public event writeCsvCompletedEventHandler writeCsvCompleted;
        
        /// <remarks/>
        public event getAllBaseInfoCompletedEventHandler getAllBaseInfoCompleted;
        
        /// <remarks/>
        public event getIncrementBaseInfoCompletedEventHandler getIncrementBaseInfoCompleted;
        
        /// <remarks/>
        public event notifyOptimizationToBeFinishedCompletedEventHandler notifyOptimizationToBeFinishedCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.Pauchie.com/", ResponseNamespace="http://www.Pauchie.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void doDASSM([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] int arg1, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg2) {
            this.Invoke("doDASSM", new object[] {
                        arg0,
                        arg1,
                        arg2});
        }
        
        /// <remarks/>
        public void doDASSMAsync(string arg0, int arg1, string arg2) {
            this.doDASSMAsync(arg0, arg1, arg2, null);
        }
        
        /// <remarks/>
        public void doDASSMAsync(string arg0, int arg1, string arg2, object userState) {
            if ((this.doDASSMOperationCompleted == null)) {
                this.doDASSMOperationCompleted = new System.Threading.SendOrPostCallback(this.OndoDASSMOperationCompleted);
            }
            this.InvokeAsync("doDASSM", new object[] {
                        arg0,
                        arg1,
                        arg2}, this.doDASSMOperationCompleted, userState);
        }
        
        private void OndoDASSMOperationCompleted(object arg) {
            if ((this.doDASSMCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.doDASSMCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.Pauchie.com/", ResponseNamespace="http://www.Pauchie.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void writeExcel([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string filePath) {
            this.Invoke("writeExcel", new object[] {
                        filePath});
        }
        
        /// <remarks/>
        public void writeExcelAsync(string filePath) {
            this.writeExcelAsync(filePath, null);
        }
        
        /// <remarks/>
        public void writeExcelAsync(string filePath, object userState) {
            if ((this.writeExcelOperationCompleted == null)) {
                this.writeExcelOperationCompleted = new System.Threading.SendOrPostCallback(this.OnwriteExcelOperationCompleted);
            }
            this.InvokeAsync("writeExcel", new object[] {
                        filePath}, this.writeExcelOperationCompleted, userState);
        }
        
        private void OnwriteExcelOperationCompleted(object arg) {
            if ((this.writeExcelCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.writeExcelCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.Pauchie.com/", ResponseNamespace="http://www.Pauchie.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string readExcel([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string filePath) {
            object[] results = this.Invoke("readExcel", new object[] {
                        filePath});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void readExcelAsync(string filePath) {
            this.readExcelAsync(filePath, null);
        }
        
        /// <remarks/>
        public void readExcelAsync(string filePath, object userState) {
            if ((this.readExcelOperationCompleted == null)) {
                this.readExcelOperationCompleted = new System.Threading.SendOrPostCallback(this.OnreadExcelOperationCompleted);
            }
            this.InvokeAsync("readExcel", new object[] {
                        filePath}, this.readExcelOperationCompleted, userState);
        }
        
        private void OnreadExcelOperationCompleted(object arg) {
            if ((this.readExcelCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.readExcelCompleted(this, new readExcelCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.Pauchie.com/", ResponseNamespace="http://www.Pauchie.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string writeCsv([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string value) {
            object[] results = this.Invoke("writeCsv", new object[] {
                        value});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void writeCsvAsync(string value) {
            this.writeCsvAsync(value, null);
        }
        
        /// <remarks/>
        public void writeCsvAsync(string value, object userState) {
            if ((this.writeCsvOperationCompleted == null)) {
                this.writeCsvOperationCompleted = new System.Threading.SendOrPostCallback(this.OnwriteCsvOperationCompleted);
            }
            this.InvokeAsync("writeCsv", new object[] {
                        value}, this.writeCsvOperationCompleted, userState);
        }
        
        private void OnwriteCsvOperationCompleted(object arg) {
            if ((this.writeCsvCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.writeCsvCompleted(this, new writeCsvCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.Pauchie.com/", ResponseNamespace="http://www.Pauchie.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string getAllBaseInfo() {
            object[] results = this.Invoke("getAllBaseInfo", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getAllBaseInfoAsync() {
            this.getAllBaseInfoAsync(null);
        }
        
        /// <remarks/>
        public void getAllBaseInfoAsync(object userState) {
            if ((this.getAllBaseInfoOperationCompleted == null)) {
                this.getAllBaseInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetAllBaseInfoOperationCompleted);
            }
            this.InvokeAsync("getAllBaseInfo", new object[0], this.getAllBaseInfoOperationCompleted, userState);
        }
        
        private void OngetAllBaseInfoOperationCompleted(object arg) {
            if ((this.getAllBaseInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getAllBaseInfoCompleted(this, new getAllBaseInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.Pauchie.com/", ResponseNamespace="http://www.Pauchie.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string getIncrementBaseInfo([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] System.DateTime arg0, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] [System.Xml.Serialization.XmlIgnoreAttribute()] bool arg0Specified) {
            object[] results = this.Invoke("getIncrementBaseInfo", new object[] {
                        arg0,
                        arg0Specified});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getIncrementBaseInfoAsync(System.DateTime arg0, bool arg0Specified) {
            this.getIncrementBaseInfoAsync(arg0, arg0Specified, null);
        }
        
        /// <remarks/>
        public void getIncrementBaseInfoAsync(System.DateTime arg0, bool arg0Specified, object userState) {
            if ((this.getIncrementBaseInfoOperationCompleted == null)) {
                this.getIncrementBaseInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetIncrementBaseInfoOperationCompleted);
            }
            this.InvokeAsync("getIncrementBaseInfo", new object[] {
                        arg0,
                        arg0Specified}, this.getIncrementBaseInfoOperationCompleted, userState);
        }
        
        private void OngetIncrementBaseInfoOperationCompleted(object arg) {
            if ((this.getIncrementBaseInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getIncrementBaseInfoCompleted(this, new getIncrementBaseInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://www.Pauchie.com/", ResponseNamespace="http://www.Pauchie.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void notifyOptimizationToBeFinished([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string arg0) {
            this.Invoke("notifyOptimizationToBeFinished", new object[] {
                        arg0});
        }
        
        /// <remarks/>
        public void notifyOptimizationToBeFinishedAsync(string arg0) {
            this.notifyOptimizationToBeFinishedAsync(arg0, null);
        }
        
        /// <remarks/>
        public void notifyOptimizationToBeFinishedAsync(string arg0, object userState) {
            if ((this.notifyOptimizationToBeFinishedOperationCompleted == null)) {
                this.notifyOptimizationToBeFinishedOperationCompleted = new System.Threading.SendOrPostCallback(this.OnnotifyOptimizationToBeFinishedOperationCompleted);
            }
            this.InvokeAsync("notifyOptimizationToBeFinished", new object[] {
                        arg0}, this.notifyOptimizationToBeFinishedOperationCompleted, userState);
        }
        
        private void OnnotifyOptimizationToBeFinishedOperationCompleted(object arg) {
            if ((this.notifyOptimizationToBeFinishedCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.notifyOptimizationToBeFinishedCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    public delegate void doDASSMCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    public delegate void writeExcelCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    public delegate void readExcelCompletedEventHandler(object sender, readExcelCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class readExcelCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal readExcelCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    public delegate void writeCsvCompletedEventHandler(object sender, writeCsvCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class writeCsvCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal writeCsvCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    public delegate void getAllBaseInfoCompletedEventHandler(object sender, getAllBaseInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getAllBaseInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getAllBaseInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    public delegate void getIncrementBaseInfoCompletedEventHandler(object sender, getIncrementBaseInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getIncrementBaseInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getIncrementBaseInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.34209")]
    public delegate void notifyOptimizationToBeFinishedCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591