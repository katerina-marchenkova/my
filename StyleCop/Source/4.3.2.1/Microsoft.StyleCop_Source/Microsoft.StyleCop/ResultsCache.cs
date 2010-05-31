namespace Microsoft.StyleCop
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;

    internal class ResultsCache
    {
        private StyleCopCore core;
        private Dictionary<string, XmlDocument> documentHash = new Dictionary<string, XmlDocument>();
        internal const string Version = "10";

        public ResultsCache(StyleCopCore core)
        {
            this.core = core;
        }

        public void Flush()
        {
            lock (this)
            {
                if (this.core.WriteResultsCache && this.core.Environment.SupportsResultsCache)
                {
                    foreach (KeyValuePair<string, XmlDocument> pair in this.documentHash)
                    {
                        this.core.Environment.SaveResultsCache(pair.Key, pair.Value);
                    }
                }
            }
        }

        private static bool IsNodeUpToDate(XmlNode timeStampNode, DateTime timeStamp)
        {
            if (((timeStamp.Year == 0) && (timeStamp.Month == 0)) && ((timeStamp.Second == 0) && (timeStamp.Millisecond == 0)))
            {
                return true;
            }
            XmlNode node = timeStampNode["timestamp"];
            if ((node != null) && (node.InnerText == timeStamp.ToString(CultureInfo.InvariantCulture)))
            {
                node = timeStampNode["milliseconds"];
                if ((node != null) && (node.InnerText == timeStamp.Millisecond.ToString(CultureInfo.InvariantCulture)))
                {
                    return true;
                }
            }
            return false;
        }

        public string LoadProject(CodeProject project)
        {
            string innerText = null;
            lock (this)
            {
                XmlNode projectNode = null;
                XmlDocument document = this.OpenCacheProject(project, out projectNode);
                if ((document == null) || (projectNode == null))
                {
                    goto Label_0060;
                }
                try
                {
                    XmlElement element = projectNode["configuration"];
                    if (element == null)
                    {
                        goto Label_003B;
                    }
                    innerText = element.InnerText;
                }
                catch (XmlException)
                {
                }
                catch (NullReferenceException)
                {
                }
                return innerText;
            Label_003B:
                if (!this.documentHash.ContainsKey(project.Location))
                {
                    this.documentHash.Add(project.Location, document);
                }
            Label_0060:
                innerText = null;
            }
            return innerText;
        }

        public bool LoadResults(SourceCode sourceCode, SourceParser parser, DateTime writeTime, DateTime settingsTimeStamp)
        {
            bool flag = false;
            lock (this)
            {
                XmlNode item = null;
                XmlDocument document = this.OpenResultsCache(sourceCode, parser, out item);
                if ((document == null) || (item == null))
                {
                    return flag;
                }
                try
                {
                    XmlElement timeStampNode = item["settings"];
                    if (((timeStampNode != null) && IsNodeUpToDate(timeStampNode, settingsTimeStamp)) && IsNodeUpToDate(item, writeTime))
                    {
                        XmlNode parentNode = item.SelectSingleNode("violations");
                        if ((parentNode != null) && parser.ImportViolations(sourceCode, parentNode))
                        {
                            flag = true;
                        }
                    }
                }
                catch (XmlException)
                {
                }
                if (!this.documentHash.ContainsKey(sourceCode.Project.Location))
                {
                    this.documentHash.Add(sourceCode.Project.Location, document);
                }
            }
            return flag;
        }

        private XmlDocument OpenCacheProject(CodeProject project, out XmlNode projectNode)
        {
            projectNode = null;
            XmlDocument document = null;
            try
            {
                lock (this)
                {
                    if (this.documentHash.TryGetValue(project.Location, out document))
                    {
                        projectNode = document.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "project[@key=\"{0}\"]", new object[] { project.Key.ToString(CultureInfo.InvariantCulture) }));
                        return document;
                    }
                    document = this.core.Environment.LoadResultsCache(project.Location);
                    if (document == null)
                    {
                        return document;
                    }
                    XmlElement element = document["stylecopresultscache"]["version"];
                    if (element.InnerText == "10")
                    {
                        projectNode = document.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "project[@key=\"{0}\"]", new object[] { project.Key.ToString(CultureInfo.InvariantCulture) }));
                        return document;
                    }
                    return null;
                }
            }
            catch (XmlException)
            {
                document = null;
            }
            catch (NullReferenceException)
            {
                document = null;
            }
            return document;
        }

        private XmlDocument OpenResultsCache(SourceCode sourceCode, SourceParser parser, out XmlNode item)
        {
            item = null;
            XmlDocument document = null;
            try
            {
                lock (this)
                {
                    if (this.documentHash.TryGetValue(sourceCode.Project.Location, out document))
                    {
                        item = document.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "sourcecode[@name=\"{0}\"][@parser=\"{1}\"]", new object[] { sourceCode.Name, parser.Id }));
                        return document;
                    }
                    document = this.core.Environment.LoadResultsCache(sourceCode.Project.Location);
                    if (document == null)
                    {
                        return document;
                    }
                    XmlElement element = document["stylecopresultscache"]["version"];
                    if (element.InnerText == "10")
                    {
                        item = document.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "sourcecode[@name=\"{0}\"][@parser=\"{1}\"]", new object[] { sourceCode.Name, parser.Id }));
                        return document;
                    }
                    return null;
                }
            }
            catch (XmlException)
            {
                document = null;
            }
            catch (NullReferenceException)
            {
                document = null;
            }
            return document;
        }

        public bool SaveDocumentResults(CodeDocument document, SourceParser parser, DateTime settingsTimeStamp)
        {
            bool flag = false;
            lock (this)
            {
                XmlDocument document2 = null;
                try
                {
                    if (!this.documentHash.ContainsKey(document.SourceCode.Project.Location))
                    {
                        XmlNode node;
                        document2 = this.OpenResultsCache(document.SourceCode, parser, out node);
                        if (document2 != null)
                        {
                            this.documentHash.Add(document.SourceCode.Project.Location, document2);
                        }
                    }
                    else
                    {
                        document2 = this.documentHash[document.SourceCode.Project.Location];
                    }
                    if (document2 != null)
                    {
                        XmlNode oldChild = document2.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "sourcecode[@name=\"{0}\"][@parser=\"{1}\"]", new object[] { document.SourceCode.Name, parser.Id }));
                        if (oldChild != null)
                        {
                            document2.DocumentElement.RemoveChild(oldChild);
                        }
                    }
                    else
                    {
                        document2 = new XmlDocument();
                        document2.AppendChild(document2.CreateElement("stylecopresultscache"));
                        XmlNode node3 = document2.CreateElement("version");
                        document2.DocumentElement.AppendChild(node3);
                        node3.InnerText = "10";
                        if (this.documentHash.ContainsKey(document.SourceCode.Project.Location))
                        {
                            this.documentHash.Remove(document.SourceCode.Project.Location);
                        }
                        this.documentHash.Add(document.SourceCode.Project.Location, document2);
                    }
                    XmlNode newChild = document2.CreateElement("sourcecode");
                    XmlAttribute attribute = document2.CreateAttribute("name");
                    attribute.Value = document.SourceCode.Name;
                    newChild.Attributes.Append(attribute);
                    document2.DocumentElement.AppendChild(newChild);
                    XmlNode node5 = document2.CreateElement("settings");
                    newChild.AppendChild(node5);
                    XmlNode node6 = document2.CreateElement("timestamp");
                    node5.AppendChild(node6);
                    node6.InnerText = settingsTimeStamp.ToString(CultureInfo.InvariantCulture);
                    node6 = document2.CreateElement("milliseconds");
                    node5.AppendChild(node6);
                    node6.InnerText = settingsTimeStamp.Millisecond.ToString(CultureInfo.InvariantCulture);
                    DateTime timeStamp = document.SourceCode.TimeStamp;
                    node6 = document2.CreateElement("timestamp");
                    newChild.AppendChild(node6);
                    node6.InnerText = timeStamp.ToString(CultureInfo.InvariantCulture);
                    node6 = document2.CreateElement("milliseconds");
                    newChild.AppendChild(node6);
                    node6.InnerText = timeStamp.Millisecond.ToString(CultureInfo.InvariantCulture);
                    if (document.SourceCode.Parser != null)
                    {
                        XmlAttribute attribute2 = document2.CreateAttribute("parser");
                        newChild.Attributes.Append(attribute2);
                        attribute2.Value = document.SourceCode.Parser.Id;
                    }
                    node6 = document2.CreateElement("violations");
                    newChild.AppendChild(node6);
                    SourceParser.ExportViolations(document, document2, node6);
                    return true;
                }
                catch (XmlException)
                {
                    return flag;
                }
            }
        }

        public bool SaveProject(CodeProject project)
        {
            bool flag = false;
            lock (this)
            {
                XmlDocument document = null;
                try
                {
                    if (!this.documentHash.TryGetValue(project.Location, out document))
                    {
                        XmlNode node;
                        document = this.OpenCacheProject(project, out node);
                        if (document != null)
                        {
                            this.documentHash.Add(project.Location, document);
                        }
                    }
                    if (document != null)
                    {
                        XmlNode oldChild = document.DocumentElement.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "project[@key=\"{0}\"]", new object[] { project.Key }));
                        if (oldChild != null)
                        {
                            document.DocumentElement.RemoveChild(oldChild);
                        }
                    }
                    else
                    {
                        document = new XmlDocument();
                        document.AppendChild(document.CreateElement("stylecopresultscache"));
                        XmlNode node3 = document.CreateElement("version");
                        document.DocumentElement.AppendChild(node3);
                        node3.InnerText = "10";
                        if (this.documentHash.ContainsKey(project.Location))
                        {
                            this.documentHash.Remove(project.Location);
                        }
                        this.documentHash.Add(project.Location, document);
                    }
                    XmlNode newChild = document.CreateElement("project");
                    XmlAttribute attribute = document.CreateAttribute("key");
                    attribute.Value = project.Key.ToString(CultureInfo.InvariantCulture);
                    newChild.Attributes.Append(attribute);
                    document.DocumentElement.AppendChild(newChild);
                    StringBuilder builder = new StringBuilder();
                    if (project.Configuration != null)
                    {
                        bool flag2 = true;
                        foreach (string str in project.Configuration.Flags)
                        {
                            if (flag2)
                            {
                                flag2 = false;
                                builder.Append(str);
                            }
                            else
                            {
                                builder.AppendFormat(";{0}", str);
                            }
                        }
                    }
                    XmlNode node5 = document.CreateElement("configuration");
                    newChild.AppendChild(node5);
                    node5.InnerText = builder.ToString();
                    flag = true;
                }
                catch (XmlException)
                {
                }
                return flag;
            }
        }
    }
}

