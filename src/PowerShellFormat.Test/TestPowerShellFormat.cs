using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using PowerShellFormat.Test.data;

namespace PowerShellFormat.Test
{

    
    [TestFixture]
    public class TestPowerShellFormat
        {
        [Test]
        public void ShouldFormatNumber()
            {
            Assert.That(
                1.PowerShellFormat().ReadToEnd(), 
                Is.EqualTo( LoadResource( "PowerShellFormat.Test.data.FormatNumber.txt" ) ) ); 
            }

        [Test]
        public void ShouldFormatArray()
            {
            var array = new int[] { 1, 2, 3, 4 };
            Assert.That( 
                array.PowerShellFormat().ReadToEnd(), 
                Is.EqualTo( LoadResource( "PowerShellFormat.Test.data.FormatArray.txt") ) );
            }

        [Test]
        public void ShouldFormatSimpleDictionary()
            {
            var simpleDictionary = new Dictionary<string, string>() { { "a", "v1" }, { "b", "v2" } };
            Assert.That(
                simpleDictionary.PowerShellFormat( 15 ).ReadToEnd(),
                Is.EqualTo(LoadResource("PowerShellFormat.Test.data.FormatSimpleDictionary.txt")) );
            }

        [Test]
        public void ShouldFormatDictionaryOfArrays()
            {
            var array = new int[] { 1, 2, 3, 4 };
            var dictionaryOfArrays = new Dictionary<string, int[]>() { { "a", array }, { "b", array } };
            Assert.That( 
                    dictionaryOfArrays.PowerShellFormat( 20 ).ReadToEnd(), 
                    Is.EqualTo( LoadResource( "PowerShellFormat.Test.data.FormatDictionaryOfArrays.txt" ) ).NoClip );
            }

        [Test]
        public void ShouldFormatAccordingToSpecification()
            {
            var array = new int[] { 1, 2, 3, 4 };
            var dictionaryOfArrays = new Dictionary<string, int[]>() { { "a", array }, { "b", array } };
            Assert.That( 
                dictionaryOfArrays, 
                new IsFormatted( to: "FormatDictionaryOfArrays.txt", @using: ps=>ps.AddCommand( "Format-Table").AddParameter( "AutoSize") ) );
            }

        class IsFormatted : Constraint
            {
            private string to;
            private Action<PowerShell> @using;
            private string actualString;
            private string expectedString;

            public IsFormatted( string to, Action<PowerShell> @using )
                {
                this.to = to;
                this.@using = @using;
                }

            public override bool Matches(object actual)
                {
                this.actual = actual;
                this.actualString = actual.PowerShellFormat( this.@using ).ReadToEnd();
                this.expectedString = LoadResource( "PowerShellFormat.Test.data." + this.to );
                return this.actualString == this.expectedString;
                }

            public override void WriteDescriptionTo( MessageWriter writer )
                {
                writer.WriteExpectedValue( this.expectedString );
                }

            public override void WriteMessageTo(MessageWriter writer)
                {
                base.WriteMessageTo( writer );
                }

            public override void WriteActualValueTo(MessageWriter writer)
                {
                writer.WriteActualValue( this.actualString );    
                }
            }
//            
//            
//            var dictionaryOfArrays = new Dictionary<string, int[]>() { { "a", array }, { "b", array } };
//            var anonymousObj = new { a = 1, b = 2, c = 3 };
//            Console.WriteLine( numberOne.PowerShellFormat().ReadToEnd() );
//            Console.WriteLine( array.PowerShellFormat().ReadToEnd() );
//            Console.WriteLine( simpleDictionary.PowerShellFormat().ReadToEnd()); 
//            Console.WriteLine( dictionaryOfArrays.PowerShellFormat().ReadToEnd());
//            Console.WriteLine( anonymousObj.PowerShellFormat().ReadToEnd());
//            Console.WriteLine( anonymousObj.PowerShellFormat().ReadToEnd());
//            Console.WriteLine( new SomeObject().PowerShellFormat().ReadToEnd());
//            Console.WriteLine( new SomeObject().PowerShellFormat( "Format-List").ReadToEnd());

//            new[] { 1, 2 }.PowerShellFormat();

        private static string LoadResource( string resourceName ) 
            {
            return new StreamReader( Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName) ).ReadToEnd();
            }

        }

    class SomeObject
        {
        public string a = "a";
        public string b = "b";
        }

    class SomeObject2
        {
        public string a = "a";
        public Dictionary<string, int[]> b = new Dictionary<string, int[]>{ { "a", new []{ 1, 2, 3 }}, { "b", new []{1, 2, 3} } };
        }

}


