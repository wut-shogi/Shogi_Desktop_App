using ShogiEngineDllTests;
using System.Diagnostics;
using Unity.VisualScripting;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

class UsiPlayer : Player
{
    public EngineState state = EngineState.Initializing;
    string move = "";
    public enum EngineState
    {
        Initializing,
        CheckIsReady,
        Ready,
        Calculating
    }
    public void StartEngine(string path)
    {
        //* Create your Process
        process = new Process();
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.CreateNoWindow = true;
        process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
        process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.StandardInput.Write("usi\n");
    }
    void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        UnityEngine.Debug.Log(outLine.Data);
        
        if(state == EngineState.Initializing)
        {
            if(outLine.Data == "usiok") {
                process.StandardInput.Write("usinewgame\n");
                process.StandardInput.Write("isready\n");
                state = EngineState.CheckIsReady;
            }
        }
        if (state == EngineState.CheckIsReady)
        {
            if (outLine.Data == "readyok")
            {
                state = EngineState.Ready;
            }
        }
        if(state == EngineState.Calculating)
        {
            if(outLine.Data.StartsWith("bestmove "))
            {
                move = outLine.Data.Split(' ')[1];
                state = EngineState.Ready;
            }
        }


    }
    public Process process;
    public UsiPlayer(string name, bool positiveZMovement,string path) : base(name, positiveZMovement)
    {
        StartEngine(path);
    }

    public override string MakeMove(string fen)
    {
        move = "";
        state = EngineState.Calculating;
        int t = int.Parse(PlayerPasser.instance.configuration.GetValueOrDefault("MaxTimeOnMove"));
        process.StandardInput.Write("position " + fen + "\n");
        process.StandardInput.Write("go movetime " + t+"\n");
        while(state!= EngineState.Ready)
        {
           Thread.Sleep(50);
        }
        return move;
    }
}