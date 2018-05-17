// Version 2.3.3
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Diagnostics;
using UnityEngine;

namespace PrimitivesPro.Primitives
{
    /// <summary>
    /// class for creating Helix primitive
    /// </summary>
    public class HelixPrimitive : Primitive
    {
        /// <summary>
        /// generate mesh geometry for Helix
        /// </summary>
        /// <param name="mesh">mesh to be generated</param>
        /// <param name="radius0">fist radius of tube</param>
        /// <param name="radius1">second radius of tube</param>
        /// <param name="height">height of tube</param>
        /// <param name="sides">number of triangle segments in radius</param>
        /// <param name="heightSegments">number of triangle segments in height</param>
        /// <param name="slice">slicing parameter</param>
        /// <param name="radialMapping">uv radial mapping for top/down of the Helix</param>
        /// <param name="normalsType">type of normals to be generated</param>
        /// <param name="pivotPosition">position of the model pivot</param>
        public static float GenerateGeometry(Mesh mesh, float radius0, float radius1, float height, int sides, int heightSegments, float slice, float angleratio, bool radialMapping, NormalsType normalsType, PivotPosition pivotPosition)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            radius0 = Mathf.Clamp(radius0, 0, 100);
            radius1 = Mathf.Clamp(radius1, 0, 100);
            height = Mathf.Clamp(height, 0, 1000);
            sides = Mathf.Clamp(sides, 3, 1000);
            heightSegments = Mathf.Clamp(heightSegments, 1, 100);

            angleratio = angleratio / 100.0f;

            mesh.Clear();

            heightSegments = Mathf.Clamp(heightSegments, 1, 250);
            sides = Mathf.Clamp(sides, 3, 250);

            int sidesPipe = sides;

            if (sidesPipe == 0)
            {
                sidesPipe = 1;
            }

            int numVertices = 0;
            int numTriangles = sidesPipe * 6 * heightSegments * 2;
            int numVerticesCaps = ((sidesPipe+1) * 2 * 2);
            int numTrianglesCaps = (sidesPipe) * 6 * 2;
            int numTrianglesPipeCaps = 0;
            int numVerticesPipeCaps = 0;

            bool hasCaps = angleratio > 0.0f || Mathf.Abs(slice) != 360.0f;

            if (hasCaps)
            {
                numTrianglesPipeCaps = 12;
                numVerticesPipeCaps = 8;
            }

            if (normalsType == NormalsType.Face)
            {
                numVertices = sidesPipe * (4 + (heightSegments - 1) * 2) * 2;
            }
            else
            {
                numVertices = (sidesPipe + 1) * (heightSegments + 1) * 2;
            }

            if (numVertices + numVerticesCaps > 60000)
            {
                UnityEngine.Debug.LogError("Too much vertices!");
                return 0.0f;
            }

            var vertices = new Vector3[numVertices + numVerticesCaps + numVerticesPipeCaps];
            var normals = new Vector3[numVertices + numVerticesCaps + numVerticesPipeCaps];
            var uvs = new Vector2[numVertices + numVerticesCaps + numVerticesPipeCaps];
            var triangles = new int[numTriangles + numTrianglesCaps + numTrianglesPipeCaps];

            var bottomCenter = Vector3.zero;
            var innerVertOffset = numVertices / 2;
            var innerTriOffset = numTriangles / 2;

            var circles = slice * Mathf.Deg2Rad / (Mathf.PI * 2.0f);
            var sideSize = circles / sides * 10;

            var normalSign = 1.0f;

//            UnityEngine.Debug.Log(circles + " " + sideSize);

            var heightStep = angleratio * sideSize;

            var indexer = new int[]
            {
                slice > 0 ? 0 : 2, 
                0, 
                slice > 0 ? 0 : -2
            };

            var pivotOffset = Vector3.zero;
            switch (pivotPosition)
            {
                case PivotPosition.Center: pivotOffset = new Vector3(0.0f, -height/2, 0.0f);
                    break;
                case PivotPosition.Top: pivotOffset = new Vector3(0.0f, -height, 0.0f);
                    break;
            }

            if (normalsType == NormalsType.Face)
            {
                var vertIndex = 0;
                var triIndex = 0;
                var heightRatio = height/heightSegments;

                var nextBottomCenter = bottomCenter;
                nextBottomCenter.y += heightStep;

                for (int i = 0; i < sidesPipe; i++)
                {
                    float angle0 = ((float)i / sides) * slice * Mathf.Deg2Rad;
                    var v0 = new Vector3(Mathf.Cos(angle0), 0.0f, Mathf.Sin(angle0)).normalized;

                    float angle1 = ((float)(i+1) / sides) * slice * Mathf.Deg2Rad;
                    var v1 = new Vector3(Mathf.Cos(angle1), 0.0f, Mathf.Sin(angle1)).normalized;

                    var currHeight = 0.0f;
                    var triVert = vertIndex;

                    var normal = (v0 + v1).normalized * normalSign;

                    for (int j = 0; j <= heightSegments; j++)
                    {
                        // generate vertices for outer side
                        vertices[vertIndex + 0] = bottomCenter + v0*radius1 + new Vector3(0, currHeight, 0) + pivotOffset;
                        vertices[vertIndex + 1] = nextBottomCenter + v1 * radius1 + new Vector3(0, currHeight, 0) + pivotOffset;

                        normals[vertIndex + 0] = normal;
                        normals[vertIndex + 1] = normal;

                        uvs[vertIndex + 0] = new Vector2((float)i / sides, (float)j/heightSegments);
                        uvs[vertIndex + 1] = new Vector2((float)(i + 1) / sides, (float)j/heightSegments);

                        // generate vertices for inner side
                        vertices[vertIndex + innerVertOffset + 0] = bottomCenter + v0 * radius0 + new Vector3(0, currHeight, 0) + pivotOffset;
                        vertices[vertIndex + innerVertOffset + 1] = nextBottomCenter + v1 * radius0 + new Vector3(0, currHeight, 0) + pivotOffset;

                        normals[vertIndex + innerVertOffset + 0] = -normal;
                        normals[vertIndex + innerVertOffset + 1] = -normal;

                        uvs[vertIndex + innerVertOffset + 0] = new Vector2((float)i / sides, (float)j / heightSegments);
                        uvs[vertIndex + innerVertOffset + 1] = new Vector2((float)(i + 1) / sides, (float)j / heightSegments);

                        vertIndex += 2;

                        currHeight += heightRatio;
                    }

                    for (int j = 0; j < heightSegments; j++)
                    {
                        // generate triangles for outer side
                        triangles[triIndex + 0 + indexer[0]] = triVert + 0;
                        triangles[triIndex + 1 + indexer[1]] = triVert + 2;
                        triangles[triIndex + 2 + indexer[2]] = triVert + 1;

                        triangles[triIndex + 3 + indexer[0]] = triVert + 2;
                        triangles[triIndex + 4 + indexer[1]] = triVert + 3;
                        triangles[triIndex + 5 + indexer[2]] = triVert + 1;

                        // generate triangles for inner side
                        triangles[triIndex + innerTriOffset + 0 + indexer[0]] = triVert + innerVertOffset + 1;
                        triangles[triIndex + innerTriOffset + 1 + indexer[1]] = triVert + innerVertOffset + 2;
                        triangles[triIndex + innerTriOffset + 2 + indexer[2]] = triVert + innerVertOffset + 0;

                        triangles[triIndex + innerTriOffset + 3 + indexer[0]] = triVert + innerVertOffset + 1;
                        triangles[triIndex + innerTriOffset + 4 + indexer[1]] = triVert + innerVertOffset + 3;
                        triangles[triIndex + innerTriOffset + 5 + indexer[2]] = triVert + innerVertOffset + 2;

                        triIndex += 6;
                        triVert += 2;
                    }

                    bottomCenter.y += heightStep;
                    nextBottomCenter.y += heightStep;
                }
            }
            else
            {
                var vertIndex = 0;
                var triIndex = 0;
                var triVert = 0;
                var heightRatio = height / heightSegments;

                for (int i = 0; i <= sidesPipe; i++)
                {
                    float angle0 = ((float)i / sides) * slice * Mathf.Deg2Rad;
                    var v0 = new Vector3(Mathf.Cos(angle0), 0, Mathf.Sin(angle0)).normalized;

                    var currHeight = 0.0f;

                    for (int j = 0; j <= heightSegments; j++)
                    {
                        // generate vertices for outer side
                        vertices[vertIndex + 0] = bottomCenter + v0 * radius1 + new Vector3(0, currHeight, 0) + pivotOffset;
                        normals[vertIndex + 0] = v0 * normalSign;
                        uvs[vertIndex + 0] = new Vector2((float)i / sides, (float)j / heightSegments);

                        // generate vertices for inner side
                        vertices[vertIndex + innerVertOffset + 0] = bottomCenter + v0 * radius0 + new Vector3(0, currHeight, 0) + pivotOffset;
                        normals[vertIndex + innerVertOffset + 0] = -v0 * normalSign;
                        uvs[vertIndex + innerVertOffset + 0] = new Vector2((float)i / sides, (float)j / heightSegments);

                        vertIndex += 1;
                        currHeight += heightRatio;
                    }

                    bottomCenter.y += heightStep;
                }



                for (int i=0; i<sidesPipe; i++)
                {
                    var triVertNext = (i+1)*(heightSegments+1);

                    for (int j=0; j<heightSegments; j++)
                    {
                        triangles[triIndex + 0 + indexer[0]] = triVert + 0;
                        triangles[triIndex + 1 + indexer[1]] = triVert + 1;
                        triangles[triIndex + 2 + indexer[2]] = triVertNext + 0;

                        triangles[triIndex + 3 + indexer[0]] = triVertNext + 0;
                        triangles[triIndex + 4 + indexer[1]] = triVert + 1;
                        triangles[triIndex + 5 + indexer[2]] = triVertNext + 1;

                        triangles[triIndex + innerTriOffset + 0 + indexer[0]] = triVertNext + innerVertOffset + 0;
                        triangles[triIndex + innerTriOffset + 1 + indexer[1]] = triVert + innerVertOffset + 1;
                        triangles[triIndex + innerTriOffset + 2 + indexer[2]] = triVert + innerVertOffset + 0;

                        triangles[triIndex + innerTriOffset + 3 + indexer[0]] = triVertNext + innerVertOffset + 1;
                        triangles[triIndex + innerTriOffset + 4 + indexer[1]] = triVert + innerVertOffset + 1;
                        triangles[triIndex + innerTriOffset + 5 + indexer[2]] = triVertNext + innerVertOffset + 0;

                        triIndex += 6;
                        triVert += 1;
                        triVertNext += 1;
                    }

                    triVert += 1;
                }
            }

            // generate caps
            {
                var vertIndex = numVertices;
                var triIndex = numTriangles;
                var triVert = vertIndex;
                var downVertOffset = numVerticesCaps/2;
                var downTriOffset = numTrianglesCaps/2;
                var downUVRatio = 0.5f*(radius0/radius1);

                bottomCenter = Vector3.zero;

                for (int i = 0; i <=sidesPipe; i++)
                {
                    float angle0 = ((float)i / sides) * slice * Mathf.Deg2Rad;
                    var v0 = new Vector3(Mathf.Cos(angle0), 0.0f, Mathf.Sin(angle0)).normalized;

                    // generate top caps
                    vertices[vertIndex + 0] = bottomCenter + v0 * radius0 + new Vector3(0, height, 0) + pivotOffset;
                    normals[vertIndex + 0] = new Vector3(0, 1, 0) * normalSign;
                    vertices[vertIndex + 1] = bottomCenter + v0 * radius1 + new Vector3(0, height, 0) + pivotOffset;
                    normals[vertIndex + 1] = new Vector3(0, 1, 0) * normalSign;

                    var uvV = new Vector2(v0.x*0.5f, v0.z*.5f);
                    var uvVInner = new Vector2(v0.x * downUVRatio, v0.z * downUVRatio);
                    var uvCenter = new Vector2(0.5f, 0.5f);

                    var ratio = (float)i / sides;

                    if (radialMapping)
                    {
                        uvs[vertIndex + 0] = new Vector2(ratio, 1.0f);
                        uvs[vertIndex + 1] = new Vector2(ratio, 0.0f);
                    }
                    else
                    {
                        uvs[vertIndex + 0] = uvCenter + uvVInner;
                        uvs[vertIndex + 1] = uvCenter + uvV;
                    }

                    // generate down caps
                    vertices[vertIndex + downVertOffset + 0] = bottomCenter + v0 * radius0 + pivotOffset;
                    normals[vertIndex + downVertOffset + 0] = new Vector3(0, -1, 0) * normalSign;
                    vertices[vertIndex + downVertOffset + 1] = bottomCenter + v0 * radius1 + pivotOffset;
                    normals[vertIndex + downVertOffset + 1] = new Vector3(0, -1, 0) * normalSign;

                    if (radialMapping)
                    {
                        uvs[vertIndex + downVertOffset + 0] = new Vector2(ratio, 1.0f);
                        uvs[vertIndex + downVertOffset + 1] = new Vector2(ratio, 0.0f);
                    }
                    else
                    {
                        uvs[vertIndex + downVertOffset + 0] = uvCenter + uvVInner;
                        uvs[vertIndex + downVertOffset + 1] = uvCenter + uvV;
                    }

                    vertIndex += 2;
                    bottomCenter.y += heightStep;
                }

                for (int i=0; i<sidesPipe; i++)
                {
                    var triVertNext = numVertices + (i + 1) * 2;
                    var triVertNextDown = numVertices + downVertOffset + (i + 1) * 2;

                    triangles[triIndex + 0 + indexer[0]] = triVertNext + 0;
                    triangles[triIndex + 1 + indexer[1]] = triVert + 1;
                    triangles[triIndex + 2 + indexer[2]] = triVert + 0;

                    triangles[triIndex + 3 + indexer[0]] = triVertNext + 1;
                    triangles[triIndex + 4 + indexer[1]] = triVert + 1;
                    triangles[triIndex + 5 + indexer[2]] = triVertNext + 0;

                    triangles[triIndex + downTriOffset + 0 + indexer[0]] = triVert + downVertOffset + 0;
                    triangles[triIndex + downTriOffset + 1 + indexer[1]] = triVert + downVertOffset + 1;
                    triangles[triIndex + downTriOffset + 2 + indexer[2]] = triVertNextDown + 0;

                    triangles[triIndex + downTriOffset + 3 + indexer[0]] = triVertNextDown + 0;
                    triangles[triIndex + downTriOffset + 4 + indexer[1]] = triVert + downVertOffset + 1;
                    triangles[triIndex + downTriOffset + 5 + indexer[2]] = triVertNextDown + 1;

                    triIndex += 6;
                    triVert += 2;
                }
            }

            // generate pipe caps
            if (hasCaps)
            {
                var triIndex = numTriangles + numTrianglesCaps;
                var vertIndex = numVertices + numVerticesCaps;

                // vertices
                if (normalsType == NormalsType.Vertex)
                {
                    vertices[vertIndex + 0] = vertices[0];
                    vertices[vertIndex + 1] = vertices[innerVertOffset];
                    vertices[vertIndex + 2] = vertices[heightSegments];
                    vertices[vertIndex + 3] = vertices[innerVertOffset + heightSegments];

                    vertices[vertIndex + 4] = vertices[(sidesPipe) * (heightSegments + 1) + heightSegments];
                    vertices[vertIndex + 5] = vertices[(sidesPipe) * (heightSegments+1) + innerVertOffset];
                    vertices[vertIndex + 6] = vertices[(sidesPipe) * (heightSegments + 1)];
                    vertices[vertIndex + 7] = vertices[(sidesPipe) * (heightSegments+1) + innerVertOffset + heightSegments];
                }
                else
                {
                    vertices[vertIndex + 0] = vertices[0];
                    vertices[vertIndex + 1] = vertices[innerVertOffset];
                    vertices[vertIndex + 2] = vertices[heightSegments * 2];
                    vertices[vertIndex + 3] = vertices[innerVertOffset + heightSegments * 2];

                    vertices[vertIndex + 4] = vertices[(sidesPipe - 1) * ((heightSegments + 1) * 2) + (heightSegments) * 2 + 1];
                    vertices[vertIndex + 5] = vertices[(sidesPipe - 1) * ((heightSegments + 1) * 2) + innerVertOffset + 1];
                    vertices[vertIndex + 6] = vertices[(sidesPipe - 1) * ((heightSegments + 1) * 2) + 1];
                    vertices[vertIndex + 7] = vertices[(sidesPipe - 1) * ((heightSegments + 1) * 2) + innerVertOffset + (heightSegments) * 2 + 1];
                }

                // triangles
                if (slice > 0)
                {
                    triangles[triIndex + 0] = vertIndex + 0;
                    triangles[triIndex + 1] = vertIndex + 1;
                    triangles[triIndex + 2] = vertIndex + 2;
                    triangles[triIndex + 3] = vertIndex + 3;
                    triangles[triIndex + 4] = vertIndex + 2;
                    triangles[triIndex + 5] = vertIndex + 1;

                    triangles[triIndex + 6] = vertIndex + 4;
                    triangles[triIndex + 7] = vertIndex + 5;
                    triangles[triIndex + 8] = vertIndex + 6;
                    triangles[triIndex + 9] = vertIndex + 7;
                    triangles[triIndex + 10] = vertIndex + 5;
                    triangles[triIndex + 11] = vertIndex + 4;
                }
                else
                {
                    triangles[triIndex + 5] = vertIndex + 0;
                    triangles[triIndex + 4] = vertIndex + 1;
                    triangles[triIndex + 3] = vertIndex + 2;
                    triangles[triIndex + 2] = vertIndex + 3;
                    triangles[triIndex + 1] = vertIndex + 2;
                    triangles[triIndex + 0] = vertIndex + 1;

                    triangles[triIndex + 11] = vertIndex + 4;
                    triangles[triIndex + 10] = vertIndex + 5;
                    triangles[triIndex + 9] = vertIndex + 6;
                    triangles[triIndex + 8] = vertIndex + 7;
                    triangles[triIndex + 7] = vertIndex + 5;
                    triangles[triIndex + 6] = vertIndex + 4;
                }

                // normals
                var normal0 = Vector3.Cross(vertices[vertIndex + 1] - vertices[vertIndex + 0], vertices[vertIndex + 2] - vertices[vertIndex + 0]);
                normals[vertIndex + 0] = normal0;
                normals[vertIndex + 1] = normal0;
                normals[vertIndex + 2] = normal0;
                normals[vertIndex + 3] = normal0;

                var normal1 = Vector3.Cross(vertices[vertIndex + 5] - vertices[vertIndex + 4], vertices[vertIndex + 6] - vertices[vertIndex + 4]);
                normals[vertIndex + 4] = normal1;
                normals[vertIndex + 5] = normal1;
                normals[vertIndex + 6] = normal1;
                normals[vertIndex + 7] = normal1;

                // uvs
                //                var uvInnerRatio = radius0/radius1;
                //                uvs[vertIndex + 0] = new Vector2(0.0f, uvInnerRatio);
                //                uvs[vertIndex + 1] = new Vector2(0.0f, 1.0f);
                //                uvs[vertIndex + 2] = new Vector2(1.0f, uvInnerRatio);
                //                uvs[vertIndex + 3] = new Vector2(1.0f, 1.0f);
                //
                //                uvs[vertIndex + 4] = new Vector2(0.0f, 0.0f);
                //                uvs[vertIndex + 5] = new Vector2(1.0f, 1.0f-uvInnerRatio);
                //                uvs[vertIndex + 6] = new Vector2(1.0f, 0.0f);
                //                uvs[vertIndex + 7] = new Vector2(0.0f, 1.0f-uvInnerRatio);

                // this code is responsible for uv mapping of the inside of tube slice cap
                // VERTICAL MAPPING FIX:
                uvs[vertIndex + 0] = new Vector2(1.0f, 1.0f);
                uvs[vertIndex + 1] = new Vector2(0.0f, 1.0f);
                uvs[vertIndex + 2] = new Vector2(1.0f, 0.0f);
                uvs[vertIndex + 3] = new Vector2(0.0f, 0.0f);

                uvs[vertIndex + 4] = new Vector2(1.0f, 1.0f);
                uvs[vertIndex + 5] = new Vector2(0.0f, 0.0f);
                uvs[vertIndex + 6] = new Vector2(1.0f, 0.0f);
                uvs[vertIndex + 7] = new Vector2(0.0f, 1.0f);
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            MeshUtils.CalculateTangents(mesh);
            ;

            stopWatch.Stop();
            return stopWatch.ElapsedMilliseconds;
        }
    }
}
