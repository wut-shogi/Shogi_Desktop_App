using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Text.RegularExpressions;

namespace ShogiEngineDllTests
{
    public class ShogiEngineInterface
    {
        [DllImport("shogi_engine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        private static extern string getAllLegalMoves(string SFENstring);

        [DllImport("shogi_engine.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        private static extern string getBestMove(string SFENstring, uint maxDepth, uint maxTime);


        public static List<(Vector2Int,Vector2Int)> GetAllMoves(string SFENstring)
        {
            string allMovesString = getAllLegalMoves(SFENstring);
            string[] movestrings = allMovesString.ToString().Split('|');
            List<(Vector2Int, Vector2Int)> v = new List<(Vector2Int, Vector2Int)> ();
            foreach(string e in movestrings) {
                v.Add((new Vector2Int(9-(e[0] - '0'), e[1]-'a'), new Vector2Int(9 - (e[2] - '0'), e[3] - 'a')));
            }

            return v;
        }

        public static string GetBestMove(string SFENstring)
        {
            return getBestMove(SFENstring, 1, 1);
        }
    }
}
