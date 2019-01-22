using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BetterLineRenderer
{
	public class DottedLineRenderer : BetterLineRenderer
	{
		[SerializeField]
		protected GameObject dotPrefab;
		private float unitDist;

		public override void SetupPath(string pName, Vector3[] pathNodes, float drawDuration, Color c, float distBetweenUnits = -1)
		{
			if(distBetweenUnits == -1)
				unitDist = 0;
			else
				unitDist = distBetweenUnits;

			base.SetupPath(pName, pathNodes, drawDuration, c, distBetweenUnits);
		}

		protected override void SetSpawnPositions()
		{
			initialised = true;
			allSpawnPositions = new Queue<GraphKeyFrame>(Mathf.RoundToInt(pathLength / unitDist));

			nodesPerSegment = new int[segLengths.Length];

			// Get node amount per segment.
			for(int i = 0; i < segLengths.Length; i++)
			{
				nodesPerSegment[i] = Mathf.FloorToInt(segLengths[i] / unitDist);
			}

			SetDots();
		}

		protected virtual void SetDots()
		{
			float t;
			for(int i = 1; i < path.Length; i++)
			{
				for(int j = 0; j < nodesPerSegment[i]; j++)
				{
					t = GetTimeForDot(i, j);
					allSpawnPositions.Enqueue(new GraphKeyFrame(t, dotPrefab, GetPosForDot(i, j)));
				}
			}
		}

		protected Vector3 GetPosForDot(int currentIndexI, int currentIndexJ)
		{
			Vector3 returnVal = path[currentIndexI];
			float divValue = nodesPerSegment[currentIndexI];
			float val = currentIndexJ / divValue;
			returnVal = Vector3.Lerp(path[currentIndexI - 1], path[currentIndexI], val);
			return returnVal;
		}

		protected float GetTimeForDot(int currentIndexI, int currentIndexJ)
		{
			float num = 0;
			num = Mathf.Lerp(GetTimeSumAtIndex(currentIndexI - 1), GetTimeSumAtIndex(currentIndexI), currentIndexJ / (float)nodesPerSegment[currentIndexI]);
			return num;
		}
	}
}