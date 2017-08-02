/*************************
 * Original url: http://wiki.unity3d.com/index.php/Element_Table_Representation
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/Element_Table_Representation.cs
 * File based on original modification date of: 10 January 2012, at 20:52. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    Sometimes it is necessary to display an array of elements in another way than with foldout elements. These two classes permit the creation of a table representation of your data. It has been inspired by java/SWT tables. 
    The following interfaces and classes are only proof of concepts. They lack a complete implementation of the different supportable datatypes. 
    The first class is actually an interface implementing a description of a table model. 
    using System;
    using System.Collections.Generic;
     
    public interface EditorTableModel<T>
    {
        void SetTableEntries(List<T> _entries);
        int GetColumnCount();
        int GetRowCount();
        bool UseHeaders();
     
        String GetColumnName(int _column);
        Object GetValue(int _column, int _row);
     
        void SetValue(int row, int column, Object _value);
    }The second class does the actual table drawing. The Pressed* delegates are not implemented as finally intended because the Remove button handling does not yet take the index parameter of the element to remove. 
    using System;
    using UnityEngine;
    using UnityEditor;
     
    public class EditorGuiTable<T>
    {
        private String tableName;
     
        public bool isVisible = true;
     
        public delegate void PressedAddButton();
        public delegate void PressedRemoveButton();
     
        public PressedAddButton addButton;
        public PressedRemoveButton removeButton;
     
        private EditorTableModel<T> model;
     
        public EditorGuiTable(String _tableName, EditorTableModel<T> _model)
        {
            tableName = _tableName;
            model = _model;
        }
     
        public void Draw()
        {
            if (!isVisible)
            {
                return;
            }
     
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (tableName.Length != 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(tableName);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
     
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
     
     
            if (model.GetRowCount() != 0)
            {
                GUILayout.BeginHorizontal();
                for (int column = 0; column < model.GetColumnCount(); column++)
                {
                    GUILayout.BeginVertical("box");
    				if (model.UseHeaders())
    				{
    		            GUILayout.BeginHorizontal("box");
                    	GUILayout.Label(model.GetColumnName(column));
    		            GUILayout.EndHorizontal();
    				}
    	            for (int row = 0; row < model.GetRowCount(); row++)
    	            {
                        System.Object obj = model.GetValue(row, column);
     
                        // Display of edit functionality for the different supported data types (here only string (non-editable) and GameObject).
                        if (obj is String)
                        {
                            GUILayout.Label(obj as String);
                        }
                        else if (obj is GameObject)
                        {
                            GameObject unObj = (GameObject)obj;
                            GameObject newObj = (GameObject)EditorGUILayout.ObjectField(unObj, typeof(GameObject), false);
                            if (newObj != unObj)
                            {
                                model.SetValue(row, column, (System.Object)newObj);
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
     
            GUILayout.BeginHorizontal();
            if (addButton != null && addButton.GetInvocationList().Length != 0)
            {
                if (GUILayout.Button("Add"))
                {
                    addButton();
                }
            }
            if (removeButton != null && removeButton.GetInvocationList().Length != 0)
            {
                if (GUILayout.Button("Remove"))
                {
                    removeButton();
                }
            }
            GUILayout.EndHorizontal();
     
     
            EditorGUILayout.EndVertical();
        }
     
        public  Vector2 ScrollPosition { get; set; }
    }Here's an example of a table model implementation (not complete): 
    using System;
    using System.Collections.Generic;
    using UnityEngine;
     
    public class BluePrintElementTableModel : EditorTableModel<BlueprintElement>
    {
        private List<BlueprintElement> elements;
     
        public BluePrintElementTableModel(List<BlueprintElement> _elements)
        {
            elements = _elements;
        }
     
        public void SetTableEntries(List<BlueprintElement> _elements)
        {
            elements = _elements;
        }
     
     
        public int GetColumnCount()
        {
            return 5;
        }
     
        public int GetRowCount()
        {
            return elements.Count;
        }
     
        public bool UseHeaders()
        {
            return true;
        }
     
        public String GetColumnName(int _column)
        {
            switch(_column)
            {
                case 0:
                    return "Name";
                case 1:
                    return "Gfx";
                case 2:
                    return "Action";
                case 3:
                    return "Type";
                case 4:
                    return "Combine";
            }
            return "Unknown";
        }
     
        public System.Object GetValue(int _row, int _column)
        {
            if ( _row < 0 || _row >= elements.Count )
                return null;
     
            BlueprintElement el = elements[_row];
            switch (_column)
            {
                case 0:
                    return el.name;
                case 1:
                    if (el.gfxAsset != null)
                        return el.gfxAsset;
                    else
                        return el.gfxAssetpath;
                case 2:
                    return el.actionScriptAssetpath;
                case 3:
                    return el.elementType.ToString();
                case 4:
                    return el.combineWith.ToString();
            }
     
            return "Unknown";
        }
     
        public void SetValue(int _row, int _column, System.Object _value)
        {
            if (_row < 0 || _row >= elements.Count)
                return;
     
            BlueprintElement el = elements[_row];
            switch (_column)
            {
                case 0:
                    el.name = (String)_value;
                    break;
                case 1:
                    if (_value is GameObject)
                    {
                       el.gfxAssetpath = BlueprintElementWizard.GetDecomposedAssetpath((GameObject)_value);
                       el.gfxAsset = (GameObject)_value;
                    }
                    break;
                case 2:
    //                return el.actionScriptAssetpath;
                    break;
                case 3:
    //                return el.elementType;
                    break;
                case 4:
    //                return el.combineWith;
                    break;
            }
     
        }
    }And the usage (C#): 
        private EditorGuiTable<BlueprintElement> bpTable;
        private BluePrintElementTableModel etm;
     
        private void DisplayBlueprintElements()
        {
            if (bpTable == null)
            {
                etm = new BluePrintElementTableModel(bpManager.blueprintElements);
                bpTable = new EditorGuiTable<BlueprintElement>("Elements", etm);
                bpTable.addButton += PressedBPElementAddButton;
            }
     
            bpTable.Draw();
        }There are still things missing such as selecting a single row to remove it (easily feasible via a selection button in the first column), but you get the idea... 
    The result will look like this:  
}
