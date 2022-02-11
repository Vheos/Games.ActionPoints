/*
#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System.Collections.Generic;

    [CustomPropertyDrawer(typeof(ActionDataNEW))]
    public class ActionDataNEW_PropertyDrawer : PropertyDrawer
    {
        // Privates
        private ReorderableList _effectsList;
        private ReorderableList _conditionsList;
        private readonly Dictionary<string, ReorderableList> _datasListsByPath = new();
        private void TryInitializeEffectsList(SerializedProperty property)
        {
            if (_effectsList != null)
                return;

            var arrayProperty = property.FindPropertyRelative(nameof(ActionDataNEW.Effects));
            _effectsList = EditorUtility.CreateReorderableList(arrayProperty, arrayProperty.displayName, false);
            Debug.Log($"\tEffects");
        }
        private void TryInitializeConditionsList(SerializedProperty property)
        {
            if (_conditionsList != null)
                return;

            var arrayProperty = property.FindPropertyRelative(nameof(ActionDataNEW.Conditions));
            _conditionsList = EditorUtility.CreateReorderableList(arrayProperty, arrayProperty.displayName, false);
            Debug.Log($"\tConditions");
        }
        private void TryInitializeDatasLists(SerializedProperty property)
        {
            if (_datasListsByPath.ContainsKey(property.propertyPath))
                return;

            var arrayProperty = property.FindPropertyRelative(nameof(ActionDataNEW.Datas));
            var newList = EditorUtility.CreateReorderableList(arrayProperty, null, true, 30f);
            newList.elementHeightCallback = index => GetPropertyHeight(arrayProperty.GetArrayElement(index), GUIContent.none);
            newList.showDefaultBackground = false;
            newList.draggable = false;

            _datasListsByPath.Add(property.propertyPath, newList);
            Debug.Log($"\tDatas");
        }

        // Overrides
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propEffects = property.FindPropertyRelative(nameof(ActionDataNEW.Effects));
            var propConditions = property.FindPropertyRelative(nameof(ActionDataNEW.Conditions));
            var propDatas = property.FindPropertyRelative(nameof(ActionDataNEW.Datas));

            var height = propEffects.GetReorderableListPropertyHeight() + propConditions.GetReorderableListPropertyHeight();
            if (propConditions.HasAnyNonNullElement())
                height += propDatas.GetReorderableListPropertyHeight() + this.GetRecursivePropertyHeight(propDatas);

            return height;
        }


        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            TryInitializeEffectsList(property);
            _effectsList.serializedProperty.AutoSizeObjectArray();
            _effectsList.DoList(rect);
            rect.yMin += _effectsList.GetHeight();

            TryInitializeConditionsList(property);
            _conditionsList.serializedProperty.AutoSizeObjectArray();
            _conditionsList.DoList(rect);
            rect.yMin += _conditionsList.GetHeight();

            if (!_conditionsList.serializedProperty.HasAnyNonNullElement())
                return;

            TryInitializeDatasLists(property);
            var dataListProperty = _datasListsByPath[property.propertyPath].serializedProperty;
            if (dataListProperty.arraySize == 0)
                dataListProperty.arraySize++;
            _datasListsByPath[property.propertyPath].DoList(rect);
        }
    }


}
#endif
*/