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
    /*
     * TODO
     *
     * 1. Редактирование словаря урока (не пугаться, заполняться будет знающими японский людьми, а не программистом)
     *  - создать урок
     *  - добавить слова, каждое слово состоит из:
     *      - хирагана / катакана (кратко: кана)
     *      - (опционально) кандзи - запись каны, но с использованием иероглифов
     *      - русский перевод
     *  - добавить кандзи
     *      - массив вариантов онъёми
     *      - массив вариантов кунъёми
     *      - массив вариантов перевода
     *  - редактирование словаря должно быть удобным и красивым
     *
     * 2. Доделать тестирование
     *
     * 2.1. Навести красоту в тестировании:
     *  - сделать форму тестирования в стиле материал дизайн
     *  - тестирование слов вперемешку
     *  - если была допущена ошибка, то через несколько вопросов повторить вопрос, в котором была ошибка
     *  - чем меньше ошибок допускается в слове, тем реже оно попадает в тесты
     *
     * 2.2. Разные режимы тестирования
     *  - создание собственных групп слов - в тест попадают не все слова, а либо вручную выбранные пользователем,
     * либо разбитые автоматически на группы из слов определенного размера (по 5, 10, 20 или другая цифры)
     *  - японский - русский, русский - японский (режим японский - русский)
     *  - режим с вариантами ответов
     *  - режим с рукописным ввода ответа
     *  - режим "повторение" - в тест попадают давно изученные слова и немного свежих
     *  - режим "изучение" - в тест попадают только новые слова
     *
     * 2.3. Окно статистики прохождения тестов
     *  - сколько было ошибок
     *  - сколько слов всего изучено
     *  - сколько еще предстоит изучить
     *
     * 3. Режим "Карточки"
     *  - карточки для заучивания слов
     *  - это просто красивое окошко, где отображается вся информация по слову
     *  - есть кнопки "предыдущее", "следующее"
     *
     * 4. Открыть и сохранить данные
     *  - словарь, статистика и остальное хранится в json файле
     *  - при открытии программы есть опция открыть такой файл
     *  - по умолчанию открывается последний открытый файл
     *  - шифровать файл не обязательно, пусть валяется в открытом виде, проще будет вносить правки вручную
     */


    public partial class MainWindow : Window
    {
        DispatcherTimer waitTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
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
