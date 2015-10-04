using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasyTermCore
{
    // --------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class CXmlNode : IDisposable
    {
        XmlNodeType m_nNodeType;
        string m_strName;

        // Reading
        XmlReader m_pReader;
        int m_nDepth;
        bool m_bEmptyElement;
        List<string> m_AttributeNames = new List<string>();
        List<string> m_AttributeValues = new List<string>();

        // Writing
        XmlWriter m_pWriter;


        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public CXmlNode()
        {
            m_nDepth = 0;
            m_nNodeType = XmlNodeType.None;
            m_bEmptyElement = false;
            m_pReader = null;
            m_pWriter = null;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementname"></param>
        /// <returns></returns>
        /// <created>UPh,08.04.2013</created>
        /// <changed>UPh,08.04.2013</changed>
        //********************************************************************************
        public CXmlNode(XmlWriter writer, string elementname)
        {
            m_nDepth = 0;
            m_nNodeType = XmlNodeType.EndElement;
            m_bEmptyElement = false;
            m_pReader = null;

            m_pWriter = writer;
            m_strName = elementname;

            m_pWriter.WriteStartElement(elementname);
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementname"></param>
        /// <returns></returns>
        /// <created>UPh,10.02.2015</created>
        /// <changed>UPh,10.02.2015</changed>
        // ********************************************************************************
        public CXmlNode(CXmlNode nodeParent, string elementname)
        {
            m_nDepth = 0;
            m_nNodeType = XmlNodeType.EndElement;
            m_bEmptyElement = false;
            m_pReader = null;

            m_pWriter = nodeParent.m_pWriter;
            m_strName = elementname;

            m_pWriter.WriteStartElement(elementname);
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,29.08.2015</created>
        /// <changed>UPh,29.08.2015</changed>
        // ********************************************************************************
        public void Dispose()
        {
            if (IsWriting && m_pWriter != null)
            {
                m_pWriter.WriteEndElement();
                m_pWriter = null;
            }
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,08.04.2013</created>
        /// <changed>UPh,08.04.2013</changed>
        //********************************************************************************
        //public ~CXmlNode()
        //{
        //    // TODO
        //    //if (IsWriting() && m_pWriter != null)
        //     //    m_pWriter.WriteEndElement();
        //}

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,08.04.2013</created>
        /// <changed>UPh,08.04.2013</changed>
        //********************************************************************************
        public bool IsReading
        {
            get
            {
                return m_pReader != null;
            }
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,08.04.2013</created>
        /// <changed>UPh,08.04.2013</changed>
        //********************************************************************************
        public bool IsWriting
        {
            get
            {
                return m_pWriter != null;
            }
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public bool ReadChildNode(ref CXmlNode node, string elementname = null)
        {
            if (!IsReading)
                return false;

            node.m_nNodeType = XmlNodeType.None;
            node.m_strName = "";
            node.m_AttributeNames.Clear();
            node.m_AttributeValues.Clear();

            if (IsElement() && m_bEmptyElement)
                return false;

            // Read until we get the matching element end
            XmlReader reader = m_pReader;

            XmlNodeType nodeType = XmlNodeType.None;
            while(reader.Read())
            {
                nodeType = reader.NodeType;

                int depth = reader.Depth;

                if (elementname != null && nodeType != XmlNodeType.Element && nodeType != XmlNodeType.EndElement)
                    continue;

                if (nodeType == XmlNodeType.Element)
                {
                    if (depth > m_nDepth + 1)
                        continue; // Skip all sub elements in this loop

                    string name = reader.LocalName;
                    m_strName = name;

                    if (elementname != null && elementname != name)
                        continue;

                    node.m_strName = name;
                    node.m_nDepth = depth;
                    node.m_bEmptyElement = reader.IsEmptyElement;
                    node.m_nNodeType = nodeType;
                    node.m_pReader = m_pReader;

                    bool hr = reader.MoveToFirstAttribute();
                    while (hr)
                    {
                        string attName = reader.LocalName;
                        string attValue = reader.Value;

                        node.m_AttributeNames.Add(attName);
                        node.m_AttributeValues.Add(attValue);

                        hr = reader.MoveToNextAttribute();
                    }

                    return true;
                }
                else if (nodeType == XmlNodeType.Text)
                {
                    if (depth > m_nDepth + 1)
                        continue; // Skip all sub elements in this loop

                    string value = reader.Value;
                    node.m_strName = value;

                    node.m_nDepth = depth;
                    node.m_bEmptyElement = true;
                    node.m_nNodeType = nodeType;
                    node.m_pReader = m_pReader;
                    return true;
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (depth <= m_nDepth + 1)
                    {
                        m_bEmptyElement = true; // prevents returning wrong nodes, if ReadChildNode is called again
                        return false;
                    }
                }
            }

            return false;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public bool IsElement(string name = null)
        {
            if (m_nNodeType != XmlNodeType.Element)
                return false;

            if (name != null && m_strName != name)
                return false;

            return true;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public bool IsText()
        {
            return m_nNodeType == XmlNodeType.Text;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public bool HasName(string name)
        {
            if (!IsElement())
                return false;

            return m_strName == name;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public string GetName()
        {
            if (!IsElement())
                return "";

            return m_strName;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public string GetText()
        {
            if (!IsText())
                return "";

            return m_strName;
        }


        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public string GetInnerText()
        {
            CXmlNode child = new CXmlNode();

            string text = "";

            while (ReadChildNode(ref child))
            {
                if (child.IsText())
                {
                    if (!string.IsNullOrEmpty(text))
                        text += " ";
                    text += child.GetText();
                }
            }

            return text;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <created>UPh,01.12.2014</created>
        /// <changed>UPh,01.12.2014</changed>
        // ********************************************************************************
        public bool HasAttribute(string name)
        {
            for (int i = 0; i < m_AttributeNames.Count; i++)
            {
                if (m_AttributeNames[i] == name)
                    return true;
            }

            return false;
        }


        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public bool GetAttribute(string name, out string value)
        {
            for (int i = 0; i < m_AttributeNames.Count; i++)
            {
                if (m_AttributeNames[i] == name)
                {
                    value = m_AttributeValues[i];
                    return true;
                }

            }

            value = "";
            return false;
        }


        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public bool GetIntAttribute(string name, out int value)
        {
            string text;
            if (GetAttribute(name, out text))
            {
                return int.TryParse(text, out value);
            }

            value = 0;
            return false;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <created>UPh,25.02.2014</created>
        /// <changed>UPh,25.02.2014</changed>
        // ********************************************************************************
        /* TODO
        bool GetDWordAttribute(string name, DWORD *value)
        {
            TCHAR text[80] = _T("");
            if (GetAttribute(name, text, _countof(text)))
            {
                *value = _ttoul(text);
                return true;
            }

            *value = 0;
            return false;
        }
        */


        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public bool GetFloatAttribute(string name, double value)
        {
            string text;
            if (GetAttribute(name, out text))
            {
                return double.TryParse(text, out value);
            }

            value = 0;
            return false;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,20.03.2013</created>
        /// <changed>UPh,20.03.2013</changed>
        //********************************************************************************
        public bool GetBoolAttribute(string name, out bool value)
        {
            string text;
            if (GetAttribute(name, out text))
            {
                int intval = 0;
                if (string.Compare(text, "true", true) == 0 ||
                    string.Compare(text, "yes", true) == 0 ||
                    (int.TryParse(text, out intval) && intval > 0))
                {
                    value = true;
                    return true;
                }

                if (string.Compare(text, "false", true) == 0 ||
                    string.Compare(text, "no", true) == 0 ||
                    (int.TryParse(text, out intval) && intval == 0))
                {
                    value = false;
                    return true;
                }
            }

            value = false;
            return false;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <created>UPh,12.11.2013</created>
        /// <changed>UPh,12.11.2013</changed>
        // ********************************************************************************
        /* TODO
        bool GetColorAttribute(string name, COLORREF *value)
        {
            int r, g, b;
            TCHAR text[80] = _T("");
            if (GetAttribute(name, text, _countof(text)) &&
                text[0] == '#' &&
                _stscanf_s(text + 1, _T("%02x%02x%02x"), &r, &g, &b) == 3)
            {
                *value = RGB(r, g, b);
                return true;
            }

            *value = 0; // BLACK
            return false;
        }
        */

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="CMap<int"></param>
        /// <param name="enummap"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <created>UPh,11.11.2013</created>
        /// <changed>UPh,11.11.2013</changed>
        // ********************************************************************************
        /* TODO

        bool GetEnumAttribute(string name, XmlEnumMap &enummap, int *value)
        {
            TCHAR text[80] = _T("");
            if (GetAttribute(name, text, _countof(text)))
            {
                for (std::pair<string, int> c : enummap)
                {
                    if (_tcscmp(c.first, text) == 0)
                    {
                        *value = c.second;
                        return true;
                    }
                }

                if (_istdigit(text[0]))
                {
                    *value = _ttoi(text);
                    return true;
                }

                ASSERT(0); // invalid enum 
            }

            *value = 0;
            return false;
        }
        */

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <created>UPh,01.04.2014</created>
        /// <changed>UPh,01.04.2014</changed>
        // ********************************************************************************
        /* TODO
        bool GetTimeAttribute(string name, CFileTime *time)
        {
            TCHAR text[80] = _T("");
            if (GetAttribute(name, text, _countof(text)))
            {
                CTMFileTime t;
                if (t.ScanIsoDateTime(text, true))
                {
                    time.SetTime(t.m_Int);
                    return true;
                }
            }

            time.SetTime(0);
            return false;
        }
        */

#if false
        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,08.04.2013</created>
        /// <changed>UPh,08.04.2013</changed>
        //********************************************************************************
        bool SerializeAttribute(string name, string *value, string def /*= null*/)
        {
            if (IsReading())
            {
                if (def == null)
                    def = _T("");
                if (!GetAttribute(name, value))
                    *value = def;
                return true;
            }
            else if (IsWriting())
            {
                WriteAttribute(name, *value);
                return true;
            }
            return false;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="nValue"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,03.06.2015</created>
        /// <changed>UPh,03.06.2015</changed>
        // ********************************************************************************
        bool SerializeAttribute(string name, LPTSTR value, int nValue, string def /*= null*/)
        {
            if (IsReading())
            {
                if (def == null)
                    def = _T("");

                if (!GetAttribute(name, value, nValue))
                    lstrcpyn(value, def, nValue);
                return true;

            }
            else if (IsWriting())
            {
                WriteAttribute(name, value);
                return true;
            }

            return false;
        }


        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,08.04.2013</created>
        /// <changed>UPh,08.04.2013</changed>
        //********************************************************************************
        bool SerializeIntAttribute(string name, int *value, int def /*= 0*/)
        {
            if (IsReading())
            {
                if (!GetIntAttribute(name, value))
                    *value = def;
                return true;
            }
            else if (IsWriting())
            {
                WriteIntAttribute(name, *value);
                return true;
            }
            return false;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,25.02.2014</created>
        /// <changed>UPh,25.02.2014</changed>
        // ********************************************************************************
        bool SerializeDWordAttribute(string name, DWORD *value, int def /*= 0*/)
        {
            if (IsReading())
            {
                if (!GetDWordAttribute(name, value))
                    *value = def;
                return true;
            }
            else if (IsWriting())
            {
                WriteDWordAttribute(name, *value);
                return true;
            }
            return false;
        }



        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,08.04.2013</created>
        /// <changed>UPh,08.04.2013</changed>
        //********************************************************************************
        bool SerializeFloatAttribute(string name, double *value, double def /*= 0.0*/, int digits /*= 16*/)
        {
            if (IsReading())
            {
                if (!GetFloatAttribute(name, value))
                    *value = def;
                return true;
            }
            else if (IsWriting())
            {
                WriteFloatAttribute(name, *value, digits);
                return true;
            }
            return false;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,08.04.2013</created>
        /// <changed>UPh,08.04.2013</changed>
        //********************************************************************************
        bool SerializeBoolAttribute(string name, bool *value, bool def /*= false*/)
        {
            if (IsReading())
            {
                if (!GetBoolAttribute(name, value))
                    *value = def;
                return true;
            }
            else if (IsWriting())
            {
                WriteBoolAttribute(name, *value);
                return true;
            }
            return false;
        }

        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,02.02.2015</created>
        /// <changed>UPh,02.02.2015</changed>
        // ********************************************************************************
        bool SerializeBoolAttribute(string name, bool *value, bool def /*= false*/)
        {
            if (IsReading())
            {
                if (!GetBoolAttribute(name, value))
                    *value = def;
                return true;
            }
            else if (IsWriting())
            {
                WriteBoolAttribute(name, *value);
                return true;
            }
            return false;
        }

        //********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,25.04.2013</created>
        /// <changed>UPh,25.04.2013</changed>
        //********************************************************************************
        bool SerializeColorAttribute(string name, COLORREF *value, COLORREF def /*= false*/)
        {
            if (IsReading())
            {
                if (!GetColorAttribute(name, value))
                    *value = def;
                return true;
            }
            else if (IsWriting())
            {
                WriteColorAttribute(name, *value);
                return true;
            }
            return false;
        }


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="CMap<int"></param>
        /// <param name="enummap"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        /// <created>UPh,11.11.2013</created>
        /// <changed>UPh,11.11.2013</changed>
        // ********************************************************************************
        /* TODO
        bool SerializeEnumAttribute(string name, XmlEnumMap &enummap, int *value, int def = 0)
        {
            if (IsReading())
            {
                if (!GetEnumAttribute(name, enummap, value))
                    *value = def;
                return true;

            }
            else if (IsWriting())
            {
                WriteEnumAttribute(name, enummap, *value);
                return true;
            }

            return false;
        }
        */


        // ********************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <created>UPh,01.04.2014</created>
        /// <changed>UPh,01.04.2014</changed>
        // ********************************************************************************
        bool SerializeTimeAttribute(string name, CFileTime *time)
        {
            if (IsReading())
            {
                return GetTimeAttribute(name, time);
            }
            else
            {
                return WriteTimeAttribute(name, time);
            }
        }
#endif

        internal void InitValues(XmlNodeType nodeType, int depth, XmlReader reader)
        {
            m_nNodeType = nodeType;
            m_nDepth= depth;
            m_pReader = reader;
        }

        internal void AddAttribute(string name, string value)
        {
            m_AttributeNames.Add(name);
            m_AttributeValues.Add(value);
        }


    }

    public  static class XmlNodeExtensions
    {
        public static bool ReadRootElement(this XmlReader reader, ref CXmlNode el)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            while(reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType != XmlNodeType.Element)
                    continue;

                int depth = reader.Depth;

                el.InitValues(XmlNodeType.Element, depth, reader);

                bool hr = reader.MoveToFirstAttribute();
                while (hr)
                {
                    string attName = reader.LocalName;
                    string attValue = reader.Value;

                    el.AddAttribute(attName, attValue);

                    hr = reader.MoveToNextAttribute();
                }


                return true;
            }

            return false;
        }
    }

}
