using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSRSave
{
    public class Character
    {
        private readonly byte[] _data;
        public int SlotNumber { get; }
        public NPCEditor Npc { get; }
        public GeneralEditor General {  get; }

        public Character(byte[] data, int slotNumber)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            SlotNumber = slotNumber;
            Npc = new NPCEditor(this);
            General = new GeneralEditor(this);
        }

        private readonly byte[] _pattern1 =
        {
            0xFF, 0xFF, 0xFF, 0xFF,
            0x00, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF,
            0x00, 0x00, 0x00, 0x00
        };

        public byte this[int offset]
        {
            get
            {
                if (offset < 0 || offset >= _data.Length)
                    throw new ArgumentOutOfRangeException(nameof(offset));
                return _data[offset];
            }
            set
            {
                if (offset < 0 || offset >= _data.Length)
                    throw new ArgumentOutOfRangeException(nameof(offset));
                _data[offset] = value;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (_data.Length <= 0x90)
                {
                    return true;
                }

                for (int i = 0x20; i <= 0x90; i++)
                {
                    if (_data[i] != 0x00)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public byte[] GetRawData() => _data;

        public IEnumerable<int> FindPattern1()
        {
            return FindPatternOffsets(_pattern1, 0x1F000, 0x1FFFF).ToList();
        }

        private IEnumerable<int> FindPatternOffsets(byte[] pattern, int startOffset, int endOffset)
        {
            if (pattern == null || pattern.Length == 0)
                yield break;

            if (startOffset < 0 || endOffset >= _data.Length || startOffset > endOffset)
                throw new ArgumentOutOfRangeException("Invalid search range.");

            int maxStart = endOffset - pattern.Length + 1;

            for (int i = startOffset; i <= maxStart; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (_data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    yield return i;
            }
        }
    }
}
