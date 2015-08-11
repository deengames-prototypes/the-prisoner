using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeenGames.ThePrisoner.Model;
using DeenGames.ThePrisoner.Helpers;
using DeenGames.ThePrisoner.Screens;

namespace UnitTest
{
    [TestClass]
    public class NodeHelperTest
    {
        [TestMethod]
        public void ParsingCanProcessSingleNodeWithSingleRecursiveLink()
        {
            string content = "Start Node: Forest\n\tOne day, deep in the forest ... \nLinks:\n\tRecurse => Forest";
            Node start = NodeHelper.GetStartNode(content);
            Assert.IsNotNull(start);
            Assert.AreEqual("Forest", start.Title);
            Assert.AreEqual("One day, deep in the forest ...\n\n", start.Content);
            Assert.AreEqual(1, start.Links.Count);
            Assert.AreEqual("Recurse", start.Links[0].Caption);
            Assert.AreEqual(start, start.Links[0].Target);
        }

        [TestMethod]
        public void ParsingCanProcessSingleNodeWithMultipleRecursiveLinks()
        {
            string content = "Start Node: Forest\n\tOne day, deep in the forest ... \nLinks:\n\tRecurse => Forest\n\tAgain => Forest\n\tRetreat => Forest";
            Node start = NodeHelper.GetStartNode(content);
            Assert.IsNotNull(start);
            Assert.AreEqual("Forest", start.Title);
            Assert.AreEqual("One day, deep in the forest ...\n\n", start.Content);
            Assert.AreEqual(3, start.Links.Count);
            Assert.AreEqual("Recurse", start.Links[0].Caption);
            Assert.AreEqual("Again", start.Links[1].Caption);
            Assert.AreEqual("Retreat", start.Links[2].Caption);
            Assert.AreEqual(start, start.Links[0].Target);
            Assert.AreEqual(start, start.Links[1].Target);
            Assert.AreEqual(start, start.Links[2].Target);
        }

        [TestMethod]
        public void ParsingCanProcessThreeNodeGraph()
        {
            string content =

@"Start Node: Forest
    One day, deep in the forest ...
Links:
    Travel the Path => Mountain
    Stay Here => Forest

Node: Mountain
    One day, high in the mountains ...
Links:
    Climb higher! => Mountain
    Diverge => Lake

Node: Lake
    You see a calm, placid blue lake.
Links:
    Swim => Lake";

            Node start = NodeHelper.GetStartNode(content);
            Assert.IsNotNull(start);
            Assert.AreEqual("Forest", start.Title);
            Assert.AreEqual("One day, deep in the forest ...\n\n", start.Content);
            Assert.AreEqual(2, start.Links.Count);
            Node mountain = start.Links[0].Target;
            Assert.AreEqual("Mountain", mountain.Title);
            Assert.AreEqual(2, mountain.Links.Count);
            Node lake = mountain.Links[1].Target;
            Assert.AreEqual("Lake", lake.Title);
            Assert.AreEqual(1, lake.Links.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParsingMultipleStartNodesThrowsAnException()
        {
            string content =

@"Start Node: Forest
    One day, deep in the forest ...
Links:
    Travel the Path => Mountain
    Stay Here => Forest

Start Node: Mountain
    One day, high in the mountains ...
Links:
    Climb higher! => Mountain
";
            NodeHelper.GetStartNode(content);
            Assert.Fail("Exception wasn't thrown; multiple start nodes is illegal!");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParsingThrowsExceptionIfLinkingNonExistentNode()
        {
            string content =

@"Start Node: Forest
    One day, deep in the forest ...
Links:
    Travel the Path => Mountain
    Stay Here => Forest";

            NodeHelper.GetStartNode(content);
            Assert.Fail("Exception wasn't thrown; there's no Mountain node!");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParsingMultipleNodesWithTheSameNameThrowsAnException()
        {
            string content =

@"Start Node: Forest
    One day, deep in the forest ...
Links:    
    Stay Here => Forest

Node: Forest
    One day, high in the mountains ...
Links:
    Climb higher! => Forest
";

            NodeHelper.GetStartNode(content);
            Assert.Fail("Exception wasn't thrown; two nodes with the same name is not legal!");
        }

        [TestMethod]
        public void ParsingAllowsNodesWithoutLinks()
        {
            string content = "Start Node: Forest\n\tOne day, deep in the forest ... \n\n";
            Node start = NodeHelper.GetStartNode(content);
            Assert.IsNotNull(start);
            Assert.AreEqual(0, start.Links.Count);
        }

        [TestMethod]
        public void ParsingCanProcessSingleNodeWithComment()
        {
            string content = "Start Node: Forest\n\tOne day, deep in the forest ... \n#This is a comment.\nLinks:\n\tRecurse => Forest";
            Node start = NodeHelper.GetStartNode(content);
            Assert.IsNotNull(start);
            Assert.AreEqual("Forest", start.Title);
            Assert.AreEqual("One day, deep in the forest ...\n\n", start.Content);
            Assert.AreEqual(1, start.Links.Count);
            Assert.AreEqual("Recurse", start.Links[0].Caption);
            Assert.AreEqual(start, start.Links[0].Target);
        }
    }
}