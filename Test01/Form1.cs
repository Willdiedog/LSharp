﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var bytes = System.IO.File.ReadAllBytes("t01.dll");
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            var module = Mono.Cecil.ModuleDefinition.ReadModule(ms);

            FillTree(module, treeView1.Nodes);
            treeView1.ExpandAll();
        }

        void FillTree(Mono.Cecil.ModuleDefinition module, TreeNodeCollection nodecoll)
        {
            TreeNode name = new TreeNode(module.Name);
            nodecoll.Add(name);
            TreeNode Reference = new TreeNode("AssemblyReferences");
            nodecoll.Add(Reference);
            foreach (var r in module.AssemblyReferences)
            {
                TreeNode node = new TreeNode(r.Name);
                Reference.Nodes.Add(node);
            }
            TreeNode TypeNode = new TreeNode("Types");
            nodecoll.Add(TypeNode);
            foreach (var t in module.Types)
            {
                TreeNode node = new TreeNode(t.Name);
                TypeNode.Nodes.Add(node);
                FillTree_Type(t, node.Nodes);
            }
        }
        void FillTree_Type(Mono.Cecil.TypeDefinition type, TreeNodeCollection nodecoll)
        {
            TreeNode Fields = new TreeNode("Fields");
            nodecoll.Add(Fields);
            foreach (var f in type.Fields)
            {
                TreeNode ff = new TreeNode(f.Name);
                Fields.Nodes.Add(ff);
            }

            TreeNode Methods = new TreeNode("Methods");
            nodecoll.Add(Methods);
            foreach (var m in type.Methods)
            {
                TreeNode mm = new TreeNode(m.Name);
                Methods.Nodes.Add(mm);
                if(m.HasParameters)
                {
                    TreeNode param = new TreeNode("Param");
                    mm.Nodes.Add(param);
                    FillTree_Type_Method_Param(m.Parameters, param.Nodes);
                }
                if (m.HasBody)
                {
                    TreeNode body = new TreeNode("body");
                    mm.Nodes.Add(body);
                    FillTree_Type_Method_Body(m.Body, body.Nodes);
                }
            }
        }
        void FillTree_Type_Method_Body(Mono.Cecil.Cil.MethodBody body,TreeNodeCollection nodecoll)
        {
            foreach (var m in body.Instructions)
            {
                TreeNode t = new TreeNode(m.ToString());
                nodecoll.Add(t);
            }
        }
        void FillTree_Type_Method_Param(Mono.Collections.Generic.Collection<Mono.Cecil.ParameterDefinition> _params, TreeNodeCollection nodecoll)
        {
            foreach(var p in _params)
            {
                TreeNode t = new TreeNode(p.ParameterType+"|"+p.Name);
                nodecoll.Add(t);
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}