using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Spectre.Console.Rendering;

namespace Spectre.Console.Testing
{
    public sealed class FakeConsole : IAnsiConsole, IDisposable
    {
        public Profile Profile { get; }
        public IAnsiConsoleCursor Cursor => new FakeAnsiConsoleCursor();
        public FakeConsoleInput Input { get; }

        IAnsiConsoleInput IAnsiConsole.Input => Input;
        public RenderPipeline Pipeline { get; }

        public Decoration Decoration { get; set; }
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public string Link { get; set; }

        public StringWriter Writer { get; }
        public string Output => Writer.ToString();
        public IReadOnlyList<string> Lines => Output.TrimEnd('\n').Split(new char[] { '\n' });

        public FakeConsole(
            int width = 80, int height = 9000, Encoding encoding = null,
            bool supportsAnsi = true, ColorSystem colorSystem = ColorSystem.Standard,
            bool legacyConsole = false, bool interactive = true)
        {
            Writer = new StringWriter();
            Input = new FakeConsoleInput();
            Pipeline = new RenderPipeline();

            Profile = new Profile("Fake console", Writer, encoding ?? Encoding.UTF8);
            Profile.Width = width;
            Profile.Height = height;
            Profile.ColorSystem = colorSystem;
            Profile.Capabilities.Ansi = supportsAnsi;
            Profile.Capabilities.Legacy = legacyConsole;
            Profile.Capabilities.Interactive = interactive;
            Profile.Capabilities.Links = true;
        }

        public void Dispose()
        {
            Writer.Dispose();
        }

        public void Clear(bool home)
        {
        }

        public void Write(IEnumerable<Segment> segments)
        {
            if (segments is null)
            {
                return;
            }

            foreach (var segment in segments)
            {
                Writer.Write(segment.Text);
            }
        }

        public string WriteNormalizedException(Exception ex, ExceptionFormats formats = ExceptionFormats.Default)
        {
            this.WriteException(ex, formats);
            return string.Join("\n", Output.NormalizeStackTrace()
                .NormalizeLineEndings()
                .Split(new char[] { '\n' })
                .Select(line => line.TrimEnd()));
        }
    }
}
