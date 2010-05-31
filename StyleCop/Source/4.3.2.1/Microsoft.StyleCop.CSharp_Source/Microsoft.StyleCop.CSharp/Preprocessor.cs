namespace Microsoft.StyleCop.CSharp
{
    using Microsoft.StyleCop;
    using System;

    public class Preprocessor : CsToken
    {
        private string preprocessorType;

        internal Preprocessor(string text, CodeLocation location, bool generated) : this(text, CsTokenClass.PreprocessorDirective, location, generated)
        {
        }

        internal Preprocessor(string text, CsTokenClass tokenClass, CodeLocation location, bool generated) : base(text, CsTokenType.PreprocessorDirective, tokenClass, location, generated)
        {
            this.preprocessorType = string.Empty;
            int num = 0;
            while (true)
            {
                if (text[num] == '#')
                {
                    break;
                }
                num++;
            }
            string str = string.Empty;
            int num2 = num;
            while ((num2 + 1) < text.Length)
            {
                if (!char.IsLetter(text[num2 + 1]))
                {
                    break;
                }
                num2++;
            }
            if (num2 > num)
            {
                str = text.Substring(num + 1, num2 - num);
            }
            this.preprocessorType = str;
        }

        public string PreprocessorType
        {
            get
            {
                return this.preprocessorType;
            }
        }
    }
}

