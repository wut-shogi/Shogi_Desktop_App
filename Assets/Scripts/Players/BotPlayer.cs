using ShogiEngineDllTests;
using System.Collections.Generic;

class BotPlayer : Player
{
    public BotPlayer(string name, bool positiveZMovement) : base(name, positiveZMovement)
    {

    }

    public override string MakeMove(string fen)
    {
        return ShogiEngineInterface.GetBestMove(fen,maxTime:uint.Parse(PlayerPasser.instance.configuration.GetValueOrDefault("MaxTimeOnMove")));
    }
}