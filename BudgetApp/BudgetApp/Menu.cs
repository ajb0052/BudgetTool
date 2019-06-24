using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace BudgetApp
{
    class MenuFile
    {


        public static void Save(Expenses expenseToBeSaved)
        {
            FileStream fileStream = File.Create(expenseToBeSaved.filename);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fileStream, expenseToBeSaved);

            //XmlSerializer x = new XmlSerializer(expenseToBeSaved.GetType());
           // x.Serialize(fileStream, expenseToBeSaved);
            fileStream.Close();
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
