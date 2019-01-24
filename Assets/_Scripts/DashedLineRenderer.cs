using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterLineRenderer
{
	public class DashedLineRenderer : DottedLineRenderer
	{
		protected override void SetDots()
		{
			float t;
			for(int i = 1; i < path.Length; i++)
			{
				for(int j = 0; j < nodesPerSegment[i]; j++)
				{
					t = GetTimeForDot(i, j);
					allSpawnPositions.Enqueue(new GraphKeyFrame(t, dotPrefab, GetPosForDot(i, j), GetNodeRotation(i)));
				}
			}
		}

		private Quaternion GetNodeRotation(int currentIndex)
		{
			Quaternion q = new Quaternion();
			q = Quaternion.LookRotation((path[currentIndex - 1] - path[currentIndex]).normalized, Vector3.forward.normalized);
			//q = new Quaternion(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
			return q;
		}

		protected override void PlaceObjectFromQueue()
		{
			// This block draws the nodes at their appropriate location at the correct timing.
			GraphKeyFrame frame = allSpawnPositions.Dequeue();
			GameObject go = Instantiate(frame.obj, frame.pos, frame.rot, transform);
			go.GetComponentInChildren<Renderer>().material.color = color;
			go.transform.localScale = new Vector3(transform.parent.localScale.x * go.transform.localScale.x, transform.parent.localScale.y * go.transform.localScale.y, transform.parent.localScale.z * go.transform.localScale.z);
		}
	}
}