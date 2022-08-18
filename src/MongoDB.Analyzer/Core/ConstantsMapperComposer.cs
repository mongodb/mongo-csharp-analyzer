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

namespace MongoDB.Analyzer.Core;

internal static class ConstantsMapperComposer
{
    public static (SyntaxNode, ConstantsMapper) Compose(SyntaxNode node, IEnumerable<ConstantsMapper> constantsMappers)
    {
        var nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();
        var composedMapper = new ConstantsMapper();
        var literals = node.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>();
        var literalsReplacement = new Dictionary<SyntaxNode, SyntaxNode>();

        foreach (var mapper in constantsMappers)
        {
            composedMapper.CopyRegisteredConstants(mapper);
        }
        composedMapper.FinalizeLiteralsRegistration();

        foreach (var literal in literals)
        {
            var variableName = ConstantsMapper.GetVariableName(literal);
            if (variableName == null)
            {
                continue;
            }
            var newLiteral = composedMapper.GetExpressionByType(SyntaxFactoryUtilities.GetSpecialType(literal.Token.Value), variableName);
            literalsReplacement.Add(literal, newLiteral);
        }

        var rewrittenBuildersNode = node.ReplaceNodes(literalsReplacement.Keys, (n, _) => literalsReplacement[n]);
        return (rewrittenBuildersNode, composedMapper);
    }
}

