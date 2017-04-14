// MySync © 2016-2017 Damian 'Erdroy' Korczowski

namespace MySync
{
    public class Projects
    {
        private static void RunJs(string js)
        {
            Program.Browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(js);

        }

        public void LoadProjects()
        {
            RunJs("clearFileChanges();");

        }

        public void SelectProject(string projectName)
        {
            
        }
    }
}
