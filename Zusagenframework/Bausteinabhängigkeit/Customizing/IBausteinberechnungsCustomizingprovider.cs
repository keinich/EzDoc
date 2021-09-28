namespace Zusagenframework.Bausteinabhängigkeit.Customizing {

  /// <summary>
  /// Customizing der Bausteinberechnung.
  /// </summary>
  interface IBausteinberechnungsCustomizingprovider {

    /// <summary>
    /// Berechnet Bausteine.
    /// Siehe <see cref="IBausteinberechnungsCustomizingprovider"/>
    /// Siehe <see href="Conceptual.Concept"/>
    /// </summary>
    /// <returns>int</returns>
    public int BerechneCustomBausteine();

  }
}
