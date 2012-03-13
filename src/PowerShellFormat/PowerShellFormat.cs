using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace PowerShellFormat
    {
    public static class ObjectEx
        {
        public static TextReader PowerShellFormat( this object obj, Action<PowerShell> block = null )
            {
            return obj.PowerShellFormat( 120, block );
            }

        public static TextReader PowerShellFormat(this object obj, int? maxWidth, Action<PowerShell> block = null )
            {
            DateTime s = DateTime.Now;
            var h = new Host( maxWidth );
            using ( Runspace rs = RunspaceFactory.CreateRunspace( h ) )
            using ( PowerShell ps = PowerShell.Create() )
                {
                rs.Open();
                ps.Runspace = rs;
                if (block != null)
                    block( ps );
                ps.AddCommand( "out-default" );
                ps.Commands.Commands[0].MergeMyResults( PipelineResultTypes.Error, PipelineResultTypes.Output );
                ps.Invoke( new object[] { obj } );
                return h.CreateReader();
                }
            }
        }

    internal class RawUserInterface : PSHostRawUserInterface
        {
        private readonly int maxWidth;

        public RawUserInterface(int maxWidth)
            {
            this.maxWidth = maxWidth;
            }

        public override ConsoleColor BackgroundColor
            {
            get { throw new NotImplementedException(); }
            set {}
            }

        public override Size BufferSize
            {
            get { return new Size( maxWidth, 24 ); }
            set { }
            }

        public override Coordinates CursorPosition
            {
            get { throw new NotImplementedException(); }
            set { }
            }

        public override int CursorSize
            {
            get { throw new NotImplementedException(); }
            set { }
            }

        public override ConsoleColor ForegroundColor
            {
            get { throw new NotImplementedException(); }
            set {}
            }

        public override bool KeyAvailable
            {
            get { throw new NotImplementedException(); }
            }

        public override Size MaxPhysicalWindowSize
            {
            get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
            }

        public override Size MaxWindowSize
            {
            get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
            }

        public override Coordinates WindowPosition
            {
            get { return new Coordinates(Console.WindowLeft, Console.WindowTop); }
            set { Console.SetWindowPosition(value.X, value.Y); }
            }

        public override Size WindowSize
            {
            get { return new Size(Console.WindowWidth, Console.WindowHeight); }
            set { Console.SetWindowSize(value.Width, value.Height); }
            }

        public override string WindowTitle
            {
            get { return Console.Title; }
            set { Console.Title = value; }
            }

        public override void FlushInputBuffer()
            {
            }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
            {
            throw new NotImplementedException();
            }

        public override KeyInfo ReadKey(ReadKeyOptions options)
            {
            throw new NotImplementedException();
            }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
            {
            throw new NotImplementedException();
            }

        public override void SetBufferContents( Coordinates origin, BufferCell[,] contents )
            {
            throw new NotImplementedException();
            }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
            {
            throw new NotImplementedException();
            }
        }

    internal class HostUserInterface : PSHostUserInterface
        {
        private readonly StringWriter output;
        private readonly RawUserInterface myRawUi;

        public HostUserInterface(StringWriter output, int maxWidth)
            {
            this.output = output;
            myRawUi = new RawUserInterface( maxWidth );
            }

        public override PSHostRawUserInterface RawUI
            {
            get { return this.myRawUi; }
            }

        public override Dictionary<string, PSObject> Prompt(
                                                        string caption, 
                                                        string message, 
                                                        System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
            {
            throw new NotImplementedException();
            }

        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
            {
            throw new NotImplementedException();
            }

        public override PSCredential PromptForCredential(
                                                         string caption, 
                                                         string message, 
                                                         string userName, 
                                                         string targetName)
            {
            throw new NotImplementedException();
            }

        public override PSCredential PromptForCredential(
                                                     string caption, 
                                                     string message, 
                                                     string userName, 
                                                     string targetName, 
                                                     PSCredentialTypes allowedCredentialTypes, 
                                                     PSCredentialUIOptions options)
            {
            throw new NotImplementedException();
            }

        public override string ReadLine()
            {
            throw new NotImplementedException();
            }

        public override System.Security.SecureString ReadLineAsSecureString()
            {
            throw new NotImplementedException();
            }

        public override void Write(string value)
            {
            this.output.Write( value );
            }

        public override void Write(
                               ConsoleColor foregroundColor, 
                               ConsoleColor backgroundColor, 
                               string value)
            {
            this.output.Write( value );
            }

        public override void WriteDebugLine(string message)
            {
            Console.WriteLine(String.Format(
                                      CultureInfo.CurrentCulture, 
                                      "DEBUG: {0}", 
                                      message));
            }

        public override void WriteErrorLine(string value)
            {
            Console.WriteLine(String.Format(
                                      CultureInfo.CurrentCulture, 
                                      "ERROR: {0}", 
                                      value));
            }

        public override void WriteLine()
            {
            this.output.WriteLine();
            }

        public override void WriteLine(string value)
            {
            this.output.WriteLine( value );
            }

        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
            {
            this.output.WriteLine(value);
            }

        public override void WriteProgress(long sourceId, ProgressRecord record)
            {
            }

        public override void WriteVerboseLine(string message)
            {
            }

        public override void WriteWarningLine(string message)
            {
            }
        }

    internal class Host : PSHost
        {
        private readonly CultureInfo        originalCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
        private readonly CultureInfo        originalUICultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;
        private readonly Guid               myId = Guid.NewGuid();
        private StringWriter                output = new StringWriter();
        private readonly HostUserInterface  hostUserInterface;

        public Host(int? maxWidth)
            {
            hostUserInterface = new HostUserInterface(output, maxWidth ?? 120 );
            }

        public override System.Globalization.CultureInfo CurrentCulture
            {
            get { return this.originalCultureInfo; }
            }

        public override System.Globalization.CultureInfo CurrentUICulture
            {
            get { return this.originalUICultureInfo; }
            }

        public override Guid InstanceId
            {
            get { return this.myId; }
            }

        public override string Name
            {
            get { return "PowerShellFormat"; }
            }

        public override PSHostUserInterface UI
            {
            get { return this.hostUserInterface; }
            }

        public override Version Version
            {
            get { return new Version(1, 0, 0, 0); }
            }

        public override void EnterNestedPrompt()
            {
            throw new NotImplementedException();
            }

        public override void ExitNestedPrompt()
            {
            throw new NotImplementedException();
            }

        public override void NotifyBeginApplication()
            {
            return;
            }

        public override void NotifyEndApplication()
            {
            return;
            }

        public override void SetShouldExit(int exitCode)
            { 
            }

        public TextReader CreateReader()
            {
            return new StringReader( this.output.ToString() );
            }
        }
    }

namespace PowerShellFormat.Test.data
{
    public abstract class MultilineText
    {
        public abstract string TransformText();
        public StringWriter GenerationEnvironment = new StringWriter();
        public static string Load()
        {
            return "a";
        }
        public void Write(string str)
        {
            GenerationEnvironment.Write(str);
        }
    }
}
