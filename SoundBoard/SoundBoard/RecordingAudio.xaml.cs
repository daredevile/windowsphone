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
using Coding4Fun.Toolkit.Audio;
using Coding4Fun.Toolkit.Audio.Helpers;
using System.IO.IsolatedStorage;
using System.IO;
using Coding4Fun.Toolkit.Controls;
using SoundBoard.ViewModels;
using Newtonsoft.Json;

namespace SoundBoard
{
    public partial class RecordingAudio : PhoneApplicationPage
    {
        MicrophoneRecorder _recorder = new MicrophoneRecorder();
        IsolatedStorageFileStream _audioStream;

        string _tempFileName = "tempWav.wav";


        public RecordingAudio()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void BuildLocalizedApplicationBar()
        {

           ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton recordAudioAppBar = new ApplicationBarIconButton();
            recordAudioAppBar.IconUri = new Uri("/Assets/AppBar/save.png", UriKind.Relative);
            recordAudioAppBar.Text = AppResources.AppBarSave;
            recordAudioAppBar.Click += recordAudioAppBar_Click;
            ApplicationBar.Buttons.Add(recordAudioAppBar);
            //recordAudioAppBar.IsEnabled = false;


        }

        void recordAudioAppBar_Click(object sender, EventArgs e)
        {
            InputPrompt fileName = new InputPrompt();
            fileName.Title = "Sound Name";
            fileName.Message = "what should we call the sound";
            fileName.Completed += fileName_Completed;
            fileName.Show();
        }

        void fileName_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                //create a sounddata obj
                SoundData soundData = new SoundData();
                soundData.FilePath = string.Format("/customAudio/{0}.wav",DateTime.Now.ToFileTime());
                soundData.Title = e.Result;

                // save wav file into direction /customAudio/
                using(IsolatedStorageFile isoStorage=IsolatedStorageFile.GetUserStoreForApplication()){
                if(!isoStorage.DirectoryExists("/customAudio/"))
                    isoStorage.CreateDirectory("/customAudio/");

                isoStorage.MoveFile(_tempFileName, soundData.FilePath);
                
                }
                // add the sounddata to app.viewmodel.customsounds
                App.ViewModel.CustomSounds.Items.Add(soundData);


                //save the list of CustomSound to isolatedStorage,applocationsetting
                var data = JsonConvert.SerializeObject(App.ViewModel.CustomSounds);
                IsolatedStorageSettings.ApplicationSettings[SoundModel.CustomSoundKey] = data;
                IsolatedStorageSettings.ApplicationSettings.Save();




                NavigationService.Navigate(new  Uri("/Mainpage.xaml",UriKind.RelativeOrAbsolute));
            

            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationBar.IsVisible = false;
            PlayButton.IsEnabled = false;
            _recorder.Start();
            RotateCircle.Begin();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _recorder.Stop();
            SaveTempAudio(_recorder.Buffer);
            PlayButton.IsEnabled = true;
            ApplicationBar.IsVisible = true;
            RotateCircle.Stop();
        }

        private void SaveTempAudio(MemoryStream buffre) {

            if (buffre == null)
                throw new ArgumentNullException("Attempting a save an empty sound buffer");


            if (_audioStream != null)
            {

                AudioPlay.Stop();
                AudioPlay.Source = null;
                _audioStream.Dispose();

            }
            var bytes = buffre.GetWavAsByteArray(_recorder.SampleRate);

            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStorage.FileExists(_tempFileName))
                    isoStorage.DeleteFile(_tempFileName);
                _tempFileName = string.Format("{0}.wav", DateTime.Now.ToFileTime());
                _audioStream = isoStorage.CreateFile(_tempFileName);
                _audioStream.Write(bytes, 0, bytes.Length);
                //play>>>

                AudioPlay.SetSource(_audioStream);


            }
        
        
        
        }

        private void PlayAudioClick(object sender, RoutedEventArgs e)
        {
            AudioPlay.Play();
        }

    }
}