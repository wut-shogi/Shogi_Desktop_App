using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ShogiEngineDllTests
{
    public class ShogiEngineInterface
    {
        [DllImport("shogi_engine.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool init();

        [DllImport("shogi_engine.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void cleanup();

        [DllImport("shogi_engine.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getAllLegalMoves(string SFENstring, byte[] output);

        [DllImport("shogi_engine.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int getBestMove(string SFENstring, uint maxDepth, uint maxTime, bool useGPU, byte[] output);


        public static bool Init()
        {
            return init();
        }

        public static void CleanUp()
        {
            cleanup();
        }

        static public (Vector2Int, Vector2Int) Move2Coord(string e)
        {
            return (new Vector2Int( (e[0] - '1'), e[1] - 'a'), new Vector2Int((e[2] - '1'), e[3] - 'a'));
        }
        public static List<(Vector2Int, Vector2Int)> GetAllMoves(string SFENstring)
        {
            try { 
            var outputBuffer = new byte[4096];
            int size = getAllLegalMoves(SFENstring, outputBuffer);
            string movesString = Encoding.UTF8.GetString(outputBuffer, 0, size);
            var movestrings =  movesString.Split('|');
            List<(Vector2Int, Vector2Int)> v = new List<(Vector2Int, Vector2Int)>();
            foreach (string e in movestrings)
            {
                v.Add(Move2Coord(e));
            }

            return v;}catch(Exception e)
            {
                GameManager.instance.win = true;
                return new List<(Vector2Int, Vector2Int)>();
            }
        }

        public static string GetBestMove(string SFENstring, uint maxDepth=1000, uint maxTime=1000, bool useGPU = true)
        {
            var outputBuffer = new byte[4096];
            int size = getBestMove(SFENstring, maxDepth, maxTime, useGPU, outputBuffer);
            return Encoding.UTF8.GetString(outputBuffer, 0, size);
        }
    }
}
