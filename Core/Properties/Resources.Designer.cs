﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NosAyudamos {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NosAyudamos.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to The current instance is read-only, so events cannot be produced..
        /// </summary>
        internal static string DomainObject_ReadOnly {
            get {
                return ResourceManager.GetString("DomainObject_ReadOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot deserialize empty data..
        /// </summary>
        internal static string PersonRepository_EmptyData {
            get {
                return ResourceManager.GetString("PersonRepository_EmptyData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot deserialize empty event type..
        /// </summary>
        internal static string PersonRepository_EmptyEventType {
            get {
                return ResourceManager.GetString("PersonRepository_EmptyEventType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Event type &apos;{0}&apos; is not valid..
        /// </summary>
        internal static string PersonRepository_InvalidEventType {
            get {
                return ResourceManager.GetString("PersonRepository_InvalidEventType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Person with ID &apos;{0}&apos; was not found..
        /// </summary>
        internal static string PersonRepository_NationalIdNotFound {
            get {
                return ResourceManager.GetString("PersonRepository_NationalIdNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Iniciando.
        /// </summary>
        internal static string Startup_Starting {
            get {
                return ResourceManager.GetString("Startup_Starting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string UI_Donee_RejectedWithEarnings {
            get {
                return ResourceManager.GetString("UI_Donee_RejectedWithEarnings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Necesitás ayuda? Por favor decinos cuánto necesitas..
        /// </summary>
        internal static string UI_Donee_SendAmount {
            get {
                return ResourceManager.GetString("UI_Donee_SendAmount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gracias por tu mensaje! Por favor envianos una foto de tu DNI para registrarte para recibir ayuda..
        /// </summary>
        internal static string UI_Donee_SendIdentifier {
            get {
                return ResourceManager.GetString("UI_Donee_SendIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gracias por tu mensaje! Cuánto querés donar?.
        /// </summary>
        internal static string UI_Donor_SendAmount {
            get {
                return ResourceManager.GetString("UI_Donor_SendAmount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Gracias por tu mensaje! Comentanos si necesitas ayuda o estás interesando en ayudar a otros..
        /// </summary>
        internal static string UI_UnknownIntent {
            get {
                return ResourceManager.GetString("UI_UnknownIntent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ayudanos a proteger el sistema verificando que sos un ser humano. Por favor respondé con el número que ves en esta imagen (hacé click para ver la imagen completa). Gracias!.
        /// </summary>
        internal static string UI_VerifyCaptcha {
            get {
                return ResourceManager.GetString("UI_VerifyCaptcha", resourceCulture);
            }
        }
    }
}
