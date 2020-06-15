﻿/* 
 * Class: CodeClear.NaturalDocs.Engine.Tests.Framework.TextCommands
 * ____________________________________________________________________________
 * 
 * A base class for automated tests where the input files are text files interpreted line by line as commands.  They're 
 * loaded from a folder, interpreted by the derived class, and the output is saved to files and compared to other files 
 * containing the expected result.
 * 
 * The benefit of this approach is that you never have to hand code the output.  You can run the tests without
 *	 an expected output file, look over the actual output file, and if it's acceptable rename it to become the
 *	 expected output file.
 *		  
 * Usage:
 * 
 *		- Derive a class that has the [TestFixture] attribute.
 *		- Create a function with the [Test] attribute that calls <TestFolder()>, pointing it to the input files.
 *		- Define <OutputOf()> to convert the data to string output.
 *		- All files in the test folder in the format "[Test Name] - Input.[extension]" will be tested when NUnit runs.
 *		- A corresponding file "[Test Name] - Actual Output.txt" will be created for each one.
 *		- If it matches the contents of the file "[Test Name] - Expected Output.txt", the test will pass.  If it doesn't,
 *		  that file doesn't exist, or an exception was thrown, the test will fail.
 *		
 */

// This file is part of Natural Docs, which is Copyright © 2003-2020 Code Clear LLC.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CodeClear.NaturalDocs.Engine;


namespace CodeClear.NaturalDocs.Engine.Tests.Framework
	{
	public abstract class TextCommands
		{

		// Group: Functions
		// __________________________________________________________________________

		/* Constructor: TextCommands
		 */
		public TextCommands ()
			{
			engineInstanceManager = null;
			}


		/* Function: OutputOf
		 * 
		 * Override this function to interpret the lines and generate the output for the passed data.
		 * 
		 * You do not need to worry about catching exceptions unless the test is supposed to trigger them.  Uncaught exceptions
		 * will be handled automatically and cause the test to fail.  If the exception was intended as part of correct operation then 
		 * you must catch it to prevent this.
		 * 
		 * This function should not return null or an empty string as part of a successful test.  Doing so will cause the test to fail.
		 * If a test is supposed to generated no output, return a string such as "test successful" instead.
		 */
		public abstract string OutputOf (IList<string> lines);


		/* Function: TestFolder
		 * Tests all the input files contained in this folder.  See <EngineInstanceManager.Start()> for how relative paths are handled.
		 */
		public void TestFolder (Path testDataFolder, Path projectConfigFolder = default(Path))
			{
			TestList allTests = new TestList();
			
			engineInstanceManager = new EngineInstanceManager();
			engineInstanceManager.Start(testDataFolder, projectConfigFolder);

			// Store this so we can still use it for error messages after the engine is disposed of.
			Path inputFolder = engineInstanceManager.InputFolder;

			try
				{
				// Build a test for each input file we find
				string[] files = System.IO.Directory.GetFiles(inputFolder);

				foreach (string file in files)
					{
					if (Test.IsInputFile(file))
						{
						Test test = Test.FromInputFile(file);

						try
							{
							string[] input = System.IO.File.ReadAllLines(test.InputFile);
							test.SetActualOutput( OutputOf(input) );
							}
						catch (Exception e)
							{  test.TestException = e;  }

						test.Run();
						allTests.Add(test);
						}
					}
				}

			finally
				{  
				engineInstanceManager.Dispose();
				engineInstanceManager = null;
				}


			if (allTests.Count == 0)
				{  Assert.Fail("There were no tests found in " + inputFolder);  }
			else if (allTests.Passed == false)
				{  Assert.Fail(allTests.BuildFailureMessage());  }
			}


		// Group: Properties
		// __________________________________________________________________________

		public NaturalDocs.Engine.Instance EngineInstance
			{
			get
				{
				if (engineInstanceManager != null)
					{  return engineInstanceManager.EngineInstance;  }
				else
					{  return null;  }
				}
			}


		// Group: Variables
		// __________________________________________________________________________

		protected EngineInstanceManager engineInstanceManager;

		}
	}