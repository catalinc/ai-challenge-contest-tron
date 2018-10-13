
using System;

public class AlphaBeta
{
    int MaxDepth;

    public AlphaBeta(int MaxDepth)
    {
        this.MaxDepth = MaxDepth;
    }

    public Move GetMove(Board board)
    {		
		return AB(MaxDepth-1, Board.LOSE, Board.WIN, board);
    }

    private Move AB(int depth, int low, int high, Board board)
    {
        if (depth == 0 || GameOver(board))
        {
           	board.CurrentPlayer.score = Score(board);
			return board.CurrentPlayer;
        }

        Move best = null; 

        foreach (Move current in board.GetValidMoves(board.CurrentPlayer))
        {
            board.Play(current);
            Move next = AB(depth - 1, -high, -low, board);
            board.Undo();
			
            if (best == null || -next.score > best.score)
            {
                low = -next.score;
                best = current;
                best.score = low;
            }

            if (low >= high)
            {
                return best;
            }
        }
        
       return best;
    }

	private bool GameOver(Board board)
	{
		return board.CountLiberties(board.CurrentPlayer) == 0 || board.IsCollision;
	}
	
    private int Score(Board board)
    {
        int playerLiberties = board.CountLiberties(board.CurrentPlayer);
        int opponentLiberties = board.CountLiberties(board.CurrentOpponent);

		if(board.IsCollision)
		{
			return Board.DRAW;
		}
        else if (playerLiberties == 0 && opponentLiberties > 0)
        {
			return Board.LOSE;
        }
        else if (opponentLiberties == 0 && playerLiberties > 0)
        {
        		return Board.WIN;
        }
        else if (opponentLiberties == 0 && playerLiberties == 0)
        {
            // both stuck
            return Board.DRAW;
        }
        else
        {
   			return board.CountVoronoiMapTerritory(board.CurrentPlayer, board.CurrentOpponent);
        }
	}
}
