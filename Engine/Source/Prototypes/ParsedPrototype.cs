﻿/* 
 * Class: CodeClear.NaturalDocs.Engine.Prototypes.ParsedPrototype
 * ____________________________________________________________________________
 * 
 * A class that wraps a <Tokenizer> for a prototype that's been marked with <PrototypeParsingTypes>, providing easier 
 * access to things like parameter lines.
 * 
 * Usage:
 * 
 *		The functions and properties obviously rely on the relevant tokens being set.  You cannot expect a proper result from
 *		<GetParameter()> or <NumberOfParameters> unless the tokens are marked with <PrototypeParsingType.StartOfParams>,
 *		<PrototypeParsingType.ParamSeparator>, etc.  Likewise, you can't get anything from <GetParameterName()> unless
 *		you also have tokens marked with <PrototypeParsingType.Name>.  However, you can set the parameter divider tokens,
 *		call <GetParameter()>, and then use those bounds to further parse the parameter and set tokens like
 *		<PrototypeParsingType.Name>.
 * 
 *		Section and parameter divisions are not calculated on the fly.  They are calculated once at object creation and then saved.
 *		If you make changes to section or parameter delimiting tokens call <RecalculateSections()> to make sure the changes are
 *		reflected in the other functions.
 */

// This file is part of Natural Docs, which is Copyright © 2003-2020 Code Clear LLC.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;
using System.Collections.Generic;
using CodeClear.NaturalDocs.Engine.Tokenization;


namespace CodeClear.NaturalDocs.Engine.Prototypes
	{
	public class ParsedPrototype
		{

		// Group: Types
		// __________________________________________________________________________


		/* Enum: ParameterStyle
		 * 
		 * C - A C-style prototype with parameters in a form similar to "int x = 12".
		 * Pascal - A Pascal-style prototype with parameters in a form similar to "x: int := 12".
		 * 
		 * Typeless prototypes will be returned as C-style.
		 */
		public enum ParameterStyle : byte
			{  C, Pascal  }



		// Group: Functions
		// __________________________________________________________________________
		
		
		/* Constructor: ParsedPrototype
		 * Creates a new parsed prototype.
		 */
		public ParsedPrototype (Tokenizer prototype, bool supportsImpliedTypes = true)
			{
			tokenizer = prototype;
			sections = null;
			mainSectionIndex = 0;

			this.supportsImpliedTypes = supportsImpliedTypes;

			RecalculateSections();
			}


		/* Function: GetAccessLevel
		 * Returns the <Languages.AccessLevel> if it can be determined by the prototype.  This should only be used with basic
		 * language support as it's not as reliable as the results from the dedicated language parsers.
		 */
		public Languages.AccessLevel GetAccessLevel ()
			{
			return sections[mainSectionIndex].GetAccessLevel();
			}

			
		/* Function: GetBeforeParameters
		 * Returns the bounds of the section of the prototype prior to the parameters and whether it exists.  If it has parameters,
		 * it will include the starting symbol of the parameter list such as the opening parenthesis.  If there are no parameters, this
		 * will return the bounds of the entire prototype.
		 */
		public bool GetBeforeParameters (out TokenIterator beforeParametersStart, out TokenIterator beforeParametersEnd)
			{
			if (sections[mainSectionIndex] is ParameterSection)
				{  
				return (sections[mainSectionIndex] as ParameterSection).GetBeforeParameters(out beforeParametersStart, out beforeParametersEnd);  
				}
			else
				{
				beforeParametersStart = tokenizer.FirstToken;
				beforeParametersEnd = tokenizer.LastToken;
				return true;
				}
			}


		/* Function: GetParameter
		 * Returns the bounds of a numbered parameter.  Numbers start at zero.  It will return false if one does not exist at that
		 * number.
		 */
		public bool GetParameter (int parameterIndex, out TokenIterator parameterStart, out TokenIterator parameterEnd)
			{
			if (sections[mainSectionIndex] is ParameterSection)
				{  
				return (sections[mainSectionIndex] as ParameterSection).GetParameterBounds(parameterIndex, out parameterStart, out parameterEnd);  
				}
			else
				{
				parameterStart = tokenizer.LastToken;
				parameterEnd = tokenizer.LastToken;
				return false;
				}
			}


		/* Function: GetParameterName
		 * Returns the bounds of the name of the passed parameter, or false if it couldn't find it.
		 */
		public bool GetParameterName (int parameterIndex, out TokenIterator parameterNameStart, out TokenIterator parameterNameEnd)
			{
			if (sections[mainSectionIndex] is ParameterSection)
				{  
				return (sections[mainSectionIndex] as ParameterSection).GetParameterName(parameterIndex, out parameterNameStart, out parameterNameEnd);  
				}
			else
				{
				parameterNameStart = tokenizer.LastToken;
				parameterNameEnd = tokenizer.LastToken;
				return false;
				}
			}


		/* Function: BuildFullParameterType
		 * 
		 * Returns the full type if one is marked by <PrototypeParsingType.Type> tokens, combining all its modifiers and qualifiers into
		 * one continuous string.
		 * 
		 * If the type and all its modifiers and qualifiers are continuous in the original <Tokenizer> it will return <TokenIterators> based
		 * on it.  However, if the type and all its modifiers and qualifiers are NOT continuous it will create a separate <Tokenizer> to hold 
		 * a continuous version of it.  The returned bounds will be <TokenIterators> based on that rather than on the original <Tokenizer>.
		 * The new <Tokenizer> will still contain the same <PrototypeParsingTypes> and <SyntaxHighlightingTypes> of the original.
		 * 
		 * If implied types is set and <SupportsImpliedTypes> is true this will return "int" for y in "int x, y".  If it is not set or 
		 * <SupportsImpliedTypes> is false then it will return false for y.
		 */
		public bool BuildFullParameterType (int parameterIndex, out TokenIterator fullTypeStart, out TokenIterator fullTypeEnd,
															bool impliedTypes = true)
			{
			if (sections[mainSectionIndex] is ParameterSection)
				{  
				return (sections[mainSectionIndex] as ParameterSection).BuildFullParameterType(parameterIndex, out fullTypeStart, out fullTypeEnd, 
																																	   (impliedTypes && supportsImpliedTypes));
				}
			else
				{
				fullTypeStart = tokenizer.LastToken;
				fullTypeEnd = tokenizer.LastToken;
				return false;
				}
			}


		/* Function: GetBaseParameterType
		 * 
		 * Returns the bounds of the type of the passed parameter, or false if it couldn't find it.  This excludes modifiers and type
		 * suffixes.
		 * 
		 * If implied types is set and <SupportsImpliedTypes> is true this will return "int" for y in "int x, y".  If it is not set or 
		 * <SupportsImpliedTypes> is false then it will return false for y.
		 */
		public bool GetBaseParameterType (int parameterIndex, out TokenIterator start, out TokenIterator end, bool impliedTypes = true)
			{
			if (sections[mainSectionIndex] is ParameterSection)
				{  
				return (sections[mainSectionIndex] as ParameterSection).GetBaseParameterType(parameterIndex, out start, out end, 
																																	  (impliedTypes && supportsImpliedTypes));
				}
			else
				{
				start = tokenizer.LastToken;
				end = tokenizer.LastToken;
				return false;
				}
			}


		/* Function: GetParameterDefaultValue
		 * Returns the bounds of the default value of the passed parameter, or false if it doesn't exist.
		 */
		public bool GetParameterDefaultValue (int parameterIndex, out TokenIterator defaultValueStart, out TokenIterator defaultValueEnd)
			{
			if (sections[mainSectionIndex] is ParameterSection)
				{  
				return (sections[mainSectionIndex] as ParameterSection).GetParameterDefaultValue(parameterIndex, out defaultValueStart, out defaultValueEnd);  
				}
			else
				{
				defaultValueStart = tokenizer.LastToken;
				defaultValueEnd = tokenizer.LastToken;
				return false;
				}
			}


		/* Function: GetAfterParameters
		 * Returns the bounds of the section of the prototype after the parameters and whether it exists.  If it does
		 * exist, the bounds will include the closing symbol of the parameter list such as the closing parenthesis.
		 */
		public bool GetAfterParameters (out TokenIterator afterParametersStart, out TokenIterator afterParametersEnd)
			{
			if (sections[mainSectionIndex] is ParameterSection)
				{
				return (sections[mainSectionIndex] as ParameterSection).GetAfterParameters(out afterParametersStart, out afterParametersEnd);
				}
			else
				{
				afterParametersStart = tokenizer.LastToken;
				afterParametersEnd = tokenizer.LastToken;
				return false;
				}
			}
			

		/* Function: RecalculateSections
		 * 
		 * Recalculates the <Sections> list.  If you've set <MainSectionIndex> manually, it will have to be set again after calling this 
		 * function.
		 * 
		 * Sections are delimited with <PrototypeParsingType.StartOfPrototypeSection> and <PrototypeParsingType.EndOfPrototypeSection>.
		 * Neither of these token types are required to appear, and if they do not the entire prototype will be in one section.  Also, they are 
		 * not required to appear together.  Sections can be delimited by only start tokens or only end tokens, whichever is most convenient 
		 * to the language parser and won't interfere with marking other types.
		 * 
		 * Each section containing <PrototypeParsingType.StartOfParams> will generate a <ParameterSection>.  All others will generate a
		 * regular <Section>.
		 */
		public void RecalculateSections ()
			{
			if (sections == null)
				{  sections = new List<Section>(1);  }
			else
				{  sections.Clear();  }

			TokenIterator startOfSection = tokenizer.FirstToken;
			TokenIterator endOfSection = startOfSection;

			int firstSectionWithName = -1;
			int firstSectionWithParams = -1;

			for (;;)
				{

				// Find the section bounds

				bool sectionIsEmpty = true;
				bool sectionHasName = false;
				bool sectionHasParams = false;

				while (endOfSection.IsInBounds)
					{
					// Break if we hit the beginning of the next section, but not if it's the start of the current section
					if (endOfSection.PrototypeParsingType == PrototypeParsingType.StartOfPrototypeSection &&
						endOfSection > startOfSection)
						{  break;  }

					if (endOfSection.FundamentalType != FundamentalType.Whitespace)
						{  sectionIsEmpty = false; }

					if (endOfSection.PrototypeParsingType == PrototypeParsingType.Name)
						{  sectionHasName = true;  }
					else if (endOfSection.PrototypeParsingType == PrototypeParsingType.StartOfParams)
						{  sectionHasParams = true;  }
					else if (endOfSection.PrototypeParsingType == PrototypeParsingType.EndOfPrototypeSection)
						{
						endOfSection.Next();
						break;
						}

					endOfSection.Next();
					}


				// Process the section

				if (!sectionIsEmpty)
					{
					endOfSection.PreviousPastWhitespace(PreviousPastWhitespaceMode.EndingBounds, startOfSection);
					startOfSection.NextPastWhitespace(endOfSection);

					if (sectionHasParams)
						{  sections.Add( new ParameterSection(startOfSection, endOfSection, supportsImpliedTypes) );  }
					else
						{  sections.Add( new Section(startOfSection, endOfSection) );  }

					if (sectionHasName && firstSectionWithName == -1)
						{  firstSectionWithName = sections.Count - 1;  }
					if (sectionHasParams && firstSectionWithParams == -1)
						{  firstSectionWithParams = sections.Count - 1;  }
					}


				// Continue?

				if (endOfSection.IsInBounds)
					{  startOfSection = endOfSection;  }
				else
					{  break;  }
				}


			// Sanity check.  This should only happen if all the sections were whitespace, which shouldn't normally happen but I
			// suppose could with a manual prototype.

			if (sections.Count < 1)
				{  sections.Add( new Section(tokenizer.FirstToken, tokenizer.LastToken) );  }


			// Determine main section

			if (sections.Count == 1)
				{  mainSectionIndex = 0;  }
			else if (firstSectionWithName != -1)
				{  mainSectionIndex = firstSectionWithName;  }
			else if (firstSectionWithParams != -1)
				{  mainSectionIndex = firstSectionWithParams;  }
			else
				{  mainSectionIndex = 0;  }
			}



		// Group: Properties
		// __________________________________________________________________________
		
		
		/* Property: Tokenizer
		 * The tokenized prototype.
		 */
		public Tokenizer Tokenizer
			{
			get
				{  return tokenizer;  }
			}


		/* Property: Sections
		 * The list of <Sections> making up the prototype.
		 */
		public List<Section> Sections
			{
			get
				{  return sections;  }
			}


		/* Property: MainSectionIndex
		 * The index into <Sections> for the one which contains the most significant content, namely the prototype's
		 * access level and function parameters.  If it's not set manually it will be a guess.
		 */
		public int MainSectionIndex
			{
			get
				{  return mainSectionIndex;  }
			set
				{  mainSectionIndex = value;  }
			}


		/* Property: NumberOfParameters
		 */
		public int NumberOfParameters
			{
			get
				{  
				if (sections[mainSectionIndex] is ParameterSection)
					{  return (sections[mainSectionIndex] as ParameterSection).NumberOfParameters;  }
				else
					{  return 0;  }
				}
			}


		/* Property: Style
		 * The format of the prototype, such as C-style parameters ("int x") or Pascal-style ("x: int").  If it has no parameters or
		 * no types this will return C.  Tokens must be marked with <PrototypeParsingType.Name>, <PrototypeParsingType.Type>,
		 * and <PrototypeParsingType.NameTypeSeparator> for this to work.
		 */
		public ParameterStyle Style
			{
			get
				{
				if (sections[mainSectionIndex] is ParameterSection)
					{  return (sections[mainSectionIndex] as ParameterSection).ParameterStyle;  }
				else
					{  return ParameterStyle.C;  }
				}
			}


		/* Property: SupportsImpliedTypes
		 * Whether the prototype's language supports implied types.
		 */
		public bool SupportsImpliedTypes
			{
			get
				{  return supportsImpliedTypes;  }
			}


		
		// Group: Variables
		// __________________________________________________________________________
		
		/* var: tokenizer
		 * The <Tokenizer> containing the full prototype.
		 */
		protected Tokenizer tokenizer;

		/* var: sections
		 * A list of <Sections> representing chunks of the prototype, or null if it hasn't been calculated yet.
		 */
		protected List<Section> sections;

		/* var: mainSectionIndex
		 * An index into <sections> representing the main one used for retrieving properties like name, access level,
		 * and parameters.
		 */
		protected int mainSectionIndex;

		/* var: supportsImpliedTypes
		 * Whether the prototype's language supports implied types.
		 */
		protected bool supportsImpliedTypes;

		}
	}