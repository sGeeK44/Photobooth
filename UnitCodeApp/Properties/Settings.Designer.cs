﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CabineParty.UnitCodeApp.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files (x86)\\Sparkbooth 4\\Sparkbooth 4.exe")]
        public string PhotoBoothExecPath {
            get {
                return ((string)(this["PhotoBoothExecPath"]));
            }
            set {
                this["PhotoBoothExecPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Sparkbooth 4")]
        public string PhotoBoothProcessName {
            get {
                return ((string)(this["PhotoBoothProcessName"]));
            }
            set {
                this["PhotoBoothProcessName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FFDFD991")]
        public string BackgroundColor {
            get {
                return ((string)(this["BackgroundColor"]));
            }
            set {
                this["BackgroundColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8")]
        public string CodeLength {
            get {
                return ((string)(this["CodeLength"]));
            }
            set {
                this["CodeLength"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DefaultBackgroundPicture.png")]
        public string BackgroundPicture {
            get {
                return ((string)(this["BackgroundPicture"]));
            }
            set {
                this["BackgroundPicture"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\fperin\\Documents\\Code.txt")]
        public string ValidCodeFilePath {
            get {
                return ((string)(this["ValidCodeFilePath"]));
            }
            set {
                this["ValidCodeFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\fperin\\Documents\\Used.txt")]
        public string UsedCodeFilePath {
            get {
                return ((string)(this["UsedCodeFilePath"]));
            }
            set {
                this["UsedCodeFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\fperin\\Documents\\sparkbooth-status.txt")]
        public string PhotoboothStatusFilePath {
            get {
                return ((string)(this["PhotoboothStatusFilePath"]));
            }
            set {
                this["PhotoboothStatusFilePath"] = value;
            }
        }
    }
}
