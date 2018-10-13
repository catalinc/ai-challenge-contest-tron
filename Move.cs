
using System;

public class Move : IEquatable<Move>
{

    public int x;
    public int y;
    public int direction;
    public int score;
    // used by A*
    public int depth;


    public Move() : this(0, 0, Board.NORTH, Board.LOSE)
    {
    }

    public Move(int x, int y) : this(x, y, Board.NORTH, Board.LOSE)
    {
    }

    public Move(int x, int y, int direction) : this(x, y, direction, Board.LOSE)
    {
    }

    public Move(int score) : this(0, 0, Board.NORTH, score)
    {
    }

    public Move(int x, int y, int direction, int score)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.score = score;
        this.depth = 0;
    }

    public Move(Move other)
    {
        this.x = other.x;
        this.y = other.y;
        this.direction = other.direction;
        this.score = other.score;
    }

	public bool Equals(Move other)
	{
        if (other == null)
        {
            return false;
        }

        return x == other.x && y == other.y;
	}
	
    public override bool Equals (object obj)
    {
        if (obj == null)
        {
            return false;
        }

        Move other = obj as Move;
		
		if(other == null)
		{
			return false;
		}
		

        return x == other.x && y == other.y;
    }

	public override int GetHashCode() 
	{
		return x*31 + y;
	}	
	
    public override string ToString ()
    {
        return string.Format("Move[x=" + x + ", y=" + y 
		                     + ", direction=" + direction 
                             + ", score=" + score + "]");
    }

}
