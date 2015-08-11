using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DeenGames.ThePrisoner.Model;
using CSharpCity.Helpers;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DeenGames.ThePrisoner.Helpers
{
    public static class NodeHelper
    {
        public static Node GetStartNode(string content)
        {
            return parseContentAndMakeGraph(content);
        }

        // Returns the starting node
        private static Node parseContentAndMakeGraph(string content)
        {
            Regex nodeStartRegex = new Regex("(Start )?Node: (.+)");
            Regex linkRegex = new Regex("(.+) => (.+)");

            Node startNode = null;
            Dictionary<string, Node> nodes = new Dictionary<string, Node>();
            Node currentNode = new Node();

            string[] lines = content.Split(new char[] { '\n' });

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (nodeStartRegex.IsMatch(line)) {
                    Match m = nodeStartRegex.Match(line);
                    // Start of a node
                    if (!string.IsNullOrEmpty(currentNode.Content)) {
                        if (nodes.ContainsKey(currentNode.Title))
                        {
                            throw new Exception("There are two nodes named " + currentNode.Title);
                        }
                        else
                        {
                            nodes[currentNode.Title] = currentNode;
                        }
                    }

                    currentNode = new Node();
                    currentNode.Title = m.Groups[2].Value.Trim();
                    if (!string.IsNullOrEmpty(m.Groups[1].Value))
                    {
                        if (startNode != null)
                        {
                            throw new Exception("Second start node occurred (" + currentNode.Title + "); original was " + startNode.Title);
                        }

                        startNode = currentNode;
                    }
                }
                else if (line.Trim().Equals("Links:"))
                {
                    i++;
                    line = lines[i];

                    while (!(nodeStartRegex.IsMatch(line)) && !string.IsNullOrEmpty(line.Trim()) && i < lines.Length) {  
                        Match l = linkRegex.Match(line);
                        NodeLink n = new NodeLink();
                        n.Caption = l.Groups[1].ToString().Trim();
                        n.TargetName = l.Groups[2].ToString().Trim(); // Map later once all nodes are loaded
                        currentNode.AddLink(n);

                        i++;
                        if (i < lines.Length)
                        {
                            line = lines[i];
                        }
                    }
                }
                else if (!line.StartsWith("#"))
                {
                    // # indicates a comment
                    currentNode.Content += line.Trim() + "\n\n";
                }
            }

            // We're done; add the final node
            if (nodes.ContainsKey(currentNode.Title))
            {
                throw new Exception("There are two nodes named " + currentNode.Title);
            }
            else
            {
                nodes[currentNode.Title] = currentNode;
            }
            nodes[currentNode.Title] = currentNode;

            // Link everything up
            foreach (Node n in nodes.Values)
            {
                foreach (NodeLink l in n.Links)
                {
                    if (!nodes.ContainsKey(l.TargetName))
                    {
                        throw new Exception("Link " + l.Caption + " in node " + n.Title + " links to non-existent node " + l.TargetName);
                    }
                    else
                    {
                        l.Target = nodes[l.TargetName];
                    }
                }
            }

            return startNode;
        }
    }
}
