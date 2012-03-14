using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace PowerShellFormat.Test
{
    [TestFixture]
    public class TestPowerShellFormat
        {
        [Test]
        public void ShouldFormatNumber()
            {
            Assert.That(
                1.ToPS().ToString(), 
                Is.EqualTo( LoadResource( "PowerShellFormat.Test.data.FormatNumber.txt" ) ) ); 
            }

        [Test]
        public void ShouldFormatArray()
            {
            var array = new int[] { 1, 2, 3, 4 };
            Assert.That( 
                array.ToPS().ToString(), 
                Is.EqualTo( LoadResource( "PowerShellFormat.Test.data.FormatArray.txt") ) );
            }

        [Test]
        public void ShouldFormatSimpleDictionary()
            {
            var simpleDictionary = new Dictionary<string, string>() { { "a", "v1" }, { "b", "v2" } };
            Assert.That(
                simpleDictionary.ToPS().ToString( width: 15),
                Is.EqualTo(LoadResource("PowerShellFormat.Test.data.FormatSimpleDictionary.txt")) );
            }

        [Test]
        public void ShouldFormatDictionaryOfArrays()
            {
            var array = new int[] { 1, 2, 3, 4 };
            var dictionaryOfArrays = new Dictionary<string, int[]>() { { "a", array }, { "b", array } };
            Assert.That( 
                    dictionaryOfArrays.ToPS().ToString( width: 30 ), 
                    new IsEqualToContentOfResource( "FormatDictionaryOfArrays.txt" ) );
            }

        [Test]
        public void ShouldFormatAccordingToCommands()
            {
            var array = new int[] { 1, 2, 3, 4 };
            var dictionaryOfArrays = new Dictionary<string, int[]>() { { "a", array }, { "b", array } };
            Assert.That( 
                dictionaryOfArrays.ToPS().FormatTable( AutoSize: true ).ToString(), 
                 new IsEqualToContentOfResource( "FormatDictionaryOfArrays.txt" ) );
            }

        class IsEqualToContentOfResource : Constraint
            {
            private readonly string _resourceName;
            private string actualString;
            private string expectedString;

            public IsEqualToContentOfResource(string resourceName)
                {
                this._resourceName = resourceName;
                }

            public override bool Matches(object actual)
                {
                this.actual = actual;
                this.actualString = (string) actual;
                this.expectedString = LoadResource( "PowerShellFormat.Test.data." + this._resourceName );
                return this.actualString == this.expectedString;
                }

            public override void WriteDescriptionTo( MessageWriter writer )
                {
                writer.WriteExpectedValue( this.expectedString );
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


