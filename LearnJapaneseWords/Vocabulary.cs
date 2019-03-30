using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearnJapaneseWords
{
    public class TestQuestion {
        /// <summary>
        /// Текст вопроса
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// Варианты ответов
        /// </summary>
        public string[] Answers { get; set; }
    }

    class Vocabulary {
        public List<Lesson> Lessons { get; set; }
        public List<Kana> Hiragana { get; set; }
        public List<Kana> Katakana { get; set; }

        public bool Load(string vocabularyPath, string hiraganaPath, string katakanaPath) {
            if (File.Exists(vocabularyPath)) {
                string json = File.ReadAllText(vocabularyPath);
                var voc = JsonConvert.DeserializeObject<Vocabulary>(json);
                Lessons = voc.Lessons;
            }

            if (File.Exists(hiraganaPath)) {
                string json = File.ReadAllText(hiraganaPath);
                Hiragana = (JsonConvert.DeserializeObject<Kana[]>(json)).ToList();
            }

            if (File.Exists(katakanaPath)) {
                string json = File.ReadAllText(katakanaPath);
                Katakana = (JsonConvert.DeserializeObject<Kana[]>(json)).ToList();
            }

            return true;
        }
    }

    public class Lesson {
        public string LessonName { get; set; }
        public List<Word> Words { get; set; }
        public List<Kanji> Kanjis { get; set; }
    }

    public class Word {
        public string JpnKana { get; set; }
        public string JpnKanji { get; set; }
        public string Rus { get; set; }
    }

    public class Kana {
        [JsonProperty("char_id")]
        public string CharId { get; set; }
        [JsonProperty("character")]
        public string Character { get; set; }
        [JsonProperty("romanization")]
        public string Romanization { get; set; }
        [JsonProperty("type")]
        public KanaSeriesType Type { get; set; }
    }

    public enum KanaSeriesType
    {
        Main = 0, Extra = 1, Multi = 2, MultiExtra = 3
    }

    public class Kanji {

    }
}
