using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterLineRenderer
{
	public class BetterLineRenderer : MonoBehaviour
	{
		protected string pathName;
		protected Vector3[] path;
		protected float duration;
		protected Color color;

		private Queue<GraphKeyFrame> allSpawnPositions;
		private float unitDist;

		private float travelledDistance;

		[SerializeField]
		private GameObject nodePrefab, unitPrefab;
		[SerializeField]
		private GameObject pathCursor;

		private Vector3 prevTravelledPos;

		protected float pathLength;
		protected float timePerSeg;
		private float distanceDelta;

		private float[] segLengths;
		private float[] timePerSegment;
		private int[] nodesPerSegment;

		public virtual void SetupPath(string pName, Vector3[] pathNodes, float drawDuration, float distBetweenUnits, Color c)
		{
			pathName = pName;
			path = pathNodes;
			duration = drawDuration;
			color = c;
			unitDist = distBetweenUnits;


			pathLength = SetPathLength();

			segLengths = GetSegmentLengths();
			timePerSegment = GetSegmentTimes(segLengths);

			SetSpawnPositions();
		}

		private void SetSpawnPositions()
		{
			allSpawnPositions = new Queue<GraphKeyFrame>(Mathf.RoundToInt(pathLength / unitDist + path.Length));
			
			nodesPerSegment = new int[segLengths.Length];

			// Get node amount per segment.
			for (int i = 0; i < segLengths.Length; i++)
			{
				nodesPerSegment[i] = Mathf.FloorToInt(segLengths[i] / unitDist);
			}

			float t;
			allSpawnPositions.Enqueue(new GraphKeyFrame(0, nodePrefab, path[0]));
			for (int i = 1; i < path.Length; i++)
			{
				for (int j = 0; j < nodesPerSegment[i]; j++)
				{
					t = GetTimeForUnit(i, j);
					allSpawnPositions.Enqueue(new GraphKeyFrame(t, unitPrefab, GetPosForUnit(i,j)));

					if(j == nodesPerSegment[i] - 1)
						allSpawnPositions.Enqueue(new GraphKeyFrame(t, nodePrefab, path[i]));
				}
				//float t2 = GetTimeForNode(i);
			}

		}

		private float GetTimeForUnit(int currentIndexI, int currentIndexJ)
		{
			float num = 0;
			num = Mathf.Lerp(GetTimeSumAtIndex(currentIndexI - 1), GetTimeSumAtIndex(currentIndexI), currentIndexJ / (float)nodesPerSegment[currentIndexI]);
			

			return num;
		}

		private float GetTimeSumAtIndex(int index)
		{
			float num = 0;
			for (int i = 0; i < index+1; i++)
			{
				num += timePerSegment[i];
			}
			return num;
		}

		private float GetTimeForNode(int currentIndex)
		{
			float num = 0;
			for (int i = 1; i < currentIndex; i++)
				num += timePerSegment[i];
			return num;
		}

		private Vector3 GetPosForUnit(int currentIndexI, int currentIndexJ)
		{
			Vector3 returnVal = path[currentIndexI];
			float divValue = nodesPerSegment[currentIndexI];
			float val = currentIndexJ / divValue;
			Debug.Log(val);
			returnVal = Vector3.Lerp(path[currentIndexI - 1], path[currentIndexI], val);
			return returnVal;
		}

		private float[] GetSegmentTimes(float[] segmentLengths)
		{
			float[] segmentTimes = new float[segmentLengths.Length];

			for (int i = 0; i < segmentLengths.Length; i++)
			{
				segmentTimes[i] = (segmentLengths[i] / pathLength) * duration;
			}

			return segmentTimes;
		}

		public virtual void StartPath(float pathDuration)
		{
			//StartCoroutine(DrawPath(GetSegmentLengths(), 0.25f));
			StartCoroutine(DoPath());
		}

		private IEnumerator DoPath()
		{
			float localTime = 0;
			while (true)
			{
				yield return null;

				while (true)
				{
					if (allSpawnPositions.Count <= 0 || allSpawnPositions.Peek().placementTime > localTime)
						break;

					GraphKeyFrame frame = allSpawnPositions.Dequeue();
					GameObject go = Instantiate(frame.obj, frame.pos, Quaternion.identity);
					go.GetComponent<Renderer>().material.color = color;
					go.transform.parent = transform;
				}

				// Mandatory timing segment. Prevents infinite loops. Leave it.
				localTime += Time.deltaTime;
				if (localTime > duration)
					break;
			}
		}

		protected virtual IEnumerator DrawPath(float[] segmentDurations, float minDistForPlacement = 0)
		{
			Debug.Log("Drawing new path");
			for (int i = 1; i < path.Length; i++)
			{
				yield return StartCoroutine(DrawSegment(segmentDurations[i], i, minDistForPlacement));
				Debug.Log("Finished segment " + i + " of " + path.Length);
			}
			Debug.Log("Finished drawing path");
		}

		protected virtual IEnumerator DrawSegment(float segmentDuration, int currentSegment, float minDistForPlacement = 0)
		{
			float localTime = 0f;
			while (true)
			{
				yield return null;
				prevTravelledPos = pathCursor.transform.position;
				pathCursor.transform.position = Vector3.Lerp(path[currentSegment - 1], path[currentSegment], localTime / segmentDuration);

				// Segment length iterating code.
				travelledDistance += Vector3.Distance(prevTravelledPos, pathCursor.transform.position);
				distanceDelta += Vector3.Distance(prevTravelledPos, pathCursor.transform.position);
				if (minDistForPlacement > 0 && distanceDelta > minDistForPlacement)
					PlaceDistUnit(pathCursor.transform.position);

				// Mandatory timing segment. Prevents infinite loops. Leave it.
				localTime += Time.deltaTime;
				if (localTime > segmentDuration)
					break;
			}

			PlaceNode(pathCursor.transform.position);
		}

		private float SetPathLength()
		{
			float val = 0;

			for (int i = 1; i < path.Length; i++)
			{
				val += Vector3.Distance(path[i - 1], path[i]);
			}
			//timePerSegment[i] = pathLength / duration;
			return val;
		}

		private float[] GetSegmentLengths()
		{
			float[] returnVals = new float[path.Length];

			returnVals[0] = 0f;

			for (int i = 1; i < path.Length; i++)
			{
				returnVals[i] = Vector3.Distance(path[i - 1], path[i]);
			}

			return returnVals;
		}

		private void PlaceNode(Vector3 pos)
		{
			GameObject go = Instantiate(nodePrefab, pos, Quaternion.identity);
			go.GetComponent<Renderer>().material.color = color;
			go.transform.parent = transform;
		}

		protected virtual void PlaceDistUnit(Vector3 pos)
		{
			distanceDelta = 0;
			GameObject go = Instantiate(unitPrefab, pos, Quaternion.identity);
			go.GetComponent<Renderer>().material.color = color;
			go.transform.parent = transform;
		}
	}
}
