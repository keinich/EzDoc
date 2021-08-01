using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzDoc.Zusagenframework.Bausteinabhängigkeit.Beschreibung {
  
  /// <summary>
  /// Definiert einen Art von Bausteine.
  /// </summary>
  class Bausteindefinition {

    /// <summary>
    /// Art des Bausteins.
    /// </summary>
    public int Art { get; set; }

    /// <summary>
    /// Beschreibt, wie Bausteine dieser Definition berechnet werden sollen.
    /// </summary>
    public Bausteinberechnungskern Bausteinberechnungskern { get; set; }

  }
}
