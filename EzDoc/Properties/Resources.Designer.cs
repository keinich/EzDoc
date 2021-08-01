﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EzDoc.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EzDoc.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///
        ///&lt;html&gt;
        ///
        ///&lt;head&gt;
        ///  &lt;link href=&quot;https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css&quot; rel=&quot;stylesheet&quot;
        ///    integrity=&quot;sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC&quot; crossorigin=&quot;anonymous&quot;&gt;
        ///  &lt;link rel=&quot;stylesheet&quot; href=&quot;navtree.css&quot;&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body&gt;
        ///  {{body}}
        ///
        ///  &lt;script src=&quot;navtree.js&quot;&gt;&lt;/script&gt;
        ///  &lt;script src=&quot;https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js&quot;
        ///          integrity=&quot;sha384-MrcW6ZMFYlzcLA8Nl [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string base_template {
            get {
                return ResourceManager.GetString("base_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div class=&quot;content-container&quot;&gt;
        ///  &lt;div id=&quot;content1&quot; class=&quot;content&quot;&gt;
        ///    Content 1. Go to &lt;p class=&quot;content-link-inline&quot; id=&quot;content2-link-inline&quot;&gt;Content 2&lt;/p&gt;
        ///  &lt;/div&gt;
        ///  &lt;div id=&quot;content2&quot; class=&quot;content&quot;&gt;
        ///    Content 2
        ///  &lt;/div&gt;
        ///&lt;/div&gt;.
        /// </summary>
        internal static string content_entry_template {
            get {
                return ResourceManager.GetString("content_entry_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div class=&quot;content-container&quot;&gt;
        ///  &lt;div id=&quot;content1&quot; class=&quot;content&quot;&gt;
        ///    Content 1. Go to &lt;p class=&quot;content-link-inline&quot; id=&quot;content2-link-inline&quot;&gt;Content 2&lt;/p&gt;
        ///  &lt;/div&gt;
        ///  &lt;div id=&quot;content2&quot; class=&quot;content&quot;&gt;
        ///    Content 2
        ///  &lt;/div&gt;
        ///&lt;/div&gt;.
        /// </summary>
        internal static string content_template {
            get {
                return ResourceManager.GetString("content_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;li&gt;&lt;a class=&quot;dropdown-item&quot; href=&quot;{{link}}&quot;&gt;{{text}}&lt;/a&gt;&lt;/li&gt;.
        /// </summary>
        internal static string navbar_dropdownitem_entry_template {
            get {
                return ResourceManager.GetString("navbar_dropdownitem_entry_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;li class=&quot;nav-item dropdown&quot;&gt;
        ///  &lt;a class=&quot;nav-link dropdown-toggle&quot; href=&quot;#&quot; id=&quot;navbarDropdown&quot; role=&quot;button&quot; data-bs-toggle=&quot;dropdown&quot;
        ///     aria-expanded=&quot;false&quot;&gt;
        ///    {{text}}
        ///  &lt;/a&gt;
        ///  &lt;ul class=&quot;dropdown-menu&quot; aria-labelledby=&quot;navbarDropdown&quot;&gt;
        ///    {{entries}}   
        ///    &lt;li&gt;
        ///      &lt;hr class=&quot;dropdown-divider&quot;&gt;
        ///    &lt;/li&gt;
        ///    &lt;li&gt;&lt;a class=&quot;dropdown-item&quot; href=&quot;#&quot;&gt;Something else here&lt;/a&gt;&lt;/li&gt;
        ///  &lt;/ul&gt;
        ///&lt;/li&gt;.
        /// </summary>
        internal static string navbar_dropdownitem_template {
            get {
                return ResourceManager.GetString("navbar_dropdownitem_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;li class=&quot;nav-item&quot;&gt;
        ///  &lt;a class=&quot;nav-link active&quot; aria-current=&quot;page&quot; href=&quot;{{link}}&quot;&gt;{{text}}&lt;/a&gt;
        ///&lt;/li&gt;.
        /// </summary>
        internal static string navbar_item_template {
            get {
                return ResourceManager.GetString("navbar_item_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;nav class=&quot;navbar navbar-expand-lg navbar-light bg-light&quot;&gt;
        ///
        ///  &lt;div class=&quot;container-fluid&quot;&gt;
        ///    &lt;a class=&quot;navbar-brand&quot; href=&quot;#&quot;&gt;Zusagenframework&lt;/a&gt;
        ///    &lt;button class=&quot;navbar-toggler&quot; type=&quot;button&quot; data-bs-toggle=&quot;collapse&quot; data-bs-target=&quot;#navbarSupportedContent&quot;
        ///      aria-controls=&quot;navbarSupportedContent&quot; aria-expanded=&quot;false&quot; aria-label=&quot;Toggle navigation&quot;&gt;
        ///      &lt;span class=&quot;navbar-toggler-icon&quot;&gt;&lt;/span&gt;
        ///    &lt;/button&gt;
        ///    &lt;div class=&quot;collapse navbar-collapse&quot; id=&quot;navbarSupportedContent&quot;&gt;
        ///     [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string navbar_template {
            get {
                return ResourceManager.GetString("navbar_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;li id=&quot;{{name}}-link&quot; class=&quot;content-link&quot;&gt;{{name}}&lt;/li&gt;.
        /// </summary>
        internal static string navtree_contentLink_template {
            get {
                return ResourceManager.GetString("navtree_contentLink_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /* Remove default bullets */
        ///
        ////* Navigation Tree */
        ////* ----------------------------------------------------------------------------------------------- */
        ///
        ///.treeview {
        ///  width: 20%;
        ///  height: 1000px;
        ///  float: left;
        ///  margin: 20px;
        ///  margin-right: 20px;
        ///  overflow-y: scroll;
        ///}
        ///
        ///ul, #navTree {
        ///  list-style-type: none;
        ///  padding-inline-start: 20px;
        ///}
        ///
        ////* Remove margins and padding from the parent ul */
        ///
        ///#navTree {
        ///  margin: 0;
        ///  padding: 0;
        ///}
        ///
        ////* Style the caret */
        ///
        ///.caret {
        ///  curso [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string navtree_css {
            get {
                return ResourceManager.GetString("navtree_css", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to var toggler = document.getElementsByClassName(&quot;caret&quot;);
        ///var contentLinks = document.getElementsByClassName(&quot;content-link&quot;);
        ///var contentLinksInline = document.getElementsByClassName(&quot;content-link-inline&quot;);
        ///var filterBox = document.getElementById(&quot;filter-box&quot;);
        ///var currentActiveContentId = &quot;&quot;
        ///var currentActiveLinkId = &quot;&quot;
        ///var i;
        ///
        ///// Register event listeners
        ///for (i = 0; i &lt; toggler.length; i++) {
        ///  toggler[i].addEventListener(&quot;click&quot;, function () {
        ///    this.parentElement.querySelector(&quot;.nested&quot;).clas [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string navtree_js {
            get {
                return ResourceManager.GetString("navtree_js", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;div class=&quot;treeview&quot;&gt;
        ///  &lt;p class=&quot;mainhead&quot;&gt;{{title}}&lt;/p&gt;
        ///  &lt;form class=&quot;d-flex&quot;&gt;
        ///    &lt;input id=&quot;filter-box&quot; class=&quot;form-control me-2&quot; type=&quot;search&quot; placeholder=&quot;Filter&quot; aria-label=&quot;Search&quot;&gt;
        ///  &lt;/form&gt;
        ///  &lt;hr&gt;
        ///  &lt;ul id=&quot;navTree&quot;&gt;
        ///    {{items}}
        ///  &lt;/ul&gt;
        ///&lt;/div&gt;
        ///
        ///
        ///.
        /// </summary>
        internal static string navtree_template {
            get {
                return ResourceManager.GetString("navtree_template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;li&gt;
        ///  &lt;span class=&quot;caret&quot;&gt;{{name}}&lt;/span&gt;
        ///  &lt;ul class=&quot;nested&quot;&gt;
        ///    {{children}}
        ///  &lt;/ul&gt;
        ///&lt;/li&gt;.
        /// </summary>
        internal static string navtree_toggler_template {
            get {
                return ResourceManager.GetString("navtree_toggler_template", resourceCulture);
            }
        }
    }
}
