using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterLineRenderer
{
	public class BetterLineRenderer : MonoBehaviour
	{
		protected string pathName;

		protected float duration;
		protected float distanceDelta;
		protected float pathLength;		

		protected Queue<GraphKeyFrame> allSpawnPositions;
		protected Color color;
		protected Vector3 prevTravelledPos;

		protected int[] nodesPerSegment;
		protected float[] segLengths;
		protected float[] timePerSegment;
		protected Vector3[] path;

		protected bool initialised = false;

		public virtual void SetupPath(string pName, Vector3[] pathNodes, float drawDuration, Color c, float distBetweenUnits = -1)
		{
			pathName = pName;
			path = pathNodes;
			duration = drawDuration;
			color = c;
			pathLength = SetPathLength();
			segLengths = GetSegmentLengths();
			timePerSegment = GetSegmentTimes(segLengths);
			SetSpawnPositions();
		}

		protected virtual void SetSpawnPositions() { }
		// Adds up all the previous time segments and returns the sum.
		

		// Begins the drawing of the line. Called from the factory.
		public virtual void StartPath(float pathDuration)
		{
			if(!initialised)
				return;
			StartCoroutine(DoPath());
		}

		// Loops over the queue and places objects where it is timed to appear.
		protected virtual IEnumerator DoPath()
		{
			float localTime = 0;
			while (true)
			{
				yield return null;

				while (true)
				{
					if(allSpawnPositions.Count <= 0 || allSpawnPositions.Peek().placementTime > localTime)
						break;

					PlaceObjectFromQueue();
				}

				// Mandatory timing segment. Prevents infinite loops. Leave it.
				localTime += Time.deltaTime;
				if (localTime > duration)
					break;
			}
		}

		protected virtual void PlaceObjectFromQueue()
		{
			// This block draws the nodes at their appropriate location at the correct timing.
			GraphKeyFrame frame = allSpawnPositions.Dequeue();
			GameObject go = Instantiate(frame.obj, frame.pos, frame.obj.transform.rotation);
			go.GetComponentInChildren<Renderer>().material.color = color;
			go.transform.parent = transform;
		}

		// Simple add function that makes a legible number out of all the segments.
		private float SetPathLength()
		{
			float val = 0;
			for(int i = 1; i < path.Length; i++)
			{
				val += Vector3.Distance(path[i - 1], path[i]);
			}
			return val;
		}

		// Calculates length per segment.
		private float[] GetSegmentLengths()
		{
			float[] returnVals = new float[path.Length];

			returnVals[0] = 0f;

			for(int i = 1; i < path.Length; i++)
			{
				returnVals[i] = Vector3.Distance(path[i - 1], path[i]);
			}

			return returnVals;
		}

		// Calculates allotted time per segment dependent on length.
		// Each segments gets the same % of the total time as the % it has of the total length.
		private float[] GetSegmentTimes(float[] segmentLengths)
		{
			float[] segmentTimes = new float[segmentLengths.Length];

			for(int i = 0; i < segmentLengths.Length; i++)
			{
				segmentTimes[i] = (segmentLengths[i] / pathLength) * duration;
			}

			return segmentTimes;
		}

		protected float GetTimeSumAtIndex(int index)
		{
			float sum = 0;
			for(int i = 1; i < index; i++)
			{
				sum += timePerSegment[i];
			}
			return sum;
		}

		protected float GetDistSumAtIndex(int index)
		{
			float sum = 0;
			for(int i = 1; i < index; i++)
			{
				sum += segLengths[i];
			}
			return sum;
		}

		#region CursorBasedRendering
		//protected virtual IEnumerator DrawPath(float[] segmentDurations, float minDistForPlacement = 0)
		//{
		//	Debug.Log("Drawing new path");
		//	for (int i = 1; i < path.Length; i++)
		//	{
		//		yield return StartCoroutine(DrawSegment(segmentDurations[i], i, minDistForPlacement));
		//		Debug.Log("Finished segment " + i + " of " + path.Length);
		//	}
		//	Debug.Log("Finished drawing path");
		//}

		//protected virtual IEnumerator DrawSegment(float segmentDuration, int currentSegment, float minDistForPlacement = 0)
		//{
		//	float localTime = 0f;
		//	while (true)
		//	{
		//		yield return null;
		//		prevTravelledPos = pathCursor.transform.position;
		//		pathCursor.transform.position = Vector3.Lerp(path[currentSegment - 1], path[currentSegment], localTime / segmentDuration);

		//		// Segment length iterating code.
		//		travelledDistance += Vector3.Distance(prevTravelledPos, pathCursor.transform.position);
		//		distanceDelta += Vector3.Distance(prevTravelledPos, pathCursor.transform.position);
		//		if (minDistForPlacement > 0 && distanceDelta > minDistForPlacement)
		//			PlaceDistUnit(pathCursor.transform.position);

		//		// Mandatory timing segment. Prevents infinite loops. Leave it.
		//		localTime += Time.deltaTime;
		//		if (localTime > segmentDuration)
		//			break;
		//	}

		//	PlaceNode(pathCursor.transform.position);
		//}



		//private void PlaceNode(Vector3 pos)
		//{
		//	GameObject go = Instantiate(nodePrefab, pos, Quaternion.identity);
		//	go.GetComponent<Renderer>().material.color = color;
		//	go.transform.parent = transform;
		//}

		//protected virtual void PlaceDistUnit(Vector3 pos)
		//{
		//	distanceDelta = 0;
		//	GameObject go = Instantiate(unitPrefab, pos, Quaternion.identity);
		//	go.GetComponent<Renderer>().material.color = color;
		//	go.transform.parent = transform;
		//}
		#endregion
	}
}
