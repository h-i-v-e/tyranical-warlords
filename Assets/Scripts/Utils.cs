using System;

public struct Coord{
	public int x, y;

	public Coord(int x, int y){
		this.x = x;
		this.y = y;
	}

	public void Set(int x, int y){
		this.x = x;
		this.y = y;
	}

	public static bool operator == (Coord a, Coord b){
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator != (Coord a, Coord b){
		return a.x != b.x && a.y != b.y;
	}

	public override bool Equals(Object a){
		return a is Coord && this == (Coord)a;
	}

	public override int GetHashCode(){
		return x.GetHashCode () ^ y.GetHashCode();
	}

	public int Offset(int size){
		return y * size + x;
	}

	public override string ToString ()
	{
		return "[Coord] (" + x + ", " + y + ")";
	}
}