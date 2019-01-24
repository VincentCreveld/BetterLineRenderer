using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterLineRenderer
{
    public class BetterLineRendererFactory : MonoBehaviour
    {
        [SerializeField]
		protected BetterLineRenderer linePrefab, dottedPrefab, dashedPrefab, namedPointsPrefab, markedCornerPrefab;

        protected Dictionary<string, BetterLineRenderer> pathDictionary = new Dictionary<string, BetterLineRenderer>();

		public static BetterLineRendererFactory instance;

		private void Awake()
		{
			if(instance == null)
			{
				instance = this;
			}else
			{
				Debug.LogError("Too many line factories in scene. Disabling BetterLineFactory instance on" + name);
			}
		}

		// Generates and draws a new line.
		public BetterLineRenderer StartNewPath(string pathName, Vector3[] pathNodes, Transform p, float drawDuration, ColorPathPair pathPair)
		{
			BetterLineRenderer renderer = null;
			switch(pathPair.pathType)
			{

				//case LinePathType.namedPoints:
				//	renderer = Instantiate(namedPointsPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
				//	renderer.SetupPath(pathName, pathNodes, p, drawDuration, pathPair.color);
				//	break;
				case LinePathType.markedCorners:
					renderer = Instantiate(markedCornerPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, p, drawDuration, pathPair.color);
					break;
				default:
					Debug.LogError("You're using the wrong factory function to draw that line. Try another function.");
					break;
			}
			return renderer;
		}

		public BetterLineRenderer StartNewPath(string pathName, Vector3[] pathNodes, Transform p, float drawDuration, float dotDist, ColorPathPair pathPair)
		{
			BetterLineRenderer renderer = null;
			switch(pathPair.pathType)
			{
				case LinePathType.dotted:
					renderer = Instantiate(dottedPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, p, drawDuration, pathPair.color, dotDist);
					break;
				case LinePathType.dashed:
					renderer = Instantiate(dashedPrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, p, drawDuration, pathPair.color, dotDist);
					break;
				case LinePathType.line:
					renderer = Instantiate(linePrefab.gameObject, pathNodes[0], Quaternion.identity).GetComponent<BetterLineRenderer>();
					renderer.SetupPath(pathName, pathNodes, p, drawDuration, pathPair.color, dotDist);
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