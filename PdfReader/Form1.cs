using System;
using System.Text;
using System.Windows.Forms;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig;
using System.Drawing;
using System.Collections.Generic;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using UglyToad.PdfPig.Content;
using System.Linq;
using UglyToad.PdfPig.Graphics;
using SixLabors.Fonts;
using TextRenderer = System.Windows.Forms.TextRenderer;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace PdfReader
{
    public partial class Form1 : Form
    {
        List<String> pages= new List<String>();
        string pageText = "";
        int page = 0;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            pageText = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var document = PdfDocument.Open(openFileDialog.FileName);
                foreach (Page page in document.GetPages())
                {
                 
                    pageText+=page.Text;
                    
                }
            }
            richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, richTextBox1.Font.Size, FontStyle.Regular);
            FillPages(richTextBox1,pageText);
        }


        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, richTextBox1.Font.Size - 1, FontStyle.Regular);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Font = new Font(richTextBox1.Font.FontFamily, richTextBox1.Font.Size +1 , FontStyle.Regular);
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(page-1<0)
            {
                return;
            }
            richTextBox1.Text = pages[--page];
            label1.Text = page.ToString();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(page+1>pages.Count-1)
            {
                return;
            }
            richTextBox1.Text = pages[++page];
            label1.Text = page.ToString();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_FontChanged(object sender, EventArgs e)
        {
            FillPages(richTextBox1,pageText);
        }



        private int CalculateCharsPerLine(RichTextBox richTextBox)
        {
            using (Graphics g = richTextBox.CreateGraphics())
            {
               
                float charSize = richTextBox.Font.Size;
                int charsPerLine = (int)(richTextBox.Width / (charSize/1.3333));
                return charsPerLine;
            }
        }

        private int CalculateLinesPerPage(RichTextBox richTextBox)
        {
            using (Graphics g = richTextBox.CreateGraphics())
            {
                SizeF charSize = g.MeasureString("a", richTextBox.Font);
                int charsPerLine = (int)(richTextBox.Width / charSize.Width);
                int linesPerPage = (int)(richTextBox.Height / charSize.Height);
                return linesPerPage;
            }
        }


        private void FillPages(RichTextBox richTextBox, string text)
        {
            pages.Clear();
            int linesPerPage = CalculateLinesPerPage(richTextBox);
            int charactersPerLine = CalculateCharsPerLine(richTextBox);

            List<string> lines = new List<string>();
            string[] words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder currentLine = new StringBuilder();

            foreach (string word in words)
            {
                if (currentLine.Length + word.Length + 1 <= charactersPerLine) // Учитываем пробел
                {
                    currentLine.Append(word);
                    currentLine.Append(" ");
                }
                else
                {
                    lines.Add(currentLine.ToString().Trim());
                    currentLine.Clear();
                    currentLine.Append(word);
                    currentLine.Append(" ");
                }
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString().Trim());
            }

            int currentPage = 1;
            int currentLines = 0;
            StringBuilder currentPageText = new StringBuilder();
            foreach (string line in lines)
            {
                if (currentLines < linesPerPage)
                {
                    currentPageText.AppendLine(line);
                    currentLines++;
                }
                else
                {
                    pages.Add($"{Environment.NewLine}{currentPageText.ToString()}{Environment.NewLine}");
                    currentPage++;
                    currentPageText.Clear();
                    currentPageText.AppendLine(line);
                    currentLines = 1; // Учитываем первую строку новой страницы
                }
            }

           
            pages.Add($"{Environment.NewLine}{currentPageText.ToString()}{Environment.NewLine}");
            if(pages.Count<=page)
            {
                page -= 1;
            }
            richTextBox1.Text = pages[page];
            label1.Text = page.ToString();
            label2.Text = "/";
            label2.Text += currentPage-1;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if(pageText.Length > 0) {
                FillPages(richTextBox1, pageText);
            }
            else
            {
                return;
            }
           
        }
    }
}
