using BenchmarkDotNet.Attributes;
using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System;

[MemoryDiagnoser]
[Config(typeof(Program.DontForceGcCollectionsConfig))]
public class ChessEngine
{
    [Params(1,2)]
    public int targetDepth { get; set; }

    int legalMoves = 20;
    int evalPieces = 20;
    Random rnd = new Random();

    public class PieceClass
    {
        public int Value { get; }
        public char Color { get; }
        public PieceClass(int v, char c) { Value = v; Color = c; }
    }
    public class PieceMove
    {
        public int From { get; }
        public int To {get;}
        public PieceMove(int f, int t) { From = f; To = t; }
    }

    IEnumerable<PieceMove> FindMoves(PieceClass[] board) => Enumerable.Range(0, legalMoves).Select(i => new PieceMove(rnd.Next(0, 63), rnd.Next(0, 63))); // Move random piece
    PieceClass[] MakeMove(PieceClass[] board, PieceMove m) { board[m.To] = board[m.From]; board[m.From] = new PieceClass(0, ' '); return board; } // Should put in move history
    PieceClass[] RedoMove(PieceClass[] board, PieceMove m) { board[m.From] = board[m.To]; board[m.To] = new PieceClass(0, ' '); return board; } // Wrong. Should take from move history
    int EvalClass(PieceClass[] board) => Enumerable.Range(1, evalPieces).Sum(i => board[i].Value); // Wrong. No minmax.

    public int SearchClass(PieceClass[] board, int depth)
    {
        if (depth < 0) return EvalClass(board); // What, no quiescence test!!
        depth--;

        var legalMoves = FindMoves(board); // Boy, no alpha beta pruning?

        var maxEval = int.MinValue;
        foreach (var move in legalMoves)
        {
            var boardAfterMove = MakeMove(board, move);
            var moveEval  = SearchClass(boardAfterMove, depth);
            maxEval = moveEval > maxEval ? moveEval : maxEval;
            board = RedoMove(boardAfterMove, move);
        }
        return maxEval;
    }

    [Benchmark]
    public int EvalClassBench()
    {
        PieceClass[] boardClass = new PieceClass[64];
        for (int i = 0; i < boardClass.Length; i++) boardClass[i] = new PieceClass(i, 'p');

        return SearchClass(boardClass, targetDepth);
    }

    public readonly struct PieceStruct
    {
        public int Value { get; }
        public char Color { get; }
        public PieceStruct(int v, char c) { Value = v; Color = c; }
    }
    public readonly struct PieceMoveStruct
    {
        public int From { get; }
        public int To { get; }
        public PieceMoveStruct(int f, int t) { From = f; To = t; }
    }

    Span<PieceMoveStruct> FindMovesStruct(Span<PieceStruct> board, Span<PieceMoveStruct> pieceMoves) {
        for (int i = 0; i < legalMoves; i++) pieceMoves[i] = new PieceMoveStruct(rnd.Next(0, 63), rnd.Next(0, 63));
        return pieceMoves.Slice(0,legalMoves);
    }

    Span<PieceStruct> MakeMoveStruct(Span<PieceStruct> board, in PieceMoveStruct m) { board[m.To] = board[m.From]; board[m.From] = new PieceStruct(0, ' '); return board; }
    Span<PieceStruct> RedoMoveStruct(Span<PieceStruct> board, in PieceMoveStruct m) { board[m.From] = board[m.To]; board[m.From] = new PieceStruct(0, ' '); return board; }
    int EvalStruct(ReadOnlySpan<PieceStruct> board)
    {
        var sum = 0;
        for (int i = 0; i < evalPieces; i++) sum += board[i].Value;
        return sum;
    }

    public int SearchStruct(Span<PieceStruct> board, int depth)
    {
        if (depth < 0) return EvalStruct(board);
        depth--;

        Span<PieceMoveStruct> pieceMoves = stackalloc PieceMoveStruct[100];
        var legalMoves = FindMovesStruct(board, pieceMoves);
        var maxEval = int.MinValue;

        foreach (var move in legalMoves)
        {
            var boardAfterMove = MakeMoveStruct(board, move);
            var moveEval = SearchStruct(boardAfterMove, depth);
            maxEval = moveEval > maxEval ? moveEval : maxEval;
            board = RedoMoveStruct(boardAfterMove, move);
        }
        return maxEval;
    }


    [Benchmark(Baseline = true)]
    public int EvalStructBench()
    {
        Span<PieceStruct> boardStruct = stackalloc PieceStruct[64];

        for (int i = 0; i < boardStruct.Length; i++) boardStruct[i] = new PieceStruct(i, 'p');

        return SearchStruct(boardStruct, targetDepth);
    }

}
