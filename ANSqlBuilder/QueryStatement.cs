using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using ANCommon.Sql;

namespace ANSqlBuilder
{
    public abstract class QueryStatement : SqlStatement
    {
        public virtual List<string> GetColumnsAlias()
        {
            return null;
        }

        public virtual string GetTableAlias()
        {
            return "row";
        }

        public IDataReader ExecuteReader()
        {
            using (var helper = new DbHelper(ConnectionStringName))
            {
                var sql = new StringBuilder();
                GetSql(DbTarget, ref sql);
                return helper.ExecuteReader(sql.ToString());
            }
        }

        public IDataReader ExecuteReader(int start_record)
        {

            using (var helper = new DbHelper(ConnectionStringName))
            {
                var sql = new StringBuilder();
                GetSql(DbTarget, ref sql);
                IDataReader reader = helper.ExecuteReader(sql.ToString());
                var i = 0;
                while (i < start_record && reader.Read())
                {
                    i++;
                }
                return reader;
            }
        }

        public DataSet ExecuteDataSet()
        {
            using (var helper = new DbHelper(ConnectionStringName))
            {
                var sql = new StringBuilder();
                GetSql(DbTarget, ref sql);

                return helper.ExecuteDataSet(sql.ToString());
            }
        }

        public DataSet ExecuteDataSet(string table_name, int start_record, int max_records)
        {
            using (var helper = new DbHelper(ConnectionStringName))
            {
                var sql = new StringBuilder();
                GetSql(DbTarget, ref sql);
                return helper.ExecuteDataSet(sql.ToString(), table_name, start_record, max_records);
            }
        }

        public string GetXmlRaw()
        {
            var ms = new MemoryStream();
            WriteXmlRaw(ms);
            ms.Position = 0;

            return (new StreamReader(ms)).ReadToEnd();
        }

        public void WriteXmlRaw(Stream w)
        {
            WriteXmlRaw(w, UTF8Encoding.UTF8);
        }

        public void WriteXmlRaw(Stream w, Encoding e)
        {
            WriteXmlRaw(w, e, "row");
        }

        public void WriteXmlRaw(Stream w, string elementName)
        {
            WriteXmlRaw(w, UTF8Encoding.UTF8, elementName);
        }

        public void WriteXmlRaw(Stream w, Encoding e, string elementName)
        {
            string text;
            var alias = GetColumnsAlias();
            var count = alias.Count;
            var writer = new XmlTextWriter(w, e);
            var reader = ExecuteReader();

            while (reader.Read())
            {
                writer.WriteStartElement(elementName);
                for (var x = 0; x < count; x++)
                {
                    
                    if (reader.IsDBNull(x))
                        text = String.Empty;
                    else
                        text = reader.GetDataTypeName(x) == "datetime" ? reader.GetDateTime(x).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ") : reader.GetValue(x).ToString();
                         
                    writer.WriteAttributeString(alias[x], text);


                }
                writer.WriteEndElement();
            }
            writer.Flush();
            reader.Close();
        }

        public string GetXmlAuto()
        {
            MemoryStream ms = new MemoryStream();
            WriteXmlAuto(ms);
            ms.Position = 0;

            return (new StreamReader(ms)).ReadToEnd();
        }

        public void WriteXmlAuto(Stream w)
        {
            WriteXmlAuto(w, UTF8Encoding.UTF8);
        }

        public void WriteXmlAuto(Stream w, Encoding encoding)
        {
            // to keep control of opened tags
            Hashtable htTags = new Hashtable();
            Hashtable htTracker = new Hashtable();

            // list of column to create xml structure
            List<string> scColumns = GetColumnsAlias();
            int scCount = scColumns.Count;

            List<XmlExplicitColumnInfo> xeColumns = new List<XmlExplicitColumnInfo>();
            int tagsCount = 0;
            string tag = "";
            for (int x = 0; x < scCount; x++)
            {
                XmlExplicitColumnInfo xeci = new XmlExplicitColumnInfo(scColumns[x]);
                if (xeci.TagNumber != tag)
                {
                    tag = xeci.TagNumber;
                    tagsCount++;
                    htTags[tag] = x;
                }
                xeColumns.Add(xeci);
            }

            XmlTextWriter writer = new XmlTextWriter(w, encoding);
            IDataReader reader = ExecuteReader();

            int xeCount = xeColumns.Count;
            int openLevels = 0;
            string text;
            while (reader.Read())
            {
                for (int t = 1; t <= htTags.Count; t++)
                {
                    int x = (int)htTags[t.ToString()];
                    XmlExplicitColumnInfo col = xeColumns[x];

                    if (t  == tagsCount)
                    {
                        writer.WriteStartElement(col.ElementName);
                        while (x < xeCount)
                        {
                            col = xeColumns[x];
                            if (reader.IsDBNull(x))
                                text = String.Empty;
                            else
                            {
                                if (reader.GetDataTypeName(x) == "datetime")
                                    text = reader.GetDateTime(x).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ");
                                else
                                    text = reader.GetValue(x).ToString();
                            }
                            if (col.Directive == "element")
                            {
                                writer.WriteStartElement(col.AttributeName);
                                writer.WriteString(text);
                                writer.WriteEndElement();
                            }
                            if (col.Directive == "cdata")
                                writer.WriteCData(text);
                            if (col.Directive == "xml")
                                writer.WriteRaw(text);
                            else if (col.Directive == "")
                                writer.WriteAttributeString(col.AttributeName, text);
                            x++;
                            
                        }
                        writer.WriteEndElement();
                    }
                    else
                    {
                        if (reader.IsDBNull(x))
                            text = String.Empty;
                        else
                        {
                            if (reader.GetDataTypeName(x) == "datetime")
                                text = reader.GetDateTime(x).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ");
                            else
                                text = reader.GetValue(x).ToString();
                        }

                        if (htTracker[col.TagNumber] == null || htTracker[col.TagNumber].ToString() != text)
                        {
                            if (htTracker[col.TagNumber] != null)
                            {
                                writer.WriteEndElement();
                                openLevels--;
                            }
                            htTracker[col.TagNumber] = text;
                            openLevels++;
                            writer.WriteStartElement(col.ElementName);

                            while (x < xeCount && col.TagNumber == t.ToString())
                            {
                                if (reader.IsDBNull(x))
                                    text = String.Empty;
                                else
                                {
                                    if (reader.GetDataTypeName(x) == "datetime")
                                        text = reader.GetDateTime(x).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ");
                                    else
                                        text = reader.GetValue(x).ToString();
                                }

                                if (col.Directive == "element")
                                {
                                    writer.WriteStartElement(col.AttributeName);
                                    writer.WriteString(text);
                                    writer.WriteEndElement();
                                }
                                if (col.Directive == "cdata")
                                    writer.WriteCData(text);
                                if (col.Directive == "xml")
                                    writer.WriteRaw(text);
                                else if (col.Directive == "")
                                    writer.WriteAttributeString(col.AttributeName, text);
                                x++;
                                if(x < xeCount)
                                    col = xeColumns[x];
                                
                            }
                        }

                    }

                }
            }
            reader.Close();
            for (int x = 0; x < openLevels; x++)
                writer.WriteEndElement();
            
            writer.Flush();
        }

        public string GetXmlExplicit()
        {
            MemoryStream ms = new MemoryStream();
            WriteXmlExplicit(ms);
            ms.Position = 0;

            return (new StreamReader(ms)).ReadToEnd();
        }        

        public void WriteXmlExplicit(Stream w)
        {
            WriteXmlExplicit(w, UTF8Encoding.UTF8);
        }


        public void WriteXmlExplicit(Stream w, Encoding encoding)
        {
            // to keep control of opened tags
            Stack<string> levels = new Stack<string>();

            // list of column to create xml structure
            List<string> scColumns = GetColumnsAlias();
            int scCount = scColumns.Count;

            List<XmlExplicitColumnInfo> xeColumns = new List<XmlExplicitColumnInfo>();
            for (int x = 2; x < scCount; x++)
                xeColumns.Add(new XmlExplicitColumnInfo(scColumns[x]));

            XmlTextWriter writer = new XmlTextWriter(w, encoding);
            IDataReader reader = ExecuteReader();

            int xeCount = xeColumns.Count;
            string text;
            while (reader.Read())
            {
                string tag = reader.GetValue(0).ToString();
                string parent = "";
                if (!reader.IsDBNull(1))
                    parent = reader.GetValue(1).ToString();

                bool first = true;
                

                for (int x = 0; x < xeCount; x++)
                {
                    XmlExplicitColumnInfo col = xeColumns[x];
                    if (col.TagNumber == tag)
                    {
                        if (first)
                        {
                            int count = levels.Count;
                            if (levels.Count == 0)
                                levels.Push(String.Format("{0}-{1}", tag, parent));
                            else
                            {
                                while (levels.Count > 0)
                                {
                                    string[] last = levels.Peek().Split('-');
                                    // if is brother
                                    if ((tag == last[0] && parent == last[1]))
                                    {
                                        writer.WriteEndElement();
                                        break;
                                    }
                                    // if is son
                                    else if (parent == last[0])
                                    {
                                        levels.Push(String.Format("{0}-{1}", tag, parent));
                                        break;
                                    }
                                    else
                                    {
                                        writer.WriteEndElement();
                                        levels.Pop();
                                    }
                                }
                            }

                            writer.WriteStartElement(col.ElementName);

                            first = false;
                        }

                        if (reader.IsDBNull(x +  2))
                            text = String.Empty;
                        else
                        {
                            if (reader.GetDataTypeName(x + 2) == "datetime")
                                text = reader.GetDateTime(x + 2).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ");
                            else
                                text = reader.GetValue(x + 2).ToString();
                        }

                        if (col.Directive == "element")
                        {
                            writer.WriteStartElement(col.AttributeName);
                            writer.WriteString(text);
                            writer.WriteEndElement();
                        }
                        else if (col.Directive == "cdata")
                            writer.WriteCData(text);
                        else if (col.Directive == "xml")
                            writer.WriteRaw(text);
                        else if(col.Directive == "hide")
                        {

                        }
                        else
                            writer.WriteAttributeString(col.AttributeName, text);

                    }
                }

            }
            reader.Close();
            for (var x = 0; x < levels.Count; x++)
                writer.WriteEndElement();
            writer.Flush();
            

        }

        public class XmlExplicitColumnInfo
        {
            public string ElementName;
            public string TagNumber;
            public string AttributeName;
            public string Directive;


            public XmlExplicitColumnInfo(string column_alias)
            {
                var datos = column_alias.Split('!');

                ElementName = datos[0];
                TagNumber = datos[1];
                AttributeName = datos[2];
                Directive = datos.Length > 3 ? datos[3] : "";

            }

        }

    }
}
