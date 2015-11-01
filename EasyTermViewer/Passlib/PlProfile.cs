using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace PassLib
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper for saving persitant data to the registry or a ini-File
    /// </summary>
    //--------------------------------------------------------------------------------
    internal class PlProfile
    {
        [DllImport("kernel32.dll")]
        static extern bool WritePrivateProfileString(string lpAppName,
           string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileString(string lpAppName,
           string lpKeyName, string lpDefault, StringBuilder lpReturnedString,
           uint nSize, string lpFileName);

        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName,
           int nDefault, string lpFileName);
           
    
        static private string m_Name; // Registryname oder Profilpfad
        static private ProfileType m_Type;

        internal enum ProfileType { Unknown, IniFile, Registry };
        
        //********************************************************************************
        /// <summary>
        /// Defines the path of the profile in the registry or as file 
        /// </summary>
        /// <param name="path">RegistryKey "Company name\Application" or path of the file</param>
        /// <param name="type">Registry or File</param>
        /// <returns></returns>
        /// <created>UPh,21.09.2006</created>
        /// <changed>UPh,21.09.2006</changed>
        //********************************************************************************
        internal static void SetProfileName(string path, ProfileType type)
        {
            m_Type = type;
            if (type == ProfileType.Registry)
            {
                m_Name = "Software\\" + path;
            }
            else if (type == ProfileType.IniFile)
            {
                m_Name = path;
            }
            else
            {
                throw new ArgumentException("Invalid ProfileType. Only Registry and File is valid.");
            }
            
        }
        
        //********************************************************************************
        /// <summary>
        /// Writes a string to the Profile
        /// </summary>
        /// <param name="section">Name of the section</param>
        /// <param name="entry">Name of the entry</param>
        /// <param name="value">String to write</param>
        /// <returns></returns>
        /// <created>UPh,21.09.2006</created>
        /// <changed>UPh,21.09.2006</changed>
        //********************************************************************************
        internal static void WriteString(string section, string entry, string value)
        {
            if (m_Name == "")
                return;
                
            switch (m_Type)
            {
                case ProfileType.Registry:
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(m_Name + "\\" + section);
                    key.SetValue(entry, value);
                    break;
                }
                
                case ProfileType.IniFile:
                {
                    WritePrivateProfileString(section, entry, value, m_Name);
                    break;
                }
                
                default:
                {
                    throw new Exception("Don't know where to write profile data to. Call PlProfile.SetProfileName().");
                }
            }
        }


        //********************************************************************************
        /// <summary>
        /// Returns a string from the profile
        /// </summary>
        /// <param name="section">Name of the section</param>
        /// <param name="entry">Name of the entry</param>
        /// <param name="def">Default string</param>
        /// <returns></returns>
        /// <created>UPh,21.09.2006</created>
        /// <changed>UPh,21.09.2006</changed>
        //********************************************************************************
        internal static string GetString(string section, string entry, string def)
        {
            string rts = def;
        
            switch (m_Type)
            {
                case ProfileType.Registry:
                    {
                        RegistryKey key = Registry.CurrentUser.OpenSubKey(m_Name + "\\" + section);
                        if (key != null)
                        {
                            object o = key.GetValue(entry);
                            if (o != null)
                                rts = o.ToString();
                            key.Close();
                        }
                        break;
                    }

                case ProfileType.IniFile:
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Capacity = 1024;
                        
                        GetPrivateProfileString(section, entry, def, sb, 1024, m_Name);
                        
                        rts = sb.ToString();
                        break;
                    }

                default:
                    {
                        throw new Exception("Don't know where to read profile data from. Call PlProfile.SetProfileName().");
                    }
                    
            }
            
            return rts;
        }

        //********************************************************************************
        /// <summary>
        /// Returns a string from the profile
        /// </summary>
        /// <param name="section">Name of the section</param>
        /// <param name="entry">Name of the entry</param>
        /// <returns>String, that has been read, "" on error</returns>
        /// <created>UPh,21.09.2006</created>
        /// <changed>UPh,21.09.2006</changed>
        //********************************************************************************
        internal static string GetString(string section, string entry)
        {
            return GetString(section, entry, "");
        }





        //********************************************************************************
        /// <summary>
        /// Writes an integer value to the profile
        /// </summary>
        /// <param name="section">Name of the section</param>
        /// <param name="entry">Name of the entry</param>
        /// <param name="value">Value to write</param>
        /// <returns></returns>
        /// <created>UPh,21.09.2006</created>
        /// <changed>UPh,21.09.2006</changed>
        //********************************************************************************
        internal static void WriteInt(string section, string entry, int value)
        {
            if (m_Name == "")
                return;

            switch (m_Type)
            {
                case ProfileType.Registry:
                    {
                        RegistryKey key = Registry.CurrentUser.CreateSubKey(m_Name + "\\" + section);
                        key.SetValue(entry, value);
                        key.Close();
                        break;
                    }

                case ProfileType.IniFile:
                    {
                        WritePrivateProfileString(section, entry, value.ToString(), m_Name);
                        break;
                    }

                default:
                    {
                        throw new Exception("Don't know where to write profile data to. Call PlProfile.SetProfileName().");
                    }
                    
            }
        }

        //********************************************************************************
        /// <summary>
        /// Returns an integer value from the profile
        /// </summary>
        /// <param name="section">Name of the section</param>
        /// <param name="entry">Name of the entry</param>
        /// <param name="def">Default value</param>
        /// <returns>Integer value. Default value on error.</returns>
        /// <created>UPh,21.09.2006</created>
        /// <changed>UPh,21.09.2006</changed>
        //********************************************************************************
        internal static int GetInt(string section, string entry, int def)
        {
            int rts = def;
            
            switch (m_Type)
            {
                case ProfileType.Registry:
                    {
                        RegistryKey key = Registry.CurrentUser.OpenSubKey(m_Name + "\\" + section);
                        if (key != null)
                        {
                            object o = key.GetValue(entry);
                            if (o != null)
                                rts = Int32.Parse(o.ToString());
                            key.Close();
                        }
                        break;
                    }

                case ProfileType.IniFile:
                    {
                        rts = (int) GetPrivateProfileInt(section, entry, def, m_Name);
                        break;
                    }

                default:
                    {
                        throw new Exception("Don't know where to read profile data from. Call PlProfile.SetProfileName().");
                    }
                    
            }
            
            return rts;
        }

        //********************************************************************************
        /// <summary>
        /// Returns an integer value from the profile
        /// </summary>
        /// <param name="section">Name of the section</param>
        /// <param name="entry">Name of the entry</param>
        /// <returns>Integer value. 0 on error.</returns>
        /// <created>UPh,21.09.2006</created>
        /// <changed>UPh,21.09.2006</changed>
        //********************************************************************************
        internal static int GetInt(string section, string entry)
        {
            return GetInt(section, entry, 0); 
        }
    }
}
