using System;
using System.Collections.Generic;
using System.Text;

namespace PassLib
{
    internal class PlRecentList
    {
        //********************************************************************************
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="section">Section name for saving RecentList in Profile</param>
        /// <param name="maxsize">Max. entries in RecentList</param>
        /// <returns></returns>
        /// <created>UPh,16.03.2007</created>
        /// <changed>UPh,16.03.2007</changed>
        //********************************************************************************
        internal PlRecentList(string section, int maxsize)
        {
            Initialize(section, maxsize);
        }

        //********************************************************************************
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="section">Section name for saving RecentList in Profile</param>
        /// <returns></returns>
        /// <created>UPh,16.03.2007</created>
        /// <changed>UPh,16.03.2007</changed>
        //********************************************************************************
        internal PlRecentList(string section)
        {
            Initialize(section, 8);
        }
        
        //********************************************************************************
        /// <summary>
        /// Save the list to the profile, see PlProfile.SetProfileName()
        /// </summary>
        /// <returns></returns>
        /// <created>UPh,16.03.2007</created>
        /// <changed>UPh,16.03.2007</changed>
        //********************************************************************************
        internal void Save()
        {
            int count = m_Items.Count;
            PlProfile.WriteInt(m_Section, "Count", count);
            
            for (int i = 0; i < count; i++)
            {
                PlProfile.WriteString(m_Section, Convert.ToString(i), m_Items[i]);
            }    
        }
        
        //********************************************************************************
        /// <summary>
        /// Selects a string to the top of the list 
        /// </summary>
        /// <param name="str">String to select</param>
        /// <param name="save">true = save the list now</param>
        /// <returns></returns>
        /// <created>UPh,16.03.2007</created>
        /// <changed>UPh,17.03.2007</changed>
        //********************************************************************************
        internal void Select(string str, bool bSave)
        {
            for (int i = 0; i < m_Items.Count; i++)
            {
                if (m_Items[i] == str)
                {
                    // Move the string to the top position
                    m_Items.RemoveAt(i);
                    m_Items.Insert(0, str);
                    
                    if (bSave)
                        Save();
                    return;
                }
            }
            
            // Add Item to the List
            if (m_Items.Count == m_MaxSize)
                m_Items.RemoveAt(m_MaxSize - 1);
                
            m_Items.Insert(0, str);
            
            if (bSave)
                Save();
        }
        

        List<string> m_Items;
        int m_MaxSize;
        string m_Section;

        
        //********************************************************************************
        /// <summary>
        /// Initializes data and loads RecentList from profile
        /// </summary>
        /// <param name="section"></param>
        /// <param name="maxsize"></param>
        /// <returns></returns>
        /// <created>UPh,16.03.2007</created>
        /// <changed>UPh,16.03.2007</changed>
        //********************************************************************************
        private void Initialize(string section, int maxsize)
        {
            m_Items = new List<string>();
            m_MaxSize = maxsize;
            m_Section = section;
            
            // Lade aus demProfil
            int count = PlProfile.GetInt(m_Section, "Count");
            for (int i = 0; i < count; i++)
            {
                string value = PlProfile.GetString(section, Convert.ToString(i));
                if (value == "")
                    continue;
                    
                m_Items.Add(value);
            }
        }
        
        //********************************************************************************
        /// <summary>
        /// Returns the current list as string array
        /// </summary>
        /// <param name="GetItems("></param>
        /// <returns></returns>
        /// <created>UPh,17.03.2007</created>
        /// <changed>UPh,17.03.2007</changed>
        //********************************************************************************
        internal string[] GetItems()
        {
            return m_Items.ToArray();
        }
        
    }
}
