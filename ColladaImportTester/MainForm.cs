using ColladaSharp;
using ColladaSharp.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using XMLSchemaDefinition;

namespace ColladaImportTester
{
    public partial class MainForm : Form
    {
        private ColladaImportOptions _options = new ColladaImportOptions();
        private CancellationTokenSource _cancel;

        public MainForm()
        {
            InitializeComponent();

            propertyGrid1.SelectedObject = _options;
            BaseXMLSchemaDefinition.OutputLine += Line;
            Collada.OutputLine += Line;
        }
        
        private void btnBrowse_Click(object sender, EventArgs e) => Browse();
        private void textBox1_MouseDown(object sender, MouseEventArgs e) => Browse();
        private void Browse()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Collada (*.dae)|*.dae";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = ofd.FileName;
                    btnImport.Enabled = true;
                }
            }
        }
        private async void btnImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
                return;
            
            _cancel = new CancellationTokenSource();
            btnCancel.Enabled = true;
            
            Progress<float> progress = new Progress<float>();
            progress.ProgressChanged += Progress_ProgressChanged;

            tabControl1.SelectTab(1);
            var scenes = await Collada.ImportAsync(textBox1.Text, _options, progress, _cancel.Token);

            btnCancel.Enabled = false;
            _cancel = null;
            
            PrintOutput(scenes);
            progress.ProgressChanged -= Progress_ProgressChanged;
            progressBar1.Value = progressBar1.Maximum;
        }
        
        private void Progress_ProgressChanged(object sender, float value)
            => progressBar1.Value = (int)(value * (progressBar1.Maximum - progressBar1.Minimum) + progressBar1.Minimum);

        private void btnCancel_Click(object sender, EventArgs e) => _cancel.Cancel();

        private void Line() => Line(string.Empty);
        private void Line(string line)
        {
            richTextBox1.Text += line + Environment.NewLine;
            Console.WriteLine(line);
        }
        private void PrintOutput(Collada.SceneCollection scenes)
        {
            if (scenes == null)
                return;

            Line();
            Line($"{scenes.Scenes.Count} scene(s)");
            int i = 0;
            foreach (var scene in scenes.Scenes)
            {
                Model m = scene.Model;
                Skeleton skel = m.Skeleton;

                Line($"Scene {++i} model: {m.Name}");

                Line();
                if (skel != null)
                {
                    Line("Has skeleton");
                    void PrintBones(IEnumerable<Bone> bones, string tabs)
                    {
                        foreach (Bone bone in bones)
                        {
                            Line(tabs + bone.Name);
                            PrintBones(bone, tabs + "   ");
                        }
                    }
                    PrintBones(skel.RootBones, "");
                }
                else
                    Line("Has no skeleton");
                Line();

                var meshes = m.Children;
                Line($"{meshes.Count} meshes");
                i = 0;
                int totalWidth = meshes.Count.ToString().Length;
                foreach (var mesh in meshes)
                {
                    Material mat = mesh.Material;

                    Line();
                    string num = (++i).ToString().PadLeft(totalWidth, '0');
                    Line($"Mesh {num}: {mesh.Name}");

                    Line();
                    var prims = mesh.Primitives;
                    Line("   " + prims.Buffers.Count.ToString() + " primitive buffers");
                    foreach (IDataBuffer buf in prims.Buffers)
                    {
                        Line($"      Buffer {buf.Index}: {buf.Type} {buf.Count} {buf.ElementType.GetFriendlyName()}");
                    }

                    Line($"   Material: {mat.Name} [{mat.Textures.Length} texture(s)]");
                    int x = 0;
                    foreach (TexRef tref in mat.Textures)
                    {
                        string output = $"      Texture {++x}: ";
                        if (tref is TexRefPath tpath)
                        {
                            output += tpath.AbsolutePath;
                        }
                        else if (tref is TexRefInternal tint)
                        {
                            output += $"(internal {tint.Format})";
                        }
                        Line(output);
                    }

                }
                Line();
            }
        }
    }
}