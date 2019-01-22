using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterLineRenderer
{
	public class StraightLineRenderer : BetterLineRenderer
	{
		[SerializeField]
		private GameObject linePrefab;
		private float segmentResolution;

		private GameObject currentWorkedOnSegment;

		private Queue<GraphKeyFrameScale> scalesAtTimeQueue;

		private GameObject[] lineSegments;

		public override void SetupPath(string pName, Vector3[] pathNodes, float drawDuration, Color c, float distBetweenUnits = -1)
		{
			if(distBetweenUnits == -1)
				segmentResolution = 0;
			else
				segmentResolution = distBetweenUnits;

			lineSegments = new GameObject[pathNodes.Length];

			base.SetupPath(pName, pathNodes, drawDuration, c, distBetweenUnits);
		}

		protected override void SetSpawnPositions()
		{
			initialised = true;
			scalesAtTimeQueue = new Queue<GraphKeyFrameScale>(Mathf.FloorToInt(pathLength / segmentResolution));
			allSpawnPositions = new Queue<GraphKeyFrame>(path.Length);

			nodesPerSegment = new int[segLengths.Length];

			// Get node amount per segment.
			for(int i = 0; i < segLengths.Length; i++)
			{
				nodesPerSegment[i] = Mathf.FloorToInt(segLengths[i] / segmentResolution);
			}

			SetLineSpawnMoments();
			SetLineScalesAtTime();
		}

		private void SetLineSpawnMoments()
		{
			float t;
			allSpawnPositions.Enqueue(new GraphKeyFrame(0, linePrefab, path[0]));
			for(int i = 0; i < path.Length - 1; i++)
			{
				t = GetTimeSumAtIndex(i);
				allSpawnPositions.Enqueue(new GraphKeyFrame(t, linePrefab, path[i], GetNodeRotation(i)));
			}
		}

		private GameObject GetCurrentObject(int index)
		{
			return lineSegments[index];
		}

		protected virtual void SetLineScalesAtTime()
		{
			float t;
			for(int i = 1; i < path.Length; i++)
			{
				for(int j = 0; j < nodesPerSegment[i]; j++)
				{
					scalesAtTimeQueue.Enqueue(new GraphKeyFrameScale(GetTimeForAtPos(i, j), GetScaleAtPos(i, j), i));

					if(j == nodesPerSegment[i] - 1)
						scalesAtTimeQueue.Enqueue(new GraphKeyFrameScale(GetTimeForAtPos(i, j), segLengths[i], i));
				}

			}
		}

		private float GetScaleAtPos(int currentIndexI, int currentIndexJ)
		{
			float returnVal;

			float divValue = nodesPerSegment[currentIndexI];
			float val = currentIndexJ / divValue;
			returnVal = Mathf.Lerp(0, segLengths[currentIndexI], val);

			return returnVal;
		}

		private Quaternion GetNodeRotation(int currentIndex)
		{
			Quaternion q = new Quaternion();
			q = Quaternion.LookRotation((path[currentIndex + 1] - path[currentIndex]).normalized, Vector3.forward.normalized);
			//q = new Quaternion(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
			return q;
		}

		protected float GetTimeForAtPos(int currentIndexI, int currentIndexJ)
		{
			float num = 0;
			num = Mathf.Lerp(GetTimeSumAtIndex(currentIndexI - 1), GetTimeSumAtIndex(currentIndexI), currentIndexJ / (float)nodesPerSegment[currentIndexI]);
			return num;
		}

		protected override IEnumerator DoPath()
		{
			float localTime = 0;
			while(true)
			{
				yield return null;

				while(true)
				{
					if(allSpawnPositions.Count <= 0 || allSpawnPositions.Peek().placementTime > localTime)
						break;

					PlaceObjectFromQueue();
				}

				while(true)
				{
					if(scalesAtTimeQueue.Count <= 0 || scalesAtTimeQueue.Peek().placementTime > localTime)
						break;

					SetScaleAtTime();
				}

				// Mandatory timing segment. Prevents infinite loops. Leave it.
				localTime += Time.deltaTime;
				if(localTime > duration)
					break;
			}
		}

		protected virtual void PlaceObjectFromQueue()
		{
			// This block draws the nodes at their appropriate location at the correct timing.
			GraphKeyFrame frame = allSpawnPositions.Dequeue();
			currentWorkedOnSegment = Instantiate(frame.obj, frame.pos, frame.rot);
			currentWorkedOnSegment.GetComponentInChildren<Renderer>().material.color = color;
			currentWorkedOnSegment.transform.parent = transform;
		}

		private void SetScaleAtTime()
		{
			GraphKeyFrameScale frame = scalesAtTimeQueue.Dequeue();
			currentWorkedOnSegment.transform.localScale = new Vector3(currentWorkedOnSegment.transform.localScale.x, currentWorkedOnSegment.transform.localScale.y, frame.scale);
		}
	}
}