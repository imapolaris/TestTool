using SeeCool.ECDIS.S57;
using SeeCool.ECDIS.S57.Enum;
using SeeCool.Geometry.Unit;
using SeeCool.Geometry.Util;
using SeeCool.GISFramework.ClientFramework;
using SeeCool.GISFramework.Object;
using Services.SeaMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using TestTool.Layers.Maps;
using VTSCore.Data.Common;

namespace VTSCore.Layers.Maps
{
    public class FeatureSelectUI
    {
        ReadOnlyCollection<S57File> Files;
        LocatorAndBorder _locator;

        public Action<string> OnRestFeatureSelect;

        public System.Windows.Interop.HwndSource WinformWindow {get; set;}

        public FeatureSelectUI()
        {
            _locator = LocatorAndBorder.Instance;
        }

        public void ShapeCommitted(Point point)
        {
            Files = MercatorMapRender.Files();
            
            List<CompactFeatureObj> result = new List<CompactFeatureObj>();
            Array types = Enum.GetValues(typeof(S57FeatureType));
            for (int i = 0; i < types.Length; i++)
            {
                CompactFeatureObj[] objs = getFeatures(point, (S57FeatureType)types.GetValue(i));
                if (objs != null && objs.Length > 0)
                    result.AddRange(objs);
            }
            clientShow();
            _client.RefreshData(result);
        }

        private void clientShow()
        {
            if (!VTSCore.Common.WindowStateDetect.ShowWindow(_client))
            {
                _client = new FeatureSelectClient();
                if (WinformWindow != null)
                    new System.Windows.Interop.WindowInteropHelper(_client) { Owner = WinformWindow.Handle };
                _client.Show();
                _client.OnResetRegion += onRestRegion;
            }
        }

        private void onRestRegion(string shape)
        {
            if (OnRestFeatureSelect != null)
                OnRestFeatureSelect(shape);
        }

        FeatureSelectClient _client;
        
        private CompactFeatureObj[] getFeatures(Point pt, S57FeatureType type)
        {
            List<tagFEATURE> list = new List<tagFEATURE>();
            if(Files != null)
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    tagFEATURE[] features = Files[i].GetFeatures(type);
                    if (features != null && features.Length > 0)
                        updateListFromFeatures(pt, list, features);
                }
            }
            List<CompactFeatureObj> result = updateResult(list);
            return result.ToArray();
        }

        private void updateListFromFeatures(Point pt, List<tagFEATURE> list, tagFEATURE[] features)
        {
            for (int j = 0; j < features.Length; j++)
            {
                tagFEATURE feature = features[j];
                if ((feature.PRIM == GeoPrimitiveType.Point || feature.PRIM == GeoPrimitiveType.Text) && feature.SG2D != null)
                {
                    Point ptFeature = _locator.Locator.MapToScreen(feature.SG2D.Points[0].X, feature.SG2D.Points[0].Y);
                    //TODO：是否要通过装配单配置 选中点误差为10像素
                    if (Math.Abs(pt.X - ptFeature.X) < 10 && Math.Abs(pt.Y - ptFeature.Y) < 10)
                        list.Add(feature);
                }
                else if (feature.PRIM == GeoPrimitiveType.Line && feature.SG2D != null)
                    updateListFromSelected(pt, list, feature);
                else if (feature.PRIM == GeoPrimitiveType.Area && feature.Area != null)
                {
                    MapPoint mp = _locator.Locator.ScreenToMap(pt.X, pt.Y);
                    if (Calculate.PtInPolygon(mp.Lon, mp.Lat, feature.Area))
                        list.Add(feature);
                }
            }
        }

        private List<CompactFeatureObj> updateResult(List<tagFEATURE> list)
        {
            List<CompactFeatureObj> result = new List<CompactFeatureObj>();
            for (int i = 0; i < list.Count; i++)
            {
                tagFEATURE feature = list[i];
                GeometryShape geoShape = null;
                if ((feature.PRIM == GeoPrimitiveType.Point || feature.PRIM == GeoPrimitiveType.Text) && feature.SG2D != null)
                    geoShape = new GeoPointShape(feature.SG2D.Points[0].X, feature.SG2D.Points[0].Y);
                else if (feature.PRIM == GeoPrimitiveType.Line && feature.SG2D != null)
                    geoShape = new GeoLineShape(feature.SG2D);
                else if (feature.PRIM == GeoPrimitiveType.Area && feature.Area != null)
                    geoShape = new GeoAreaShape(feature.Area);
                if (geoShape != null && _locator.Rect.IntersectsWith(geoShape.Bounds))
                {
                    string name = string.Empty;
                    string[] names = feature.GetAttribute(S57AttributeType.NOBJNM);
                    if (names != null && names.Length > 0)
                        name = names[0];
                    string fileName = feature.File.Header.FileName;
                    int tempIndex = fileName.LastIndexOf('\\') + 1;
                    CompactFeatureObj obj = new CompactFeatureObj(string.Empty, name, string.Empty, geoShape, fileName.Substring(tempIndex, fileName.LastIndexOf('.') - tempIndex), feature.OBJL.ToString());
                    result.Add(obj);
                    setAttribute(feature, obj);
                }
            }
            return result;
        }

        private void updateListFromSelected(Point pt, List<tagFEATURE> list, tagFEATURE feature)
        {
            List<PointD[]> listPds = Calculate.CalcPolylineIntersect(feature.SG2D.Points, _locator.Rect);
            bool isSelected = false;
            foreach (PointD[] pd in listPds)
            {
                if (isSelected)
                    break;
                for (int k = 0; k < pd.Length - 1; k++)
                {
                    Point start = _locator.Locator.MapToScreen(pd[k].X, pd[k].Y);
                    Point end = _locator.Locator.MapToScreen(pd[k + 1].X, pd[k + 1].Y);
                    //TODO：是否要通过装配单配置 选中线误差为5像素
                    if (ScreenCalcUtil.CalcDisP2L(DrawingPoint(start), DrawingPoint(end), DrawingPoint(pt)) < 5)
                    {
                        isSelected = true;
                        break;
                    }
                }
            }
            if (isSelected)
                list.Add(feature);
        }

        private void setAttribute(tagFEATURE feature, CompactFeatureObj obj)
        {
            foreach (ushort attrId in feature.ATTFRead)
            {
                onSetAttribute(attrId, feature, obj);
            }
            foreach (ushort attrId in feature.NATFRead)
            {
                onSetAttribute(attrId, feature, obj);
            }
        }

        private void onSetAttribute(ushort key, tagFEATURE feature, CompactFeatureObj obj)
        {
            S57AttributeType attrType = (S57AttributeType)key;
            string[] attr = feature.GetAttribute(attrType);
            if (attr != null)
                obj[attrType.ToString()] = attr;
        }

        private System.Drawing.Point DrawingPoint(Point point)
        {
            return new System.Drawing.Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }
    }
}
