using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Holo2DAppDemo
{
    public class MainPageViewModel : ViewModelBase
    {
        private string output;

        public string Title {  get { return "HoloLens 2D App Demo"; } }

        public string Output { get { return this.output; } set { this.output = value; OnPropertyChanged(); } }

        private CoreDispatcher dispatcher;

        public MainPageViewModel(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public async Task LogMessageAsync(string message)
        {
            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                if (!string.IsNullOrEmpty(message))
                {
                    StateHelper.AppendString($"{message}");
                }

                this.Output = StateHelper.PreviousString;
            }));
        }
    }
}
