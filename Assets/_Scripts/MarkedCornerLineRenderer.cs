using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BetterLineRenderer
{
	public class MarkedCornerLineRenderer : BetterLineRenderer
	{
		[SerializeField]
		private GameObject cornerPrefab;

		protected override void SetSpawnPositions()
		{
			initialised = true;
			allSpawnPositions = new Queue<GraphKeyFrame>(Mathf.RoundToInt(path.Length));
			SetCorners();
		}

		private void SetCorners()
		{
			float t;
			allSpawnPositions.Enqueue(new GraphKeyFrame(0, cornerPrefab, path[0]));
			for(int i = 1; i < path.Length; i++)
			{
				t = GetTimeSumAtIndex(i);
				allSpawnPositions.Enqueue(new GraphKeyFrame(t, cornerPrefab, path[i]));
			}
		}
	}
}