using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Radar
{
    class RadarColorTableDataInfo: INotifyPropertyChanged
    {
        List<ColorTableDataConfig> _colorTableConfig = new List<ColorTableDataConfig>();

        public static RadarColorTableDataInfo Instance { get; private set; }
        string ConfigPath;

        static RadarColorTableDataInfo()
        {
            Instance = new RadarColorTableDataInfo();
        }

        private RadarColorTableDataInfo()
        {
            string path = Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            ConfigPath = Path.Combine(path, "RadarColorTableDataConfig.xml");
            Reset();
        }

        public void Reset()
        {
            _colorTableConfig = new List<ColorTableDataConfig>();
            try
            {
                _colorTableConfig.AddRange(ConfigFile<ColorTableDataConfig[]>.FromFile(ConfigPath));
            }
            catch { }

            if (_colorTableConfig == null || _colorTableConfig.Count == 0)
            {
                try
                {
                    _colorTableConfig.AddRange(ColorTableDataConfig.InitColorTableDatas());
                    saveConfig();
                }
                catch { }
            }
            FirePropertyChanged("ResetColorTableDataConfig");
        }

        private void saveConfig()
        {
            ConfigFile<ColorTableDataConfig[]>.SaveToFile(ConfigPath, _colorTableConfig.ToArray());
        }

        public int Count { get { return _colorTableConfig.Count; } }

        public ColorTableData GetColorTableData(int index)
        {
            if (index >= 0 && index < _colorTableConfig.Count)
                return _colorTableConfig[index].ColorTableData();
            else
                return _colorTableConfig[0].ColorTableData();
        }

        public ColorTableDataConfig GetTableDataConfig(int index)
        {
            if (index >= 0 && index < _colorTableConfig.Count)
                return _colorTableConfig[index];
            else
                return _colorTableConfig[0];
        }

        public void Add(ColorTableDataConfig colorTableDataConfig)
        {
            _colorTableConfig.Add(colorTableDataConfig);
            saveConfig();
            FirePropertyChanged("AddColorTable");
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _colorTableConfig.Count)
            {
                _colorTableConfig.RemoveAt(index);
                saveConfig();
                FirePropertyChanged("RemoveColorTable");
            }
        }

        public void Editor(int index, ColorTableDataConfig colorTableDataConfig)
        {
            if(index >= 0 && index < _colorTableConfig.Count)
            {
                _colorTableConfig[index] = colorTableDataConfig;
                saveConfig();
                FirePropertyChanged("EditorColorTable");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
