using UnityEngine;

public class SmoothFilter : IHeightMapFilter{
	private static float Diff(float a, float b){
		return (b + (a - b) * 0.25f);
	}

	public static int ComputeSmoothedSize(int size){
		return (size << 1) - 2;
	}

	public static int ComputeStartSize(int endSize, int smoothings){
		for (int i = 0; i != smoothings; ++i){
			endSize = (endSize + 2) >> 1;
		}
		return endSize < 4 ? 4 : endSize;
	}

	public HeightMap Filter(HeightMap input){
		int size = input.Size;
		int newSize = ComputeSmoothedSize(size);
		Debug.Log ("Size: " + newSize);
		HeightMap output = new HeightMap(newSize);
		int last = size - 1, offset = 0;
		output [0] = input [0];
		for (int x = 1; x != last; ++x){
			output[++offset] = Diff(input [x - 1], input [x]);
			output[++offset] = Diff(input [x + 1], input [x]);
		}
		output[++offset] = input[last];
		for (int y = 1; y != last; ++y){
			int lastRow = y * size;
			int b = lastRow - size;
			float lastA = input [b];
			b += size;
			float currentA = input [b];
			b += size;
			float nextA = input [b];
			output[++offset] = Diff(lastA, currentA);
			output [offset + newSize] = Diff(nextA, currentA);
			lastRow -= size;
			++offset;
			for (int x = 1; x != last; ++x, offset+=2){
				b = lastRow + x;
				lastA = Diff(input [b - 1], input [b]);
				float lastB = Diff(input [b + 1], input [b]);
				b += size;
				currentA = Diff(input [b - 1], input [b]);
				float currentB = Diff(input [b + 1], input [b]);
				b += size;
				nextA = Diff(input [b - 1], input [b]);
				float nextB = Diff(input [b + 1], input [b]);
				int nextOffset = offset + newSize;
				output[offset] = Diff(lastA, currentA);
				output [nextOffset] = Diff(nextA, currentA);
				output [offset + 1] = Diff(lastB, currentB);
				output [nextOffset + 1] = Diff(nextB, currentB);
			}
			b = lastRow + last;
			lastA = input [b];
			b += size;
			currentA = input [b];
			b += size;
			nextA = input [b];
			output[offset] = Diff(lastA, currentA);
			offset += newSize;
			output [offset] = Diff(nextA, currentA);

		}
		output [++offset] = input [last * size];
		int finalRow = last * size + 1;
		for (int x = 1; x != last; ++x, ++finalRow){
			output[++offset] = Diff(input [finalRow - 1], input [finalRow]);
			output[++offset] = Diff(input [finalRow + 1], input [finalRow]);
		}
		output[++offset] = input[finalRow];
		return output;
	}
}
