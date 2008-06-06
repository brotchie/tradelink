using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

using Microsoft.CSharp;

namespace Tript
{
	public class CSScriptCompiler
    { // this class came from the article http://www.codeproject.com/KB/cs/CSharpScript.aspx


        public static void RunScript(string scriptfile, params string[] args)
        {
            StreamReader sr = new StreamReader(scriptfile);
            RunScript(sr, args);
        }

        public static void RunScript(Stream data, params string[] args)
        {
            RunScript(new StreamReader(data), args);
        }

		public static void RunScript(StreamReader stream, params string[] args)
		{
			StreamReader sr = stream;	// Reads the script file
			List<string> usings = new List<string>();				// #using directives
			List<string> imports = new List<string>();			// #import directives
			StringWriter source = new StringWriter();				// For writing script file
			bool HasMain = false;														// Is there a Main routine
			StringWriter finalsource = new StringWriter();	// For building source code

			// Parse the source file
			while (sr.Peek() != -1)
			{
				string line = sr.ReadLine().Trim();
				// Check for certain directives
				if (line.StartsWith("#using"))
				{
					if (line.Substring(line.Length - 1, 1) == ";")
						line = line.Remove(line.Length - 1, 1);
					usings.Add(line.Substring(6).Trim());
				}
				else if (line.StartsWith("#import"))
				{
					imports.Add(line.Substring(7).Trim());
				}
				else if (line.StartsWith("#include"))
				{
					int posStart = line.IndexOf('"') + 1;
					int posEnd = line.LastIndexOf('"');
					line = line.Substring(posStart, posEnd - posStart);
					// If relative then convert to absolute based on script file
					if (!Path.IsPathRooted(line))
						line = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), line);
					// Open the file for reading and write it to the stream
					using (StreamReader includefile = new StreamReader(line))
					{
						while (includefile.Peek() != -1)
							source.WriteLine(includefile.ReadLine());
						includefile.Close();
						source.Flush();
					}
				}
				else
				{
					source.WriteLine(line);
					// Check for the main routine (hopefully the regex is ok :) )
					// Can't use HasMain = Regex... because this will be evaluated everyline
					if (Regex.IsMatch(line, @"^\s*((public|protected|internal|private)\s+)?(internal\s+)?(static){1}\s+(int|void)\s+(Main)\s*\(\s*((string|System.String)\[\]\s+\w+)?\s*\)\s*(\{)?\s*$", (RegexOptions.Singleline | RegexOptions.Compiled)))
					{
						HasMain = true;
					}
				}
			}

			// Close the streams we don't need
			sr.Close();
			sr.Dispose();

			// Add default namespaces and imports from css.exe.settings.xml file
			try
			{

				XmlDocument dom = new XmlDocument();
				dom.Load(Path.ChangeExtension(Application.ExecutablePath, ".exe.settings.xml"));
				// Load namespaces
				foreach (XmlElement elUsing in dom["SETTINGS"]["DEFAULTNAMESPACES"].GetElementsByTagName("NAMESPACE"))
				{
					if (!usings.Contains(elUsing.InnerText))
						usings.Add(elUsing.InnerText);
				}
				// Load dlls
				foreach (XmlElement elDll in dom["SETTINGS"]["DEFAULTIMPORTS"].GetElementsByTagName("DLL"))
				{
					if (!imports.Contains(elDll.InnerText))
						imports.Add(elDll.InnerText);
				}
			}
			catch
			{
				;	// Do Nothing, just to bomb out if no file exists or incorrectly
				// formatted xml file, nasty, but you should know what you're doing.
			}

			// Build the script file
			int prefixedLines = 0;
			//  Print the usings
			foreach (string use in usings)
			{
				finalsource.WriteLine("using {0};", use);
				prefixedLines++;
			}
			//  Print the namespace
			finalsource.WriteLine("namespace PiLogic.Utilities.CSharp.Script\n{");
			prefixedLines += 2;
			//  Print the class
			finalsource.WriteLine("class ScriptFile\n{");
			prefixedLines += 2;
			//  Decide if we need to encapsulte inside a "Main"
			if (HasMain == false)
			{
				finalsource.WriteLine("static void Main(string[] args)\n{");
				prefixedLines += 2;
			}
			//  Dump the entire contents of the parsed file
			finalsource.WriteLine(source.ToString());
			//  We can now close the source stream
			source.Close();
			source.Dispose();
			// Close the main if required
			if (HasMain == false)
			{
				finalsource.WriteLine("}");
			}
			// Close the class and namespace
			finalsource.WriteLine("}\n}");
			// Generate the compiler options
			CompilerParameters cps = new CompilerParameters();
			cps.GenerateExecutable = true;
			cps.GenerateInMemory = true;		// Don't know what this is for because it creates a file anyway :(
			// Add dlls
			foreach (string dll in imports)
				cps.ReferencedAssemblies.Add(dll);
			// Compile the source code
			CompilerResults cr = new CSharpCodeProvider().CompileAssemblyFromSource(cps, finalsource.ToString());
			// Close the final stream
			finalsource.Close();
			finalsource.Dispose();
			// Check for errors
			if (cr.Errors.Count > 0)
			{
				// Has errors so display them
				foreach (CompilerError ce in cr.Errors)
				{
					// Check to see if the error was in the prefixed code or in the script file.
					if ((ce.Line - prefixedLines) < 0)
					{
						Console.WriteLine("Error in Source Code, most likely cause is that a ; was not added at the end of a directive line.\nError = {0}", ce.ToString());
					}
					else
					{
						Console.WriteLine("{4} [{0}] at Line {1} Column {2}: {3}", ce.ErrorNumber, ce.Line - prefixedLines, ce.Column, ce.ErrorText, (ce.IsWarning) ? "Warning" : "Error");
					}
				}
				//Console.Write("Press any key to continue...");
				//Console.ReadKey();
			}
			else
			{
				try
				{
					// Check to see if Main expects arguments and then invoke it
					if (cr.CompiledAssembly.EntryPoint.GetParameters().Length == 0)
					{
						cr.CompiledAssembly.EntryPoint.Invoke(null, null);
					}
					else
					{
						cr.CompiledAssembly.EntryPoint.Invoke(null, new object[] { args });
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.ReadLine();
				}
			}
		}

        // NOT USING THE FOLLOWING ANYMORE 
        /*
        private static void Main(string[] args)
        {
            // If no arguments then display error
            if ((args.Length == 0) || (!File.Exists(args[0])))
            {
                Console.WriteLine("A script file must be provided on the command-line.");
            }
            else
            {
                // Run the script (we'll leave the first argument so 
                // that the script knows where it is :)
                CSScriptCompiler.RunScript(args[0], args);
            }
        }
         */
	}
}