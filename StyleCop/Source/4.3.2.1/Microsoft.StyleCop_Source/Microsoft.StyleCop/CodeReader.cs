namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public class CodeReader
    {
        private int bufferLength;
        private const int CharacterBlockSize = 80;
        private char[] charBuffer;
        private TextReader code;
        private int position;

        public CodeReader(TextReader code)
        {
            Param.RequireNotNull(code, "code");
            this.code = code;
        }

        private bool LoadBuffer(int count)
        {
            if (this.bufferLength > ((this.position + count) - 1))
            {
                return true;
            }
            int index = this.bufferLength - this.position;
            char[] buffer = new char[80 + index];
            for (int i = 0; i < index; i++)
            {
                buffer[i] = this.charBuffer[this.position + i];
            }
            int num3 = this.code.ReadBlock(buffer, index, 80);
            this.bufferLength = index + num3;
            this.position = 0;
            this.charBuffer = buffer;
            return (this.bufferLength >= count);
        }

        public char Peek()
        {
            if (!this.LoadBuffer(1))
            {
                return '\0';
            }
            return this.charBuffer[this.position];
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification="There is no danger of overflow.")]
        public char Peek(int index)
        {
            if (!this.LoadBuffer(index + 1))
            {
                return '\0';
            }
            return this.charBuffer[this.position + index];
        }

        public char ReadNext()
        {
            if (!this.LoadBuffer(1))
            {
                return '\0';
            }
            return this.charBuffer[this.position++];
        }

        public char[] ReadNext(int count)
        {
            Param.RequireGreaterThanOrEqualToZero(count, "count");
            if (!this.LoadBuffer(count))
            {
                return null;
            }
            char[] chArray = new char[count];
            for (int i = 0; i < count; i++)
            {
                chArray[i] = this.charBuffer[this.position++];
            }
            return chArray;
        }

        public string ReadString(int count)
        {
            Param.RequireGreaterThanOrEqualToZero(count, "count");
            char[] chArray = this.ReadNext(count);
            if (chArray == null)
            {
                return null;
            }
            return new string(chArray);
        }
    }
}

