﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Testflow.SequenceManager.Resources.locale {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class i18n_sequence_zh {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal i18n_sequence_zh() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Testflow.SequenceManager.Resources.locale.i18n_sequence_zh", typeof(i18n_sequence_zh).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 无效的文件类型。.
        /// </summary>
        internal static string InvalidFileType {
            get {
                return ResourceManager.GetString("InvalidFileType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 非法的模型版本号{0}。.
        /// </summary>
        internal static string InvalidModelVersion {
            get {
                return ResourceManager.GetString("InvalidModelVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 序列信息和序列参数配置不匹配。.
        /// </summary>
        internal static string UnmatchedData {
            get {
                return ResourceManager.GetString("UnmatchedData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 哈希校验不匹配。序列信息和参数配置信息可能不匹配。继续加载可能会导致非预期的错误。.
        /// </summary>
        internal static string UnmatchedHash {
            get {
                return ResourceManager.GetString("UnmatchedHash", resourceCulture);
            }
        }
    }
}