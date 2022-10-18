using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Assets.Scripts.Components;
using System.Linq;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MeshDraw3D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private MeshCollider drawArea;
    [SerializeField] private float minDistance;
    [SerializeField] private float height;
    [SerializeField] private GameObject spawnedObject;
    [SerializeField] private Material DrawMat;
    private GameObject drawing;
    private bool drawingStarted;


    private bool IsCursorInDrawArea
    {
        get
        {
            return drawArea.bounds.Contains(cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 11)));
        }
    }

    private IEnumerator Draw()
    {
        drawingStarted = true;
        drawing = new GameObject("Drawing");
        drawing.AddComponent<MeshFilter>();
        drawing.AddComponent<MeshRenderer>();
        drawing.GetComponent<MeshRenderer>().material = DrawMat;
        drawing.transform.localScale = new Vector3(1, 1, 0);
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>(new Vector3[8]);

        // Start draw position.
        Vector3 startPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Vector3 temp = new Vector3(startPosition.x, startPosition.y, 0.5f);
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = temp;

        }
        List<int> triangles = new List<int>(new int[36]);

        #region TriangleFaces
        // Front Face
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        // Top Face
        triangles[6] = 2;
        triangles[7] = 3;
        triangles[8] = 4;
        triangles[9] = 2;
        triangles[10] = 4;
        triangles[11] = 5;

        // Right Face
        triangles[12] = 1;
        triangles[13] = 2;
        triangles[14] = 5;
        triangles[15] = 1;
        triangles[16] = 5;
        triangles[17] = 6;

        // Left Face
        triangles[18] = 0;
        triangles[19] = 7;
        triangles[20] = 4;
        triangles[21] = 0;
        triangles[22] = 4;
        triangles[23] = 3;

        // Back Face
        triangles[24] = 5;
        triangles[25] = 4;
        triangles[26] = 7;
        triangles[27] = 5;
        triangles[28] = 7;
        triangles[29] = 6;

        // Bottom Face
        triangles[30] = 0;
        triangles[31] = 6;
        triangles[32] = 7;
        triangles[33] = 0;
        triangles[34] = 1;
        triangles[35] = 6;
        #endregion

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        drawing.GetComponent<MeshFilter>().mesh = mesh;
        Vector3 lastMousePosition = startPosition;

        while (IsCursorInDrawArea)
        {
            var mousePos = Input.mousePosition;
            float distance = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10)), lastMousePosition);



            if (distance > minDistance)
            {
                vertices.AddRange(new Vector3[4]);
                triangles.AddRange(new int[30]);

                int vIndex = vertices.Count - 8;
                // Previous Vertices Indices
                int vIndex0 = vIndex + 3;
                int vIndex1 = vIndex + 2;
                int vIndex2 = vIndex + 1;
                int vIndex3 = vIndex + 0;
                // New Vertices Indices
                int vIndex4 = vIndex + 4;
                int vIndex5 = vIndex + 5;
                int vIndex6 = vIndex + 6;
                int vIndex7 = vIndex + 7;


                Vector3 currentMousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                Vector3 mouseForwardVector = (currentMousePosition - lastMousePosition).normalized;

                Vector3 topRightVertex = currentMousePosition + Vector3.Cross(mouseForwardVector, Vector3.back) * height;
                Vector3 bottomRightVertex = currentMousePosition + Vector3.Cross(mouseForwardVector, Vector3.forward) * height;
                Vector3 topLeftVertex = new Vector3(topRightVertex.x, topRightVertex.y, 1);
                Vector3 bottomLeftVertex = new Vector3(bottomRightVertex.x, bottomRightVertex.y, 1);


                vertices[vIndex4] = topLeftVertex;
                vertices[vIndex5] = topRightVertex;
                vertices[vIndex6] = bottomRightVertex;
                vertices[vIndex7] = bottomLeftVertex;

                int tIndex = triangles.Count - 30;
                #region NewIndexes
                // New Top Face
                triangles[tIndex + 0] = vIndex2;
                triangles[tIndex + 1] = vIndex3;
                triangles[tIndex + 2] = vIndex4;
                triangles[tIndex + 3] = vIndex2;
                triangles[tIndex + 4] = vIndex4;
                triangles[tIndex + 5] = vIndex5;
                // New Right Face
                triangles[tIndex + 6] = vIndex1;
                triangles[tIndex + 7] = vIndex2;
                triangles[tIndex + 8] = vIndex5;
                triangles[tIndex + 9] = vIndex1;
                triangles[tIndex + 10] = vIndex5;
                triangles[tIndex + 11] = vIndex6;
                // New Left Face
                triangles[tIndex + 12] = vIndex0;
                triangles[tIndex + 13] = vIndex7;
                triangles[tIndex + 14] = vIndex4;
                triangles[tIndex + 15] = vIndex0;
                triangles[tIndex + 16] = vIndex4;
                triangles[tIndex + 17] = vIndex3;


                // New Back Face (No need, close the mesh at the end)
                //triangles[tIndex + 18] = vIndex5;
                //triangles[tIndex + 19] = vIndex4;
                //triangles[tIndex + 20] = vIndex7;
                //triangles[tIndex + 21] = vIndex5;
                //triangles[tIndex + 22] = vIndex7;
                //triangles[tIndex + 23] = vIndex6;


                // New Bottom Face
                triangles[tIndex + 24] = vIndex0;
                triangles[tIndex + 25] = vIndex6;
                triangles[tIndex + 26] = vIndex7;
                triangles[tIndex + 27] = vIndex0;
                triangles[tIndex + 28] = vIndex1;
                triangles[tIndex + 29] = vIndex6;

                #endregion

                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();
                lastMousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                yield return null;
            }

            distance = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10)), lastMousePosition);
            yield return null;
        }
        spawnedObject = drawing;

    }



    public void StartDraw(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (!IsCursorInDrawArea)
        {
            return;
        }

        StartCoroutine(Draw());

    }

    public void EndDraw(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (!drawingStarted)
        {

            return;
        }
        drawingStarted = false;
        StopAllCoroutines();
        Redraw();
        CalculateNormals();
        SpawnObject();
    }

    private void Redraw()
    {




        Mesh mesh = drawing.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        List<int> triangles = mesh.triangles.ToList();


        for (int i = 1; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices[i].x + (vertices[0].x * -1),
                vertices[i].y + (vertices[0].y * -1),
                vertices[i].z + (vertices[0].z * -1));
           

        }

        vertices[0] = Vector3.zero;

        drawing.GetComponent<MeshFilter>().mesh.vertices = vertices;

        //Close the end
        triangles[triangles.Count - 12] = vertices.ToList().Count - 3;
        triangles[triangles.Count - 11] = vertices.ToList().Count - 4;
        triangles[triangles.Count - 10] = vertices.ToList().Count - 1;
        triangles[triangles.Count - 9] = vertices.ToList().Count - 3;
        triangles[triangles.Count - 8] = vertices.ToList().Count - 1;
        triangles[triangles.Count - 7] = vertices.ToList().Count - 2;

        mesh.triangles = triangles.ToArray();
        mesh.vertices = vertices;


    }

    public void SpawnObject()
    {
        Mesh mesh = drawing.GetComponent<MeshFilter>().mesh;

        Mesh goMesh = new Mesh();
        goMesh.vertices = mesh.vertices;
        goMesh.triangles = mesh.triangles;
        goMesh.normals = mesh.normals;

        GameObject go = ObjectPoolManager.Instance.GetObject("SpawnedObject");
        go.transform.position = Vector3.zero;
        go.transform.GetComponent<MeshFilter>().mesh = goMesh;
        go.transform.GetComponent<MeshCollider>().sharedMesh = goMesh;

        go.SetActive(true);
        go.transform.position = new Vector3(6, 5, 15);
        Destroy(drawing);
    }

    private void CalculateNormals()
    {
        new MeshImporter(drawing).Import();
        ProBuilderMesh proMesh = drawing.GetComponent<ProBuilderMesh>();
        Normals.CalculateNormals(proMesh);
        proMesh.ToMesh();
        proMesh.Refresh();
    }
}



