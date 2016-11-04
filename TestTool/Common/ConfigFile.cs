using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VTSCore.Data.Common
{
    public static class ConfigFile<T> where T : class
    {
        static ILog LogService { get { return LogManager.GetLogger("ConfigFile"); } }
        static public T FromFile(string fileName)
        {
            try
            {
                if (!System.IO.File.Exists(fileName))
                {
                    LogService.Warn("未能找到文件：" + fileName);
                    return null;
                }
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    return xs.Deserialize(fs) as T;
                }
            }
            catch (Exception ex)
            {
                LogService.Error("读取异常：" + fileName + Environment.NewLine + ex.Message);
            }
            return null;
        }

        static public bool SaveToFile(string fileName, T config)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(fs, config);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.Error("保存异常：" + fileName + Environment.NewLine + ex.Message);
            }

            return false;
        }
    }
}
