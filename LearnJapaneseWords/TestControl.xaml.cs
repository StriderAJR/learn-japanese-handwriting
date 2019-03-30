using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
using Microsoft.Ink;

namespace LearnJapaneseWords
{
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class TestControl : UserControl {
        private Recognizer jpnRecognizer = null;

        private readonly List<TestQuestion> _questions;
        private List<TestQuestion> query;
        private int currentQuestion = -1;

        public TestControl(string name, string headerText, List<TestQuestion> questions)
        {
            InitializeComponent();
            tbHeader.Text = headerText;

            Name = name;
            _questions = questions;
            query = new List<TestQuestion>(_questions);

            foreach (var rec in new Recognizers())
            {
                Debug.WriteLine(rec.Name);
                if (rec.Name.Contains("日本語")) jpnRecognizer = rec;
            }

            currentQuestion = 0;
            RefreshQuestionText();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
        }

        private void TheInkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            Debug.WriteLine("TheInkCanvas_StrokeCollected");
            Recognise();
        }

        private void TheInkCanvas_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Debug.WriteLine("TheInkCanvas_ManipulationStarted");
        }

        private void BtnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestion + 1 < query.Count)
            {
                currentQuestion++;
            }
            else
            {
                currentQuestion = 0;
            }
            RefreshQuestionText();
        }

        private void BtnCheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            var answers = query[currentQuestion].Answers;
            var userAnswer = tbRecognised.Text;
            tbAnswer.Text = string.Join(", ", answers);
            if (answers.Contains(userAnswer))
            {
                tbResult.Background = Brushes.LightGreen;
                tbResult.Text = "Верно";
            }
            else
            {
                tbResult.Background = Brushes.PaleVioletRed;
                tbResult.Text = "Неверно";
                if (currentQuestion + 5 >= query.Count) query.Add(query[currentQuestion]);
                else query.Insert(currentQuestion + 5, query[currentQuestion]);
            }

            lblAnswer.Visibility = Visibility.Visible;
            tbAnswer.Visibility = Visibility.Visible;
            tbResult.Visibility = Visibility.Visible;
        }

        private void Recognise()
        {
            if (inkCanvas.Strokes.Count == 0)
            {
                tbRecognised.Text = string.Empty;
                return;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                inkCanvas.Strokes.Save(ms);
                var ink = new Ink();
                ink.Load(ms.ToArray());

                using (RecognizerContext context = jpnRecognizer.CreateRecognizerContext())
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

        private void RefreshQuestionText()
        {
            var currentWord = query[currentQuestion];
            tbQuestionNumber.Text = $"{currentQuestion + 1} из {query.Count}";
            tbQuestion.Text = currentWord.Question;
            inkCanvas.Strokes.Clear();
            tbAnswer.Text = string.Empty;
            tbAnswer.Background = Brushes.Transparent;
            tbRecognised.Text = string.Empty;
            tbResult.Text = string.Empty;
            lblAnswer.Visibility = Visibility.Hidden;
            tbAnswer.Visibility = Visibility.Hidden;
            tbResult.Visibility = Visibility.Hidden;
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.RemoveAt(inkCanvas.Strokes.Count-1);
        }
    }
}
