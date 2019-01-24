using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterLineRenderer
{
	public class DebugLineFactory : BetterLineRendererFactory
	{

		public bool is3D = true;
		public float drawTime = 5f, distBetweenDots = 0.2f;
		public int pathLength = 10;
		public Color pathcolor;

		[ContextMenu("Draw test path")]
		public void DrawTestPath()
		{
			string n = "testPath" + pathDictionary.Count;
			pathDictionary.Add(n, StartNewPath("testPath" + pathDictionary.Count, GenerateRandomPath(pathLength), drawTime, distBetweenDots, new ColorPathPair(pathcolor, LinePathType.line)));
			pathDictionary[n].StartPath(drawTime);

		}

		[ContextMenu("Draw test 2path")]
		public void DrawTestPath2()
		{
			LinePathType l;
			Vector3[] gennedPath = GenerateRandomPath(pathLength);
			for (int i = 0; i < 2; i++)
			{
				string n = "testPath" + pathDictionary.Count;
				if (i == 0)
				{
					l = LinePathType.dashed;
					pathDictionary.Add(n, StartNewPath("testPath" + pathDictionary.Count, gennedPath, drawTime, distBetweenDots, new ColorPathPair(Color.green, l)));
					pathDictionary[n].StartPath(drawTime);
				}
				else
				{
					l = LinePathType.markedCorners;
					pathDictionary.Add(n, StartNewPath("testPath" + pathDictionary.Count, gennedPath, drawTime, new ColorPathPair(Color.red, l)));
					pathDictionary[n].StartPath(drawTime);
				}
			}
		}

		private Vector3[] GenerateRandomPath(int pointAmt)
		{
			Vector3[] returnVals = new Vector3[pointAmt];

			returnVals[0] = Vector3.zero;

			for (int i = 1; i < pointAmt; i++)
			{
				returnVals[i] = returnVals[i - 1] + new Vector3(Random.Range(0f, 3f), Random.Range(0f, 3f), (is3D) ? Random.Range(0f, 3f) : 0);
			}

			return returnVals;
		}
	}
}