namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class CodeLocation
    {
        private CodePoint endPoint;
        private CodePoint startPoint;

        public CodeLocation()
        {
            this.startPoint = new CodePoint();
            this.endPoint = new CodePoint();
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="OnLine", Justification="On Line is two words in this context.")]
        public CodeLocation(int index, int endIndex, int indexOnLine, int endIndexOnLine, int lineNumber, int endLineNumber)
        {
            Param.RequireGreaterThanOrEqualToZero(index, "index");
            Param.RequireGreaterThanOrEqualTo(endIndex, index, "endIndex");
            Param.RequireGreaterThanOrEqualToZero(indexOnLine, "indexOnLine");
            Param.RequireGreaterThanOrEqualToZero(endIndexOnLine, "endIndexOnLine");
            Param.RequireGreaterThanZero(lineNumber, "lineNumber");
            Param.RequireGreaterThanOrEqualTo(endLineNumber, lineNumber, "endLineNumber");
            this.startPoint = new CodePoint(index, indexOnLine, lineNumber);
            this.endPoint = new CodePoint(endIndex, endIndexOnLine, endLineNumber);
        }

        public static CodeLocation Join(CodeLocation location1, CodeLocation location2)
        {
            int indexOnLine;
            int num2;
            if (location1 == null)
            {
                return location2;
            }
            if (location2 == null)
            {
                return location1;
            }
            if (location1.StartPoint.LineNumber == location2.StartPoint.LineNumber)
            {
                indexOnLine = Math.Min(location1.StartPoint.IndexOnLine, location2.StartPoint.IndexOnLine);
                num2 = Math.Max(location1.EndPoint.IndexOnLine, location2.EndPoint.IndexOnLine);
            }
            else if (location1.StartPoint.LineNumber < location2.StartPoint.LineNumber)
            {
                indexOnLine = location1.StartPoint.IndexOnLine;
                num2 = location2.EndPoint.IndexOnLine;
            }
            else
            {
                indexOnLine = location2.StartPoint.IndexOnLine;
                num2 = location1.EndPoint.IndexOnLine;
            }
            return new CodeLocation(Math.Min(location1.StartPoint.Index, location2.StartPoint.Index), Math.Max(location1.EndPoint.Index, location2.EndPoint.Index), indexOnLine, num2, Math.Min(location1.StartPoint.LineNumber, location2.StartPoint.LineNumber), Math.Max(location2.EndPoint.LineNumber, location2.EndPoint.LineNumber));
        }

        public static CodeLocation Join<T>(Node<T> token1, Node<T> token2) where T: Token
        {
            return Join((token1 == null) ? null : token1.Value.Location, (token2 == null) ? null : token2.Value.Location);
        }

        public static CodeLocation Join<T>(CodeLocation location, Node<T> token) where T: Token
        {
            return Join(location, (token == null) ? null : token.Value.Location);
        }

        public static CodeLocation Join<T>(Node<T> token, CodeLocation location) where T: Token
        {
            return Join((token == null) ? null : token.Value.Location, location);
        }

        public static CodeLocation Join(CodeLocation location, Token token)
        {
            return Join(location, (token == null) ? null : token.Location);
        }

        public static CodeLocation Join(Token token, CodeLocation location)
        {
            return Join((token == null) ? null : token.Location, location);
        }

        public static CodeLocation Join(Token token1, Token token2)
        {
            return Join((token1 == null) ? null : token1.Location, (token2 == null) ? null : token2.Location);
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="EndPoint", Justification="EndPoint is two words in this context, to be consistent with StartPoint")]
        public CodePoint EndPoint
        {
            get
            {
                return this.endPoint;
            }
        }

        public int LineSpan
        {
            get
            {
                if ((this.startPoint != null) && (this.endPoint != null))
                {
                    return ((this.endPoint.LineNumber - this.startPoint.LineNumber) + 1);
                }
                return 0;
            }
        }

        public CodePoint StartPoint
        {
            get
            {
                return this.startPoint;
            }
        }
    }
}

