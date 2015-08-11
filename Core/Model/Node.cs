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
using System.Reflection;
using CSharpCity.Helpers;
using System.Collections.Generic;

namespace DeenGames.ThePrisoner.Model
{
    public class Node
    {
        private IList<NodeLink> _links = new List<NodeLink>();

        public string Content { get; set; }
        public string Title { get; set; }
        public string Template { get; set; }

        public Node()
        {
            this.Template = "default";
        }

        public void AddLink(NodeLink l)
        {
            this._links.Add(l);
        }

        public IList<NodeLink> Links { get { return this._links; } }

        public override string ToString()
        {
            return string.Format("{0} ({1} links)", this.Title, this.Links.Count);
        }
    }
}
