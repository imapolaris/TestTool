
namespace VTSCore.Layers.Tracks
{
    public class SeecoolRadarReceiver : ReceiverBase
    {
        public delegate void OnTarget(int id, double lat, double lon, double sog, double cog, int mmsi, string name);
        public event OnTarget TargetEvent;
        private void fireOnTarget(int id, double lat, double lon, double sog, double cog, int mmsi, string name)
        {
            OnTarget callback = TargetEvent;
            if (callback != null)
                callback(id, lat, lon, sog, cog, mmsi, name);
        }

        public override void Consume(string line)
        {
            string[] comps = line.Split(',');
            if (comps.Length >= 10 && comps[0] == "SCR")
            {
                try
                {
                    int id = int.Parse(comps[1]);
                    double lat = double.Parse(comps[2]);
                    double lon = double.Parse(comps[3]);
                    double sog = double.Parse(comps[4]);
                    double cog = double.Parse(comps[5]);
                    int mmsi = 0;
                    int.TryParse(comps[7], out mmsi);
                    string name = comps[8];
                    fireOnTarget(id, lat, lon, sog, cog, mmsi, name);
                }
                catch
                {
                }
            }
            if (comps.Length >= 7 && comps[0] == "SCP")
            {
                try
                {
                    int id = int.Parse(comps[1]);
                    double lat = double.Parse(comps[2]);
                    double lon = double.Parse(comps[3]);
                    double sog = double.Parse(comps[5]);
                    double cog = double.Parse(comps[6]);
                    int mmsi = 0;
                    if (comps.Length >= 8)
                        int.TryParse(comps[7], out mmsi);
                    string name = string.Empty;
                    if (comps.Length >= 9)
                        name = comps[8];
                    fireOnTarget(id, lat, lon, sog, cog, mmsi, name);
                }
                catch
                {
                }
            }
            if (comps.Length >= 9 && comps[0] == "Track")
            {
                try
                {
                    int id = int.Parse(comps[1]);
                    double lat = double.Parse(comps[2]);
                    double lon = double.Parse(comps[3]);
                    double sog = double.Parse(comps[4]);
                    double cog = double.Parse(comps[5]);
                    int mmsi = 0;
                    int.TryParse(comps[6], out mmsi);
                    string name = "raw track:" + id.ToString();
                    fireOnTarget(id, lat, lon, sog, cog, mmsi, name);
                }
                catch
                {
                }
            }
        }
    }
}
