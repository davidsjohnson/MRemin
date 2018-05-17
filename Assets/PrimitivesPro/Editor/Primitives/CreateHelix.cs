// Version 2.3.3
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using PrimitivesPro.Editor;
using PrimitivesPro.Primitives;
using UnityEditor;

[CustomEditor(typeof(PrimitivesPro.GameObjects.Helix))]
public class CreateHelix : CreatePrimitive
{
    private bool useFlipNormals = false;

    [MenuItem(MenuDefinition.Helix)]
    public static void Create()
    {
        var obj = PrimitivesPro.GameObjects.Helix.Create(3, 4f, 1, 100, 1, 1600, 16, false, NormalsType.Vertex, PivotPosition.Center);
        obj.SaveStateAll();

        Selection.activeGameObject = obj.gameObject;
    }

    public override void OnInspectorGUI()
    {
        if (!base.IsValid())
        {
            return;
        }

        var obj = Selection.activeGameObject.GetComponent<PrimitivesPro.GameObjects.Helix>();

        if (target != obj)
        {
            return;
        }

        Utils.Toggle("Show scene handles", ref obj.showSceneHandles);
        bool colliderChange = Utils.MeshColliderSelection(obj);

        EditorGUILayout.Separator();

        useFlipNormals = obj.flipNormals;
        bool uiChange = false;

        uiChange |= Utils.SliderEdit("Inner radius", 0, 1000, ref obj.radius0);
        uiChange |= Utils.SliderEdit("Outer radius", 0, 1000, ref obj.radius1);
        uiChange |= Utils.SliderEdit("Height", 0, 1000, ref obj.height);

        if (obj.radius0 > obj.radius1)
        {
            obj.radius0 = obj.radius1;
        }

        EditorGUILayout.Separator();

        uiChange |= Utils.SliderEdit("Sides", 3, 1000, ref obj.sides);
        uiChange |= Utils.SliderEdit("Height segments", 1, 100, ref obj.heightSegments);
        uiChange |= Utils.SliderEdit("Slice", -10000, 10000, ref obj.slice);
        uiChange |= Utils.SliderEdit("Angle", -100, 100, ref obj.angleRatio);
        uiChange |= Utils.Toggle("Radial mapping", ref obj.radialMapping);

        EditorGUILayout.Separator();

        uiChange |= Utils.NormalsType(ref obj.normalsType);
        uiChange |= Utils.PivotPosition(ref obj.pivotPosition);

        uiChange |= Utils.Toggle("Flip normals", ref useFlipNormals);
        uiChange |= Utils.Toggle("Share material", ref obj.shareMaterial);
        uiChange |= Utils.Toggle("Fit collider", ref obj.fitColliderOnChange);

        Utils.StatWindow(Selection.activeGameObject);

        Utils.ShowButtons<PrimitivesPro.GameObjects.Helix>(this);

        if (uiChange || colliderChange)
        {
            if (obj.generationMode == 0 && !colliderChange)
            {
                obj.GenerateGeometry();

                if (useFlipNormals)
                {
                    obj.FlipNormals();
                }
            }
            else
            {
                obj.GenerateColliderGeometry();
            }
        }
    }
}
