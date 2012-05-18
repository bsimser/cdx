using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CdxLib.GamerServices.HighScore
{
    public class HighScores
    {
        private Dictionary<string, HighScoreTable> _highscoreTables;

        /// <summary>
        ///   Class constructor.
        /// </summary>
        public HighScores()
        {
            // Set default property values
            FileName = "Scores.dat";

            // Initialize the highscore tables
            Clear();
        }

        /// <summary>
        ///   The filename to and from which the highscore data will be written.
        ///   This can be either a fully specified path and filename, or just
        ///   a filename alone (in which case the file will be written to the
        ///   game engine assembly directory).
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///   Initialize a named high score table
        /// </summary>
        /// <param name = "tableName">The name for the table to initialize</param>
        /// <param name = "tableSize">The number of entries to store in this table</param>
        public void InitializeTable(string tableName, int tableSize)
        {
            // Delegate to the other version of this function
            InitializeTable(tableName, tableSize, "");
        }

        /// <summary>
        ///   Initialize a named high score table
        /// </summary>
        /// <param name = "tableName">The name for the table to initialize</param>
        /// <param name = "tableSize">The number of entries to store in this table</param>
        /// <param name = "tableDescription">A description of this table to show to the player</param>
        public void InitializeTable(string tableName, int tableSize, string tableDescription)
        {
            if (!_highscoreTables.ContainsKey(tableName))
            {
                _highscoreTables.Add(tableName, new HighScoreTable(tableSize, tableDescription));
            }
        }

        /// <summary>
        ///   Retrieve the high score table with the specified name
        /// </summary>
        /// <param name = "tableName"></param>
        /// <returns></returns>
        public HighScoreTable GetTable(string tableName)
        {
            return _highscoreTables.ContainsKey(tableName) ? _highscoreTables[tableName] : null;
        }

        /// <summary>
        ///   Remove all high score tables from the object
        /// </summary>
        /// <remarks>
        ///   To clear the scores for an individual table, retrieve the
        ///   table object using GetTable and call the Clear method on that instead.
        /// </remarks>
        public void Clear()
        {
            // Create the table dictionary if it doesn't already exist
            if (_highscoreTables == null)
            {
                _highscoreTables = new Dictionary<string, HighScoreTable>();
            }

            // Tell any known tables to clear their content
            foreach (var table in _highscoreTables.Values)
            {
                table.Clear();
            }
        }

        /// <summary>
        ///   Load the high scores from the storage file
        /// </summary>
        /// <remarks>
        ///   Ensure that the tables have been created using InitializeTable
        ///   prior to loading the scores.
        /// </remarks>
        public void LoadScores()
        {
            // Just in case we have any problems...
            try
            {
                // Clear any existing scores
                Clear();

                // Get access to the isolated storage
                string fileContent;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.FileExists(FileName))
                    {
                        // The score file doesn't exist
                        return;
                    }
                    // Read the contents of the file
                    using (var sr = new StreamReader(store.OpenFile(FileName, FileMode.Open)))
                    {
                        fileContent = sr.ReadToEnd();
                    }
                }

                // Parse the content XML that was loaded
                var xDoc = XDocument.Parse(fileContent);
                // Create a query to read the score details from the xml
                if (xDoc.Root != null)
                {
                    var result = from c in xDoc.Root.Descendants("entry")
                                 select new
                                            {
                                                TableName = c.Parent.Parent.Element("name").Value,
                                                Name = c.Element("name").Value,
                                                Score = c.Element("score").Value,
                                                Date = c.Element("date").Value
                                            };
                    // Loop through the resulting elements
                    foreach (var el in result)
                    {
                        // Add the entry to the table.
                        var table = GetTable(el.TableName);
                        if (table != null) table.AddEntry(el.Name, int.Parse(el.Score), DateTime.Parse(el.Date));
                    }
                }
            }
            catch
            {
                // A problem occurred, but don't re-throw the exception or the
                // user won't be able to relaunch the game. Instead just ignore
                // the error and carry on regardless.
                // We will ensure that a partial load hasn't taken place however
                // which could cause unexpected problems, we'll reset back to defaults
                // instead.
                Clear();
            }
        }

        /// <summary>
        ///   Save the scores to the storage file
        /// </summary>
        public void SaveScores()
        {
            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb);

            // Begin the document
            xmlWriter.WriteStartDocument();
            // Write the HighScores root element
            xmlWriter.WriteStartElement("highscores");

            // Loop for each table
            foreach (var tableName in _highscoreTables.Keys)
            {
                // Retrieve the table object for this table name
                var table = _highscoreTables[tableName];

                // Write the Table element
                xmlWriter.WriteStartElement("table");
                // Write the table Name element
                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteString(tableName);
                xmlWriter.WriteEndElement(); // name

                // Create the Entries element
                xmlWriter.WriteStartElement("entries");

                // Loop for each entry
                foreach (var entry in table.Entries.Where(entry => entry.Date != DateTime.MinValue))
                {
                    // Write the Entry element
                    xmlWriter.WriteStartElement("entry");
                    // Write the score, name and date
                    xmlWriter.WriteStartElement("score");
                    xmlWriter.WriteString(entry.Score.ToString());
                    xmlWriter.WriteEndElement(); // score
                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteString(entry.Name);
                    xmlWriter.WriteEndElement(); // name
                    xmlWriter.WriteStartElement("date");
                    xmlWriter.WriteString(entry.Date.ToString("yyyy-MM-ddTHH:mm:ss"));
                    xmlWriter.WriteEndElement(); // date
                    // End the Entry element
                    xmlWriter.WriteEndElement(); // entry
                }

                // End the Entries element
                xmlWriter.WriteEndElement(); // entries

                // End the Table element
                xmlWriter.WriteEndElement(); // table
            }

            // End the root element
            xmlWriter.WriteEndElement(); // highscores
            xmlWriter.WriteEndDocument();

            // Close the xml writer, which will put the finished document into the stringbuilder
            xmlWriter.Close();

            // Get access to the isolated storage
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Create a file and attach a streamwriter
                using (var sw = new StreamWriter(store.CreateFile(FileName)))
                {
                    // Write the XML string to the streamwriter
                    sw.Write(sb.ToString());
                }
            }
        }
    }
}