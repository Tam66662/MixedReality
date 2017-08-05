using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Holo2DAppDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            this.viewModel = new MainPageViewModel(this.Dispatcher);
            this.DataContext = this.viewModel;
            this.Loaded += this.MainPage_Loaded;
        }

        private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                if (e.PropertyName == nameof(this.viewModel.Output))
                {
                    ScrollToBottom(mainTextBox);
                }
            }));
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void ScrollToBottom(TextBox textBox)
        {
            var grid = (Grid)VisualTreeHelper.GetChild(textBox, 0);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            {
                object obj = VisualTreeHelper.GetChild(grid, i);
                if (!(obj is ScrollViewer)) continue;
                ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f, true);
                break;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScrollToBottom(mainTextBox);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.viewModel.LogMessageAsync("OnNavigatedTo");
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await this.viewModel.LogMessageAsync("OnNavigatedFrom");
        }
    }
}
