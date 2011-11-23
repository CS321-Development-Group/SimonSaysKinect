using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Kinect_Simon_Says
{
    public struct highscore
    {
        public string initials;
        public int score;
        public highscore(string _initials = "", int _score = 0)
        {
            initials = _initials;
            score = _score;
        }
    }
    class HighScores
    {
        List<highscore> highscores;
        public HighScores()
        {
            highscores = new List<highscore>();
            loadHighScores();
        }
        ~HighScores()
        {
            saveHighScores();
        }
        private void saveHighScores()
        {
            //save highscores to xml
            if (System.IO.File.Exists("HighScores.xml"))
            {
                System.IO.File.Delete("HighScores.xml");
            }
            XmlTextWriter Writer = new XmlTextWriter("HighScores.xml", null);
            Writer.WriteStartDocument();
            Writer.WriteStartElement("HighScores");
            Writer.WriteEndElement();
            Writer.Close();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("HighScores.xml");
            XmlElement subRoot;
            XmlElement Score;
            XmlElement Initials;
            XmlText score;
            XmlText initials;

            for (int i = 0; i < highscores.Count; i++)
            {
                subRoot = xmlDoc.CreateElement("HighScore");
                Score = xmlDoc.CreateElement("Score");
                Initials = xmlDoc.CreateElement("Initials");
                score = xmlDoc.CreateTextNode(highscores[i].score.ToString());
                initials = xmlDoc.CreateTextNode(highscores[i].initials.ToString());
                Score.AppendChild(score);
                Initials.AppendChild(initials);
                subRoot.AppendChild(Score);
                subRoot.AppendChild(Initials);
                xmlDoc.DocumentElement.AppendChild(subRoot);               
            }
            //save document
            xmlDoc.Save("HighScores.xml");

        }

        private void loadHighScores()
        {
            //Load high scores from xml
            if(System.IO.File.Exists("HighScores.xml"))
            {
                XmlNodeList xmlnodes;
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load("HighScores.xml");
                xmlnodes = xmldoc.SelectNodes("//HighScores/HighScore");
                foreach (XmlNode node in xmlnodes)
                {
                    addHighScore(XmlConvert.ToInt32(node["Score"].InnerText), node["Initials"].InnerText);
                }
            }
        }
        public void addHighScore(int _score, string _initials)
        {
            int count = highscores.Count;
            if (count == 0)
                highscores.Add(new highscore(_initials, _score));
            for(int i = 0; i < count; i++)
            {
                if (_score >= highscores[i].score)
                {
                    highscores.Insert(i, new highscore(_initials, _score));
                    if(highscores.Count > 10)
                        highscores.RemoveAt(count);
                    i = count + 1;
                }
                else if(count < 10 && i == count - 1)
                    highscores.Add(new highscore(_initials, _score));
            }            
        }
        public List<highscore> getHighScores()
        {
            return highscores;
        }
        public bool isHighScore(int _score)
        {
            bool retval = false;
            if (highscores.Count < 10 || _score >= highscores[highscores.Count - 1].score)
                retval = true;
            
            return retval;
        }
    }
}
