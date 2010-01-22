namespace Microsoft.StyleCop.CSharp
{
    using System;

    internal enum Rules
    {
        AccessModifierMustBeDeclared,
        CurlyBracketsForMultiLineStatementsMustNotShareLine,
        StatementMustNotBeOnSingleLine,
        ElementMustNotBeOnSingleLine,
        CurlyBracketsMustNotBeOmitted,
        AllAccessorsMustBeMultiLineOrSingleLine,
        ElementsMustBeDocumented,
        PartialElementsMustBeDocumented,
        ElementDocumentationMustHaveSummary,
        PartialElementDocumentationMustHaveSummary,
        ElementDocumentationMustHaveSummaryText,
        PartialElementDocumentationMustHaveSummaryText,
        ElementParametersMustBeDocumented,
        ElementParameterDocumentationMustDeclareParameterName,
        ElementParameterDocumentationMustHaveText,
        ElementReturnValueMustBeDocumented,
        ElementReturnValueDocumentationMustHaveText,
        VoidReturnValueMustNotBeDocumented,
        GenericTypeParametersMustBeDocumented,
        GenericTypeParametersMustBeDocumentedPartialClass,
        GenericTypeParameterDocumentationMustMatchTypeParameters,
        GenericTypeParameterDocumentationMustDeclareParameterName,
        GenericTypeParameterDocumentationMustHaveText,
        ElementDocumentationMustNotHaveDefaultSummary,
        DocumentationMustContainValidXml,
        EnumerationItemsMustBeDocumented,
        ElementParameterDocumentationMustMatchElementParameters,
        PropertyDocumentationMustHaveValue,
        DocumentationTextMustNotBeEmpty,
        DocumentationTextMustEndWithAPeriod,
        DocumentationTextMustBeginWithACapitalLetter,
        DocumentationTextMustContainWhitespace,
        DocumentationMustMeetCharacterPercentage,
        DocumentationTextMustMeetMinimumCharacterLength,
        ConstructorSummaryDocumentationMustBeginWithStandardText,
        DestructorSummaryDocumentationMustBeginWithStandardText,
        DocumentationHeadersMustNotContainBlankLines,
        IncludedDocumentationFileDoesNotExist,
        IncludedDocumentationXPathDoesNotExist,
        IncludeNodeDoesNotContainValidFileAndPath,
        PropertySummaryDocumentationMustMatchAccessors,
        PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess,
        ElementDocumentationMustNotBeCopiedAndPasted,
        SingleLineCommentsMustNotUseDocumentationStyleSlashes,
        FileMustHaveHeader,
        FileHeaderMustShowCopyright,
        FileHeaderMustHaveCopyrightText,
        FileHeaderCopyrightTextMustMatch,
        FileHeaderMustContainFileName,
        FileHeaderFileNameDocumentationMustMatchFileName,
        FileHeaderMustHaveValidCompanyText,
        FileHeaderCompanyNameTextMustMatch,
        FileHeaderMustHaveSummary,
        ThisMissing,
        BaseUsed,
        ConstFieldNamesMustBeginWithUpperCaseLetter,
        NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter,
        AccessibleFieldsMustBeginWithUpperCaseLetter,
        FieldNamesMustBeginWithLowerCaseLetter,
        FieldNamesMustNotUseHungarianNotation,
        VariableNamesMustNotBePrefixed,
        ElementMustBeginWithLowerCaseLetter,
        ElementMustBeginWithUpperCaseLetter,
        InterfaceNamesMustBeginWithI,
        FieldNamesMustNotBeginWithUnderscore,
        FieldNamesMustNotContainUnderscore,
        CodeMustNotContainMultipleBlankLinesInARow,
        ClosingCurlyBracketsMustNotBePrecededByBlankLine,
        OpeningCurlyBracketsMustNotBePrecededByBlankLine,
        OpeningCurlyBracketsMustNotBeFollowedByBlankLine,
        ClosingCurlyBracketMustBeFollowedByBlankLine,
        SingleLineCommentMustBePrecededByBlankLine,
        ElementsMustBeSeparatedByBlankLine,
        ElementDocumentationHeaderMustBePrecededByBlankLine,
        ElementDocumentationHeadersMustNotBeFollowedByBlankLine,
        ChainedStatementBlocksMustNotBePrecededByBlankLine,
        SingleLineCommentsMustNotBeFollowedByBlankLine,
        WhileDoFooterMustNotBePrecededByBlankLine,
        ElementsMustAppearInTheCorrectOrder,
        PartialElementsMustDeclareAccess,
        ElementsMustBeOrderedByAccess,
        StaticElementsMustAppearBeforeInstanceElements,
        FileMayOnlyContainASingleClass,
        FileMayOnlyContainASingleNamespace,
        CodeAnalysisSuppressionMustHaveJustification,
        DebugAssertMustProvideMessageText,
        DebugFailMustProvideMessageText,
        ArithmeticExpressionsMustDeclarePrecedence,
        ConditionalExpressionsMustDeclarePrecedence,
        RemoveUnnecessaryCode,
        RemoveDelegateParenthesisWhenPossible,
        FieldsMustBePrivate,
        ConstantsMustAppearBeforeFields,
        KeywordsMustBeSpacedCorrectly,
        CommasMustBeSpacedCorrectly,
        SemicolonsMustBeSpacedCorrectly,
        SymbolsMustBeSpacedCorrectly,
        OpeningParenthesisMustBeSpacedCorrectly,
        ClosingParenthesisMustBeSpacedCorrectly,
        OpeningSquareBracketsMustBeSpacedCorrectly,
        ClosingSquareBracketsMustBeSpacedCorrectly,
        OpeningCurlyBracketsMustBeSpacedCorrectly,
        ClosingCurlyBracketsMustBeSpacedCorrectly,
        OpeningGenericBracketsMustBeSpacedCorrectly,
        ClosingGenericBracketsMustBeSpacedCorrectly,
        OpeningAttributeBracketsMustBeSpacedCorrectly,
        ClosingAttributeBracketsMustBeSpacedCorrectly,
        NullableTypeSymbolsMustNotBePrecededBySpace,
        MemberAccessSymbolsMustBeSpacedCorrectly,
        IncrementDecrementSymbolsMustBeSpacedCorrectly,
        NegativeSignsMustBeSpacedCorrectly,
        PositiveSignsMustBeSpacedCorrectly,
        DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly,
        ColonsMustBeSpacedCorrectly,
        TabsMustNotBeUsed,
        CodeMustNotContainMultipleWhitespaceInARow,
        CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation,
        StatementMustNotUseUnnecessaryParenthesis,
        PreprocessorKeywordsMustNotBePrecededBySpace,
        OperatorKeywordMustBeFollowedBySpace,
        SingleLineCommentsMustBeginWithSingleSpace,
        DocumentationLinesMustBeginWithSingleSpace,
        UsingDirectivesMustBePlacedWithinNamespace,
        OpeningParenthesisMustBeOnDeclarationLine,
        ClosingParenthesisMustBeOnLineOfLastParameter,
        ClosingParenthesisMustBeOnLineOfOpeningParenthesis,
        ParameterMustNotSpanMultipleLines,
        CommaMustBeOnSameLineAsPreviousParameter,
        ParameterListMustFollowDeclaration,
        ParameterMustFollowComma,
        SplitParametersMustStartOnLineAfterDeclaration,
        ParametersMustBeOnSameLineOrSeparateLines,
        CodeMustNotContainEmptyStatements,
        CodeMustNotContainMultipleStatementsOnOneLine,
        BlockStatementsMustNotContainEmbeddedComments,
        BlockStatementsMustNotContainEmbeddedRegions,
        DoNotPrefixCallsWithBaseUnlessLocalImplementationExists,
        PrefixLocalCallsWithThis,
        DeclarationKeywordsMustFollowOrder,
        ProtectedMustComeBeforeInternal,
        SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives,
        UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives,
        UsingDirectivesMustBeOrderedAlphabeticallyByNamespace,
        UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName,
        PropertyAccessorsMustFollowOrder,
        EventAccessorsMustFollowOrder,
        CommentsMustContainText,
        QueryClauseMustFollowPreviousClause,
        QueryClausesMustBeOnSeparateLinesOrAllOnOneLine,
        QueryClauseMustBeginOnNewLineWhenPreviousClauseSpansMultipleLines,
        QueryClausesSpanningMultipleLinesMustBeginOnOwnLine,
        UseBuiltInTypeAlias,
        AvoidVarType,
        UseStringEmptyForEmptyStrings,
        DoNotPlaceRegionsWithinElements,
        DoNotUseRegions
    }
}
