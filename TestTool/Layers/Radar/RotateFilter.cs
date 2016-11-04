using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarImageProjection;

namespace VTSCore.Layers.Radar
{
    internal class RotateFilter
    {
        private double _rotateAngle;
        public RotateFilter(double rotateAngle)
        {
            _rotateAngle = rotateAngle;
            while (_rotateAngle < 0)
                _rotateAngle += 360;
            while (_rotateAngle >= 360)
                _rotateAngle -= 360;
        }

        byte[] _cloneData = new byte[0];
        public void Filter(RadarInfo info, byte[] data)
        {
            if (_rotateAngle != 0)
            {
                int scanlines = info.ScanlineCount;
                int samples = info.SampleCount;
                int delta = (int)Math.Round(_rotateAngle * scanlines / 360);
                if (_cloneData.Length != data.Length)
                    _cloneData = new byte[data.Length];
                Array.Copy(data, _cloneData, data.Length);
                for (int i = 0; i < scanlines; i++)
                {
                    int j = (i + delta) % scanlines;
                    Array.Copy(_cloneData, j * samples, data, i * samples, samples);
                }
            }
        }
    }
}
