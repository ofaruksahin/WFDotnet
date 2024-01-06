using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WFDotnet.Code.Common.Constants;
using WFDotnet.Code.Common.DataTypes.Interfaces;
using WFDotnet.Code.Common.Extensions;
using WFDotnet.Code.DataModel.Models;
using WFDotnet.Code.Generation.Common;
using WFDotnet.Code.Generation.Common.Models;

namespace WFDotnet.Code.Generation.DataModel
{
    public class DataModelCodeGenerator : WFCodeGenerator<Model>
    {
        private const string ModuleName = "DataModel";

        public override WFCodeGenerationResult Generate(Model instance)
        {
            try
            {
                ClassDeclarationSyntax modelClass = SyntaxFactory.ClassDeclaration(instance.Name);
                modelClass = modelClass.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                var defaultUsings = GetUsings();

                foreach (var property in instance.Properties)
                {
                    if (!property.PropertyType.CanBeInstantiated() && !property.PropertyType.HasGenericType())
                        modelClass = modelClass.AddMembers(GenerateNotGenericPredifinedTypeCode(property));
                    else
                        modelClass = modelClass.AddMembers(GenericDefinedTypeCode(property));

                    var propertyNamespace = property.PropertyType.Namespace;

                    if (!defaultUsings.Any(f => f == propertyNamespace))
                        defaultUsings.Add(propertyNamespace);
                }


                SyntaxTree syntaxTree = SyntaxFactory.SyntaxTree(
                        SyntaxFactory.CompilationUnit()
                            .AddUsings(
                                defaultUsings.Select(f => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(f))).ToArray()
                            )
                            .AddMembers(
                                SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(GeneratedCodeDefaultNamespace.GetDefaultNamespace(ModuleName)))
                                    .AddMembers(modelClass))
                    );

                SyntaxNode root = syntaxTree.GetRoot();

                return WFCodeGenerationResult.Success(root.NormalizeWhitespace().ToFullString());
            }
            catch (Exception ex)
            {
                return WFCodeGenerationResult.Fail(ex.Message);
            }
        }

        private PropertyDeclarationSyntax GenerateNotGenericPredifinedTypeCode(Property modelPropertyInfo)
        {
            var propertySyntax = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(modelPropertyInfo.PropertyType.GetFullName(false)), modelPropertyInfo.Name);
            propertySyntax = propertySyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            propertySyntax = propertySyntax.WithAccessorList(SyntaxFactory.AccessorList(
                    SyntaxFactory.List(new AccessorDeclarationSyntax[]
                    {
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    })
                ));

            propertySyntax = PropertySyntaxAddAttributes(propertySyntax, modelPropertyInfo);

            return propertySyntax;
        }

        private PropertyDeclarationSyntax GenericDefinedTypeCode(Property modelPropertyInfo)
        {
            var hasGenericType = modelPropertyInfo.PropertyType.GetGenericArguments().Any();

            var typeFullName = modelPropertyInfo.PropertyType.GetFullName(hasGenericType);

            var lastDotIndex = typeFullName.LastIndexOf('.');

            var @namespace = typeFullName.Substring(0, lastDotIndex)
                .TrimEnd('.');

            var typeName = typeFullName.Substring(lastDotIndex, typeFullName.Length - lastDotIndex)
                .TrimStart('.');


            List<SyntaxNodeOrToken> syntaxNodeOrTokens = GetSyntaxNodeOrTokens(modelPropertyInfo.PropertyType);

            var typeArgumentList = SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList<TypeSyntax>(syntaxNodeOrTokens));

            QualifiedNameSyntax propertyType = null;

            if (typeArgumentList.Arguments.Count > 0)
            {
                propertyType = SyntaxFactory.QualifiedName(
                    SyntaxFactory.ParseName(@namespace),
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeName))
                        .WithTypeArgumentList(typeArgumentList));
            }
            else
            {
                propertyType = SyntaxFactory.QualifiedName(
                    SyntaxFactory.ParseName(@namespace),
                    SyntaxFactory.IdentifierName(typeName));
            }

            var property = SyntaxFactory.PropertyDeclaration(propertyType, modelPropertyInfo.Name)
               .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
               .WithAccessorList(
                   SyntaxFactory.AccessorList(
                       SyntaxFactory.List(new AccessorDeclarationSyntax[]
                       {
                                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                       }))
               )
               .WithInitializer(
                   SyntaxFactory.EqualsValueClause(
                       SyntaxFactory.ObjectCreationExpression(propertyType)
                           .WithArgumentList(SyntaxFactory.ArgumentList())
                   )
               ).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            property = PropertySyntaxAddAttributes(property, modelPropertyInfo);

            return property;
        }

        private List<SyntaxNodeOrToken> GetSyntaxNodeOrTokens(Type type)
        {
            List<SyntaxNodeOrToken> syntaxNodeOrTokens = new List<SyntaxNodeOrToken>();

            bool addComma = false;

            var genericArguments = type.GetGenericArguments();


            for (var i = 0; i < genericArguments.Length; i++)
            {
                if (addComma)
                {
                    syntaxNodeOrTokens.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                    addComma = false;
                }

                var genericArgument = genericArguments[i];

                var hasGenericArguments = genericArgument.GetGenericArguments().Any();
                var genericArgumentName = genericArgument.GetFullName(hasGenericArguments);

                if (hasGenericArguments)
                {
                    var syntax = SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.GenericName(SyntaxFactory.Identifier(genericArgumentName))
                        .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList<TypeSyntax>(GetSyntaxNodeOrTokens(genericArgument)))));

                    syntaxNodeOrTokens.Add(syntax.FirstOrDefault().SyntaxTree.GetRoot());
                }
                else
                {
                    syntaxNodeOrTokens.Add(SyntaxFactory.ParseTypeName(genericArgumentName));
                }

                addComma = true;
            }

            return syntaxNodeOrTokens;
        }

        private PropertyDeclarationSyntax PropertySyntaxAddAttributes(PropertyDeclarationSyntax propertyDeclarationSyntax, Property modelPropertyInfo)
        {
            if (!modelPropertyInfo.Attributes.Any()) return propertyDeclarationSyntax;

            List<AttributeSyntax> attributeSyntaxes = new List<AttributeSyntax>();

            foreach (var attributeInfo in modelPropertyInfo.Attributes)
            {
                bool hasAttributeProperties = false;
                string argumentAttributeText = "";

                if (attributeInfo.Values != null)
                {
                    foreach (var attributeProperty in attributeInfo.Values)
                    {
                        if (!hasAttributeProperties)
                        {
                            hasAttributeProperties = true;
                            argumentAttributeText += SyntaxFactory.Token(SyntaxKind.OpenParenToken).Text;
                        }

                        var attributeValue = attributeProperty.Value;

                        if (attributeProperty.TypeFinder is IAttributeValue readValue)
                            attributeValue = readValue.GetAttributeValue(attributeValue);

                        argumentAttributeText += $"{attributeProperty.Key} = {attributeValue} ,";
                    }

                    argumentAttributeText = argumentAttributeText.TrimEnd(SyntaxFactory.Token(SyntaxKind.CommaToken).Text[0]);
                    argumentAttributeText += SyntaxFactory.Token(SyntaxKind.CloseParenToken).Text;
                }

                AttributeSyntax attributeSyntax = null;

                if (!hasAttributeProperties)
                {
                    attributeSyntax = SyntaxFactory.Attribute(SyntaxFactory.ParseName(attributeInfo.AttributeType.FullName));
                }
                else
                {
                    attributeSyntax = SyntaxFactory.Attribute(
                        SyntaxFactory.ParseName(attributeInfo.AttributeType.FullName),
                        SyntaxFactory.ParseAttributeArgumentList(argumentAttributeText));
                }

                attributeSyntaxes.Add(attributeSyntax);
            }

            if (!attributeSyntaxes.Any()) return propertyDeclarationSyntax;

            var attributeList = SyntaxFactory.AttributeList(
                    SyntaxFactory.SeparatedList<AttributeSyntax>(attributeSyntaxes));

            return propertyDeclarationSyntax.WithAttributeLists(SyntaxFactory.SingletonList(attributeList));
        }
    }
}

