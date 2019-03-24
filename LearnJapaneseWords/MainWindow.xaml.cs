using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Ink;

namespace LearnJapaneseWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer waitTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        Vocabulary vocabulary = new Vocabulary();

        public MainWindow()
        {
            InitializeComponent();
            waitTimer.Tick += (sender, args) => { Recognise(); };
            vocabulary.Load("..\\..\\..\\mnn1_vocabulary.json");
        }

        private void Recognise() {
            if (theInkCanvas.Strokes.Count == 0) {
                textBox1.Text = string.Empty;
                return;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                theInkCanvas.Strokes.Save(ms);
                var myInkCollector = new InkCollector();
                var ink = new Ink();
                ink.Load(ms.ToArray());

                using (RecognizerContext context = new RecognizerContext())
                {
                    context.Strokes = ink.Strokes;
                    var result = context.Recognize(out var status);
                    if (status == RecognitionStatus.NoError)
                        textBox1.Text = result.TopString;
                    else
                        MessageBox.Show("Recognition failed");

                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            theInkCanvas.Strokes.Clear();
        }

        private void TheInkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            Debug.WriteLine("TheInkCanvas_StrokeCollected");
            waitTimer.Stop();
            waitTimer.Start();
        }

        private void TheInkCanvas_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Debug.WriteLine("TheInkCanvas_ManipulationStarted");
            waitTimer.Stop();
        }

        private void BtnBeginTest_Click(object sender, RoutedEventArgs e) {
            MenuGrid.Visibility = Visibility.Collapsed;
            TestGrid.Visibility = Visibility.Visible;
        }
    }
}
