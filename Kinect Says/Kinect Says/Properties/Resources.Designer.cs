﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kinect_Says.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kinect_Says.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Another application is using the Kinect..
        /// </summary>
        internal static string KinectAppConflict {
            get {
                return ResourceManager.GetString("KinectAppConflict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attach Kinect to PC to play..
        /// </summary>
        internal static string NoKinectError {
            get {
                return ResourceManager.GetString("NoKinectError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kinect is attached to PC, but AC power is missing..
        /// </summary>
        internal static string NoPowerError {
            get {
                return ResourceManager.GetString("NoPowerError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kinect is attached to PC, but an error occured, so it isn&apos;t ready..
        /// </summary>
        internal static string NoSpeechError {
            get {
                return ResourceManager.GetString("NoSpeechError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or more of the Speech prerequisites has not been installed.  Please consult the README for more information..
        /// </summary>
        internal static string NotReady {
            get {
                return ResourceManager.GetString("NotReady", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Say something out loud to control the game!     Say colors and shapes like: &quot;Green Circles&quot;   &quot;Yellow Stars&quot;   &quot;Black Triangles&quot;   &quot;All Colors&quot;     Or say commands like: &quot;Speed Up&quot;   &quot;Slow Down&quot;   &quot;Bigger&quot;   &quot;Smaller&quot;   &quot;Stop&quot;   &quot;Go&quot;   &quot;Giant&quot;        Or say &quot;Reset&quot; to start over!.
        /// </summary>
        internal static string Vocabulary {
            get {
                return ResourceManager.GetString("Vocabulary", resourceCulture);
            }
        }
    }
}
