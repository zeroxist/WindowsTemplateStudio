using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Param_ItemNamespace.Views
{
    // TODO WTS: This page exists purely as an example of how to launch a specific page in response to a protocol launch and pass it a value. It is expected that you will delete this page once you have changed the handling of a protocol launch to meet your needs and redirected to another of your pages.
    public sealed partial class UriSchemeExamplePage : Page, System.ComponentModel.INotifyPropertyChanged
    {
        public UriSchemeExamplePage()
        {
            InitializeComponent();
        }

        // This property is just for displaying the passed in value
        private string _secret;
        public string Secret
        {
            get { return _secret; }
            set { Set(ref _secret, value); }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Capture the passed in value and assign it to a property that's displayed on the view
            this.Secret =  e.Parameter.ToString();
        }
    }
}