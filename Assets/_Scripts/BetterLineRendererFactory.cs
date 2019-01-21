using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterLineRenderer
{
    public class BetterLineRendererFactory : MonoBehaviour
    {
        [SerializeField]
        private BetterLineRenderer linePrefab, dottedPrefab, dashedPrefab, namedPointsPrefab;

        private Dictionary<string, BetterLineRenderer> pathDictionary = new Dictionary<string, BetterLineRenderer>();

		public bool is3D = true;
		public float drawTime = 5f, distBetweenDots = 0.2f;
		public int pathLength = 10;
		public Color pathcolor;

        [ContextMenu("Draw test path")]
        public void DrawTestPath()
        {
            string n = "testPath" + pathDictionary.Count;
            pathDictionary.Add(n, StartNewPath("testPath" + pathDictionary.Count, GenerateRandomPath(pathLength), drawTime, distBetweenDots, new ColorPathPair(pathcolor, LinePathType.dotted)));
            pathDictionary[n].StartPath(drawTime);
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
        private BetterLineRenderer StartNewPath(string pathName, Vector3[] pathNodes, float drawDuration, float dotDist, params ColorPathPair[] pathPairs)
        {
            BetterLineRenderer renderer = null;
            foreach (var pathPair in pathPairs)
            {
                switch (pathPair.pathType)
                {
                    case LinePathType.line:
                        renderer = Instantiate(linePrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
                        renderer.SetupPath(pathName, pathNodes, drawDuration, dotDist, pathPair.color);
                        break;
                    case LinePathType.dotted:
                        renderer = Instantiate(dottedPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
                        renderer.SetupPath(pathName, pathNodes, drawDuration, dotDist, pathPair.color);
                        break;
                    case LinePathType.dashed:
                        renderer = Instantiate(dashedPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
                        renderer.SetupPath(pathName, pathNodes, drawDuration, dotDist, pathPair.color);
                        break;
                    case LinePathType.namedPoints:
                        renderer = Instantiate(namedPointsPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
                        renderer.SetupPath(pathName, pathNodes, drawDuration, dotDist, pathPair.color);
                        break;
                    default:
                        break;
                }
            }

            return renderer;
        }
    }

    public enum LinePathType { line, dotted, dashed, namedPoints }

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

		public GraphKeyFrame(float time, GameObject go, Vector3 position)
		{
			placementTime = time;
			obj = go;
			pos = position;
		}

		public override string ToString()
		{
			string s = "Time: " + placementTime + " Pos: " + pos;
			return s;
		}
	}
}