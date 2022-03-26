using System;
using Planet.Setting;
using UnityEditor;

namespace Planet
{
    [CustomEditor(typeof(PlanetMesh))]
    public class PlanetMeshEditor : Editor
    {
        private PlanetMesh _planetMesh;
        private Editor shapeEditor;

        private void OnEnable()
        {
            _planetMesh = target as PlanetMesh;
            ;
        }

        public override void OnInspectorGUI()
        {
            using (var check  = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (check.changed)
                {
                    _planetMesh.Generate();
                }
            }

            DrawSettingEditor(_planetMesh.ShapeSettting, _planetMesh.OnShapeSetttingUpdated,
                ref _planetMesh.shpaeSetttingsFoldOut, ref shapeEditor);
        }

        private void DrawSettingEditor(ShapeSettting planetMeshShapeSettting, Action onShapeSetttingUpdated, ref bool planetMeshShpaeSetttingsFoldOut, ref Editor editor)
        {
            if (planetMeshShapeSettting != null)
            {
                planetMeshShpaeSetttingsFoldOut =
                    EditorGUILayout.InspectorTitlebar(planetMeshShpaeSetttingsFoldOut, planetMeshShapeSettting);
                if (planetMeshShpaeSetttingsFoldOut)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        CreateCachedEditor(planetMeshShapeSettting, null, ref editor);
                        editor.OnInspectorGUI();
                        if (check.changed)
                        {
                            if (onShapeSetttingUpdated != null)
                            {
                                onShapeSetttingUpdated();
                            }
                        }
                    }
                }
            }
        }
    }
}