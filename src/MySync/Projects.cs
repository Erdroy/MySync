// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using CefSharp;

namespace MySync
{
    public class Projects
    {
        private IFrame _frame;
        
        private static void RunJs(string js)
        {
            Program.Browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(js);
        }

        public void LoadProjects()
        {
            _frame = Program.Browser.GetBrowser().MainFrame;

            RunJs("clearFileChanges();");
        }

        public void SelectProject(string projectName)
        {
            // close old project
            // open project and calculate changes

        }
        
        public void Discard()
        {

        }

        public void Pull()
        {

        }

        public void Push()
        {

        }
    }
}
