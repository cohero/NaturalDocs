﻿
// This file is part of Natural Docs, which is Copyright © 2003-2020 Code Clear LLC.
// Natural Docs is licensed under version 3 of the GNU Affero General Public License (AGPL)
// Refer to License.txt for the complete details


using System;


namespace CodeClear.NaturalDocs.Engine.Tokenization
	{

	/* Enum: FundamentalType
	 * 
	 * The type of token it is on the most basic level.
	 * 
	 *		Null - Returned when the token iterator is out of bounds.
	 *		LineBreak - A single line break in CR, LF, or CR/LF format.
	 *		Whitespace - A series of consecutive space and/or tab characters.
	 *		Text - A series of consecutive ASCII letters, numbers, and/or characters above ASCII 0x7F.
	 *					This does *not* include underscores.
	 *		Symbol - One character not mentioned above, which are all the symbol characters available
	 *						  on the standard US QWERTY keyboard plus ASCII control characters.
	 */
	public enum FundamentalType : byte
		{  Null = 0, LineBreak, Whitespace, Text, Symbol  }


	/* Enum: CommentParsingType
	 * 
	 * The type of token it is as is relevant to comment parsing.
	 * 
	 *		Null - Returned when the token iterator is out of bounds or if a token hasn't been assigned one of these 
	 *					values yet.
	 *		
	 *		CommentSymbol - A comment symbol or part of one.
	 *		CommentDecoration - A symbol that only provides decoration for a comment, such as part of 
	 *													a horizontal line.
	 *										 
	 *		PossibleOpeningTag - An opening symbol that's a candidate for being part of a link, bold, or underline tag.
	 *		PossibleClosingTag - A closing symbol that's a candidate for being part of a link, bold, or underline tag.
	 *		OpeningTag - An opening symbol that's a part of a link, bold, or underline tag.
	 *		ClosingTag - A closing symbol that's a part of a link, bold, or underline tag.
	 *		
	 *		URL - Part of an URL.
	 *		EMail - Part of an e-mail address.
	 */
	public enum CommentParsingType : byte
		{  
		Null = 0, 
		CommentSymbol, CommentDecoration,	
		PossibleOpeningTag, PossibleClosingTag,
		OpeningTag, ClosingTag,
		URL, EMail
		}


	/* Enum: SyntaxHighlightingType
	 * 
	 * The type of token it is as is relevant to prototype parsing.
	 * 
	 * Null - Returned when the token is out of bounds or one of these values hasn't been assigned to it yet.
	 * Keyword - A reserved word, like "int".
	 * Number - A numeric constant, like "12", "0xFF", or "-1.5".  The format doesn't matter.
	 * String - A string.  Also covers char constants for languages that have them.
	 * Comment - A comment, both symols and content.
	 * PreprocessingDirective - A preprocessing directive such as "#define x".
	 * Metadata - Code metadata such as the "[Flags]" attribute in C#.
	 */
	public enum SyntaxHighlightingType :  byte
		{
		Null = 0,
		Keyword, Number, String, Comment,
		PreprocessingDirective, Metadata
		}


	/* Enum: PrototypeParsingType
	 * 
	 * Null - Returned when the token is out of bounds or one of these values hasn't been assigned to it yet.
	 * 
	 * StartOfPrototypeSection - The first token of a new prototype section.  It is possible for these to appear without
	 *										 corresponding <EndOfPrototypeSections>.
	 * EndOfPrototypeSection - The last token of a prototype section, causing the next token to start a new one.  It is possible
	 *										for these to appear without corresponding <StartOfPrototypeSections>.
	 * 
	 * StartOfParams - The start of a parameter list, such as an opening parenthesis.
	 * EndOfParams - The end of a parameter list, such as a closing parenthesis.
	 * ParamSeparator - A separator between parameters, such as a comma.
	 * 
	 * Type - The type excluding all modifiers and qualifiers, such as "int" in "unsigned int" or "Class" in "PkgA.PkgB.Class".
	 * TypeModifier - A separate word modifying a type, such as "const" in "const int".
	 * TypeQualifier - Everything prior to the ending word in a qualified type, such as "PkgA.PkgB." in "PkgA.PkgB.Class".
	 * OpeningTypeModifier - An opening symbol modifying a type, such as "[" in "int[]" or "<" in "List<int>".
	 * ClosingTypeModifier - A closing symbol modifying a type, such as "]" in "int[]" or ">" in "List<int>".
	 * 
	 * NameTypeSeparator - In languages that use them, the symbol separating a variable name from its type, such as ":" in
	 *									"x: int".  In languages that simply use a space this type won't appear.
	 *
	 * Name - The name of the parameter or the code element being defined by the prototype.
	 * 
	 * ParamModifier - Any parameter modifiers.  These usually appear with the name but are part of the type, and aren't
	 *						   shared with other parameters inheriting the type, such as "*" in "int *x" in C++.
	 * OpeningParamModifier - An opening symbol modifying a parameter.  These usually appear with the name but are part
	 *									  of the type, such as "[" in "int x[5]".
	 * ClosingParamModifier - A closing symbol modifying a parameter.  These usually appear with the name but are part of
	 *									 the type, such as "]" in "int x[5]".
	 *														
	 * DefaultValueSeparator - The symbol separating the name and type from its default value, such as "=" or ":=".
	 * DefaultValue - The default value of the parameter.
	 * 
	 * PropertyValueSeparator - The symbol separating a property name from its value, such as "=" or ":".
	 * PropertyValue - The value of a property, such as "12" in "@RequestForEnhancement(id = 12)" in Java annotations.
	 */
	public enum PrototypeParsingType :  byte
		{
		Null = 0,

		StartOfPrototypeSection, EndOfPrototypeSection,

		StartOfParams, EndOfParams, ParamSeparator,

		Type, TypeModifier, TypeQualifier, OpeningTypeModifier, ClosingTypeModifier,

		NameTypeSeparator, 
		
		Name, ParamModifier, OpeningParamModifier, ClosingParamModifier,

		DefaultValueSeparator, DefaultValue,

		PropertyValueSeparator, PropertyValue
		}


	/* Enum: ClassPrototypeParsingType
	 * 
	 * Null - Returned when the token is out of bounds or one of these values hasn't been assigned to it yet.
	 * 
	 * StartOfPrePrototypeLine - The first token of a new pre-prototype line.  Each token marked with this starts a new line.
	 * PrePrototypeLine - Part of a line that should be shown before the prototype.
	 * 
	 * StartOfParents - The start of a parent list.
	 * ParentSeparator - A separator between parents, such as a comma.
	 * EndOfParents - The end of a parent list for languages like Python that use parenthesis.
	 * 
	 * Modifier - A separate word modifying the class or parent, such as "public" or "static".
	 * Keyword - The keyword used to declare the class, such as "class", "struct", or "interface".
	 * Name - The name of the class or parent including qualifiers, such as "PkgA.PkgB.Class".
	 * 
	 * TemplateSuffix - Extra template information after a class or parent, such as "<T>" in "List<T>".
	 * 
	 * StartOfPostPrototypeLine - The first token of a new post-prototype line.  Each token marked with this starts a new line.
	 * PostPrototypeLine - Part of a line that should be shown after the prototype.
	 * 
	 * StartOfBody - The start of the class's body if it is present in the prototype.  Nothing beyond that is demarcated, including the end
	 *								of the body.  This token is just present to indicate that the prototype does contain a body.
	 */
	public enum ClassPrototypeParsingType :  byte
		{
		Null = 0,

		StartOfPrePrototypeLine, PrePrototypeLine,

		StartOfParents, ParentSeparator, EndOfParents,

		Modifier, Keyword, Name,

		TemplateSuffix,

		StartOfPostPrototypeLine, PostPrototypeLine,

		StartOfBody
		}

	}