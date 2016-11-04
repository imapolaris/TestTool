using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTSCore.Layers.Maps
{
    public class MapBorderInfo
    {
        const double _scaleInf = 1000;
        const double _scaleSup = 30000000;
        const double _lonInf = 99;
        const double _lonSup = 148;
        const double _latInf = 0;
        const double _latSup = 50;

        public static double CorrectionScale(double scale)
        {
            if (scale < _scaleInf)
                scale = _scaleInf;
            else if (scale > _scaleSup)
                scale = _scaleSup;
            return scale;
        }
        public static VTSCore.Data.Common.MapPoint CorrectionCenter(VTSCore.Data.Common.MapPoint center)
        {
            if (center.Lon < _lonInf)
                center.Lon = _lonInf;
            else if (center.Lon > _lonSup)
                center.Lon = _lonSup;
            if (center.Lat < _latInf)
                center.Lat = _latInf;
            else if (center.Lat > _latSup)
                center.Lat = _latSup;
            return center;
        }
    }
}
