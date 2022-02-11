#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;

    static public class EditorUtility
    {
        // UTILITY
        public const float REORDERABLE_LIST_HEADER_HEIGHT = 15f;
        public const float REORDERABLE_LIST_ELEMENT_HEIGHT = 18f;
        public const float REORDERABLE_LIST_BUTTONS_HEIGHT = -3f;
        static public float LineHeight
        => EditorGUIUtility.singleLineHeight;
        static public ReorderableList CreateReorderableList(SerializedProperty arrayProperty, string headerText = null, bool drawButtons = true, float indent = 0)
        {
            ReorderableList newList = new(arrayProperty.serializedObject, arrayProperty, true, headerText.IsNotNullOrEmpty(), drawButtons, drawButtons);
            newList.drawElementCallback = DrawElement;
            newList.drawHeaderCallback = DrawHeader;
            newList.elementHeightCallback = GetElementHeight;
            newList.headerHeight = REORDERABLE_LIST_HEADER_HEIGHT;
            newList.footerHeight = REORDERABLE_LIST_BUTTONS_HEIGHT;
            newList.drawNoneElementCallback = t => { };
            return newList;

            // Methods
            void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                rect.xMin += indent;
                EditorGUI.PropertyField(rect, arrayProperty.GetArrayElement(index), GUIContent.none);
            }
            void DrawHeader(Rect rect)
            => EditorGUI.LabelField(rect, headerText);
            float GetElementHeight(int index)
            => REORDERABLE_LIST_ELEMENT_HEIGHT;
        }

        // EXTENSIONS
        static public Rect ConsumeX(this ref Rect t, float a, bool aRelative = default, float b = default, bool bRelative = default)
        {
            Rect r = t;
            if (aRelative)
                r.width *= a;
            else
                r.width = a;

            t.xMin += r.width;

            if (b != default)
                if (bRelative)
                    r.width *= 1f - b;
                else
                    r.width -= b;

            return r;
        }
        static public SerializedProperty FindPropertyBackingField(this SerializedObject t, string a)
        => t.FindProperty($"<{a}>k__BackingField");
        static public SerializedProperty FindPropertyBackingFieldRelative(this SerializedProperty t, string a)
        => t.FindPropertyRelative($"<{a}>k__BackingField");

        // Array
        static public SerializedProperty GetArrayElement(this SerializedProperty t, int a)
        => t.GetArrayElementAtIndex(a);
        static public SerializedProperty GetArrayElementFromEnd(this SerializedProperty t, int a)
        => t.GetArrayElementAtIndex(t.arraySize - 1 - a);
        static public Object GetArrayObject(this SerializedProperty t, int a)
        => t.GetArrayElementAtIndex(a).objectReferenceValue;
        static public Object GetArrayObjectFromEnd(this SerializedProperty t, int a)
        => t.GetArrayElementAtIndex(t.arraySize - 1 - a).objectReferenceValue;
        static public Object SetArrayObject(this SerializedProperty t, int a, Object b)
        => t.GetArrayElementAtIndex(a).objectReferenceValue = b;
        static public Object SetArrayObjectFromEnd(this SerializedProperty t, int a, Object b)
        => t.GetArrayElementAtIndex(t.arraySize - 1 - a).objectReferenceValue = b;
        static public bool HasAnyNonNullElement(this SerializedProperty t)
        {
            for (int i = 0; i < t.arraySize; i++)
                if (t.GetArrayElement(i).objectReferenceValue != null)
                    return true;
            return false;
        }
        static public void AutoSizeObjectArray(this SerializedProperty t)
        {
            if (t.arraySize == 0
            || t.GetArrayObjectFromEnd(0) != null)
            {
                t.arraySize++;
                t.SetArrayObjectFromEnd(0, null);
            }
            else
                while (t.arraySize > 1
                && t.GetArrayObjectFromEnd(1) == null)
                    t.arraySize--;
        }
        static public void DrawCustomObjectArray(this SerializedProperty t, ref Rect rect, float spacing = 0)
        {
            for (int i = 0; i < t.arraySize; i++)
            {
                EditorGUI.PropertyField(rect, t.GetArrayElementAtIndex(i), GUIContent.none);
                rect.y += rect.height + spacing;
            }
        }
        static public int GetRecursiveArrayCount(this SerializedProperty t)
        {
            int count = t.arraySize;
            for (int i = 0; i < count; i++)
                count += t.GetArrayElementAtIndex(i).FindPropertyRelative(t.name).GetRecursiveArrayCount();
            return count.ClampMin(1);
        }
        static public float GetReorderableListPropertyHeight(this SerializedProperty t)
        => REORDERABLE_LIST_HEADER_HEIGHT + (REORDERABLE_LIST_ELEMENT_HEIGHT + 2f) * t.arraySize.ClampMin(1) + 8f + REORDERABLE_LIST_BUTTONS_HEIGHT;
        static public float GetReorderableListRecursivePropertyHeight(this SerializedProperty t)
        {
            float r = t.GetReorderableListPropertyHeight();
            for (int i = 0; i < t.arraySize; i++)
                r += t.GetArrayElementAtIndex(i).FindPropertyRelative(t.name).GetReorderableListRecursivePropertyHeight();
            return r;
        }
        static public float GetRecursivePropertyHeight(this PropertyDrawer t, SerializedProperty recurisveArrayProperty)
        {
            float r = 0f;
            for (int i = 0; i < recurisveArrayProperty.arraySize; i++)
                r += t.GetPropertyHeight(recurisveArrayProperty.GetArrayElementAtIndex(i), GUIContent.none);
            return r;
        }
    }
}
#endif