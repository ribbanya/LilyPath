﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LilyPath;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace LilyPathDemo
{
    public partial class MainForm : Form
    {
        private Dictionary<string, Type> _pages;

        public MainForm ()
        {
            InitializeComponent();
            PopulateList();

            List<string> keys = new List<string>(_pages.Keys);
            keys.Sort();

            foreach (string key in keys)
                listBox1.Items.Add(key);

            listBox1.SelectedValueChanged += ListBoxSelectedValueChanged;
            listBox1.SelectedItem = "Water Lily";

            drawingControl1.FpsUpdated += (s, e) => {
                _tbFPS.Text = "FPS: " + Math.Round(drawingControl1.Fps, 2);
            };
        }

        private void ListBoxSelectedValueChanged (object sender, EventArgs e)
        {
            string key = (string)listBox1.SelectedItem;
            if (key == null)
                return;

            if (_pages.ContainsKey(key)) {
                Type type = _pages[key];
                ConstructorInfo cinfo = type.GetConstructor(Type.EmptyTypes);
                TestSheet sheet = cinfo.Invoke(null) as TestSheet;
                drawingControl1.Sheet = sheet;
            }
        }

        private void PopulateList ()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            _pages = new Dictionary<string, Type>();

            foreach (Module module in assembly.GetModules()) {
                foreach (Type type in module.GetTypes()) {
                    if (type.IsSubclassOf(typeof(TestSheet))) {
                        string name = type.Name;
                        foreach (TestNameAttribute attr in type.GetCustomAttributes(typeof(TestNameAttribute), true))
                            name = attr.Name;

                        _pages.Add(name, type);
                    }
                }
            }
        }

        private void solidToolStripMenuItem_Click (object sender, EventArgs e)
        {
            DemoState.FillMode = FillMode.Solid;
            solidToolStripMenuItem.Checked = true;
            wireframeToolStripMenuItem.Checked = false;
        }

        private void wireframeToolStripMenuItem_Click (object sender, EventArgs e)
        {
            DemoState.FillMode = FillMode.WireFrame;
            solidToolStripMenuItem.Checked = false;
            wireframeToolStripMenuItem.Checked = true;
        }

        private void multisampleAAToolStripMenuItem_Click (object sender, EventArgs e)
        {
            DemoState.MultisampleAA = !DemoState.MultisampleAA;
            multisampleAAToolStripMenuItem.Checked = DemoState.MultisampleAA;
        }
    }

    public static class DemoState
    {
        public static FillMode FillMode { get; set; }
        public static bool MultisampleAA { get; set; }

        static DemoState () {
            FillMode = FillMode.Solid;
            MultisampleAA = false;
        }
    }
}
