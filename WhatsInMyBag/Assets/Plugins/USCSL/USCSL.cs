#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace USCSL
{
    public static class USCSL_Library
    {
        public static void SetDirty(Object o)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(o);
     
            if(o is GameObject) {
                Scene scene = ((GameObject) o).scene;
                EditorSceneManager.MarkSceneDirty(scene);
            }
     
            if(o is Component) {
                GameObject go = ((Component) o).gameObject;
                Scene scene = go.scene;
                EditorSceneManager.MarkSceneDirty(scene);
            }
#endif
        }
        
        public static void ChangeLayerRecursive(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                ChangeLayerRecursive(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }
    }
}
