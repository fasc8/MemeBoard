using mrousavy;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace MemeBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Storyboard Storyboard => (Storyboard)this.Resources["imageRotationStoryboard"];
        private DoubleAnimation StoryboardAnimation => (DoubleAnimation)this.Storyboard.Children[0];


        private const double SpeedRatio = .1;

        //private MemeRepo memeRepo = new MemeRepo(@"C:\Users\stream\Desktop\memes2");
        private MemeRepo memeRepo = new MemeRepo(@"C:\Users\fasc\Pictures\Memes");
        private List<HotKey> keyBindings = new List<HotKey>();

        private Meme currentMeme = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ToggleMeme(Meme meme)
        {
            if (this.IsVisible && this.currentMeme == meme)
            {
                this.Hide();
                return;
            }

            Storyboard.Stop();
            ImageBehavior.SetAnimatedSource(this.image, null);

            if (meme.IsAnimated)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(meme.Path);
                img.EndInit();
                ImageBehavior.SetAnimatedSource(image, img);
            }
            else
            {
                this.image.Source = new BitmapImage(new Uri(meme.Path));
            }

            this.currentMeme = meme;

            this.ShowActivated = false;
            this.Show();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            Native.SetWindowExTransparent(hwnd);
        }

        private void RefreshKeyBindings()
        {
            this.keyBindings.ForEach(k => k.Dispose());
            this.keyBindings.Clear();


            foreach (var meme in this.memeRepo.Memes)
            {
                if (Enum.TryParse<Key>(meme.Prefix, true, out var result))
                    this.keyBindings.Add(new HotKey(ModifierKeys.Control, 
                        result, this, _ => this.ToggleMeme(meme)));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshKeyBindings();

            this.memeRepo.Updated += () => this.Dispatcher.Invoke(this.RefreshKeyBindings);

            new HotKey(ModifierKeys.Control, Key.PageUp, this, _ => {
                this.StoryboardAnimation.From = 0;
                this.StoryboardAnimation.To = 360;
                this.Storyboard.Begin();
            });
            new HotKey(ModifierKeys.Control, Key.PageDown, this, _ => {
                this.StoryboardAnimation.From = 360;
                this.StoryboardAnimation.To = 0;
                this.Storyboard.Begin();
            });
            new HotKey(ModifierKeys.Control, Key.End, this, _ => {
                this.Storyboard.Stop();
            });
            new HotKey(ModifierKeys.Control, Key.Up, this, _ =>
            {
                this.Storyboard.SpeedRatio += SpeedRatio;
                this.Storyboard.Begin();
            });
            new HotKey(ModifierKeys.Control, Key.Down, this, _ =>
            {
                if (this.Storyboard.SpeedRatio > 0)
                {
                    this.Storyboard.SpeedRatio -= SpeedRatio;
                }
                this.Storyboard.Begin();
            });
        }
        
        private void TrayExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
