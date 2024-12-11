using System.ComponentModel.Composition;
using Widgets.Common;

namespace Radio
{
    [Export(typeof(IPlugin))]
    internal class RadioPlugin : IPlugin
    {
        public string Name => MainWindow.WidgetName;
        public string? ConfigFile => MainWindow.SettingsFile;
        public WidgetDefaultStruct WidgetDefaultStruct()
        {
            return MainWindow.WidgetDefaultStruct();
        }
        public WidgetWindow WidgetWindow()
        {
            return new MainWindow().WidgetWindow();
        }
    }
}
