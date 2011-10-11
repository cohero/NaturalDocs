﻿/*
	Include in output:

	This file is part of Natural Docs, which is Copyright © 2003-2011 Greg Valure.
	Natural Docs is licensed under version 3 of the GNU Affero General Public
	License (AGPL).  Refer to License.txt or www.naturaldocs.org for the
	complete details.

	This file may be distributed with documentation files generated by Natural Docs.
	Such documentation is not covered by Natural Docs' copyright and licensing,
	and may have its own copyright and distribution terms as decided by its author.

*/

"use strict";


/* Class: NDCore
	_____________________________________________________________________________

    Various helper functions to be used throughout the other scripts.
*/
var NDCore = new function ()
	{


	// Group: Selection Functions
	// ____________________________________________________________________________


	/* Function: GetElementsByClassName

		Returns an array of HTML elements matching the passed class name.  IE 8 and earlier don't have the native DOM function
		so this simulates it.

		The tag hint is used to help optimize the IE version since it uses getElementsByTagName and this will cut down the number
		of results it has to sift through.  However, you must remember that it's a hint and not a filter -- you can't rely on the results 
		only being elements of that tag type because it won't apply when using the native DOM function.
	*/
	this.GetElementsByClassName = function (baseElement, className, tagHint)
		{
		if (baseElement.getElementsByClassName)
			{  return baseElement.getElementsByClassName(className);  }
		
		if (!tagHint)
			{  tagHint = "*";  }

		var tagArray = baseElement.getElementsByTagName(tagHint);
		var matchArray = new Array();

		var tagIndex = 0;
		var matchIndex = 0;

		while (tagIndex < tagArray.length)
			{
			if (this.HasClass(tagArray[tagIndex], className))
				{
				matchArray[matchIndex] = tagArray[tagIndex];
				matchIndex++;
				}

			tagIndex++;
			}

		return matchArray;
		};



	// Group: Class Functions
	// ____________________________________________________________________________


	/* Function: HasClass
		Returns whether the passed HTML element uses the passed class.
	*/
	this.HasClass = function (element, targetClassName)
		{
		var index = element.className.indexOf(targetClassName);

		if (index != -1)
			{
			if ( (index == 0 || element.className.charAt(index - 1) == ' ') &&
				 (index + targetClassName.length == element.className.length ||
				  element.className.charAt(index + targetClassName.length) == ' ') )
				{  return true;  }
			}

		return false;
		};


	/* Function: AddClass
		Adds a class to the passed HTML element.
	*/
	this.AddClass = function (element, newClassName)
		{
		var index = element.className.indexOf(newClassName);

		if (index != -1)
			{
			if ( (index == 0 || element.className.charAt(index - 1) == ' ') &&
				 (index + newClassName.length == element.className.length ||
				  element.className.charAt(index + newClassName.length) == ' ') )
				{  return;  }
			}

		if (element.className.length == 0)
			{  element.className = newClassName;  }
		else
			{  element.className += " " + newClassName;  }
		};


	/* Function: RemoveClass
		Removes a class from the passed HTML element.
	*/
	this.RemoveClass = function (element, targetClassName)
		{
		var index = element.className.indexOf(targetClassName);

		while (index != -1)
			{
			if ( (index == 0 || element.className.charAt(index - 1) == ' ') &&
				 (index + targetClassName.length == element.className.length ||
				  element.className.charAt(index + targetClassName.length) == ' ') )
				{
				var newClassName = "";

				// We'll leave surrounding spaces alone.
				if (index > 0)
					{  newClassName += element.className.substr(0, index);  }
				if (index + targetClassName.length != element.className.length)
					{  newClassName += element.className.substr(index + targetClassName.length);  }

				element.className = newClassName;
				return;
				}

			index = element.className.indexOf(targetClassName, index + 1);
			}
		};



	// Group: Positioning Functions
	// ________________________________________________________________________


	/* Function: WindowClientWidth
		 A browser-agnostic way to get the window's client width.
	*/
	this.WindowClientWidth = function ()
		{
		var width = window.innerWidth;

		// Internet Explorer
		if (width === undefined)
			{  width = document.documentElement.clientWidth;  }

		return width;
		};


	/* Function: WindowClientHeight
		 A browser-agnostic way to get the window's client height.
	*/
	this.WindowClientHeight = function ()
		{
		var height = window.innerHeight;

		// Internet Explorer
		if (height === undefined)
			{  height = document.documentElement.clientHeight;  }

		return height;
		};


	/* Function: SetToAbsolutePosition
		Sets the element to the absolute position and size passed as measured in pixels.  This assumes the element is 
		positioned using fixed or absolute.  It accounts for all sizing weirdness so that the ending offsetWidth and offsetHeight
		will match what you passed regardless of any borders or padding.  If any of the coordinates are undefined it will be
		left alone.
	*/
	this.SetToAbsolutePosition = function (element, x, y, width, height)
		{
		var pxRegex = /^([0-9]+)px$/i;

		if (x != undefined && element.offsetLeft != x)
			{  element.style.left = x + "px";  }
		if (y != undefined && element.offsetTop != y)
			{  element.style.top = y + "px";  }
			
		// We have to use the non-standard (though universally supported) offsetWidth instead of the W3C-approved scrollWidth.
		// In all browsers offsetWidth returns the full width of the element in pixels including the border.  In Firefox and Opera 
		// scrollWidth will do the same, but in IE and WebKit it's instead equivalent to clientWidth which doesn't include the border.
		if (width != undefined && element.offsetWidth != width)
			{
			// If the width isn't already specified in pixels, set it to pixels.  We can't figure out the difference between the style
			// and offset widths otherwise.  This might cause an extra resize, but only the first time.
			if (!pxRegex.test(element.style.width))
				{  
				element.style.width = width + "px";  

				if (element.offsetWidth != width)
					{
					var adjustment = width - element.offsetWidth;
					element.style.width = (width + adjustment) + "px";
					}
				}
			else
				{  
				var styleWidth = RegExp.$1;
				var adjustment = styleWidth - element.offsetWidth;
				element.style.width = (width + adjustment) + "px";
				}
			}

		// Copypasta for height
		if (height != undefined && element.offsetHeight != height)
			{
			if (!pxRegex.test(element.style.height))
				{  
				element.style.height = height + "px";  

				if (element.offsetHeight != height)
					{
					var adjustment = height - element.offsetHeight;
					element.style.height = (height + adjustment) + "px";
					}
				}
			else
				{  
				var styleHeight = RegExp.$1;
				var adjustment = styleHeight - element.offsetHeight;
				element.style.height = (height + adjustment) + "px";
				}
			}
		};



	// Group: Hash and Path Functions
	// ________________________________________________________________________


	/* Function: SameHash
		Returns whether the two passed hashes are functionally the same.  The difference between this 
		and a straight string comparison is that "#", "", and undefined are equal.
	*/
	this.SameHash = function (hashA, hashB)
		{
		if (hashA === hashB)
			{  return true;  }

		if (hashA === "" || hashA === "#")
			{  hashA = undefined;  }
		if (hashB === "" || hashB === "#")
			{  hashB = undefined;  }

		return (hashA === hashB);
		};


	/* Function: IsFileHashPath
	*/
	this.IsFileHashPath = function (hashPath)
		{
		return (hashPath.match(/^File[0-9]*:/) != null);
		};

	/* Function: FileHashPathToContentPath
	*/
	this.FileHashPathToContentPath = function (hashPath)
		{
		var prefix = hashPath.match(/^File([0-9]*):/);
		var path = "files" + prefix[1] + "/" + hashPath.substr(prefix[0].length);

		var lastSeparator = path.lastIndexOf('/');
		var filename = path.substr(lastSeparator + 1);
		filename = filename.replace(/\./g, '-');
		
		return path.substr(0, lastSeparator + 1) + filename + ".html";
		};

	/* Function: FileHashPathToSummaryPath
	*/
	this.FileHashPathToSummaryPath = function (hashPath)
		{
		var prefix = hashPath.match(/^File([0-9]*):/);
		var path = "files" + prefix[1] + "/" + hashPath.substr(prefix[0].length);

		var lastSeparator = path.lastIndexOf('/');
		var filename = path.substr(lastSeparator + 1);
		filename = filename.replace(/\./g, '-');
		
		return path.substr(0, lastSeparator + 1) + filename + "-Summary.js";
		};

	/* Function: FileHashPathToSummaryToolTipsPath
	*/
	this.FileHashPathToSummaryToolTipsPath = function (hashPath)
		{
		var prefix = hashPath.match(/^File([0-9]*):/);
		var path = "files" + prefix[1] + "/" + hashPath.substr(prefix[0].length);

		var lastSeparator = path.lastIndexOf('/');
		var filename = path.substr(lastSeparator + 1);
		filename = filename.replace(/\./g, '-');
		
		return path.substr(0, lastSeparator + 1) + filename + "-SummaryToolTips.js";
		};



	// Group: Browser Functions
	// ________________________________________________________________________


	/* Function: IsIE
		Returns whether or not you're using Internet Explorer.  If you're going to use <IEVersion()> later, you might
		want to skip this call and test its result for undefined instead.
	*/
	this.IsIE = function ()
		{
		return (navigator.userAgent.indexOf("MSIE") != -1);
		};

	/* Function: IEVersion
		Returns the major IE version as an integer, or undefined if not using IE.
	*/
	this.IEVersion = function ()
		{
		var ieIndex = navigator.userAgent.indexOf("MSIE");

		if (ieIndex == -1)
			{  return undefined;  }
		else
			{
			// parseInt() allows random crap to appear after the numbers.  It will still interpret only the leading digit
			// characters at that location and return successfully.
			return parseInt(navigator.userAgent.substr(ieIndex + 5));
			}
		};

	/* Function: AddIEClassesToBody
		If the current browser is Internet Explorer 6 through 8, add IE6, IE7, or IE8 classes to HTML.body.  We're not 
		doing a more generalized thing like Natural Docs 1.x did because it's not generally good practice and none of 
		the other browsers should be broken enough to need it anymore.
	*/
	this.AddIEClassesToBody = function ()
		{
		var ieVersion = this.IEVersion();

		if (ieVersion >= 6 && ieVersion <= 8)  // 7 covers IE8 in IE7 compatibility mode
			{  this.AddClass(document.body, "IE" + ieVersion);  }
		};



	// Group: Prototype Functions
	// ________________________________________________________________________


	/* Function: ChangePrototypeToLongForm
		Changes the passed NDPrototype element to use the long form.  The prototype *must* be in the short form.
	*/
	this.ChangePrototypeToLongForm = function (prototype)
		{
		var newPrototype = document.createElement("div");
		newPrototype.id = prototype.id;
		newPrototype.className = prototype.className;

		this.RemoveClass(newPrototype, "ShortForm");
		this.AddClass(newPrototype, "LongForm");

		var table = prototype.firstChild;
		var newTable = document.createElement("table");
		newPrototype.appendChild(newTable);

		var newRow = newTable.insertRow(-1);
		newRow.appendChild(table.rows[0].cells[0].cloneNode(true));

		newRow = newTable.insertRow(-1);
		newRow.appendChild(table.rows[0].cells[1].cloneNode(true));

		newRow = newTable.insertRow(-1);
		newRow.appendChild(table.rows[0].cells[2].cloneNode(true));

		prototype.parentNode.replaceChild(newPrototype, prototype);
		};

	
	/* Function: ChangePrototypeToShortForm
		Changes the passed NDPrototype element to use the short form.  The prototype *must* be in the long form.
	*/
	this.ChangePrototypeToShortForm = function (prototype)
		{
		var newPrototype = document.createElement("div");
		newPrototype.id = prototype.id;
		newPrototype.className = prototype.className;

		this.RemoveClass(newPrototype, "LongForm");
		this.AddClass(newPrototype, "ShortForm");

		var table = prototype.firstChild;
		var newTable = document.createElement("table");
		newPrototype.appendChild(newTable);

		var newRow = newTable.insertRow(-1);
		newRow.appendChild(table.rows[0].cells[0].cloneNode(true));
		newRow.appendChild(table.rows[1].cells[0].cloneNode(true));
		newRow.appendChild(table.rows[2].cells[0].cloneNode(true));

		prototype.parentNode.replaceChild(newPrototype, prototype);
		};

	};



// Section: Extension Functions
// ____________________________________________________________________________


/* Function: String.StartsWith
	Returns whether the string starts with or is equal to the passed string.
*/
String.prototype.StartsWith = function (other)
	{
	if (other === undefined)
		{  return false;  }

	return (this.length >= other.length && this.substr(0, other.length) == other);
	};