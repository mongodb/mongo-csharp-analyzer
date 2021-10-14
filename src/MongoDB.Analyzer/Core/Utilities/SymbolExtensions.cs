using Microsoft.CodeAnalysis;
using System.Linq;

namespace MongoDB.Analyzer.Core
{
    internal static class SymbolExtensions
    {
        public static bool ImplementsInterface(this ITypeSymbol typeSymbol, string interfaceName) =>
            typeSymbol.TypeKind switch
            {
                TypeKind.Class => typeSymbol.Interfaces.Any(i => i.Name == interfaceName),
                TypeKind.Interface => typeSymbol.Name == interfaceName,
                _ => false
            };

        public static bool IsIMongoQueryable(this ITypeSymbol typeSymbol) =>
            ImplementsInterface(typeSymbol, "IMongoQueryable");

        public static bool IsMongoQueryable(this ITypeSymbol typeSymbol) =>
            typeSymbol.Name == "MongoQueryable";
    }
}
