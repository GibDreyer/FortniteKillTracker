using System.Diagnostics.Eventing.Reader;

namespace FortniteKillTracker.Data
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Kills { get; set; }
        public int Downs { get; set; }
        public bool HasCrown { get; set; }
        public ThreatLevel ThreatLevel => GetThreatLevel();

        // Ids of the players killed
        public List<int> PlayersKilled { get; set; } = new();
        // Ids of the players downed
        public List<int> PlayersDowned { get; set; } = new();


        public ThreatLevel GetThreatLevel()
        {
            if (Kills > 7)
                return ThreatLevel.Hard;
            else if (HasCrown || Kills > 3)
                return ThreatLevel.Moderate;
            else if (HasCrown)
                return ThreatLevel.Easy;
            else
                return ThreatLevel.None;
        }
    }
}
