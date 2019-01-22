using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterLineRenderer
{
    public class BetterLineRendererFactory : MonoBehaviour
    {
        [SerializeField]
        private BetterLineRenderer linePrefab, dottedPrefab, dashedPrefab, namedPointsPrefab, markedCornerPrefab;

        private Dictionary<string, BetterLineRenderer> pathDictionary = new Dictionary<string, BetterLineRenderer>();


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
			for(int i = 0; i < 2; i++)
			{
				string n = "testPath" + pathDictionary.Count;
				if(i == 0)
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

		// Generates and draws a new line.
		private BetterLineRenderer StartNewPath(string pathName, Vector3[] pathNodes, float drawDuration, ColorPathPair pathPair)
		{
			BetterLineRenderer renderer = null;
			switch(pathPair.pathType)
			{
				
				case LinePathType.namedPoints:
					renderer = Instantiate(namedPointsPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, drawDuration, pathPair.color);
					break;
				case LinePathType.markedCorners:
					renderer = Instantiate(markedCornerPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, drawDuration, pathPair.color);
					break;
				default:
					Debug.LogError("You're using the wrong factory function to draw that line. Try another function.");
					break;
			}
			return renderer;
		}

		private BetterLineRenderer StartNewPath(string pathName, Vector3[] pathNodes, float drawDuration, float dotDist, ColorPathPair pathPair)
		{
			BetterLineRenderer renderer = null;
			switch(pathPair.pathType)
			{
				case LinePathType.dotted:
					renderer = Instantiate(dottedPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, drawDuration, pathPair.color, dotDist);
					break;
				case LinePathType.dashed:
					renderer = Instantiate(dashedPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, drawDuration, pathPair.color, dotDist);
					break;
				case LinePathType.line:
					renderer = Instantiate(linePrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, drawDuration, pathPair.color, dotDist);
					break;
				default:
					Debug.LogError("You're using the wrong factory function to draw that line. Try another function.");
					break;
			}
			return renderer;
		}
	}

	public enum LinePathType { line, dotted, dashed, namedPoints, markedCorners }

    public struct ColorPathPair
    {
        public Color color;
        public LinePathType pathType;

        public ColorPathPair(Color c, LinePathType p)
        {
            color = c;
            pathType = p;
        }
    }

	public class GraphKeyFrame
	{
		public float placementTime;
		public GameObject obj;
		public Vector3 pos;
		public Quaternion rot;

		public GraphKeyFrame(float time, GameObject go, Vector3 position, Quaternion rotation = new Quaternion())
		{
			placementTime = time;
			obj = go;
			pos = position;
			rot = rotation;
		}

		public override string ToString()
		{
			string s = "Time: " + placementTime + " Pos: " + pos;
			return s;
		}
	}

	public class GraphKeyFrameScale
	{
		public int index;
		public float placementTime;
		public float scale;

		public GraphKeyFrameScale(float time, float scaleAtTime, int _index)
		{
			placementTime = time;
			scale = scaleAtTime;
			index = _index;
		}

		public override string ToString()
		{
			string s = "Time: " + placementTime + " Scale: " + scale;
			return s;
		}
	}
}