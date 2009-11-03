namespace Microsoft.StyleCop
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public sealed class CodePoint
    {
        private readonly int index;
        private readonly int indexOnLine;
        private readonly int lineNumber;

        public CodePoint()
        {
            this.lineNumber = 1;
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="OnLine", Justification="On Line is two words in this context.")]
        public CodePoint(int index, int indexOnLine, int lineNumber)
        {
            this.lineNumber = 1;
            Param.RequireGreaterThanOrEqualToZero(index, "index");
            Param.RequireGreaterThanOrEqualToZero(indexOnLine, "indexOnLine");
            Param.RequireGreaterThanZero(lineNumber, "lineNumber");
            this.index = index;
            this.indexOnLine = indexOnLine;
            this.lineNumber = lineNumber;
        }

        public static CodePoint Join(CodePoint point1, CodePoint point2)
        {
            int indexOnLine;
            Param.RequireNotNull(point1, "point1");
            Param.RequireNotNull(point2, "point2");
            if (point1 == null)
            {
                return point2;
            }
            if (point2 == null)
            {
                return point1;
            }
            if (point1.LineNumber == point2.LineNumber)
            {
                indexOnLine = Math.Min(point1.IndexOnLine, point2.IndexOnLine);
            }
            else if (point1.LineNumber < point2.LineNumber)
            {
                indexOnLine = point1.IndexOnLine;
            }
            else
            {
                indexOnLine = point2.IndexOnLine;
            }
            return new CodePoint(Math.Min(point1.Index, point2.Index), indexOnLine, Math.Min(point1.LineNumber, point2.LineNumber));
        }

        public int Index
        {
            get
            {
                return this.index;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId="OnLine", Justification="On Line is two words in this context.")]
        public int IndexOnLine
        {
            get
            {
                return this.indexOnLine;
            }
        }

        public int LineNumber
        {
            get
            {
                return this.lineNumber;
            }
        }
    }
}

