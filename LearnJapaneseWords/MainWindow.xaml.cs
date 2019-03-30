using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
     *
     * 5. Сделать проверку, установлен ли в системе распознаватель рукописного японского ввода.
     * Лайфхак (ПРОВЕРИТЬ!): язык установить, а потом удалить - распознаватель останется, видимо,
     * файлы в винде остаются
     */


    public partial class MainWindow : Window
    {
        Vocabulary vocabulary = new Vocabulary();
        RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();

        public MainWindow()
        {
            InitializeComponent();
            vocabulary.Load("..\\..\\..\\mnn1_vocabulary.json", "..\\..\\..\\hiragana.json", "..\\..\\..\\katakana.json");
        }

        private void BeginTest(string headerText, List<TestQuestion> test) {
            MenuGrid.Visibility = Visibility.Collapsed;

            TestControl testControl = new TestControl("TestControl", headerText, test);
            RootGrid.Children.Add(testControl);
        }


        static int GetNextInt32(RNGCryptoServiceProvider rnd)
        {
            byte[] randomInt = new byte[4];
            rnd.GetBytes(randomInt);
            return Convert.ToInt32(randomInt[0]);
        }

        private void BtnBeginTest_Click(object sender, RoutedEventArgs e)
        {
            var test = vocabulary.Lessons[0].Words.Select(x =>
                new TestQuestion
                {
                    Question = x.Rus,
                    Answers = new[] { x.JpnKana, x.JpnKanji }
                }).OrderBy(x => GetNextInt32(rnd)).ToList();
            BeginTest("Слова", test);
        }

        private void BtnHiraganaTest_Click(object sender, RoutedEventArgs e)
        {
            var test = vocabulary.Hiragana.Where(x => x.Type == KanaSeriesType.Main).Select(x =>
                new TestQuestion
                {
                    Question = x.Romanization,
                    Answers = new[] { x.Character }
                }).OrderBy(x => GetNextInt32(rnd)).ToList();
            BeginTest("Хирагана", test);
        }

        private void BtnKatakanaTest_Click(object sender, RoutedEventArgs e)
        {
            var test = vocabulary.Katakana.Where(x => x.Type == KanaSeriesType.Main).Select(x =>
                new TestQuestion
                {
                    Question = x.Romanization,
                    Answers = new[] { x.Character }
                }).OrderBy(x => GetNextInt32(rnd)).ToList();
            BeginTest("Катакана", test);
        }
    }
}
