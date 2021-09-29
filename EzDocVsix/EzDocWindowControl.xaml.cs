using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EzDocVsix {
  /// <summary>
  /// Interaction logic for EzDocWindowControl.
  /// </summary>
  public partial class EzDocWindowControl : UserControl {
    /// <summary>
    /// Initializes a new instance of the <see cref="EzDocWindowControl"/> class.
    /// </summary>
    public EzDocWindowControl() {
      this.InitializeComponent();

    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      InitProjects();
    }

    private void InitProjects() {
      List<Project> projects = TryGetProjects();
      this.MyContainer.Children.Clear();
      if (projects.Count >= 0) {
        Button b = new Button() {
          Content = $"Reload Projects"
        };
        b.Click += (s, e) => InitProjects();

        this.MyContainer.Children.Add(b);
      }
      foreach (Project project in projects) {
        Button b = new Button() {
          Content = $"Generate Docu for {project.Name}",
          Tag = project
        };
        b.Click += OnProjectButtonClick;
        this.MyContainer.Children.Add(b);
      }
    }

    private void OnProjectButtonClick(object sender, RoutedEventArgs e) {
      Button b = (Button)sender;
      Project p = (Project)b.Tag;
      Task<ProjectHelpers.Result> generateTask = ProjectHelpers.GenerateDocu1Async(p);
      generateTask.Wait();
      ProjectHelpers.Result r = generateTask.Result;
      if (r.Success) {
        MessageBox.Show(r.Message, "Success");
      } else {
        MessageBox.Show(r.Message, "Failure");
      }
    }

    private List<Project> TryGetProjects() {
      //if (Scope == EnvDTE.vsBuildScope.vsBuildScopeSolution) {
      //errorListProvider.Tasks.Clear();
      List<Project> result = new List<Project>();
      DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
      var sol = dte2.Solution;
      var projs = sol.Projects;
      foreach (var proj in sol) {
        var project = proj as Project;
        result.Add(project);
        if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder) {
          var innerProjects = GetSolutionFolderProjects(project);
          foreach (var innerProject in innerProjects) {
            result.Add(innerProject);
          }
        }
      }
      return result;
    }

    private IEnumerable<Project> GetSolutionFolderProjects(Project project) {
      List<Project> projects = new List<Project>();
      var y = (project.ProjectItems as ProjectItems).Count;
      for (var i = 1; i <= y; i++) {
        var x = project.ProjectItems.Item(i).SubProject;
        var subProject = x as Project;
        if (subProject != null) {
          //Carried out work and added projects as appropriate
          projects.Add(subProject);
        }
      }

      return projects;
    }
    //}
  }
}