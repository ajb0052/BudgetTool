using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace BudgetApp
{
    class MenuFile
    { 

        public static void Save(Expenses expenses)
        {


            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//Budget.xml";
            FileStream file = File.Create(path);


            XmlSerializer x = new XmlSerializer(typeof(Expenses));
            x.Serialize(file, expenses);
            file.Close();
        }

        public static void SaveAs(String filename, Expenses expenseToBeFiled)
        {
            // Create Stream
            FileStream fs = File.Create(filename);

            // Serialize
            XmlSerializer x = new XmlSerializer(expenseToBeFiled.GetType());
            x.Serialize(fs, expenseToBeFiled);
            fs.Close();
        }

    }

    class MenuEdit
    {

    }

    class MenuSettings
    {

    }

    class MenuHelp
    {

    }
}
