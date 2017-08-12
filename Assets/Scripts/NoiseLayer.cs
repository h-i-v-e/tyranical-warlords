using System;
using UnityEngine;

public struct NoiseLayer
{
	public float coherence;

	public NoiseLayer (float coherence)
	{
		this.coherence = coherence;
	}

	private static Vector3 Diff(Vector3 a, Vector3 b, Vector3 scaleShift){
		return (b + (a - b) * 0.25f) + scaleShift;
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

	public Vector3[] Smooth(Vector3[] input, int size){
		int newSize = ComputeSmoothedSize(size);
		float scaleShiftStep = (1f - newSize / ((size - 1) * 2f)) / (newSize - 1);
		Vector3 scaleShift = new Vector3 (scaleShiftStep, 0f, 0f);
		Vector3[] output = new Vector3[newSize * newSize];
		int last = size - 1, offset = 0;
		output [0] = input [0];
		for (int x = 1; x != last; ++x, scaleShift.x += scaleShiftStep){
			output[++offset] = Diff(input [x - 1], input [x], scaleShift);
			output[++offset] = Diff(input [x + 1], input [x], scaleShift);
		}
		output[++offset] = input[last] + scaleShift;
		scaleShift.z = scaleShiftStep;
		for (int y = 1; y != last; ++y, scaleShift.z += scaleShiftStep){
			int lastRow = y * size;
			scaleShift.x = 0f;
			int b = lastRow - size;
			Vector3 lastA = input [b];
			b += size;
			Vector3 currentA = input [b];
			b += size;
			Vector3 nextA = input [b];
			output[++offset] = Diff(lastA, currentA, scaleShift);
			output [offset + newSize] = Diff(nextA, currentA, scaleShift);
			scaleShift.x = scaleShiftStep;
			lastRow -= size;
			++offset;
			for (int x = 1; x != last; ++x, offset+=2, scaleShift.x += scaleShiftStep){
				b = lastRow + x;
				lastA = Diff(input [b - 1], input [b], scaleShift);
				Vector3 lastB = Diff(input [b + 1], input [b], scaleShift);
				b += size;
				currentA = Diff(input [b - 1], input [b], scaleShift);
				Vector3 currentB = Diff(input [b + 1], input [b], scaleShift);
				b += size;
				nextA = Diff(input [b - 1], input [b], scaleShift);
				Vector3 nextB = Diff(input [b + 1], input [b], scaleShift);
				int nextOffset = offset + newSize;
				output[offset] = Diff(lastA, currentA, scaleShift);
				output [nextOffset] = Diff(nextA, currentA, scaleShift);
				output [offset + 1] = Diff(lastB, currentB, scaleShift);
				output [nextOffset + 1] = Diff(nextB, currentB, scaleShift);
			}
			b = lastRow + last;
			lastA = input [b];
			b += size;
			currentA = input [b];
			b += size;
			nextA = input [b];
			output[offset] = Diff(lastA, currentA, scaleShift);
			offset += newSize;
			output [offset] = Diff(nextA, currentA, scaleShift);

		}
		scaleShift.x = 0f;
		output [++offset] = input [last * size] + scaleShift;
		int finalRow = last * size + 1;
		scaleShift.x = scaleShiftStep;
		for (int x = 1; x != last; ++x, ++finalRow, scaleShift.x += scaleShiftStep){
			output[++offset] = Diff(input [finalRow - 1], input [finalRow], scaleShift);
			output[++offset] = Diff(input [finalRow + 1], input [finalRow], scaleShift);
		}
		output[++offset] = input[finalRow] + scaleShift;
		return output;
	}

	public void Apply(Vector3[] vertices, int size, float height){
		float offsetX = UnityEngine.Random.value * 4000000f;
		float offsetY = UnityEngine.Random.value * 4000000f;
		float zScale = 1f / size;
		for (int y = 0, offset = 0; y != size; ++y, offset += size) {
			for (int x = 0; x != size; ++x) {
				vertices [offset + x].y += Mathf.PerlinNoise (offsetX + x * coherence, offsetY + y * coherence) * zScale * height;
			}
		}
	}
}

