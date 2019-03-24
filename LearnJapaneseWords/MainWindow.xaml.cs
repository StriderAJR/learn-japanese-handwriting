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

        private int currentQuestion = -1;

        public MainWindow()
        {
            InitializeComponent();
            waitTimer.Tick += (sender, args) => { Recognise(); };
            vocabulary.Load("..\\..\\..\\mnn1_vocabulary.json");

            MenuGrid.Visibility = Visibility.Visible;
            TestGrid.Visibility = Visibility.Collapsed;
        }

        private void Recognise() {
            if (theInkCanvas.Strokes.Count == 0) {
                tbRecognised.Text = string.Empty;
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
                        tbRecognised.Text = result.TopString;
                    else
                        MessageBox.Show("Recognition failed");

                }
            }
        }

        private void RefreshQuestionText() {
            var currentWord = vocabulary.Lessons[0].Words[currentQuestion];
            tbQuestionNumber.Text = $"{currentQuestion + 1} из {vocabulary.Lessons[0].Words.Count}";
            tbQuestion.Text = currentWord.Rus;
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

            currentQuestion = 0;
            RefreshQuestionText();
        }

        private void BtnNextQuestion_Click(object sender, RoutedEventArgs e) {
            if (currentQuestion + 1 < vocabulary.Lessons[0].Words.Count) {
                currentQuestion++;
            }
            else {
                currentQuestion = 0;
            }
            RefreshQuestionText();
        }

        private void BtnCheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            var answer = vocabulary.Lessons[0].Words[currentQuestion];
            var userAnswer = tbRecognised.Text;
            tbAnswer.Text = $"{answer.JpnKana} / {answer.JpnKanji}";
            if (userAnswer == answer.JpnKana || userAnswer == answer.JpnKanji)
            {
                tbAnswer.Background = Brushes.LightGreen;
            }
            else
            {
                tbAnswer.Background = Brushes.PaleVioletRed;
            }
        }
    }
}
