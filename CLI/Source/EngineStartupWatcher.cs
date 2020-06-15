﻿/* 
 * Class: CodeClear.NaturalDocs.CLI.EngineStartupWatcher
 * ____________________________________________________________________________
 * 
 * A class that handles events that may occur during engine initialization.
 */

// This file is part of Natural Docs, which is Copyright © 2003-2020 Code Clear LLC.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;


namespace CodeClear.NaturalDocs.CLI
	{
	public class EngineStartupWatcher : Engine.IStartupWatcher
		{
		
		// Group: Functions
		// __________________________________________________________________________
		
		public EngineStartupWatcher ()
			{
			status = new StatusManagers.PossiblyLongStartupOperation();
			}

		public void OnStartPossiblyLongOperation (string operationName)
			{
			status.Start(operationName);
			}
		
		public void OnEndPossiblyLongOperation ()
			{
			status.End();
			}
		

		// Group: Variables
		// __________________________________________________________________________
		
		protected StatusManagers.PossiblyLongStartupOperation status;
		
		}
	}