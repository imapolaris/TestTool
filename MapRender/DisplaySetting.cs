using SeeCool.ECDIS.S52;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SeaMap
{
    public class DisplaySetting:ICloneable
    {
        /// <summary>
        /// 海图旋转度
        /// </summary>
        public double Rotate { get; set; }

        /// <summary>o
        /// 显示模式
        /// </summary>
        public DisplaySchema DisplaySchema { get; set; }

        /// <summary>
        /// 显示项
        /// </summary>
        public DisplayMember DisplayMember { get; set; }
        public ColorSchema Palette { get; set; }
        public Dictionary<string, bool> ViewingGroup { get; set; }
        public S52DisplaySetting ToS52DisplaySetting()
        {
            var setting = new S52DisplaySetting();

            setViewingGroup(setting);

			switch (this.DisplaySchema)
			{
				case SeaMap.DisplaySchema.None:
					setting.Display = SeeCool.ECDIS.S52.DisplaySchema.None; break;
				case SeaMap.DisplaySchema.Base:
					setting.Display = SeeCool.ECDIS.S52.DisplaySchema.Base; break;
				case SeaMap.DisplaySchema.Standard:
					setting.Display = SeeCool.ECDIS.S52.DisplaySchema.Standard; break;
				case SeaMap.DisplaySchema.All:
					setting.Display = SeeCool.ECDIS.S52.DisplaySchema.All; break;
			}
            switch (this.Palette)
            {
                case SeaMap.ColorSchema.None:
                    setting.Palette = SeeCool.ECDIS.S52.ColorSchema.None;break;
                case SeaMap.ColorSchema.Day:
                    setting.Palette = SeeCool.ECDIS.S52.ColorSchema.Day;break;
                case SeaMap.ColorSchema.Dusk:
                    setting.Palette = SeeCool.ECDIS.S52.ColorSchema.Dusk; break;
                case SeaMap.ColorSchema.Night:
                    setting.Palette = SeeCool.ECDIS.S52.ColorSchema.Night; break;
            }
            setting.ShowCompass = DisplayMember.HasFlag(DisplayMember.ShowCompass);
            setting.ShowDptareSafeLine = DisplayMember.HasFlag(DisplayMember.ShowDeepWaterSafeLine);
            setting.ShowImportantText = DisplayMember.HasFlag(DisplayMember.ShowImportantWords);
            setting.ShowOtherText = DisplayMember.HasFlag(DisplayMember.ShowOtherWords);
            setting.ShowLightDesc = DisplayMember.HasFlag(DisplayMember.ShowLamplightDescription);
            setting.ShowObstacle = DisplayMember.HasFlag(DisplayMember.ShowBarrier);
            setting.ShowGrid = DisplayMember.HasFlag(DisplayMember.ShowLatAndLonNet);
            setting.ShowSimplePoint = DisplayMember.HasFlag(DisplayMember.ShowSeaMapSymbol);
            setting.ShowScaleBar = DisplayMember.HasFlag(DisplayMember.ShowScale);
            setting.ShowPlainAreaBound = DisplayMember.HasFlag(DisplayMember.ShowSymbolRegion);
            setting.ShowSoundg = DisplayMember.HasFlag(DisplayMember.ShowDeepWaterPoint);
            setting.ShowScaleBar = DisplayMember.HasFlag(DisplayMember.ShowScale);
            return setting;
        }

        private void setViewingGroup(S52DisplaySetting setting)
        {
            if(ViewingGroup!=null)
               foreach(var v in ViewingGroup)               
                   if (setting.ViewingGroup.ContainsKey(v.Key))
                       setting.ViewingGroup[v.Key] = v.Value;               
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public static Dictionary<string,bool> CreateDefaultViewingGroup()
        {
            Dictionary<string,bool> viewingGroup = new Dictionary<string, bool>();
            Int32[] enumValues = (Int32[])Enum.GetValues(typeof(ViewingGroup));
            foreach (Int32 intValue in enumValues)
            {
                if (!viewingGroup.ContainsKey(intValue.ToString()))
                    viewingGroup.Add(intValue.ToString(), true);
                else
                {
                    string key = intValue.ToString();
                }
            }
            return viewingGroup;
        }

        public static string GetViewingGroupHeader(string vgIdStr)
        {
            ViewingGroup vg = (ViewingGroup)(int.Parse(vgIdStr));
            return vg.ToString();
        }
    }

    
    public enum DisplaySchema
    {
        None,
        Base,
        Standard,
        All,
    }
    public enum ColorSchema
    {
        None = 0,
        Day = 1,
        Night = 2,
        Dusk = 3,
    }
    [Flags]
    public enum DisplayMember
    {
        /// <summary>
        /// 显示重要文字
        /// </summary>
        ShowImportantWords = 1,
        /// <summary>
        /// 显示其他文字
        /// </summary>
        ShowOtherWords = 2,
        /// <summary>
        /// 显示指北针
        /// </summary>
        ShowCompass = 4,
        /// <summary>
        /// 显示比例尺
        /// </summary>
        ShowScale = 8,
        /// <summary>
        /// 显示经纬度网
        /// </summary>
        ShowLatAndLonNet = 16,
        /// <summary>
        /// 显示水深点
        /// </summary>
        ShowDeepWaterPoint = 32,
        /// <summary>
        /// 显示碍航物
        /// </summary>
        ShowBarrier = 64,
        /// <summary>
        /// 显示水深安全线
        /// </summary>
        ShowDeepWaterSafeLine = 128,
        /// <summary>
        /// 显示灯光描述
        /// </summary>
        ShowLamplightDescription = 256,
        /// <summary>
        /// 传统纸海图符号
        /// </summary>
        ShowSeaMapSymbol = 512,
        /// <summary>
        /// 区域符号边
        /// </summary>
        ShowSymbolRegion = 1024,
    }
}
