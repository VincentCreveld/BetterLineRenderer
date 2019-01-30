using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDataVisualization : MonoBehaviour
{
	private GameObject terrain;
	public Material mat;
	public int resolution = 250;

	// Use this for initialization
	private void Start()
	{
		//Vector3[] verts = new Vector3[4];
		//Vector2[] uv = new Vector2[4];
		//int[] tris = new int[6];

		//verts[0] = new Vector3(0, 0, 1);
		//verts[1] = new Vector3(1, 0, 1);
		//verts[2] = new Vector3(0, 0, 0);
		//verts[3] = new Vector3(1, 0, 0);

		//uv[0] = new Vector2(0,1);
		//uv[0] = new Vector2(1,1);
		//uv[0] = new Vector2(0,0);
		//uv[0] = new Vector2(1,0);

		//tris[0] = 0;
		//tris[1] = 1;
		//tris[2] = 2;
		//tris[3] = 2;
		//tris[4] = 1;
		//tris[5] = 3;

		//Mesh mesh = new Mesh();

		//mesh.vertices = verts;
		//mesh.uv = uv;
		//mesh.triangles = tris;

		//terrain = new GameObject("DataTerrain", typeof(MeshRenderer), typeof(MeshFilter));
		//terrain.transform.localScale = new Vector3(30, 30, 30);

		//terrain.GetComponent<MeshFilter>().mesh = mesh;
		DrawFunc();
	}

	private void CreateMesh(Func<int,int,float> getY)
	{
		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();

		//Bottom left section of the map, other sections are similar
		for (int i = 0; i < resolution; i++)
		{
			for (int j = 0; j < resolution; j++)
			{
				//Add each new vertex in the plane
				verts.Add(new Vector3(i, getY(i, j), j));
				//Skip if a new square on the plane hasn't been formed
				if (i == 0 || j == 0) continue;
				//Adds the index of the three vertices in order to make up each of the two tris
				tris.Add(resolution * i + j); //Top right
				tris.Add(resolution * i + j - 1); //Bottom right
				tris.Add(resolution * (i - 1) + j - 1); //Bottom left - First triangle
				tris.Add(resolution * (i - 1) + j - 1); //Bottom left 
				tris.Add(resolution * (i - 1) + j); //Top left
				tris.Add(resolution * i + j); //Top right - Second triangle
			}
		}

		Vector2[] uvs = new Vector2[verts.Count];
		for (var i = 0; i < uvs.Length; i++) //Give UV coords X,Z world coords
			uvs[i] = new Vector2(verts[i].x, verts[i].z);

		terrain = new GameObject("ProcPlane"); //Create GO and add necessary components
		terrain.AddComponent<MeshFilter>();
		terrain.AddComponent<MeshRenderer>();
		Mesh procMesh = new Mesh();
		procMesh.vertices = verts.ToArray(); //Assign verts, uvs, and tris to the mesh
		procMesh.uv = uvs;
		procMesh.triangles = tris.ToArray();
		procMesh.RecalculateNormals(); //Determines which way the triangles are facing
		terrain.GetComponent<MeshFilter>().mesh = procMesh; //Assign Mesh object to MeshFilter
		terrain.GetComponent<Renderer>().material = mat;
	}

	private float Test(int x, int y)
	{
		return x * y * 0.01f;
	}

	private float Test2(int x, int y)
	{
		return (Mathf.Pow(x, 2) * (y / 3)) * 0.1f;
	}

	private void DrawFunc()
	{
		CreateMesh(Test2);
	}
}
