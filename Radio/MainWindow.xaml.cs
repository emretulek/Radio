using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using LibVLCSharp.Shared;
using Newtonsoft.Json;
using Widgets.Common;

namespace Radio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWidgetWindow
    {
        public readonly static string WidgetName = "Radio";
        public readonly static string SettingsFile = "settings.radio.json";
        private readonly Config config = new(SettingsFile);
        private RadioViewModel.SettingsStruct Settings = RadioViewModel.Default;
        public RadioViewModel Radio { get; set; }
        private DispatcherTimer? _timer;
        private DoubleAnimation? MarqueeAnimation;

        public MainWindow()
        {
            LoadSettings();

            Radio = new RadioViewModel(Settings);
            InitializeComponent();
            DataContext = Radio;
        }

        // Window to WdigetWindow
        public WidgetWindow WidgetWindow()
        {
            return new WidgetWindow(this);
        }

        // WidgetWindow default settings
        public static WidgetDefaultStruct WidgetDefaultStruct()
        {
            return new()
            {
                Width = 400,
                MinHeight = 200,
                MaxHeight = 600,
                SizeToContent = SizeToContent.Height
            };
        }

        // config file load
        private void LoadSettings()
        {
            try
            {
                Settings.FontSize = PropertyParser.ToFloat(config.GetValue("font_size"), Settings.FontSize);
                Settings.PrimaryColor = PropertyParser.ToColorBrush(config.GetValue("primary_color"), Settings.PrimaryColor.ToString());
                Settings.PrimaryColorLight = PropertyParser.ToColorBrush(config.GetValue("primary_color_light"), Settings.PrimaryColorLight.ToString());
                Settings.SecondaryColor = PropertyParser.ToColorBrush(config.GetValue("secondary_color"), Settings.SecondaryColor.ToString());
                Settings.SecondaryColorLight = PropertyParser.ToColorBrush(config.GetValue("secondary_color_light"), Settings.SecondaryColorLight.ToString());
                Settings.TextColor = PropertyParser.ToColorBrush(config.GetValue("text_color"), Settings.TextColor.ToString());
                Settings.Volume = PropertyParser.ToInt(config.GetValue("radio_volume"), Settings.Volume);
                Settings.RadioIndex = PropertyParser.ToInt(config.GetValue("radio_index"), Settings.RadioIndex);
                var stringJson = PropertyParser.ToString(config.GetValue("radio_list"));
                Settings.RadioList = JsonConvert.DeserializeObject<ObservableCollection<Radio>>(stringJson) ?? Settings.RadioList;
            }
            catch (Exception)
            {
                config.Add("font_size", Settings.FontSize);
                config.Add("primary_color", Settings.PrimaryColor);
                config.Add("primary_color_light", Settings.PrimaryColorLight);
                config.Add("secondary_color", Settings.SecondaryColor);
                config.Add("secondary_color_light", Settings.SecondaryColorLight);
                config.Add("text_color", Settings.TextColor);
                config.Add("radio_volume", Settings.Volume);
                config.Add("radio_index", Settings.RadioIndex);
                config.Add("radio_list", Settings.RadioList ?? []);

                config.Save();
            }
        }


        private void OrderedRadioListAndUpdateSettings()
        {
            var orderedRadioList = Radio.RadioList.OrderBy(item => item.Name).ToList();
            var selectedRadio = Radio.SelectedRadio;
            Radio.RadioList.Clear();

            foreach (var radio in orderedRadioList)
            {
                Radio.RadioList.Add(radio);
            }

            Radio.SelectedRadio = selectedRadio;
            RadioListBox.SelectedItem = Radio.SelectedRadio;
            Settings.RadioList = Radio.RadioList;
            Settings.RadioIndex = RadioListBox.SelectedIndex;
   
            RadioListBox.ScrollIntoView(RadioListBox.SelectedItem);

            config.Add("font_size", Settings.FontSize);
            config.Add("primary_color", Settings.PrimaryColor);
            config.Add("primary_color_light", Settings.PrimaryColorLight);
            config.Add("secondary_color", Settings.SecondaryColor);
            config.Add("secondary_color_light", Settings.SecondaryColorLight);
            config.Add("text_color", Settings.TextColor);
            config.Add("radio_volume", Settings.Volume);
            config.Add("radio_index", Settings.RadioIndex);
            config.Add("radio_list", Settings.RadioList ?? []);

            config.Save();
        }


        // After content rendering 
        protected override void OnContentRendered(EventArgs e)
        {
            var mainWindowsResource = this.Resources.MergedDictionaries;
            var styleResource = mainWindowsResource.FirstOrDefault(d => d.Source.OriginalString == "Style.xaml");

            if (styleResource != null)
            {
                styleResource["PrimaryColor"] = Settings.PrimaryColor.Color;
                styleResource["PrimaryColorLight"] = Settings.PrimaryColorLight.Color;
                styleResource["SecondaryColor"] = Settings.SecondaryColor.Color;
                styleResource["SecondaryColorLight"] = Settings.SecondaryColorLight.Color;
                styleResource["TextColor"] = Settings.TextColor.Color;
                styleResource["AnimateBrush_1"] = Settings.PrimaryColor;
                styleResource["AnimateBrush_2"] = Settings.PrimaryColorLight;
            }

            Lazy_SelectionChanged();
            RadioListBox.ScrollIntoView(RadioListBox.SelectedItem);
            base.OnContentRendered(e);
        }

        // Add new radio with popup window
        private void AddRadioWindow_Click(object sender, RoutedEventArgs e)
        {
            var popup = new PopupWindow
            {
                Owner = this,
                Width = 300,
                SizeToContent = SizeToContent.Height
            };

            if (popup.ShowDialog() == true && popup.NewRadio != null)
            {
                Radio.RadioList.Add(popup.NewRadio);
                Radio.SelectedRadio = popup.NewRadio;
                OrderedRadioListAndUpdateSettings();
            }
        }

        // Delete radio with delete key
        private void DeleteRadio(Radio? radioList = null)
        {
            radioList ??= Radio.SelectedRadio;

            if (radioList != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                   $"Are you sure delete {radioList.Name} radio?",
                   "Delete Confirmation", MessageBoxButton.YesNo
                   );

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var willdeleteIndex = RadioListBox.Items.IndexOf(radioList);
                    Radio.RadioList.Remove(radioList);
                    if(willdeleteIndex > 0)
                    {
                        RadioListBox.SelectedItem = RadioListBox.Items[willdeleteIndex-1];
                        Radio.SelectedRadio = (Radio)RadioListBox.SelectedItem;
                    }
      
                    OrderedRadioListAndUpdateSettings();
                }
            }
        }

        // RadioListbox item context menu
        // Delete Radio
        // Edit Radio
        public void ListBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && e.OriginalSource is DependencyObject source)
            {
                var listBoxItem = ItemsControl.ContainerFromElement(listBox, source) as ListBoxItem;

                if (listBoxItem?.DataContext is Radio radio)
                {
                    ContextMenu contextMenu = new();
                    MenuItem option1 = new() { Header = "Edit", Icon = "🖉" };
                    option1.Click += (sender, e) =>
                    {
                        var popup = new PopupWindow(radio)
                        {
                            Owner = this,
                            Width = 300,
                            SizeToContent = SizeToContent.Height,
                        };

                        if (popup.ShowDialog() == true && popup.NewRadio != null)
                        {
                            if (Radio.RadioList.Contains(popup.NewRadio))
                            {
                                OrderedRadioListAndUpdateSettings();
                            }
                        }
                    };
                    MenuItem option2 = new() { Header = "Delete", Icon = "✗" };
                    option2.Click += (sender, e) =>
                    {
                        DeleteRadio(radio);
                    };
                    contextMenu.Items.Add(option1);
                    contextMenu.Items.Add(option2);

                    contextMenu.IsOpen = true;
                }
            }

            e.Handled = true;
        }

        // When window closed
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Radio.Stop();
            Radio.Dispose();

            config.Add("radio_index", RadioListBox.SelectedIndex);
            config.Add("radio_list", Settings.RadioList);
            config.Save();

            Logger.Info($"{WidgetName} is closed");
        }

        // Sortcut keys
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.MediaPlayPause || e.Key == Key.Space || e.Key == Key.Enter)
            {
                PlayButton_Click(this, e);
            }
            else if (e.Key == Key.MediaNextTrack || e.Key == Key.Right || e.Key == Key.Down)
            {
                ForwardButton_Click(this, e);
            }
            else if (e.Key == Key.MediaPreviousTrack || e.Key == Key.Left || e.Key == Key.Up)
            {
                BackwardButton_Click(this, e);
            }
            else if (e.Key == Key.Delete)
            {
                DeleteRadio();
            }

            e.Handled = true;
        }

        // Animated text update for content length
        private void ScrollingTextChanging(object? sender, SizeChangedEventArgs e)
        {
            if (Radio.PlayerState() == VLCState.Playing)
            {
                double windowWidth = ActualWidth;
                ScrollingText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                MarqueeAnimation = new DoubleAnimation
                {
                    From = windowWidth,
                    To = ScrollingText.DesiredSize.Width * -1,
                    Duration = new Duration(TimeSpan.FromSeconds(10)),
                    RepeatBehavior = RepeatBehavior.Forever
                };
                TextTransform.BeginAnimation(TranslateTransform.XProperty, MarqueeAnimation);
            }
        }

        // Play & Stop Button
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("AnimateNotes");

            if (Radio.PlayerState() != VLCState.Playing)
            {
                Radio.Play();
                TextTransform.BeginAnimation(TranslateTransform.XProperty, MarqueeAnimation);
                storyboard.Begin();
                return;
            }

            if (Radio.PlayerState() != VLCState.Stopped)
            {
                Radio.Stop();
                TextTransform.BeginAnimation(TranslateTransform.XProperty, null);
                storyboard.Stop();
                return;
            }
        }

        // Backward Button
        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            RadioListBox.SelectedIndex = Math.Max(RadioListBox.SelectedIndex - 1, 0);
            RadioListBox.ScrollIntoView(RadioListBox.SelectedItem);
        }

        // Forward Button
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            RadioListBox.SelectedIndex = Math.Min(RadioListBox.SelectedIndex + 1, RadioListBox.Items.Count);
            RadioListBox.ScrollIntoView(RadioListBox.SelectedItem);
        }

        // Delay selected radio changing
        private void Lazy_SelectionChanged()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Selection_Timer_Tick;
        }

        // Delay timer tick
        private async void Selection_Timer_Tick(object? sender, EventArgs e)
        {
            _timer?.Stop();

            if (Radio.PlayerState() == VLCState.Playing)
            {
                Radio.Stop();
                await Task.Delay(500);
                Radio.Play();
            }
        }

        // Radio Listbox selected chaned
        private void RadioListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _timer?.Stop();
            ListBoxItem selectedItem = (ListBoxItem)RadioListBox.ItemContainerGenerator.ContainerFromItem(RadioListBox.SelectedItem);

            if (selectedItem == null) return;

            BackwardButton.IsEnabled = !(RadioListBox.SelectedIndex <= 0);
            ForwardButton.IsEnabled = !(RadioListBox.SelectedIndex >= RadioListBox.Items.Count - 1);

            _timer?.Start();
        }

        // When Mohusewhell event only 1 item change
        private void RadioListScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var listBox = sender as ListBox;
            var scrollViewer = FindScrollViewer(listBox);

            if (listBox != null && scrollViewer != null)
            {
                var scrollAmount = 1;

                if (e.Delta > 0)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - Math.Abs(scrollAmount));
                }
                else
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + Math.Abs(scrollAmount));
                }
                e.Handled = true;
            }
        }

        // Helper function for scrollviewer
        private static ScrollViewer? FindScrollViewer(DependencyObject? element)
        {
            if (element is ScrollViewer scrollViewer) return scrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var result = FindScrollViewer(child);
                if (result != null) return result;
            }

            return null;
        }
    }
}