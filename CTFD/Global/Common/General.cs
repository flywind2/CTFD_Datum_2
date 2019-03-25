using CTFD.View.Control.Thermal;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CTFD.Global.Common
{
    public static partial class General
    {

        public static void RaiseGlobalHandler(GlobalEvent globalEvent, object value = null) => General.GlobalHandler?.Invoke(null, new GlobalEventArgs(globalEvent, value));

        public static string FindStringResource(string resourceKey) => General.FindResource(resourceKey).ToString();

        public static PathGeometry FindPathResource(string resourceKey) => (General.FindResource(resourceKey) as PathGeometry);

        public static object FindResource(string resourceKey) => App.Current.FindResource(resourceKey);

        public static T GetParentElement<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            T result = default(T);
            while (parent != null)
            {
                if (parent is T) { result = (T)parent; break; }
                else parent = VisualTreeHelper.GetParent(parent);
            }
            return result;
        }

        public static byte[] JsonSerialize<T>(T entity)
        {
            var result = default(byte[]);
            try
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(entity.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    jsonSerializer.WriteObject(stream, entity);
                    result = stream.ToArray();
                }
            }
            catch { }
            return result;
        }

        public static T JsonDeserialize<T>(byte[] bytes)
        {
            T result = default(T);
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                    result = (T)jsonSerializer.ReadObject(ms);
                }
            }
            catch { }
            return result;
        }

        public static string JsonSerializeToString<T>(T entity) => Encoding.UTF8.GetString(General.JsonSerialize(entity));

        public static T JsonDeserializeFromString<T>(string jsonString) => General.JsonDeserialize<T>(Encoding.UTF8.GetBytes(jsonString));

        public static string ReadConfig(string key) => ConfigurationManager.AppSettings[key];

        public static bool WriteConfiguration(string key, string value)
        {
            var result = false;
            try
            {
                System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings[key].Value = value;
                configuration.Save();
                result = true;
            }
            catch { }
            return result;
        }

        public static void SetBinding(ThermalUnit source, ThermalUnit target)
        {
            target.SetBinding(ThermalUnit.Y1Property, new Binding(Properties.Resources.Y2) { Source = source });
        }

        public static void SetBinding(ThermalCell source, ThermalCell target)
        {
            General.SetBinding(source.GetLastThermalUnit(), target.GetFirstThermalUnit());
        }

        public static void WriteJsonFile(object obj, string fileName, Encoding encoding) => File.WriteAllText(fileName, General.JsonSerializeToString(obj), encoding);

        public static T ReadJsonFile<T>(string fileName, Encoding encoding) => General.JsonDeserializeFromString<T>(File.ReadAllText(fileName, encoding));

        public static void ReadSetup() => General.WorkingData.Configuration = General.ReadJsonFile<Model.RuntimeData.Setup>($"{Environment.CurrentDirectory}{Properties.Resources.SetupFilePath}", Encoding.Default);

        public static void WriteSetup()
        {
            var aa = General.WorkingData.Configuration;

            General.WriteJsonFile(aa, $"{Environment.CurrentDirectory}{Properties.Resources.SetupFilePath}", Encoding.Default);
        }

        public static DataTable ReadExcel(string fileName, string sheetName)
        {
            var result = new DataTable();
            //string excelConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", fileName);
            string excelConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'", fileName);

            string excelCommandString = string.Format("select * from [{0}$]", sheetName);
            using (OleDbConnection connection = new OleDbConnection(excelConnectionString))
            {
                connection.Open();
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(excelCommandString, excelConnectionString)) adapter.Fill(result);
            }
            return result;
        }

        public static string GetEnumDescription(Enum enumObject)
        {
            Type type = enumObject.GetType();
            System.Reflection.MemberInfo[] memberInfos = type.GetMember(enumObject.ToString());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                if (memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attrs && attrs.Length > 0)
                {
                    return attrs[0].Description;
                }
            }
            return enumObject.ToString();
        }

        public static T GetEnumItem<T>(string enumName)
        {
            return (T)Enum.Parse(typeof(T), enumName);
        }

        public static string CreateHoleName(int rowNumber, int columnNumber)
        {
            var rowLetter = string.Empty;
            switch (rowNumber)
            {
                case 0: { rowLetter = "A"; break; }
                case 1: { rowLetter = "B"; break; }
                case 2: { rowLetter = "C"; break; }
                case 3: { rowLetter = "D"; break; }
                default: break;
            }
            if (++columnNumber == 0) columnNumber = 8;
            var columnName = columnNumber.ToString("00");
            return $"{rowLetter}{columnName}";
        }

        public static void ShowToast(string message) => General.RaiseGlobalHandler(GlobalEvent.ShowToast, message);

        public static void ShowFault(bool isShow) => General.RaiseGlobalHandler(GlobalEvent.ShowFault, isShow);

    }
}
