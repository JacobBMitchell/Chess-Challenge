using ChessChallenge.API;
using Raylib_cs;
using System;
using System.Linq;

public class MyBot : IChessBot
{

    // Remove bad moves Keep good moves
    public Move Think(Board board, Timer timer)
    {
        // Piece values: null, pawn, knight, bishop, rook, queen, king
        int[] pieceValues = { 0, 100, 300, 320, 500, 900, 90000 };
        PieceType[] pieceTypes = { PieceType.None, PieceType.Pawn, PieceType.Knight,PieceType.Bishop,PieceType.Rook,PieceType.Queen, PieceType.King};
        bool isWhite = board.IsWhiteToMove;

        //make move weight double array -> (e4, 50), (e5, 51) and get max points attributed to move

        Move[] legalMoves = board.GetLegalMoves();
        var movesAndScores = new Tuple<Move, int>[legalMoves.Length];
        for(int i = 0; i < legalMoves.Length; i++)
        {
            movesAndScores[i] = new Tuple<Move, int>(legalMoves[i], 0);
        }

        // Pick a move to play if nothing better is found
        Move moveToPlay = legalMoves[0];

        //EVALUATE BOARD POSITION

        int whiteValue = GetMaterial(true, board, pieceValues, pieceTypes);
        int blackValue = GetMaterial(false, board, pieceValues, pieceTypes);
        bool isWinning = (whiteValue > blackValue) == isWhite;

        foreach (var move_score in movesAndScores)
        {
            Move move = move_score.Item1;
            int score = move_score.Item2;
            // Always play checkmate in one CHECKS
            if (MoveIsCheckmate(board, move))
            {
                return move;
            }

            

            //King Safety
            //piece Difference
            


            //piece danger

            // Find highest value capture CAPTURES
            Piece capturedPiece = board.GetPiece(move.TargetSquare);
            int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];
            int attackingPieceValue = pieceValues[(int)board.GetPiece(move.StartSquare).PieceType];

            if (capturedPieceValue >= attackingPieceValue)
            {
                //moveToPlay = move;
                score += (capturedPieceValue - attackingPieceValue); 
            }
            if(!capturedPiece.IsNull || capturedPieceValue < attackingPieceValue)
            {
                // check for recapture
                //update board w/ move
                if (board.SquareIsAttackedByOpponent(move.TargetSquare))
                {
                    break;
                }
                //see if in available moves a capture for opponent is available
                else
                {
                    moveToPlay = move;
                }

                //if(recapture(board))
                //true no move
                //flase move
            }

            //ATTACKS


        }

        //Contingent on no checks captures attacks
        return moveToPlay;
    }

    //get value of board per side
    private static int GetMaterial(bool isWhite, Board board, int[] pieceValues, PieceType[] pieceTypes)
    {
        int material = 0; 
        //get score of all pieces -the king
        for (int i = 1; i < pieceValues.Length-1; i++)
        {
            PieceList pieces = board.GetPieceList(pieceTypes[i], isWhite);
            material += pieceValues[i]*pieces.Count;
            };
        return material;
    }

    // Test if this move gives checkmate
    private static bool  MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }

}