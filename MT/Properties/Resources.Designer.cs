﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MT.Properties {
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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MT.Properties.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Estimado %userid%,
        ///
        ///Te informamos que la empresa %empresa%, que agregaste o actualizaste recientemente, no ha sido
        ///aprobada por nuestro sistema. 
        ///
        ///Cordialmente,
        ///
        ///El equipo de %sitio%
        ///%sitio_url%.
        /// </summary>
        public static string msj_emp_anulada {
            get {
                return ResourceManager.GetString("msj_emp_anulada", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Estimado %userid%,
        ///
        ///Tu empresa %empresa%, recientemente agregada por tí, ya está disponible en nuestro sistema!
        ///
        ///Cordialmente,
        ///
        ///El equipo de %sitio%
        ///%sitio_url%.
        /// </summary>
        public static string msj_emp_bienvenida {
            get {
                return ResourceManager.GetString("msj_emp_bienvenida", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Estimado %userid%,
        ///
        ///Recientemente has agregado a nuestro sistema la empresa %empresa%.
        ///Te informamos que recibirás un mensaje privado indicándote cuando la misma se encuentre ya disponible en nuestro sistema.
        ///
        ///Cordialmente,
        ///
        ///El equipo de %sitio%
        ///%sitio_url%.
        /// </summary>
        public static string msj_emp_pendiente {
            get {
                return ResourceManager.GetString("msj_emp_pendiente", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Èstimado %userid%,
        ///
        ///Bienvenido a %sitio%!
        ///
        ///Cordialmente,
        ///
        ///El equipo de %sitio%
        ///%sitio_url%.
        /// </summary>
        public static string msj_usr_bienvenida {
            get {
                return ResourceManager.GetString("msj_usr_bienvenida", resourceCulture);
            }
        }
    }
}
