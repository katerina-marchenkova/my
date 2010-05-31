namespace Microsoft.StyleCop.CSharp
{
    using System;
    using System.Text;
    using System.Xml;

    internal static class CommentVerifier
    {
        internal const int MinimumCharacterPercentage = 40;
        internal const int MinimumHeaderCommentLength = 10;

        public static string ExtractTextFromCommentXml(XmlNode commentXml)
        {
            StringBuilder builder = new StringBuilder();
            foreach (XmlNode node in commentXml.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Text)
                {
                    builder.Append(node.Value);
                }
                else if (node.Name == "paramref")
                {
                    XmlAttribute attribute = node.Attributes["name"];
                    if (attribute != null)
                    {
                        builder.Append(attribute.Value);
                    }
                }
                else if (((node.Name == "see") || (node.Name == "seealso")) && (node.ChildNodes.Count == 0))
                {
                    XmlAttribute attribute2 = node.Attributes["cref"];
                    if (attribute2 != null)
                    {
                        builder.Append(attribute2.Value);
                    }
                }
                if (node.HasChildNodes && ((node.ChildNodes.Count > 0) || (node.ChildNodes[0].NodeType != XmlNodeType.Text)))
                {
                    builder.Append(ExtractTextFromCommentXml(node));
                }
            }
            return builder.ToString().Trim();
        }

        public static InvalidCommentType IsGarbageComment(string comment)
        {
            InvalidCommentType valid = InvalidCommentType.Valid;
            string str = comment.Trim();
            if (string.IsNullOrEmpty(str))
            {
                valid |= InvalidCommentType.Empty;
            }
            if (str.Length < 10)
            {
                valid |= InvalidCommentType.TooShort;
            }
            if (!char.IsUpper(str[0]) && !char.IsDigit(str[0]))
            {
                valid |= InvalidCommentType.NoCapitalLetter;
            }
            if (str[str.Length - 1] != '.')
            {
                valid |= InvalidCommentType.NoPeriod;
            }
            float num = 0f;
            float num2 = 0f;
            bool flag = false;
            foreach (char ch in str)
            {
                if (char.IsLetter(ch))
                {
                    num++;
                }
                else if (char.IsWhiteSpace(ch))
                {
                    flag = true;
                }
                else
                {
                    num2++;
                }
            }
            if ((num == 0f) || (((num / (num + num2)) * 100f) < 40f))
            {
                valid |= InvalidCommentType.TooFewCharacters;
            }
            if (!flag)
            {
                valid |= InvalidCommentType.NoWhitespace;
            }
            return valid;
        }

        public static InvalidCommentType IsGarbageComment(XmlNode commentXml)
        {
            string innerText = commentXml.InnerText;
            if (commentXml.HasChildNodes && ((commentXml.ChildNodes.Count > 1) || (commentXml.ChildNodes[0].NodeType != XmlNodeType.Text)))
            {
                innerText = ExtractTextFromCommentXml(commentXml);
            }
            return IsGarbageComment(innerText);
        }
    }
}

