#if UNITY_EDITOR
using Assets.Scripts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FindReferencesWindow : EditorWindow
{
    static Dictionary<GameObject, string> found_references;
    static GameObject selected_object;
    
    private void OnGUI()
    {
        if (found_references.Count>0)
        {
            GUILayout.Label("Found "+ found_references.Count + " references for "+ selected_object.name + ".");

            foreach (GameObject o in found_references.Keys)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(found_references[o], EditorStyles.boldLabel);

                if (GUILayout.Button("select"))
                {
                    Selection.activeGameObject = o;
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("No references found.", EditorStyles.boldLabel);
        }
    }

    [MenuItem("GameObject/Find references", false, 0)]
    private static void FindReferences()
    {
        FindReferencesWindow window = (FindReferencesWindow)EditorWindow.GetWindow(typeof(FindReferencesWindow));
        
        found_references = new Dictionary<GameObject, string>();
        List<int> references = new List<int>();

        GameObject g0 = (GameObject)Selection.activeObject;
        selected_object = g0;

        Component[] myComponents0 = g0.GetComponents(typeof(Component));
        references.Add(g0.GetInstanceID());
        foreach (Component myComp in myComponents0)
        {
            references.Add(myComp.GetInstanceID());
        }

        foreach (GameObject g in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            Component[] myComponents = g.GetComponents(typeof(Component));
            foreach (Component myComp in myComponents)
            {
                if (myComp != null)
                {
                    System.Type myObjectType = myComp.GetType();
                    //Debug.Log("Checking component in " + DisplayGoHierarchy(g) + ": " + myObjectType);

                    SerializedObject serObj = new SerializedObject(myComp);
                    SerializedProperty prop = serObj.GetIterator();
                    while (prop.NextVisible(true))
                    {
                        try
                        {
                            if (prop.propertyType + "" == "ObjectReference" && references.Contains(prop.objectReferenceInstanceIDValue))
                            {
                                found_references.Add(g, DisplayGoHierarchy(g) + " -> " + myObjectType + " -> " + prop.name);
                                //Debug.Log("Found reference for " + g0.name + "" + " in " + DisplayGoHierarchy(g) + " -> " + myObjectType + " -> " + prop.name);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        window.title = "References ("+ found_references.Count+")";
        window.Show();
    }

    static string DisplayGoHierarchy(GameObject o)
    {
        if (o.transform.parent == null)
            return o.name;
        else
            return DisplayGoHierarchy(o.transform.parent.gameObject) + "/" + o.name;
    }
}
#endif
