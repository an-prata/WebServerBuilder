using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebServerBuilder
{
	public partial class Form1 : Form
	{
		ScriptBuilder scriptBuilder;

		public Form1()
		{
			InitializeComponent();
			scriptBuilder = new ScriptBuilder("temp");
			Text = "Untitled";
			
		}

		private void buttonEnter_Click(object sender, EventArgs e)
		{
			try
			{
				string[] get = scriptBuilder.MakeGet(textBoxFilePath.Text, textBoxURL.Text);
				List<string> lines = textBoxScript.Lines.ToList();
				foreach (string s in get) lines.Add(s);
				textBoxScript.Lines = lines.ToArray();
			}
			catch (FileNotFoundException)
			{
				MessageBox.Show("Please enter a valid file path.");
			}
			catch
			{
				MessageBox.Show("Something went wrong making a Get statement.");
			}
		}

		private void buttonFileBrowse_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = openFileDialog.ShowDialog();

			if (dialogResult == DialogResult.OK) textBoxFilePath.Text = openFileDialog.FileName;
		}

		private void buttonEditMode_Click(object sender, EventArgs e)
		{
			if (textBoxURL.Enabled && textBoxFilePath.Enabled)
			{
				textBoxFilePath.Enabled = false;
				textBoxURL.Enabled = false;
				textBoxScript.ReadOnly = false;
				buttonEditMode.Text = "Exit Edit Mode";
				buttonEnter.Enabled = false;
				buttonFileBrowse.Enabled = false;
			}
			else
			{
				textBoxFilePath.Enabled = true;
				textBoxURL.Enabled = true;
				textBoxScript.ReadOnly = true;
				buttonEditMode.Text = "Enter Edit Mode";
				buttonEnter.Enabled = true;
				buttonFileBrowse.Enabled = true;
			}
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string lines = "";
			foreach (string line in textBoxScript.Lines) lines += line + '\n';
			scriptBuilder.SaveFile(lines);

			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Javascript File|*.js",
				Title = "Save Node.js Script"
			};

			DialogResult dialogResult = saveFileDialog.ShowDialog();

			if (dialogResult == DialogResult.OK)
			{
				if (saveFileDialog.FileName != null || saveFileDialog.FileName != "")
				{
					scriptBuilder.Dispose();
					scriptBuilder = new ScriptBuilder(saveFileDialog.FileName);
					Text = saveFileDialog.FileName;
				}
				else
				{
					return;
				}
			}
				
			scriptBuilder.SaveFile(lines);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = openFileDialog.ShowDialog();

			if (dialogResult == DialogResult.OK)
			{
				if (openFileDialog.FileName == scriptBuilder.scriptPath) return;
				Text = openFileDialog.FileName;
				scriptBuilder.Dispose();
				scriptBuilder = new ScriptBuilder(openFileDialog.FileName);
				textBoxScript.Lines = scriptBuilder.GetLines();
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string lines = "";
			foreach (string line in textBoxScript.Lines) lines += line + '\n';

			if (scriptBuilder.scriptPath == "temp")
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					Filter = "Javascript File|*.js",
					Title = "Save Node.js Script"
				};

				DialogResult dialogResult = saveFileDialog.ShowDialog();

				if (dialogResult == DialogResult.OK)
				{
					if (saveFileDialog.FileName != null || saveFileDialog.FileName != "")
					{
						scriptBuilder.Dispose();
						scriptBuilder = new ScriptBuilder(saveFileDialog.FileName);
						Text = saveFileDialog.FileName;
					}
					else
					{
						return;
					}
				}
			}
			
			scriptBuilder.SaveFile(lines);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			scriptBuilder.Dispose();
			scriptBuilder = new ScriptBuilder("temp", FileMode.Create);
			textBoxScript.Lines = new string[] { "express = require('express');", "path = require('path');", "", "app = express();" };
			Text = "Untitled";
		}

        private void addListenToolStripMenuItem_Click(object sender, EventArgs e)
        {
			List<string> lines = textBoxScript.Lines.ToList();
			lines.Add(ScriptBuilder.MakeListen(80));
			textBoxScript.Lines = lines.ToArray();
		}
    }
}
