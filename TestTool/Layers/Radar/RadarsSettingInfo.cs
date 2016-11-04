using Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Radar
{
    public enum RadarChangedStatus
    {
        新增,
        删除
    }
    public struct OperatingQueue
    {
        public RadarChangedStatus Status;
        public int Id;
    }

    public class RadarsSettingInfo: INotifyPropertyChanged
    {
        List<RadarSettingInfo> _radars = new List<RadarSettingInfo>();
        public List<OperatingQueue> ChangedMuster = new List<OperatingQueue>();
        public RadarSettingInfo[] Radars { get { return _radars.ToArray(); } }
        public int Count { get { return _radars.Count; } }
        public static RadarsSettingInfo Instance { get; private set; }
        string ConfigPath;
        ILog LogService { get { return LogManager.GetLogger(GetType()); } }

        static RadarsSettingInfo()
        {
            Instance = new RadarsSettingInfo();
        }

        private RadarsSettingInfo()
        {
            string path = Directory.GetParent(System.Windows.Forms.Application.LocalUserAppDataPath).FullName;
            ConfigPath = Path.Combine(path, "RadarInfomationConfig.xml");
        }

        public RadarSettingInfo RadarPrev
        {
            get
            {
                if (_prevIndex >= 0 && _prevIndex < Count)
                    return _radars[_prevIndex];
                else
                    return null;
            }
        }

        public void Reset()
        {
            ChangedMuster.Clear();
            _radars.Clear();
            var config = ConfigFile<RadarConnection[]>.FromFile(ConfigPath);
            if (config == null)
                config = new RadarConnection[0];
            for (int i = 0; i < config.Length; i++ )
            {
                RadarSettingInfo radar = new RadarSettingInfo(config[i]);
                _radars.Add(radar);
            }
            FirePropertyChanged("ReadAllConfig");
        }
        bool isChangedRadarAddress()
        {
            var config = ConfigFile<RadarConnection[]>.FromFile(ConfigPath);
            if (config == null)
                config = new RadarConnection[0];
            if(config.Length != _radars.Count)
                return true;
            for(int i = 0; i < _radars.Count; i++)
            {
                if (!_radars[i].RadarAddress.IsSquels(config[i]))
                    return true;
            }
            return false;
        }

        public void SaveConformIfChanged()
        {
            if(isChangedRadarAddress())
                Save();
        }

        public void SetConfigIfChanged()
        {
            foreach (var radar in _radars)
            {
                if (!radar.IsEnable)
                    continue;
                if (radar.RadarStatus != null && !radar.IsSameRadarConfig())
                {
                    DialogResult msgBox = System.Windows.Forms.MessageBox.Show("雷达“" + radar.RadarName + "”配置参数已修改,是否进行保存?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    if (msgBox == DialogResult.Yes)
                        radar.SaveRadarConfig();
                }
                if(!radar.AreSameRadarChannels())
                {
                    DialogResult msgBox = System.Windows.Forms.MessageBox.Show("雷达“" + radar.RadarName + "”通道已修改,是否进行保存?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    if (msgBox == DialogResult.Yes)
                        radar.SetRadarChannals();
                }
            }
        }

        public void Save()
        {
            try
            {
                RadarConnection[] config = new RadarConnection[_radars.Count];
                for (int i = 0; i < _radars.Count; i++)
                    config[i] = _radars[i].RadarAddress;
                ConfigFile<RadarConnection[]>.SaveToFile(ConfigPath, config);
                LogService.InfoFormat("雷达配置保存成功");
            }
            catch(Exception ex)
            {
                LogService.Error(ex.ToString());
                MessageBox.Show("雷达配置保存失败！");
            }
        }

        public bool Add(RadarConnection radar)
        {
            for (int i = 0; i < _radars.Count; i++)
            {
                if (_radars[i].RadarAddress.IsSquels(radar))
                {
                    DialogResult msgBox = MessageBox.Show("已包含相同的数据来源,是否继续?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                    if(msgBox == DialogResult.Yes)
                        break;
                    if (msgBox == DialogResult.No)
                        return false;
                }
            }
            RadarSettingInfo info = new RadarSettingInfo(radar);
            _radars.Add(info);
            ChangedMuster.Add(new OperatingQueue() { Status = RadarChangedStatus.新增, Id = _radars.Count - 1 });
            FirePropertyChanged("AddRadar");
            return true;
        }
        
        public void RemoveAt(int index)
        {
            if(index >= 0 && index < _radars.Count)
            {
                ChangedMuster.Add(new OperatingQueue() { Status = RadarChangedStatus.删除, Id = index });
                _radars.RemoveAt(index);
                FirePropertyChanged("RemoveRadar");
            }
        }
        int _prevIndex = -1;
        public int SelectedIndex
        {
            get { return _prevIndex; }
            set
            {
                if (value != _prevIndex)
                {
                    _prevIndex = value;
                    FirePropertyChanged("SelectedIndex");
                }
            }
        }

        public void CenteredRadar()
        {
            if (RadarPrev != null)
                RadarPrev.GotoPositioning();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int GetRadarIndexFromName(string name)
        {
            for (int i = 0; i < Radars.Length; i++ )
            {
                if (Radars[i].RadarName == name)
                    return i;
            }
            return -1;
        }

        public void RadarEnableChanged()
        {
            FirePropertyChanged("RadarEnableChanged");
        }
    }
}