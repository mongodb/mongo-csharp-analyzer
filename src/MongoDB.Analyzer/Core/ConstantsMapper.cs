// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Text.RegularExpressions;

namespace MongoDB.Analyzer.Core;

internal sealed class ConstantsMapper
{
    private const double DoubleSuffix = 0.5; // (x.5).ToString("G17") == x.5
    private const string StringPrefix = "s__";
    private const string RegexLookahead = "(?![\\w\"\\.])";
    private const string RegexLookbehind = "(?<![\\w\"\\.])";
    private const string VariableAnnotation = "VariableName";
    private const string WildcardRegex = "\\.\\$\\*\\*";
    private const string WildcardSuffix = ".$**";

    private IDictionary<string, LiteralExpressionSyntax> _originalToSyntax;
    private IDictionary<string, string> _mqlRemapping;
    private HashSet<long> _registeredNumericConstants;
    private HashSet<string> _registeredStringConstants;

    private int _nextConstant = 0;
    private int _overflowConstantInt8;

    private bool _allConstantsRegistered = false;
    
    public void CopyRegisteredConstants(ConstantsMapper mapper)
    {
        _registeredNumericConstants ??= new HashSet<long>();
        _registeredStringConstants ??= new HashSet<string>();
        _registeredNumericConstants.AddRange(mapper._registeredNumericConstants);
        _registeredStringConstants.AddRange(mapper._registeredStringConstants);
    }

    public void FinalizeLiteralsRegistration()
    {
        if (_allConstantsRegistered)
        {
            throw new InvalidOperationException("Literals registration is finalized.");
        }

        _overflowConstantInt8 = _registeredNumericConstants?.Contains(sbyte.MaxValue) == true ? GetNextConstantInt8(sbyte.MaxValue) : sbyte.MaxValue;
        _allConstantsRegistered = true;
    }

    public LiteralExpressionSyntax GetExpressionByType(SpecialType specialType, string originalValue)
    {
        AssertLiteralsRegistered();

        if (_originalToSyntax.TryGetValueSafe(originalValue, out var expressionSyntax))
        {
            return expressionSyntax;
        }

        var syntaxToken = specialType switch
        {
            SpecialType.System_Byte or
            SpecialType.System_SByte => SyntaxFactory.Literal(GetNextConstantInt8()),
            SpecialType.System_UInt16 or
            SpecialType.System_Int16 or
            SpecialType.System_Int32 or
            SpecialType.System_UInt32 or
            SpecialType.System_Int64 or
            SpecialType.System_UInt64 => SyntaxFactory.Literal(GetNextConstantInt()),
            SpecialType.System_Double => SyntaxFactory.Literal(GetNextConstantInt() + DoubleSuffix),
            SpecialType.System_String => SyntaxFactory.Literal(GetNextConstantString()),
            _ => (SyntaxToken?)null
        };

        if (syntaxToken != null)
        {
            var isString = specialType == SpecialType.System_String;
            var expressionKind = isString ? SyntaxKind.StringLiteralExpression : SyntaxKind.NumericLiteralExpression;

            var syntaxAnnotation = new SyntaxAnnotation(VariableAnnotation, originalValue);
            expressionSyntax = SyntaxFactory.LiteralExpression(expressionKind, syntaxToken.Value).WithAdditionalAnnotations(syntaxAnnotation);

            _originalToSyntax ??= new Dictionary<string, LiteralExpressionSyntax>();
            _originalToSyntax[originalValue] = expressionSyntax;

            AddMapping(syntaxToken.Value.ValueText, originalValue, isString);
        }

        return expressionSyntax;
    }

    public LiteralExpressionSyntax GetExpressionForConstant(SpecialType specialType, object constant)
    {
        AssertLiteralsRegistered();

        if (constant == null)
        {
            return null;
        }

        // Skip replacing for doubles and strings if not needed
        if (constant is double @double && ((@double - (long)@double) != DoubleSuffix) ||
            constant is string @string && !@string.StartsWith(StringPrefix))
        {
            return SyntaxFactoryUtilities.GetConstantExpression(constant);
        }

        return GetExpressionByType(specialType, constant.ToString());
    }

    public LiteralExpressionSyntax GetExpressionForEnum(SpecialType specialType, object constant)
    {
        AssertLiteralsRegistered();

        if (constant == null)
        {
            return null;
        }

        // Skip replacing for doubles and strings if not needed
        if (constant is double @double && ((@double - (long)@double) != DoubleSuffix) ||
            constant is string @string && !@string.StartsWith(StringPrefix))
        {
            return SyntaxFactoryUtilities.GetConstantExpression(constant);
        }

        return GetExpressionByType(specialType, constant.ToString());
    }

    public LiteralExpressionSyntax GetExpressionForLiteral(LiteralExpressionSyntax literalExpressionSyntax)
    {
        AssertLiteralsRegistered();

        if (literalExpressionSyntax == null)
        {
            return null;
        }

        // Skip replacing for doubles and strings if not needed
        var value = literalExpressionSyntax.Token.Value;
        if (value is null ||
            value is double @double && ((@double - (long)@double) != DoubleSuffix) ||
            value is string @string && !@string.StartsWith(StringPrefix))
        {
            return literalExpressionSyntax;
        }

        var specialType = literalExpressionSyntax.IsKind(SyntaxKind.StringLiteralExpression) ?
            SpecialType.System_String : SpecialType.System_Int32;

        return GetExpressionByType(specialType, value.ToString());
    }

    public static string GetVariableName(LiteralExpressionSyntax literalExpressionSyntax) => literalExpressionSyntax.GetAnnotations(VariableAnnotation).FirstOrDefault()?.Data;

    public void RegisterLiteral(LiteralExpressionSyntax literalExpressionSyntax)
    {
        if (literalExpressionSyntax == null)
        {
            return;
        }

        var value = literalExpressionSyntax.Token.Value;

        if (value == null)
        {
            return;
        }
        else if (value is double @double)
        {
            // No need to register doubles in different format
            if ((@double - (long)@double) == DoubleSuffix)
            {
                RegisterNumeric((long)@double);
            }
        }
        else if (value is string @string)
        {
            // No need to register strings in different format
            if (@string.StartsWith(StringPrefix))
            {
                RegisterString(@string);
            }
        }
        else
        {
            var @long = Convert.ToInt64(value);
            RegisterNumeric(@long);
        }
    }

    public string RemapConstants(string expression)
    {
        if (_mqlRemapping != null)
        {
            foreach (var pair in _mqlRemapping)
            {
                expression = Regex.Replace(expression, pair.Key, pair.Value);
            }
        }

        return expression;
    }

    private void AssertLiteralsRegistered()
    {
        if (!_allConstantsRegistered)
        {
            throw new InvalidOperationException("Literals registration not finished");
        }
    }

    private void AddMapping(string source, string target, bool isString)
    {
        if (_mqlRemapping == null)
        {
            _mqlRemapping = new Dictionary<string, string>
            {
                // Overflow for byte/sbyte constants amount. For simplicity support up to 126 byte/sbyte constants per expression
                [$"{RegexLookbehind}{_overflowConstantInt8}{RegexLookahead}"] = "Unknown"
            };
        }

        if (isString)
        {
            _mqlRemapping[$"{RegexLookbehind}\"{source}\"{RegexLookahead}"] = target;
            _mqlRemapping[$"/{source}/"] = $"/{target}/";
            _mqlRemapping[$"{RegexLookbehind}\"{source}{WildcardRegex}\"{RegexLookahead}"] = $"{target}{WildcardSuffix}";
        }
        else
        {
            _mqlRemapping[$"{RegexLookbehind}{source}{RegexLookahead}"] = target;
        }
    }

    private int GetNextConstantInt()
    {
        if (_registeredNumericConstants != null)
        {
            while (_registeredNumericConstants.Contains(_nextConstant) && _nextConstant >= 0)
            {
                _nextConstant++;
            }
        }

        return _nextConstant++;
    }

    private int GetNextConstantInt8(sbyte? overflowValue = null)
    {
        var result = GetNextConstantInt();

        if (result > sbyte.MaxValue)
        {
            // corner case, all 128 values are used
            return overflowValue ?? _overflowConstantInt8;
        }

        return result;
    }

    private string GetNextConstantString()
    {
        var nextIntConstant = GetNextConstantInt();

        if (_registeredStringConstants != null)
        {
            while (_registeredStringConstants.Contains($"{StringPrefix}{nextIntConstant}") && _nextConstant >= 0)
            {
                nextIntConstant = GetNextConstantInt();
            }
        }

        return $"{StringPrefix}{nextIntConstant}";
    }

    private void RegisterNumeric(long value)
    {
        _registeredNumericConstants ??= new HashSet<long>();
        _registeredNumericConstants.Add(value);
    }

    private void RegisterString(string value)
    {
        _registeredStringConstants ??= new HashSet<string>();
        _registeredStringConstants.Add(value);
    }
}
