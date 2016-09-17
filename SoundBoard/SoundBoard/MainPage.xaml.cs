using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SoundBoard.Resources;
using SoundBoard.ViewModels;
using Coding4Fun.Toolkit;
using Coding4Fun.Toolkit.Controls;
using System.IO;
using System.IO.IsolatedStorage;

namespace SoundBoard
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;

            if (selector == null)
                return;
            SoundData data = selector.SelectedItem as SoundData;
            if (data == null)
                return;

            if (File.Exists(data.FilePath))
            {
                PlayAudio.Source = new Uri(data.FilePath, UriKind.RelativeOrAbsolute);
            }
            else
            {
                using (IsolatedStorageFile storageFolder = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(data.FilePath, FileMode.Open, FileAccess.Read, storageFolder))
                    {
                        PlayAudio.SetSource(stream);
                    }
                }

            }
            selector.SelectedItem = null;
}
        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarRecord = new ApplicationBarIconButton();
            appBarRecord.IconUri=new Uri("/Assets/AppBar/microphone.png",UriKind.Relative);
            appBarRecord.Text = AppResources.AppBarRecord;
            appBarRecord.Click += appBarRecord_Click;

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarAbout = new ApplicationBarMenuItem();
            appBarAbout.Text=AppResources.AppBarAbout;
            appBarAbout.Click += appBarAbout_Click;
            ApplicationBar.Buttons.Add(appBarRecord);
            ApplicationBar.MenuItems.Add(appBarAbout);
        }

        void appBarRecord_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RecordingAudio.xaml", UriKind.RelativeOrAbsolute));

        }

        void appBarAbout_Click(object sender, EventArgs e)
        {
            AboutPrompt AboutPrompt = new AboutPrompt();
            AboutPrompt.Show("Hosam Habib", "Hosam", "HosamHabib747@outlook.com", "http://example.com");
        }


        
    }
}