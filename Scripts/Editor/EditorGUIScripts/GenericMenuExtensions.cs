/*************************
 * Original url: http://wiki.unity3d.com/index.php/GenericMenuExtensions
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorGUIScripts/GenericMenuExtensions.cs
 * File based on original modification date of: 11 March 2015, at 18:14. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorGUIScripts
{
    
    
    Contents [hide] 
    1 Description 
    2 Building a GenericMenu WITH these extensions 
    3 Building a GenericMenu WITHOUT these extensions 
    4 Assets/Editor/GenericMenuExtensions.cs 
    5 License 
    
    Description This editor script defines extension methods for Unity's GenericMenu class which aim to reduce the amount of code clutter when building menus. 
    The source code on this page is released under the MIT license. 
    Building a GenericMenu WITH these extensions private void ShowContextMenu(Object obj) {
        var menu = new GenericMenu();
     
        menu.AddItem("Set As Default")
            .On(obj == _defaultObject)
            .Action(() => {
                _defaultObject = obj;
            });
     
        menu.AddSeparator();
     
        menu.AddItem("Copy Values")
            .Action(() => {
                _clipboardObject = obj;
            });
     
        menu.AddItem("Paste Values")
            .Enable(_clipboardObject != null)
            .Action(() => {
                EditorUtility.CopySerialized(_clipboardObject, obj);
            });
     
        menu.AddItem("Edit Script")
            .Visible(obj is IHasEditableScript)
            .Action(() => {
                BeginEditingScript(obj);
            });
     
        menu.ShowAsContext();
    }Building a GenericMenu WITHOUT these extensions private static GUIContent s_SetAsDefaultContent = new GUIContent("Set As Default");
    private static GUIContent s_CopyValuesContent = new GUIContent("Copy Values");
    private static GUIContent s_PasteValuesContent = new GUIContent("Paste Values");
    private static GUIContent s_EditScriptContent = new GUIContent("Edit Script");
     
    private void ShowContextMenu(Object obj) {
        var menu = new GenericMenu();
     
        menu.AddItem(s_SetAsDefaultContent, obj == _defaultObject, () => {
            _defaultObject = obj;
        });
     
        menu.AddSeparator("");
     
        menu.AddItem(s_CopyValuesContent, false, () => {
            _clipboardObject = obj;
        });
     
        if (_clipboardObject != null) {
            menu.AddItem(s_PasteValuesContent, false, () => {
                EditorUtility.CopySerialized(_clipboardObject, obj);
            });
        }
        else {
            menu.AddDisabledItem(s_PasteValuesContent);
        }
     
        if (obj is IHasEditableScript) {
            menu.AddItem(s_EditScriptContent, false, () => {
                BeginEditingScript(obj);
            });
        }
     
        menu.ShowAsContext();
    }Assets/Editor/GenericMenuExtensions.cs // Copyright (c) 2014 Rotorz Limited. All rights reserved.
    // License: BSD (2 clause)
     
    using System;
    using System.Collections.Generic;
     
    using UnityEngine;
    using UnityEditor;
     
    /// <summary>
    /// Extension methods for <see cref="UnityEditor.GenericMenu"/>.
    /// </summary>
    public static class GenericMenuExtensions {
     
        private static Dictionary<string, GUIContent> s_ContentCache = new Dictionary<string, GUIContent>();
     
        private static GUIContent GetContent(string text) {
            GUIContent content;
            if (!s_ContentCache.TryGetValue(text, out content)) {
                content = new GUIContent(text);
                s_ContentCache[text] = content;
            }
            return content;
        }
     
        /// <summary>
        /// Begin adding an item to the <see cref="GenericMenu"/>.
        /// </summary>
        /// <example>
        /// <para>Here are some usage examples:</para>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem(new GUIContent("Reset"))
        ///     .Enable(Selection.activeObject != null)
        ///     .Action(() => {
        ///         // Place reset logic here...
        ///     });
        /// 
        /// menu.AddItem(new GUIContent("Set as Default Object"))
        ///     .Enable(Selection.activeObject != null)
        ///     .On(Selection.activeObject == currentDefaultObject)
        ///     .Action(() => {
        ///         currentDefaultObject = Selection.activeObject;
        ///     });
        /// 
        /// menu.ShowAsContext();
        /// ]]></code>
        /// </example>
        /// <param name="menu">The menu that is being constructed.</param>
        /// <param name="content">Content of menu item.</param>
        /// <returns>
        /// Context object used whilst adding a menu item.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <list type="bullet">
        /// <item>If <paramref name="menu"/> is a value of <c>null</c>.</item>
        /// <item>If <paramref name="content"/> is a value of <c>null</c>.</item>
        /// </list>
        /// </exception>
        public static IGenericMenuAddItemContext AddItem(this GenericMenu menu, GUIContent content) {
            if (menu == null)
                throw new ArgumentNullException("menu");
            if (content == null)
                throw new ArgumentNullException("content");
     
            return GenericMenuAddItemContext.GetContext(menu, content);
        }
     
        /// <summary>
        /// Begin adding an item to the <see cref="GenericMenu"/>.
        /// </summary>
        /// <example>
        /// <para>Here are some usage examples:</para>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("Reset")
        ///     .Enable(Selection.activeObject != null)
        ///     .Action(() => {
        ///         // Place reset logic here...
        ///     });
        /// 
        /// menu.AddItem("Set as Default Object")
        ///     .Enable(Selection.activeObject != null)
        ///     .On(Selection.activeObject == currentDefaultObject)
        ///     .Action(() => {
        ///         currentDefaultObject = Selection.activeObject;
        ///     });
        /// 
        /// menu.ShowAsContext();
        /// ]]></code>
        /// </example>
        /// <param name="menu">The menu that is being constructed.</param>
        /// <param name="text">Text of menu item.</param>
        /// <returns>
        /// Context object used whilst adding a menu item.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <list type="bullet">
        /// <item>If <paramref name="menu"/> is a value of <c>null</c>.</item>
        /// <item>If <paramref name="text"/> is a value of <c>null</c>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="text"/> is an empty string.
        /// </exception>
        public static IGenericMenuAddItemContext AddItem(this GenericMenu menu, string text) {
            if (menu == null)
                throw new ArgumentNullException("menu");
            if (text == null)
                throw new ArgumentNullException("text");
            if (text == "")
                throw new ArgumentException("Empty string.", "text");
     
            return GenericMenuAddItemContext.GetContext(menu, GetContent(text));
        }
     
        /// <summary>
        /// Adds separator to the <see cref="GenericMenu"/>.
        /// </summary>
        /// <param name="menu">The menu that is being constructed.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="menu"/> is a value of <c>null</c>.
        /// </exception>
        public static void AddSeparator(this GenericMenu menu) {
            if (menu == null)
                throw new ArgumentNullException("menu");
     
            menu.AddSeparator("");
        }
     
    }
     
    /// <summary>
    /// Describes current context of adding an item to a <see cref="GenericMenu"/>.
    /// </summary>
    /// <seealso cref="GenericMenuExtensions.AddItem(GenericMenu, GUIContent)"/>
    /// <seealso cref="GenericMenuExtensions.AddItem(GenericMenu, string)"/>
    public interface IGenericMenuAddItemContext {
     
        /// <summary>
        /// Specifies whether menu item should be enabled.
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("This item is disabled!")
        ///     .Enable(false)
        ///     .Action(() => {
        ///         Debug.Log("User cannot make this appear in log!");
        ///     });
        /// ]]></code>
        /// </example>
        /// <param name="enable">Specifies whether item is enabled.</param>
        /// <returns>
        /// The <see cref="IGenericMenuAddItemContext"/> instance for chained method calls.
        /// </returns>
        IGenericMenuAddItemContext Enable(bool enable);
     
        /// <summary>
        /// Specifies whether menu item should be enabled.
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("This item is disabled!")
        ///     .Enable(() => false)
        ///     .Action(() => {
        ///         Debug.Log("User cannot make this appear in log!");
        ///     });
        /// ]]></code>
        /// </example>
        /// <param name="predicate">Predicate that determines whether item is enabled by
        /// by returning a value of <c>true</c>.</param>
        /// <returns>
        /// The <see cref="IGenericMenuAddItemContext"/> instance for chained method calls.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="predicate"/> is a value of <c>null</c>.
        /// </exception>
        IGenericMenuAddItemContext Enable(Func<bool> predicate);
     
        /// <summary>
        /// Specifies whether menu item should be visible in menu.
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("Visible when item is selected!")
        ///     .Visible(Selection.activeObject != null)
        ///     .Action(() => {
        ///         string objectName = Selection.activeObject.name;
        ///         Debug.Log(string.Format("Selected object is called '{0}'.", objectName));
        ///     });
        /// ]]></code>
        /// </example>
        /// <param name="visible">Specifies whether item is visible.</param>
        /// <returns>
        /// The <see cref="IGenericMenuAddItemContext"/> instance for chained method calls.
        /// </returns>
        IGenericMenuAddItemContext Visible(bool visible);
     
        /// <summary>
        /// Specifies whether menu item should be visible.
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("Visible when item is selected!")
        ///     .Visible(() => Selection.activeObject != null)
        ///     .Action(() => {
        ///         string objectName = Selection.activeObject.name;
        ///         Debug.Log(string.Format("Selected object is called '{0}'.", objectName));
        ///     });
        /// ]]></code>
        /// </example>
        /// <param name="predicate">Predicate that determines whether item is visible by
        /// returning a value of <c>true</c>.</param>
        /// <returns>
        /// The <see cref="IGenericMenuAddItemContext"/> instance for chained method calls.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="predicate"/> is a value of <c>null</c>.
        /// </exception>
        IGenericMenuAddItemContext Visible(Func<bool> predicate);
     
        /// <summary>
        /// Specifies whether menu item should be in an "on" state (aka checked).
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("Item is checked!")
        ///     .On(true)
        ///     .Action(() => {
        ///         Debug.Log("Selected checked menu item!");
        ///     });
        /// ]]></code>
        /// </example>
        /// <param name="on">Indicates if menu item is "on".</param>
        /// <returns>
        /// The <see cref="IGenericMenuAddItemContext"/> instance for chained method calls.
        /// </returns>
        IGenericMenuAddItemContext On(bool on);
     
        /// <summary>
        /// Specifies whether menu item should be in an "on" state (aka checked).
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("Item is checked!")
        ///     .On(() => true)
        ///     .Action(() => {
        ///         Debug.Log("Selected checked menu item!");
        ///     });
        /// ]]></code>
        /// </example>
        /// <param name="predicate">Predicate that determines whether item is "on" by
        /// returning a value of <c>true</c>.</param>
        /// <returns>
        /// The <see cref="IGenericMenuAddItemContext"/> instance for chained method calls.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="predicate"/> is a value of <c>null</c>.
        /// </exception>
        IGenericMenuAddItemContext On(Func<bool> predicate);
     
        /// <summary>
        /// Finalize addition of menu item by assigning an action to it.
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("Log message to console!")
        ///     .Action(() => {
        ///         Debug.Log("Hello, world!");
        ///     });
        /// ]]></code>
        /// </example>
        /// <param name="action">Delegate to perform menu action.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="action"/> is a value of <c>null</c>.
        /// </exception>
        void Action(GenericMenu.MenuFunction action);
     
        /// <summary>
        /// Finalize addition of menu item by assigning an action to it.
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// var menu = new GenericMenu();
        /// 
        /// menu.AddItem("Log message to console!")
        ///     .Action(userData => {
        ///         Debug.Log("Hello, {0}!", (string)userData);
        ///     }, "my friend");
        /// ]]></code>
        /// </example>
        /// <param name="action">Delegate to perform menu action.</param>
        /// <param name="userData">Additional data that is made available to menu action.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="action"/> is a value of <c>null</c>.
        /// </exception>
        void Action(GenericMenu.MenuFunction2 action, object userData);
     
    }
     
    internal sealed class GenericMenuAddItemContext : IGenericMenuAddItemContext {
     
        private static Stack<GenericMenuAddItemContext> s_Pool = new Stack<GenericMenuAddItemContext>();
     
        public static GenericMenuAddItemContext GetContext(GenericMenu menu, GUIContent content) {
            var context = s_Pool.Count > 0
                ? s_Pool.Pop()
                : new GenericMenuAddItemContext();
     
            context._menu = menu;
            context._content = content;
            context._visible = true;
            context._enabled = true;
            context._on = false;
     
            return context;
        }
     
        private static void ReturnContext(GenericMenuAddItemContext context) {
            context._menu = null;
            context._content = null;
     
            s_Pool.Push(context);
        }
     
        private GenericMenu _menu;
        private GUIContent _content;
        private bool _visible;
        private bool _enabled;
        private bool _on;
     
        #region IGenericMenuAddItemContext Members
     
        public IGenericMenuAddItemContext Enable(bool enable) {
            _enabled = enable;
            return this;
        }
     
        public IGenericMenuAddItemContext Enable(Func<bool> predicate) {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
     
            _enabled = predicate();
            return this;
        }
     
        public IGenericMenuAddItemContext Visible(bool visible) {
            _visible = visible;
            return this;
        }
     
        public IGenericMenuAddItemContext Visible(Func<bool> predicate) {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
     
            _visible = predicate();
            return this;
        }
     
        public IGenericMenuAddItemContext On(bool on) {
            _on = on;
            return this;
        }
     
        public IGenericMenuAddItemContext On(Func<bool> predicate) {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
     
            _on = predicate();
            return this;
        }
     
        public void Action(GenericMenu.MenuFunction action) {
            try {
                if (action == null)
                    throw new ArgumentNullException("action");
     
                if (!_visible)
                    return;
                if (_enabled)
                    _menu.AddItem(_content, _on, action);
                else
                    _menu.AddDisabledItem(_content);
            }
            finally {
                ReturnContext(this);
            }
        }
     
        public void Action(GenericMenu.MenuFunction2 action, object userData) {
            try {
                if (action == null)
                    throw new ArgumentNullException("action");
     
                if (!_visible)
                    return;
                if (_enabled)
                    _menu.AddItem(_content, _on, action, userData);
                else
                    _menu.AddDisabledItem(_content);
            }
            finally {
                ReturnContext(this);
            }
        }
     
        #endregion
     
    }License The MIT License (MIT) 
    Copyright (c) 2014 Rotorz Limited 
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
    -- End of License -- 
}
