using System;
using System.Net;
using System.Collections.Generic;

namespace DeenGames.ThePrisoner.Model
{
    public class CoreModel
    {
        private static CoreModel instance = new CoreModel();

        public static CoreModel Instance { get { return instance; } }

        public DateTime StartTime { get; set; }
        public IList<string> ChoicesTaken { get; set; }
    }
}
