using Seecool.Radar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Plotting
{
    public class RadarRegionsInfo
    {
        /// <summary>
        /// 拷贝regions组
        /// </summary>
        /// <param name="resource">非空</param>
        /// <returns></returns>
        public static RadarRegion[] Copy(RadarRegion[] resource)//nonull resource
        {
            List<RadarRegion> regions = new List<RadarRegion>();
            for (int i = 0; i < resource.Length; i++)
                regions.Add(Copy(resource[i]));
            return regions.ToArray();
        }
        /// <summary>
        /// 拷贝region
        /// </summary>
        /// <param name="resource">非空</param>
        /// <returns></returns>
        public static RadarRegion Copy(RadarRegion resource)
        {
            RadarRegion region = new RadarRegion()
            {
                Name = resource.Name,
                IsMask = resource.IsMask,
                ManualIdenfity = resource.ManualIdenfity,
                PassThrough = resource.PassThrough,
            };
            List<Seecool.Radar.Unit.PointD> points = new List<Seecool.Radar.Unit.PointD>();
            if (resource.Polygon != null)
            {
                for (int i = 0; i < resource.Polygon.Length; i++)
                    points.Add(resource.Polygon[i]);
            }
            region.Polygon = points.ToArray();
            return region;
        }

        public static bool AreEqual(RadarRegion[] regions1, RadarRegion[] regions2)
        {
            if (regions1 == null || regions2 == null)
                return (regions1 == null && regions2 == null);
            if (regions1.Length != regions2.Length)
                return false;
            for (int i = 0; i < regions1.Length; i++)
            {
                var region2 = regions2.First(r => r.Name == regions1[i].Name);
                if (!AreEqual(regions1[i], region2))
                    return false;
            }
            return true;
        }

        public static bool AreEqual(RadarRegion region1, RadarRegion region2)
        {
            if (region1 == null || region2 == null)
                return (region1 == null && region2 == null);
            if (region1.Name != region2.Name || region1.IsMask != region2.IsMask || region1.ManualIdenfity != region2.ManualIdenfity || region1.PassThrough != region2.PassThrough)
                return false;
            if (region1.Polygon == null || region2.Polygon == null)
                return (region1.Polygon == null && region2.Polygon == null);
            if (region1.Polygon.Length != region2.Polygon.Length)
                return false;
            for (int i = 0; i < region1.Polygon.Length; i++)
            {
                if (region1.Polygon[i] != region2.Polygon[i])
                    return false;
            }
            return true;
        }
    }
}
