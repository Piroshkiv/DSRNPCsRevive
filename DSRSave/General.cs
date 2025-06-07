using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DSRSave
{
    public class GeneralEditor
    {
        private readonly Character _character;
        private const int CharacterName1Offset = 0x108;
        private const int CharacterName2Offset = 0x18C;
        private const int MaxLengthBytes = 64;

        public GeneralEditor(Character character)
        {
            _character = character;
        }

        public string Name1
        {
            get => ReadUtf16String(CharacterName1Offset);
            set => WriteUtf16String(CharacterName1Offset, value);
        }

        public string Name2
        {
            get => ReadUtf16String(CharacterName2Offset);
            set => WriteUtf16String(CharacterName2Offset, value);
        }

        private string ReadUtf16String(int offset)
        {
            var data = _character.GetRawData();

            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            int maxLength = Math.Min(MaxLengthBytes, data.Length - offset);

            int length = 0;
            for (int i = 0; i < maxLength - 1; i += 2)
            {
                if (data[offset + i] == 0x00 && data[offset + i + 1] == 0x00)
                {
                    length = i;
                    break;
                }
            }

            if (length == 0)
                length = maxLength - (maxLength % 2);

            return System.Text.Encoding.Unicode.GetString(data, offset, length);
        }

        private void WriteUtf16String(int offset, string value)
        {
            var data = _character.GetRawData();

            if (offset < 0 || offset >= data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var encoded = System.Text.Encoding.Unicode.GetBytes(value ?? string.Empty);

            int maxLength = Math.Min(MaxLengthBytes, data.Length - offset);

            int writeLength = Math.Min(encoded.Length, maxLength - 2);

            Array.Copy(encoded, 0, data, offset, writeLength);

            data[offset + writeLength] = 0x00;
            data[offset + writeLength + 1] = 0x00;

            for (int i = offset + writeLength + 2; i < offset + maxLength; i++)
            {
                data[i] = 0x00;
            }
        }
    }
}
