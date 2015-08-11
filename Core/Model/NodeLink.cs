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

namespace DeenGames.ThePrisoner.Model
{
    public class NodeLink
    {
        public string Caption { get; set; }
        public Node Target { get; set; }
        internal string TargetName { get; set; }

        public override string ToString()
        {
            return this.Caption + " => " + Target.Title;
        }
    }
}
