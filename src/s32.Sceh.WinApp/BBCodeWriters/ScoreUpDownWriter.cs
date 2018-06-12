using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using s32.Sceh.DataModel;

namespace s32.Sceh.WinApp.BBCodeWriters
{
    public class ScoreUpDownWriter : IBBCodeWriter
    {
        private static readonly Regex ScoreRe = new Regex(@"^\[([+-]?[0-9]+)\] ?", RegexOptions.None);

        public int Score { get; set; }

        public bool CanWrite(List<SteamApp> steamApps)
        {
            return true;
        }

        public void Write(TextBox tbEditor, List<SteamApp> steamApps)
        {
            var text = tbEditor.Text;
            int score;
            if (text.Length > 0)
            {
                int pos = tbEditor.CaretIndex;
                if (pos >= text.Length)
                    pos = text.Length - 1;
                int left = text.LastIndexOf('\n', pos) + 1;
                int right = text.IndexOf('\n', pos);
                if (right < left)
                    right = text.Length;

                var match = ScoreRe.Match(text, left, right - left);
                
                if (match.Success)
                {
                    score = Int32.Parse(match.Groups[1].Value);
                    score += Score;
                    tbEditor.SelectionStart = match.Index;
                    tbEditor.SelectionLength = match.Length;
                }
                else
                {
                    score = Score;
                    tbEditor.CaretIndex = left;
                }
            }
            else
            {
                score = Score;
            }

            if (score == 0)
            {
                tbEditor.SelectedText = String.Empty;
            }
            else
            {
                tbEditor.SelectedText = String.Format("[{0:+0;-0}] ", score);
                tbEditor.CaretIndex = tbEditor.SelectionStart + tbEditor.SelectionLength;
            }
        }
    }
}
