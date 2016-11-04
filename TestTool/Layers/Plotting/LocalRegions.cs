using Common.Logging;
using Seecool.Radar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Plotting
{
    public class LocalRegions
    {
        string _configPath;
        public static LocalRegions Instance { get; private set; }
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }
        static LocalRegions()
        {
            Instance = new LocalRegions();
        }
        private LocalRegions()
        {
            string path = System.IO.Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            _configPath = System.IO.Path.Combine(path, "InvalidRadarAreaData.txt");
        }


        public RadarRegion[] Regions
        {
            get { return readLocalRegions(); }
            set { saveLocalRegions(value); }
        }

        private RadarRegion[] readLocalRegions()
        {
            List<RadarRegion> regions = new List<RadarRegion>();
            try
            {
                if(System.IO.File.Exists(_configPath))
                {
                    System.IO.StreamReader objReader = new System.IO.StreamReader(_configPath);
                    string sLine = "";
                    while (sLine != null)
                    {
                        sLine = objReader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(sLine))
                        {
                            RadarRegion region = RadarRegionFromString.GetRegion(sLine);
                            regions.Add(region);
                        }
                    }
                    objReader.Close();
                }
            }
            catch (Exception ex)
            {
                LogService.WarnFormat(ex.ToString());
            }
            return regions.ToArray();
        }
        private void saveLocalRegions(RadarRegion[] regions)
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(_configPath, System.IO.FileMode.Create);
                System.IO.StreamWriter swWriteFile = new System.IO.StreamWriter(fs);
                foreach (var region in regions)
                {
                    string strReadLine = region.Name;
                    for (int j = 0; j < region.Polygon.Length; j++)
                        strReadLine += "," + region.Polygon[j].X + "," + region.Polygon[j].Y;
                    swWriteFile.WriteLine(strReadLine); //写入读取的每行数据
                    swWriteFile.Flush();
                }
                swWriteFile.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
