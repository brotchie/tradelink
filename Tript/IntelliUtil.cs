using System;
using System.Collections.Generic;
using System.Text;

namespace Tript
{
    public class IntelliUtil
    {
        #region Util methods

        /// <summary>
        /// Takes an assembly filename, opens it and retrieves all types.
        /// </summary>
        /// <param name="filename">Filename to open</param>
        private void readAssembly(string filename)
        {
            this.treeViewItems.Nodes.Clear();
            namespaces = new Hashtable();
            assembly = Assembly.LoadFrom(this.openFileDialog1.FileName);

            Type[] assemblyTypes = assembly.GetTypes();
            this.treeViewItems.Nodes.Clear();

            // Cycle through types
            foreach (Type type in assemblyTypes)
            {
                if (type.Namespace != null)
                {
                    if (namespaces.ContainsKey(type.Namespace))
                    {
                        // Already got namespace, add the class to it
                        TreeNode treeNode = (TreeNode)namespaces[type.Namespace];
                        treeNode = treeNode.Nodes.Add(type.Name);
                        this.addMembers(treeNode, type);

                        if (type.IsClass)
                        {
                            treeNode.Tag = MemberTypes.Custom;
                        }
                    }
                    else
                    {
                        // New namespace
                        TreeNode membersNode = null;

                        if (type.Namespace.IndexOf(".") != -1)
                        {
                            // Search for already existing parts of the namespace
                            nameSpaceNode = null;
                            foundNode = false;

                            this.currentPath = "";
                            searchTree(this.treeViewItems.Nodes, type.Namespace, false);

                            // No existing namespace found
                            if (nameSpaceNode == null)
                            {
                                // Add the namespace
                                string[] parts = type.Namespace.Split('.');

                                TreeNode treeNode = treeViewItems.Nodes.Add(parts[0]);
                                string sNamespace = parts[0];

                                if (!namespaces.ContainsKey(sNamespace))
                                {
                                    namespaces.Add(sNamespace, treeNode);
                                }

                                for (int i = 1; i < parts.Length; i++)
                                {
                                    treeNode = treeNode.Nodes.Add(parts[i]);
                                    sNamespace += "." + parts[i];
                                    if (!namespaces.ContainsKey(sNamespace))
                                    {
                                        namespaces.Add(sNamespace, treeNode);
                                    }
                                }

                                membersNode = treeNode.Nodes.Add(type.Name);
                            }
                            else
                            {
                                // Existing namespace, add this namespace to it,
                                // and add the class
                                string[] parts = type.Namespace.Split('.');
                                TreeNode newNamespaceNode = null;

                                if (!namespaces.ContainsKey(type.Namespace))
                                {
                                    newNamespaceNode = nameSpaceNode.Nodes.Add(parts[parts.Length - 1]);
                                    namespaces.Add(type.Namespace, newNamespaceNode);
                                }
                                else
                                {
                                    newNamespaceNode = (TreeNode)namespaces[type.Namespace];
                                }

                                if (newNamespaceNode != null)
                                {
                                    membersNode = newNamespaceNode.Nodes.Add(type.Name);
                                    if (type.IsClass)
                                    {
                                        membersNode.Tag = MemberTypes.Custom;
                                    }
                                }
                            }

                        }
                        else
                        {
                            // Single root namespace, add to root
                            membersNode = treeViewItems.Nodes.Add(type.Namespace);
                        }

                        // Add all members
                        if (membersNode != null)
                        {
                            this.addMembers(membersNode, type);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Adds all members to the node's children, grabbing the parameters
        /// for methods.
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="type"></param>
        private void addMembers(TreeNode treeNode, System.Type type)
        {
            // Get all members except methods
            MemberInfo[] memberInfo = type.GetMembers();
            for (int j = 0; j < memberInfo.Length; j++)
            {
                if (memberInfo[j].ReflectedType.IsPublic && memberInfo[j].MemberType != MemberTypes.Method)
                {
                    TreeNode node = treeNode.Nodes.Add(memberInfo[j].Name);
                    node.Tag = memberInfo[j].MemberType;
                }
            }

            // Get all methods
            MethodInfo[] methodInfo = type.GetMethods();
            for (int j = 0; j < methodInfo.Length; j++)
            {
                TreeNode node = treeNode.Nodes.Add(methodInfo[j].Name);
                string parms = "";

                ParameterInfo[] parameterInfo = methodInfo[j].GetParameters();
                for (int f = 0; f < parameterInfo.Length; f++)
                {
                    parms += parameterInfo[f].ParameterType.ToString() + " " + parameterInfo[f].Name + ", ";
                }

                // Knock off remaining ", "
                if (parms.Length > 2)
                {
                    parms = parms.Substring(0, parms.Length - 2);
                }

                node.Tag = parms;
            }
        }

        /// <summary>
        /// Searches the tree view for a namespace, saving the node. The method
        /// stops and returns as soon as the namespace search can't find any
        /// more items in its path, unless continueUntilFind is true.
        /// </summary>
        /// <param name="treeNodes"></param>
        /// <param name="path"></param>
        /// <param name="continueUntilFind"></param>
        private void searchTree(TreeNodeCollection treeNodes, string path, bool continueUntilFind)
        {
            if (this.foundNode)
            {
                return;
            }

            string p = "";
            int n = 0;
            n = path.IndexOf(".");

            if (n != -1)
            {
                p = path.Substring(0, n);

                if (currentPath != "")
                {
                    currentPath += "." + p;
                }
                else
                {
                    currentPath = p;
                }

                // Knock off the first part
                path = path.Remove(0, n + 1);
            }
            else
            {
                currentPath += "." + path;
            }

            for (int i = 0; i < treeNodes.Count; i++)
            {
                if (treeNodes[i].FullPath == currentPath)
                {
                    if (continueUntilFind)
                    {
                        nameSpaceNode = treeNodes[i];
                    }

                    nameSpaceNode = treeNodes[i];

                    // got a dot, continue, or return
                    this.searchTree(treeNodes[i].Nodes, path, continueUntilFind);

                }
                else if (!continueUntilFind)
                {
                    foundNode = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Searches the tree until the given path is found, storing
        /// the found node in a member var.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="treeNodes"></param>
        private void findNode(string path, TreeNodeCollection treeNodes)
        {
            for (int i = 0; i < treeNodes.Count; i++)
            {
                if (treeNodes[i].FullPath == path)
                {
                    this.findNodeResult = treeNodes[i];
                    break;
                }
                else if (treeNodes[i].Nodes.Count > 0)
                {
                    this.findNode(path, treeNodes[i].Nodes);
                }
            }
        }

        /// <summary>
        /// Called when a "." is pressed - the previous word is found,
        /// and if matched in the treeview, the members listbox is
        /// populated with items from the tree, which are first sorted.
        /// </summary>
        /// <returns>Whether an items are found for the word</returns>
        private bool populateListBox()
        {
            bool result = false;
            string word = this.getLastWord();

            //System.Diagnostics.Debug.WriteLine(" - Path: " +word);

            if (word != "")
            {
                findNodeResult = null;
                findNode(word, this.treeViewItems.Nodes);

                if (this.findNodeResult != null)
                {
                    this.listBoxAutoComplete.Items.Clear();

                    if (this.findNodeResult.Nodes.Count > 0)
                    {
                        result = true;

                        // Sort alphabetically (this could be replaced with
                        // a sortable treeview)
                        MemberItem[] items = new MemberItem[this.findNodeResult.Nodes.Count];
                        for (int n = 0; n < this.findNodeResult.Nodes.Count; n++)
                        {
                            MemberItem memberItem = new MemberItem();
                            memberItem.DisplayText = this.findNodeResult.Nodes[n].Text;
                            memberItem.Tag = this.findNodeResult.Nodes[n].Tag;

                            if (this.findNodeResult.Nodes[n].Tag != null)
                            {
                                System.Diagnostics.Debug.WriteLine(this.findNodeResult.Nodes[n].Tag.GetType().ToString());
                            }

                            items[n] = memberItem;
                        }
                        Array.Sort(items);

                        for (int n = 0; n < items.Length; n++)
                        {
                            int imageindex = 0;

                            if (items[n].Tag != null)
                            {
                                // Default to method (contains text for parameters)
                                imageindex = 2;
                                if (items[n].Tag is MemberTypes)
                                {
                                    MemberTypes memberType = (MemberTypes)items[n].Tag;

                                    switch (memberType)
                                    {
                                        case MemberTypes.Custom:
                                            imageindex = 1;
                                            break;
                                        case MemberTypes.Property:
                                            imageindex = 3;
                                            break;
                                        case MemberTypes.Event:
                                            imageindex = 4;
                                            break;
                                    }
                                }
                            }

                            this.listBoxAutoComplete.Items.Add(new GListBoxItem(items[n].DisplayText, imageindex));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Autofills the selected item in the member listbox, by
        /// taking everything before and after the "." in the richtextbox,
        /// and appending the word in the middle.
        /// </summary>
        private void selectItem()
        {
            if (this.wordMatched)
            {
                int selstart = this.richTextBox1.SelectionStart;
                int prefixend = this.richTextBox1.SelectionStart - typed.Length;
                int suffixstart = this.richTextBox1.SelectionStart + typed.Length;

                if (suffixstart >= this.richTextBox1.Text.Length)
                {
                    suffixstart = this.richTextBox1.Text.Length;
                }

                string prefix = this.richTextBox1.Text.Substring(0, prefixend);
                string fill = this.listBoxAutoComplete.SelectedItem.ToString();
                string suffix = this.richTextBox1.Text.Substring(suffixstart, this.richTextBox1.Text.Length - suffixstart);

                this.richTextBox1.Text = prefix + fill + suffix;
                this.richTextBox1.SelectionStart = prefix.Length + fill.Length;
            }
        }

        /// <summary>
        /// Searches backwards from the current caret position, until
        /// a space or newline is found.
        /// </summary>
        /// <returns>The previous word from the carret position</returns>
        private string getLastWord()
        {
            string word = "";

            int pos = this.richTextBox1.SelectionStart;
            if (pos > 1)
            {

                string tmp = "";
                char f = new char();
                while (f != ' ' && f != 10 && pos > 0)
                {
                    pos--;
                    tmp = this.richTextBox1.Text.Substring(pos, 1);
                    f = (char)tmp[0];
                    word += f;
                }

                char[] ca = word.ToCharArray();
                Array.Reverse(ca);
                word = new String(ca);

            }
            return word.Trim();

        }
        #endregion
    }
}
