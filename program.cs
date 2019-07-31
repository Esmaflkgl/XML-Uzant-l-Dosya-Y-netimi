using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace xmlFileExplorer
{
    public partial class Form1 : Form
    {
        NodeInfo myTree= null;//dinamik doldurcam.
        TreeNode myTopTreeItem = null;//createGUITree en tepeyi oluşturmak için
        IDictionary<int, NodeInfo> myNodeMap = new Dictionary<int, NodeInfo>();



        public Form1()
        {
            InitializeComponent();
        }

         private void button1_Click(object sender, EventArgs e) 
        {
            FolderBrowserDialog fBrowser = new FolderBrowserDialog();
            fBrowser.ShowNewFolderButton = false;
            fBrowser.ShowDialog();
            string secilenDizin = fBrowser.SelectedPath;
            filePath_Text.Text = secilenDizin;
            if (myTree != null)
                foreach (NodeInfo node in myTree.childrenNodes)
                    if(node.path == secilenDizin)
                    {
                        MessageBox.Show("Bu Klasör Zaten Var!");
                        return;
                    }
            fillTree(secilenDizin, ref myTree);

            traverseTree(myTree);

            myTopTreeItem = null;
            createGUITree(myTree, ref myTopTreeItem);

            leftTree.Nodes.Clear();
            leftTree.Nodes.Add(myTopTreeItem);
        }



        private void fillTree(string dirFilePath,ref  NodeInfo parentTree)
        {                                                            
            NodeInfo currentNode = new NodeInfo();
            myNodeMap[currentNode.id] = currentNode;

            bool isDir= Directory.Exists(dirFilePath); 
            if (isDir) 
            {                
                 currentNode.path = dirFilePath;
                System.IO.DirectoryInfo dir= new System.IO.DirectoryInfo(dirFilePath);
                currentNode.name = dir.Name;
                currentNode.isDir = true;

                foreach (string childDir in Directory.GetDirectories(dirFilePath)) 
                {                   
                    fillTree(childDir, ref currentNode); 
                }

                foreach (string childFile in Directory.GetFiles(dirFilePath)) 
                {
                    if (childFile.EndsWith(".xml"))
                        fillTree(childFile, ref currentNode);
                }
            }

            else 
            {
                currentNode.path = dirFilePath; 
                System.IO.FileInfo file = new System.IO.FileInfo(dirFilePath); 
                currentNode.name = file.Name;
                currentNode.isDir = false; 
            }

            if (parentTree == null) 
            {
                parentTree = currentNode;
            }
            else
            {
                parentTree.childrenNodes.Add(currentNode); 
            }                                                              
        }  

        private void traverseTree(NodeInfo currentNode) 
        {
            if(currentNode.isDir == false && currentNode.fileInfo != "Parent")
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(currentNode.path);
                var list = doc.DocumentElement.ChildNodes;
                int sayı = list.Count;
                currentNode.fileInfo = "Sayı: " + sayı.ToString();
            }
            for (int index= 0; index < currentNode.childrenNodes.Count; ++index)
            {
                traverseTree(currentNode.childrenNodes[index]);            }
        }

        private void parseNode(NodeInfo currentNode)
        {
            if (currentNode == null)
            {
                return;     
             }

           
            XmlDocument doc = new XmlDocument();
            doc.Load(currentNode.path);
            XmlNode xml_node = doc.ChildNodes[1];
             RigthTree.Nodes.Clear();
            RigthTree.Nodes.Add(new TreeNode(doc.DocumentElement.Name));
            TreeNode tree_node = RigthTree.Nodes[0];
            add_nodes(xml_node, tree_node);
            tree_node.Toggle();

        }

        public void add_nodes(XmlNode x_node, TreeNode t_node) // burada okunacak
        {
            XmlNode xnode;
            TreeNode tnode;
            XmlNodeList node_list;
            int i;
            XmlDocument doc = new XmlDocument();
            
            if (x_node.HasChildNodes)
            {
                node_list = x_node.ChildNodes;
                for (i = 0; i <= node_list.Count - 1; i++)
                {
                    xnode = x_node.ChildNodes[i];
                    t_node.Nodes.Add(new TreeNode(xnode.Name));
                    XmlNodeList elemList = doc.GetElementsByTagName("");
                 


                 for (int a = 0; a < elemList.Count; a++)
                    {
                        RigthTree.Nodes.Add(new TreeNode(elemList[a].InnerXml.ToString()));
                                            }
                    tnode = t_node.Nodes[i];
                    add_nodes(xnode, tnode);

                }
            }
            else
            {
                t_node.Text = x_node.InnerText.ToString();
            }
        }

        private void createGUITree(NodeInfo currentInfo, ref TreeNode parentNode)
        {       //node elimde
            TreeNode currentItem = new TreeNode(); 
            currentItem.Text = currentInfo.name + " : " + currentInfo.fileInfo;//burada xml bilgileri geliyo xml içinde kaç parent olduğu.... burada içinde ne olduğu görünüyor
            currentItem.Tag = currentInfo.id;  

            for (int index = 0; index < currentInfo.childrenNodes.Count; ++index)
            {
                createGUITree(currentInfo.childrenNodes[index], ref currentItem);
            }
