
using System;
using System.Collections;
using System.Collections.Generic;

public class Board
{
    public const int WIN = 10000000;
    public const int LOSE = -WIN;
    public const int DRAW = 50;

    public const int NORTH = 1;
    public const int EAST = 2;
    public const int SOUTH = 3;
    public const int WEST = 4;

    public readonly int[] DIRECTIONS = new int[] {NORTH, EAST, SOUTH, WEST};

    public readonly int[][] DIRECTION_OFFSETS = new int[][]
    {
        // x, y
		new int[]{},		// 0
        new int[]{0, -1},   // NORTH
        new int[]{1, 0},    // EAST
        new int[]{0, 1},    // SOUTH
        new int[]{-1, 0}    // WEST
    };

    public int Height;
    public int Width;
    public bool[,] walls;

    public Move Me;
    public Move Opponent;
    public Stack<Move> MyHistory;
    public Stack<Move> OpponentHistory;

    public Board()
    {
        this.MyHistory = new Stack<Move>();
        this.OpponentHistory = new Stack<Move>();
    }

    public bool IsMyTurn
    {
        get { return MyHistory.Count == OpponentHistory.Count; }
    }

    public Move CurrentPlayer
    {
        get { return IsMyTurn ? Me : Opponent; }
    }

    public Move CurrentOpponent
    {
        get { return IsMyTurn ? Opponent : Me; }
    }
    
	public bool IsCollision
	{
		get { return CurrentPlayer.Equals(CurrentOpponent); }
	}
	
	public int MovesCount
	{
		get { return MyHistory.Count; }
	}
	
    public void Play(Move move)
    {
        if (IsMyTurn) 
        {
            MyHistory.Push(new Move(Me));
            Me = move;
        }
        else
        {
            OpponentHistory.Push(new Move(Opponent));
            Opponent = move;
        }
        
        walls[move.x, move.y] = true;
    }

    public void Undo()
    {
        if(IsMyTurn)
        {
            walls[Opponent.x, Opponent.y] = false;
            Opponent = OpponentHistory.Pop();
        }
        else
        {
            walls[Me.x, Me.y] = false;
            Me = MyHistory.Pop();
        }        
    }
    
    public List<Move> GetValidMoves(Move origin)
    {
        List<Move> moves = new List<Move>();
		int x, y;
		
		foreach (int direction in DIRECTIONS)
        {
            x = DIRECTION_OFFSETS[direction][0] + origin.x;
            y = DIRECTION_OFFSETS[direction][1] + origin.y;
            if (!(x <= 0
                    || y <= 0
                    || x >= Width
                    || y >= Height
                    || walls[x, y]) 
			    			|| (MovesCount % 2 == 1 && (x == CurrentOpponent.x && y == CurrentOpponent.y)))
            {
                moves.Add(new Move(x, y, direction));
            }
        }
		
        return moves;
    }

    public int CountLiberties(Move move)
    {
        int count = 0;
        int x, y;

        foreach (int direction in DIRECTIONS)
        {
            x = DIRECTION_OFFSETS[direction][0] + move.x;
            y = DIRECTION_OFFSETS[direction][1] + move.y;
            if (!(x <= 0
                    || y <= 0
                    || x >= Width
                    || y >= Height
                    || walls[x, y]) || 
			    			(MovesCount % 2 == 1 && (x == CurrentOpponent.x && y == CurrentOpponent.y)))
            {
                count++;
            }
        }
        
        return count;
    }

    public int CountVoronoiMapTerritory(Move red, Move blue)
    {
		Dictionary<Move, bool> visited = new Dictionary<Move, bool>();
        Stack<Move> open = new Stack<Move>();
        int count = 0;

        open.Push(red);
        while (open.Count > 0)
        {
            Move current = open.Pop();
            foreach (Move move in GetValidMoves(current))
            {
                if (!visited.ContainsKey(move))
                {
					visited[move] = true;
					if (Distance(move, red) <= Distance(move, blue))
                    {
                        count++;
                    }
                    open.Push(move);
                } 
            }
        }

        return count;
    }

    public int CountReachable(Move start)
    {
        List<int[]> visited = new List<int[]>();
        Stack<int[]> open = new Stack<int[]>();
        int count = 0;

        open.Push(new int[] { start.x, start.y });

        while (open.Count > 0)
        {
            int[] current = open.Pop();
            foreach(int[] next in GetValidMoves(current[0], current[1]))
            {
                count++;
                walls[next[0], next[1]] = true;
                open.Push(next);
                visited.Add(next);
            }
        }
    
        foreach(int[] move in visited)
        {
            walls[move[0], move[1]] = false;
        }
        
        return count;
    }

    List<int[]> GetValidMoves(int xStart, int yStart)
    {
        List<int[]> moves = new List<int[]>();
		int x, y;
        
		foreach (int direction in DIRECTIONS)
        {
            x = DIRECTION_OFFSETS[direction][0] + xStart;
            y = DIRECTION_OFFSETS[direction][1] + yStart;
            if (!(x <= 0
                    || y <= 0
                    || x >= Width
                    || y >= Height
                    || walls[x, y]))
            {
                moves.Add(new int[]{ x, y });
            }
        }

        return moves;
    }
	
    public int Distance(Move red, Move blue)
    {
        return Abs(red.x - blue.x) + Abs(red.y - blue.y);
    }

	private int Abs(int a) 
	{
		return a < 0 ? -a : a;
	}
	
    public void Initialize()
    {
        string firstLine = "";
        try
        {
            int c;
            while ((c = Console.Read()) >= 0)
            {
                if (c == '\n')
                {
                    break;
                }
                firstLine += (char)c;
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Could not read from stdin. Reason: " + e.Message);
            Environment.Exit(1);
        }

        firstLine = firstLine.Trim();
        if (firstLine.Equals("") || firstLine.Equals("exit"))
        {
            Environment.Exit(1); // If we get EOF or "exit" instead of numbers
            // on the first line, just exit. Game is over.
        }
        string[] tokens = firstLine.Split(' ');
        if (tokens.Length != 2)
        {
            Console.Error.WriteLine("FATAL ERROR: the first line of input should " +
                                    "be two integers separated by a space. " +
                                    "Instead, got: " + firstLine);
            Environment.Exit(1);
        }

        try
        {
            Width = Convert.ToInt32(tokens[0]);
            Height = Convert.ToInt32(tokens[1]);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("FATAL ERROR: invalid map dimensions: " +
                                    firstLine + ". Reason: " + e.Message);
            Environment.Exit(1);
        }
        walls = new bool[Width,Height];
        bool foundMyLocation = false;
        bool foundHisLocation = false;
        int numSpacesRead = 0;
        int x = 0, y = 0;
        while (y < Height)
        {
            int c = 0;
            try
            {
                c = Console.Read();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("FATAL ERROR: exception while reading " +
                                        "from stdin. Reason: " + e.Message);
                Environment.Exit(1);
            }
            if (c < 0)
            {
                break;
            }
            switch (c)
            {
            case '\n':
                if (x != Width)
                {
                    Console.Error.WriteLine("Invalid line length: " + x + "(line " + y + ")");
                    Environment.Exit(1);
                }
                ++y;
                x = 0;
                continue;
            case '\r':
                continue;
            case ' ':
                walls[x,y] = false;
                break;
            case '#':
                walls[x,y] = true;
                break;
            case '1':
                if (foundMyLocation)
                {
                    Console.Error.WriteLine("FATAL ERROR: found two locations " +
                                            "for player " +
                                            "1 in the map! First location is (" +
                                            Me.x + "," +
                                            Me.y +
                                            "), second location is (" + x + "," +
                                            y + ").");
                    Environment.Exit(1);
                }
                walls[x,y] = true;
                Me = new Move(x, y);
                foundMyLocation = true;
                break;
            case '2':
                if (foundHisLocation)
                {
                    Console.Error.WriteLine("FATAL ERROR: found two locations for player " +
                                            "2 in the map! First location is (" +
                                            Opponent.x + "," +
                                            Opponent.y + "), second location " +
                                            "is (" + x + "," + y + ").");
                    Environment.Exit(1);
                }
                walls[x,y] = true;
                Opponent = new Move(x, y);
                foundHisLocation = true;
                break;
            default:
                Console.Error.WriteLine("FATAL ERROR: invalid character received. " +
                                        "ASCII value = " + c);
                Environment.Exit(1);
                break;
            }
            ++x;
            ++numSpacesRead;
        }
        if (numSpacesRead != Width * Height)
        {
            Console.Error.WriteLine("FATAL ERROR: wrong number of spaces in the map. " +
                                    "Should be " + (Width * Height) + ", but only found " +
                                    numSpacesRead + " spaces before end of stream.");
            Environment.Exit(1);
        }
        if (!foundMyLocation)
        {
            Console.Error.WriteLine("FATAL ERROR: did not find a location for player 1!");
            Environment.Exit(1);
        }
        if (!foundHisLocation)
        {
            Console.Error.WriteLine("FATAL ERROR: did not find a location for player 2!");
            Environment.Exit(1);
        }

        MyHistory.Clear();
        OpponentHistory.Clear();
    }
	
	public void MakeMove(Move move) 
	{
		Console.WriteLine(move.direction);
	}

}
