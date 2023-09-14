namespace FortniteKillTracker.Data
{
    public class Match
    {
        public int Id { get; set; }
        public int PlayersLeft { get; set; }
        public List<Player> Players { get; set; } = new();
        public List<FeedUpdate> FeedUpdates { get; set; } = new();
    }
}
