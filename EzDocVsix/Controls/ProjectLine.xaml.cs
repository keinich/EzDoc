using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EzDocVsix.Controls {

  public class ProjectLineViewModel {
    private string _ProjectName;

    public string ProjectName {
      get { return _ProjectName; }
      set { _ProjectName = value; }
    }

  }

  /// <summary>
  /// Interaction logic for ProjectLine.xaml
  /// </summary>
  public partial class ProjectLine : UserControl {
    public ProjectLine() {
      InitializeComponent();
      this.DataContext = new ProjectLineViewModel() { ProjectName = "Project 1" };
    }

    /// <summary>
    /// Handles click on the button by displaying a message box.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event args.</param>
    [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
    private void button1_Click(object sender, RoutedEventArgs e) {
      MessageBox.Show(
          string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
          "EzDocWindow");
    }

    private String _ProjectName = "Project 1";
    public String ProjectName {
      get { return _ProjectName; }
      set {
        _ProjectName = value;
      }
    }
  }
}
