using System;
using System.Text;
using System.Collections.Generic;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharp.CrestronIO;                   //File Ops
using Crestron.SimplSharp.CrestronXml;
using Crestron.SimplSharp.CrestronXmlLinq;              //XML Handling

namespace StringToXMLNameSpace
{
    public class StringToXML
    {
        //Instantiate The Delegate / Event Handler
        public ReadLineFromSharp myReadLineFromSharp { get; set; }
        public delegate void ReadLineFromSharp(SimplSharpString value);
        List<string> ListIn;
        List<string> ListOut;
        public string fPath;
        public string recordLabel;
        private FileStream myStream;
        XmlReader myXMLReader;

        /// <summary>
        /// SIMPL+ can only execute the default constructor. If you have variables that require initialization, please
        /// use an Initialize method
        /// </summary>
        public StringToXML()
        {
            ListIn = new List<string>();
            ListOut = new List<string>();
        }

        public void Initialize(String pathFromSplus, String rLabel)
        {
            try
            {
                fPath = pathFromSplus;
                recordLabel = rLabel;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Exception: {0}", e.Message);
            }
        }

        public void ClearArray()
        {
            try
            {
                ListIn.Clear();
            }
            catch (Exception e)
            {
                ErrorLog.Error("Exception: {0}", e.Message);
            }
        }

        public void ReadFile()
        {
            try
            {
                OpenLocalFile();
            }
            catch (Exception e)
            {
                ErrorLog.Error("Exception: {0}", e.Message);
            }
        }

        public void ReadLine(ushort LineIndex)
        {
            try
            {
                myReadLineFromSharp(new SimplSharpString(ListOut[LineIndex].ToString()));
            }
            catch (Exception e)
            {
                ErrorLog.Error("Exception: {0}", e.Message);
            }
        }

        public void WriteLine(String DataVal)
        {
            try
            {
                ListIn.Add(DataVal);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Exception: {0}", e.Message);
            }
        }

        public void WriteFile()
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("Stored Strings File"),
                new XComment(DateTime.Now.ToString()));
            var SavedStrings = new XElement(XGen("StringToXML"));
            foreach (var item in ListIn)
            {
                SavedStrings.Add(new XElement(XGen(recordLabel), item));
            }
            doc.Add(SavedStrings);
            using (var fs = new FileStream(fPath, FileMode.Create, FileAccess.Write))
            {
                using (var xw = new XmlWriter(fs))
                {
                    doc.Save(xw);
                }
            }
        }
        private XName XGen(string value)
        {
            XName newValue = value;
            return newValue;
        }

        public bool OpenLocalFile()
        {
            bool returnvalue = true;
            try
            {
                myStream = new FileStream(fPath, FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                ErrorLog.Error("File Not Found Exception: {0}", e.Message);
                return false;
            }
            catch (DirectoryNotFoundException e)
            {
                ErrorLog.Error("Directory Not Found Exception: {0}", e.Message);
                return false;
            }
            catch (PathTooLongException e)
            {
                ErrorLog.Error("Path Too Long Exception: {0}", e.Message);
                return false;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Exception: {0}", e.Message);
                return false;
            }
            try
            {
                int x = 0;

                myXMLReader = new XmlReader(myStream);

                ListOut.Clear();
                while (!myXMLReader.EOF)
                {
                    x = x++;
                    if (x > 400)
                        throw new Exception("Looped 400 times; no End in sight!\n");
                    
                    myXMLReader.Read();

                    if ((myXMLReader.NodeType == XmlNodeType.Element) && (myXMLReader.Name == recordLabel))
                    {
                        //for reasons I can't fathom, the name and value appear to be separate elemets; do another read to get the record value.
                        //This logic assumes that the value for a record is always going to be the next element after the name.
                        myXMLReader.Read(); 
                        ListOut.Add(myXMLReader.Value.ToString()); 
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Exception: {0}", e.Message);
                return false;
            }
            finally
            {
                myStream.Close();
            }
            return returnvalue;
        }
    }
}
