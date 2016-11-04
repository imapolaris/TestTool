
namespace VTSCore.Layers.Tracks
{
    public class SeecoolVideoReceiver : ReceiverBase
    {
        public delegate void OnTarget(int id, double lat, double lon, double sog, double cog);
        public event OnTarget TargetEvent;
        private void fireOnTarget(int id, double lat, double lon, double sog, double cog)
        {
            OnTarget callback = TargetEvent;
            if (callback != null)
                callback(id, lat, lon, sog, cog);
        }

        public delegate void OnDropTarget(int id);
        public event OnDropTarget DropTargetEvent;
        private void fireOnDropTarget(int id)
        {
            OnDropTarget callback = DropTargetEvent;
            if (callback != null)
                callback(id);
        }

        public override void Consume(string line)
        {
            string[] comps = line.Split(',');
            if (comps.Length >= 6 && comps[0] == "Track")
            {
                int id;
                double lat, lon, sog, cog;
                if (int.TryParse(comps[1], out id)
                    && double.TryParse(comps[2], out lat)
                    && double.TryParse(comps[3], out lon)
                    && double.TryParse(comps[4], out sog)
                    && double.TryParse(comps[5], out cog))
                {
                    fireOnTarget(id, lat, lon, sog, cog);
                }
            }
            else if (comps.Length >= 2 && comps[0] == "DropTrack")
            {
                int id;
                if (int.TryParse(comps[1], out id))
                    fireOnDropTarget(id);
            }
        }
    }
}
