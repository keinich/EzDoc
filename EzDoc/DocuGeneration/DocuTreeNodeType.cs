using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.DocuGeneration {

  /// <summary>
  /// Type of the Node.
  /// </summary>
  public enum DocuTreeNodeType {
    /// <summary>
    /// Namespace
    /// </summary>
    Namespace,

    /// <summary>
    /// Type
    /// </summary>
    Type,

    /// <summary>
    /// Property
    /// </summary>
    Property,

    /// <summary>
    /// Method
    /// </summary>
    Method,
    Field
  }

}
