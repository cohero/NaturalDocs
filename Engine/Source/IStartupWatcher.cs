﻿/* 
 * Interface: CodeClear.NaturalDocs.Engine.IStartupWatcher
 * ____________________________________________________________________________
 * 
 * An interface for any class that wants to watch events that occur during initialization.
 * 
 */

// This file is part of Natural Docs, which is Copyright © 2003-2020 Code Clear LLC.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;


namespace CodeClear.NaturalDocs.Engine
	{
	public interface IStartupWatcher
		{
		
		/* Function: OnStartPossiblyLongOperation
		 * Called whenever an operation is starting that *might* take a long time.  In some cases it will be over instantaneously,
		 * in others it could cause a significant, user-noticeable delay.  The operation name will be one of the following, but you
		 * must design the watcher code for any possibility because future versions may add new operation names:
		 * 
		 *    "PurgingOutputWorkingData" - Output temporary files are being deleted.  If there are a lot of them, such as many folders
		 *																full of partial PDF files, this can take a long time.
		 *		 "PurgingOutputFiles" - Output files are being deleted.  If there are a lot of them, such as many folders full of HTML files,
		 *												 this can take a long time.
		 */
		void OnStartPossiblyLongOperation (string operationName);
		
		/* Function: OnEndPossiblyLongOperation
		 * Called when the last operation specified with <OnStartPossiblyLongOperation()> ends.
		 */
		void OnEndPossiblyLongOperation ();
		
		}
	}