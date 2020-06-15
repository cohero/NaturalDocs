/* 
 * Class: CodeClear.NaturalDocs.Engine.Config.Targets.IgnoredSourceFolder
 * ____________________________________________________________________________
 * 
 * The configuration of an ignored source folder.
 */

// This file is part of Natural Docs, which is Copyright © 2003-2020 Code Clear LLC.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;
using CodeClear.NaturalDocs.Engine.Config;
using CodeClear.NaturalDocs.Engine.Errors;


namespace CodeClear.NaturalDocs.Engine.Config.Targets
	{
	public class IgnoredSourceFolder : FilterBase
		{
		
		// Group: Functions
		// __________________________________________________________________________
		
		
		public IgnoredSourceFolder (PropertyLocation propertyLocation) : base (propertyLocation)
			{
			folder = null;
			folderPropertyLocation = Source.NotDefined;
			}

		public IgnoredSourceFolder (IgnoredSourceFolder toCopy) : base (toCopy)
			{
			folder = toCopy.folder;
			folderPropertyLocation = toCopy.folderPropertyLocation;
			}

		public override FilterBase Duplicate ()
			{
			return new IgnoredSourceFolder(this);
			}

		public override bool Validate (Errors.ErrorList errorList, int targetIndex)
			{
			if (System.IO.Directory.Exists(folder) == false)
				{  
				errorList.Add( 
					Locale.Get("NaturalDocs.Engine", "Project.txt.IgnoredSourceFolderDoesNotExist(folder)", folder),
					folderPropertyLocation,
					"FilterTargets[" + targetIndex + "].Folder" );

				return false;
				}

			return true;
			}



		// Group: Properties
		// __________________________________________________________________________


		/* Property: Folder
		 * The <Path> that should have its contents ignored.
		 */
		public Path Folder
		    {
		    get
		        {  return folder;  }
			set
				{  folder = value;  }
		    }


		
		// Group: Property Locations
		// __________________________________________________________________________
		
					
		/* Property: FolderPropertyLocation
		 * Where <Folder> is defined.
		 */
		public PropertyLocation FolderPropertyLocation
		    {
		    get
		        {  return folderPropertyLocation;  }
		    set
		        {  folderPropertyLocation = value;  }
		    }

	
		
		// Group: Variables
		// __________________________________________________________________________
		

		protected Path folder;

		protected PropertyLocation folderPropertyLocation;

		}
	}