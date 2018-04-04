using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser.Parser
{
    public sealed class NoteStream
    {
        static NoteStream()
        {
        }

        private NoteStream()
        {
        }

        public static NoteStream Instance
        {
            get { return instance; }
        }

        private static readonly NoteStream instance = Initialize();

        private FileStream _data;
        private List<Tuple<Int64, int>> _index;
        private byte[] _buf;

        private static NoteStream Initialize()
        {
            string dbName = "Notes.db";
            string dbFile = AppDomain.CurrentDomain.BaseDirectory + dbName;
            
            NoteStream ns = new NoteStream();
            ns._data = new FileStream(dbFile, FileMode.Create, FileAccess.ReadWrite);
            ns._index = new List<Tuple<Int64, int>>();
            ns._buf = new byte[1024];
            return ns;
        }

        public int StoreNote(string text)
        {
            lock (_buf)
            {
                Int64 pos = _data.Position;
                int bLen = Encoding.UTF8.GetByteCount(text);
                if (bLen < 512)
                {
                    Encoding.UTF8.GetBytes(text, 0, text.Length, _buf, 0);
                    _data.Write(_buf, 0, bLen);
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(text);
                    _data.Write(bytes, 0, bLen);
                }
                _index.Add(new Tuple<long, int>(pos, bLen));
                return _index.Count - 1;
            }
        }

        public string GetNote(int key)
        {
            var item = _index[key];
            _data.Seek(item.Item1, SeekOrigin.Begin);
            if (item.Item2 < 1024)
            {
                _data.Read(_buf, 0, item.Item2);
                return Encoding.UTF8.GetString(_buf, 0, item.Item2);
            }
            else
            {
                var buf = new byte[item.Item2];
                _data.Read(buf, 0, item.Item2);
                return Encoding.UTF8.GetString(buf);
            }
        }
    }
}
