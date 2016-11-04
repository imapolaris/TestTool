using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Maps
{
 	public class DisplaySetting
	{
        /// <summary>
        /// 显示模式
        /// </summary>
        public DisplaySchema DisplaySchema { get; set; }
        /// <summary>
        /// 显示项
        /// </summary>
        public DisplayMember DisplayMember { get; set; }
        public ColorSchema Palette { get; set; }
        public Dictionary<string, bool> ViewingGroup { get; private set; }
		public DisplaySetting()
		{
			this.DisplaySchema = Maps.DisplaySchema.Standard;
			this.DisplayMember = Maps.DisplayMember.ShowCompass | Maps.DisplayMember.ShowBarrier | Maps.DisplayMember.ShowImportantWords | Maps.DisplayMember.ShowScale;
            this.Palette = ColorSchema.Day;
            ViewingGroup = Services.SeaMap.DisplaySetting.CreateDefaultViewingGroup();
		}

        internal Services.SeaMap.DisplaySetting ToSeaMapDisplaySetting()
        {
            var setting = new Services.SeaMap.DisplaySetting();
            setting.DisplayMember = (Services.SeaMap.DisplayMember)this.DisplayMember;
            setting.DisplaySchema = (Services.SeaMap.DisplaySchema)this.DisplaySchema;
            setting.Palette = (Services.SeaMap.ColorSchema)this.Palette;
            setting.ViewingGroup = this.ViewingGroup;
            return setting;
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
