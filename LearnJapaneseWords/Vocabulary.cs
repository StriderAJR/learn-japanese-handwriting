using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearnJapaneseWords
{
    class Vocabulary {
        public List<Lesson> Lessons { get; set; }

        public bool Load(string path) {
            if (!File.Exists(path)) return false;

            string json = File.ReadAllText(path);
            var voc = JsonConvert.DeserializeObject<Vocabulary>(json);
            Lessons = voc.Lessons;

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

    public class Kanji {

    }
}
