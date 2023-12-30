using ShogiEngineDllTests;

class BotPlayer : Player
{
    public BotPlayer(string name, bool positiveZMovement) : base(name, positiveZMovement)
    {

    }

    public override string MakeMove(string fen)
    {
        return ShogiEngineInterface.GetBestMove(fen);
    }
}