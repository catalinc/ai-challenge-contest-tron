using System;
using System.Collections.Generic;
using System.IO;

class MyTronBot {

	const bool DEBUG = false;
	const int MAX_DEPTH  = 9; 
	
	static AlphaBeta strategy = new AlphaBeta(MAX_DEPTH);
	static Board board = new Board();
   
   public static void Main()
   {
		if (DEBUG)
		{
			Console.SetIn(new StreamReader("test.map"));
		}
		
		while(true)
		{
			board.Initialize();
			board.MakeMove(strategy.GetMove(board));
		}
   }
}
       
