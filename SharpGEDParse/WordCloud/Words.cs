//
// https://www.codeproject.com/Articles/224231/Word-Cloud-Tag-Cloud-Generator-Control-for-NET-Win
//

using System;
using System.Collections.Generic;

namespace WordCloud
{
    public interface IWord : IComparable<IWord>
    {
        string Text { get; }
        int Occurrences { get; }
        string GetCaption();
    }

    public class Word : IWord
    {
        public string Text { get; private set; }
        public int Occurrences { get; private set; }

        public Word(KeyValuePair<string, int> textOccurrencesPair)
            : this(textOccurrencesPair.Key, textOccurrencesPair.Value)
        {
        }

        public Word(string text, int occurrences)
        {
            Text = text;
            Occurrences = occurrences;
        }

        public int CompareTo(IWord other)
        {
            return Occurrences - other.Occurrences;
        }

        public string GetCaption()
        {
            return string.Format("{0} - occurrences", Occurrences);
        }
    }

}
