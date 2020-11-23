﻿#if UNITY_EDITOR
using Pumkin.AvatarTools.Interfaces;
using Pumkin.AvatarTools.UI;
using Pumkin.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Assets.PumkinsAvatarTools.Scripts.Editor.Core.Extensions;
using UnityEditor;
using UnityEngine;

namespace Pumkin.Core.Helpers
{
    public static class UIHelpers
    {

        public static bool DrawFoldout(bool value, GUIContent content, bool toggleOnClick, GUIStyle style)
        {
            //Temporary wrapper until I decide to replace this with something nicer looking
            Rect rect = GUILayoutUtility.GetRect(content, Styles.Box, GUILayout.ExpandWidth(true));
            float oldLabel = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = rect.width;
            bool result = EditorGUI.Foldout(rect, value, content, true, style);
            EditorGUIUtility.labelWidth = oldLabel;
            return result;
            //return EditorGUILayout.Foldout(value, content, toggleOnClick, style);
        }

        public static void DrawGUILine(float height = 1f, bool spaceBefore = true, bool spaceAfter = true)
        {
            if(spaceBefore)
                EditorGUILayout.Space();
            GUILayout.Box(GUIContent.none, Styles.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(height));
            if(spaceAfter)
                EditorGUILayout.Space();
        }

        public static void VerticalBox(Action action, GUIStyle style = null, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style ?? Styles.Box, options);
            action.Invoke();
            EditorGUILayout.EndVertical();
        }

        public static void HorizontalBox(Action action, GUIStyle style = null, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style ?? Styles.Box, options);
            action.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawIndented(int indentLevel, Action action)
        {
            int old = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indentLevel;
            action.Invoke();
            EditorGUI.indentLevel = old;
        }

        public static void DrawUIPairs(List<IItem> items)
        {
            if(items.Count == 0)
                return;

            float width = GUILayoutUtility.GetLastRect().width / 2;
            var options = new GUILayoutOption[] { GUILayout.Width(width), GUILayout.ExpandWidth(true) };

            using(var sub = items.GetFlipFlopEnumerator())
            {
                bool beganHorizontal = false;
                while(sub.MoveNext())
                {
                    if(beganHorizontal && sub.Current.Settings != null) //End pair if second item has settings
                    {
                        EditorGUILayout.EndHorizontal();
                        beganHorizontal = false;
                    }
                    else if(!sub.FlipState && sub.Current.Settings == null)  //Don't start a pair if current item has settings
                    {
                        EditorGUILayout.BeginHorizontal();
                        beganHorizontal = true;
                    }

                    sub.Current?.DrawUI(options);

                    if(sub.FlipState && beganHorizontal)
                    {
                        EditorGUILayout.EndHorizontal();
                        beganHorizontal = false;
                    }
                }

                if(beganHorizontal)
                    EditorGUILayout.EndHorizontal();
            }
        }

        public static void DrawFoldoutListScrolling<T>(List<T> list, ref bool expanded, ref Vector2 scroll,
            string label) where T : class
        {
            expanded = EditorGUILayout.Foldout(expanded, label);
            EditorGUILayout.BeginScrollView(scroll);
            if(expanded)
                DrawList(list);
            EditorGUILayout.EndScrollView();
        }

        public static void DrawFoldoutList<T>(List<T> list, ref bool expanded, string label) where T : class
        {
            expanded = EditorGUILayout.Foldout(expanded, label);
            if(expanded)
                DrawList(list);
        }

        /// <summary>
        /// Draws list as object fields with size field
        /// </summary>
        /// <typeparam name="T">Type of object in object field</typeparam>
        /// <param name="list">List of elements</param>
        /// <returns>True if list was changed</returns>
        public static bool DrawList<T>(List<T> list) where T : class
        {
            bool changed = false;
            EditorGUILayout.Space();

            int size = list.Count;
            EditorGUI.BeginChangeCheck();
            size = EditorGUILayout.IntField("Size", size);
            if(EditorGUI.EndChangeCheck())
            {
                changed = true;
                list.ResizeWithDefaults(size);
            }

            return DrawListElements(list) || changed;
        }

        /// <summary>
        /// Draws a list of elements as object fields with Add, Remove and Clear buttons
        /// </summary>
        /// <typeparam name="T">Type of object in object field</typeparam>
        /// <param name="list">List of elements</param>
        /// <returns>True if list was changed</returns>
        public static bool DrawListWithAddButtons<T>(List<T> list) where T : class
        {
            bool changed = DrawListElements(list);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();
                if(GUILayout.Button(Icons.RemoveAll, Styles.IconButton))
                    list.Clear();
                if(GUILayout.Button(Icons.Remove, Styles.IconButton))
                    list.ResizeWithDefaults(list.Count - 1);
                if(GUILayout.Button(Icons.Add, Styles.IconButton))
                    list.ResizeWithDefaults(list.Count + 1);
            }
            EditorGUILayout.EndHorizontal();
            return changed;
        }

        /// <summary>
        /// Draws a list of elements as object fields
        /// </summary>
        /// <typeparam name="T">Type of object in object field</typeparam>
        /// <param name="list">List of elements</param>
        /// <returns>True if list was changed</returns>
        static bool DrawListElements<T>(List<T> list) where T : class
        {
            bool changed = false;
            for(int i = 0; i < list.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                var obj = EditorGUILayout.ObjectField($"Element {i}", list[i] as UnityEngine.Object, typeof(T), true);
                if(EditorGUI.EndChangeCheck())
                {
                    list[i] = obj as T;
                    changed = true;
                }
            }

            return changed;
        }
    }
}
#endif