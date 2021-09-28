﻿using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace EzDocVsix {
  /// <summary>
  /// This class implements the tool window exposed by this package and hosts a user control.
  /// </summary>
  /// <remarks>
  /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
  /// usually implemented by the package implementer.
  /// <para>
  /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
  /// implementation of the IVsUIElementPane interface.
  /// </para>
  /// </remarks>
  [Guid("2305df81-783f-440b-97db-77a21506fd48")]
  public class EzDocWindow : ToolWindowPane {

    /// <summary>
    /// Initializes a new instance of the <see cref="EzDocWindow"/> class.
    /// </summary>
    public EzDocWindow() : base(null) {
      this.Caption = "EzDocWindow";

      // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
      // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
      // the object returned by the Content property.
      this.Content = new EzDocWindowControl();
    }
  }
}
