// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

using System;
using System.Collections.Generic;

namespace OscJack
{
    //
    // Data handle class that provides offset values to each data element
    // within a shared data buffer
    //
    public sealed class OscDataHandle
    {
        public static readonly byte[] ZeroByte = new byte[0];

        #region Public methods

        public int GetElementCount()
        {
            return _typeTags.Count;
        }

        public char[] GetTypeTags()
        {
            return _typeTags.ToArray();
        }

        public bool HasElementInt(int index)
        {
            if (_typeTags.Count <= index) return false;
            return _typeTags[index] == 'i';
        }

        public bool HasElementFloat(int index)
        {
            if (_typeTags.Count <= index) return false;
            return _typeTags[index] == 'f';
        }

        public bool HasElementString(int index)
        {
            if (_typeTags.Count <= index) return false;
            return _typeTags[index] == 's';
        }

        public bool HasElementBlob(int index)
        {
            if (_typeTags.Count <= index) return false;
            return _typeTags[index] == 'b';
        }

        public int GetElementAsInt(int index)
        {
            if (index >= _typeTags.Count) return 0;
            var tag = _typeTags[index];
            var offs = _offsets[index];
            if (tag == 'i') return OscDataTypes.ReadInt(_sharedBuffer, offs);
            if (tag == 'f') return (int)OscDataTypes.ReadFloat(_sharedBuffer, offs);
            return 0;
        }

        public float GetElementAsFloat(int index)
        {
            if (index >= _typeTags.Count) return 0;
            var tag = _typeTags[index];
            var offs = _offsets[index];
            if (tag == 'f') return OscDataTypes.ReadFloat(_sharedBuffer, offs);
            if (tag == 'i') return OscDataTypes.ReadInt(_sharedBuffer, offs);
            return 0;
        }

        public string GetElementAsString(int index)
        {
            if (index >= _typeTags.Count) return "";
            var tag = _typeTags[index];
            var offs = _offsets[index];
            if (tag == 's') return OscDataTypes.ReadString(_sharedBuffer, offs);
            if (tag == 'i') return OscDataTypes.ReadInt(_sharedBuffer, offs).ToString();
            if (tag == 'f') return OscDataTypes.ReadFloat(_sharedBuffer, offs).ToString();
            return "";
        }

        public byte[] GetElementAsBlob(int index)
        {
            if (index >= _typeTags.Count) return ZeroByte;
            var tag = _typeTags[index];
            var offs = _offsets[index];
            if (tag == 'b') return OscDataTypes.ReadBlob(_sharedBuffer, offs);
            return ZeroByte;
        }

        public object[] GetElementAsObjects()
        {
            int count = GetElementCount();
            object[] objects = new object[count];

            for (int i = 0; i < count; i++)
            {
                var tag = _typeTags[i];
                var offs = _offsets[i];
                if (tag == 'i')
                {
                    objects[i] = OscDataTypes.ReadInt(_sharedBuffer, offs);
                }
                else if (tag == 'f')
                {
                    objects[i] = OscDataTypes.ReadFloat(_sharedBuffer, offs);
                }
                else if (tag == 's')
                {
                    objects[i] = OscDataTypes.ReadString(_sharedBuffer, offs);
                }
                else if (tag == 'b')
                {
                    objects[i] = OscDataTypes.ReadBlob(_sharedBuffer, offs);
                }
            }

            return objects;
        }

        public bool TryGetElementAsInt(int index, out int value)
        {
            bool hasElement = HasElementInt(index);
            value = GetElementAsInt(index);
            return hasElement;
        }

        public bool TryGetElementAsFloat(int index, out float value)
        {
            bool hasElement = HasElementFloat(index);
            value = GetElementAsFloat(index);
            return hasElement;
        }

        public bool TryGetElementAsString(int index, out string value)
        {
            bool hasElement = HasElementString(index);
            value = GetElementAsString(index);
            return hasElement;
        }

        public bool TryGetElementAsBlob(int index, out byte[] value)
        {
            bool hasElement = HasElementBlob(index);
            value = GetElementAsBlob(index);
            return hasElement;
        }

        #endregion

        #region Internal method

        internal void Scan(Byte[] buffer, int offset)
        {
            // Reset the internal state.
            _sharedBuffer = buffer;
            _typeTags.Clear();
            _offsets.Clear();

            // Read type tags.
            offset++; // ","

            while (true)
            {
                var tag = (char)buffer[offset];
                if (!OscDataTypes.IsSupportedTag(tag)) break;
                _typeTags.Add(tag);
                offset++;
            }

            offset += OscDataTypes.GetStringSize(buffer, offset);

            // Determine the offsets of the each element.
            foreach (var tag in _typeTags)
            {
                _offsets.Add(offset);

                if (tag == 'i' || tag == 'f')
                {
                    offset += 4;
                }
                else if (tag == 's')
                {
                    offset += OscDataTypes.GetStringSize(buffer, offset);
                }
                else if (tag == 'b')
                {
                    offset += OscDataTypes.ReadInt(buffer, offset) + 4;
                }
                else
                {
                    new Exception("Unsupported tag: " + tag);
                }
            }
        }

        #endregion

        #region Private members

        Byte[] _sharedBuffer;

        List<char> _typeTags = new List<char>(8);
        List<int> _offsets = new List<int>(8);

        #endregion
    }
}
