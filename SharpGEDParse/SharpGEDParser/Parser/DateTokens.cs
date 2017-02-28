using System;
using System.Collections.Generic;

// A simple, brute-force tokenizer for GEDCOM Event Date parsing.
// * Whitespace is ignored
// * Numbers are numeric only (no decimal/scientific)
// * Words are letters and period (e.g. "B.C.")
// * Special: GEDCOM calendar escape (e.g. "@#JULIAN@")
// * Special: 'phrase' (e.g. "(tombstone)")
// * Limited number of symbols
// * Single line only (control characters are 'unknown')
// * Nested won't work (e.g. "(phrase (phrase) blah)")
// * All else is 'unknown'
//
// Adapted from code by Andrew Deren
// https://www.codeproject.com/Articles/7664/StringTokenizer
//

namespace SharpGEDParser.Parser
{
    public struct Token
    {
        public TokType type;
        public int offset;
        public int length;

        public string getString(string orig)
        {
            return orig.Substring(offset, length).ToUpper();
        }

        public int getInt(string orig)
        {
            return int.Parse(orig.Substring(offset, length));
        }
    }

    public enum TokType
    {
        WHITE,
        EOF,
        CALEN,
        SYMB,
        NUM,
        WORD,
        UNK,
        PHRASE
    }

    public class DateTokens
    {
        private string _str;
        private int _pos;
        private int _maxlen;

        private List<Token> _tokens;

        public List<Token> Tokenize(string datestr)
        {
            _str = datestr;
            _pos = 0;
            _maxlen = datestr.Length;
            
            if (_tokens == null)
                _tokens = new List<Token>(4);
            else
                _tokens.Clear();

            Token tok;
            do
            {
                tok = Next();

                if (tok.type != TokType.EOF &&
                    tok.type != TokType.WHITE)
                    _tokens.Add(tok);

            } while (tok.type != TokType.EOF);

            return _tokens;
        }

        private char LookAhead()
        {
            char ch = _pos >= _maxlen ? '\0' : _str[_pos];
            return ch;
        }

        private void Consume()
        {
            _pos++;
        }

        private Token Next()
        {
            char ch = LookAhead();
            switch (ch)
            {
                case '\0':
                    return makeToken(TokType.EOF);
                case ' ':
                case '\n':
                    return White();
                case '@':
                    return Cal();
                case '(':
                    return Phrase();
                case '-':
                case '~':
                case '/':
                case ',':
                    return SingleTok(TokType.SYMB);
                default:
                    if (Char.IsDigit(ch))
                        return Num();
                    if (Char.IsLetter(ch))
                        return Word();
                    return SingleTok(TokType.UNK);
            }
        }

        private Token SingleTok(TokType _type)
        {
            var tok = makeToken(_type, _pos, 1);
            Consume();
            return tok;
        }

        private Token makeToken(TokType _type, int off = -1, int len = -1)
        {
            return new Token {length = len, offset = off, type = _type};
        }

        private Token Cal()
        {
            // found a leading '@' for calendar
            // grab all until the next '@'
            Consume();
            int savepos = _pos;
            while (true)
            {
                char ch = LookAhead();
                if (ch == '@' || ch == ' ' || ch == '\t' || ch == '\0')
                {
                    var tok = makeToken(TokType.CALEN, savepos, _pos - savepos);
                    if (ch == '@')
                        Consume();
                    return tok;
                }
                Consume();
            }
        }

        private Token Num()
        {
            int savepos = _pos;
            while (true)
            {
                char ch = LookAhead();
                if (!Char.IsDigit(ch))
                    break;
                Consume();
            }
            return makeToken(TokType.NUM, savepos, _pos - savepos);
        }

        private Token Word()
        {
            int savepos = _pos;
            while (true)
            {
                char ch = LookAhead();
                if (!(Char.IsLetter(ch) || ch == '.'))
                    break;
                Consume();
            }
            return makeToken(TokType.WORD, savepos, _pos - savepos);
        }

        private Token White()
        {
            int savepos = _pos;
            while (true)
            {
                char ch = LookAhead();
                if (ch != ' ' && ch != '\t')
                    break;
                Consume();
            }
            return makeToken(TokType.WHITE, savepos, _pos - savepos);
        }

        private Token Phrase()
        {
            // found a leading '(' for phrase
            // grab all until the next ')'
            Consume();
            int savepos = _pos;
            while (true)
            {
                char ch = LookAhead();
                if (ch == ')' || ch == '\0')
                {
                    var tok = makeToken(TokType.PHRASE, savepos, _pos - savepos);
                    if (ch == ')')
                        Consume();
                    return tok;
                }
                Consume();
            }
        }

    }
}
