using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json;
using System.Text.Json.Serialization;
using WFDotnet.Code.Common.Constants;
using WFDotnet.Code.DataModel;
using WFDotnet.Code.Generation.Common;
using WFDotnet.Code.Generation.Common.Models;
using WFDotnet.Code.Generation.DataModel.Extensions;

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
                    if (!property.CanBeInstantiated() && !property.HasGenericType())
                        modelClass = modelClass.AddMembers(GenerateNotGenericPredifinedTypeCode(property));
                    else
                        modelClass = modelClass.AddMembers(GenericDefinedTypeCode(property));

                    var propertyNamespace = property.Type.Namespace;

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

        private PropertyDeclarationSyntax GenerateNotGenericPredifinedTypeCode(ModelPropertyInfo modelPropertyInfo)
        {
            var propertySyntax = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(modelPropertyInfo.Type.GetFullName(false)), modelPropertyInfo.Name);
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

        private PropertyDeclarationSyntax GenericDefinedTypeCode(ModelPropertyInfo modelPropertyInfo)
        {
            var hasGenericType = modelPropertyInfo.Type.GetGenericArguments().Any();

            var typeFullName = modelPropertyInfo.Type.GetFullName(hasGenericType);

            var lastDotIndex = typeFullName.LastIndexOf('.');

            var @namespace = typeFullName.Substring(0, lastDotIndex)
                .TrimEnd('.');

            var typeName = typeFullName.Substring(lastDotIndex, typeFullName.Length - lastDotIndex)
                .TrimStart('.');


            List<SyntaxNodeOrToken> syntaxNodeOrTokens = GetSyntaxNodeOrTokens(modelPropertyInfo.Type);

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

        private PropertyDeclarationSyntax PropertySyntaxAddAttributes(PropertyDeclarationSyntax propertyDeclarationSyntax, ModelPropertyInfo modelPropertyInfo)
        {
            if (!modelPropertyInfo.Attributes.Any()) return propertyDeclarationSyntax;

            List<AttributeSyntax> attributeSyntaxes = new List<AttributeSyntax>();

            foreach (var attributeInfo in modelPropertyInfo.Attributes)
            {
                bool hasAttributeProperties = false;
                string argumentAttributeText = "";

                if (attributeInfo.Value != null)
                {
                    var attributeProperties = attributeInfo.Value.GetType().GetProperties();

                    foreach (var attributeProperty in attributeProperties)
                    {
                        if (!hasAttributeProperties)
                        {
                            hasAttributeProperties = true;
                            argumentAttributeText += SyntaxFactory.Token(SyntaxKind.OpenParenToken).Text;
                        }

                        var attrValue = attributeProperty.GetValue(attributeInfo.Value);

                        if (attributeProperty.PropertyType == typeof(string))
                            argumentAttributeText += $"{attributeProperty.Name} = \"{attrValue}\" ,";
                        else if(attributeProperty.PropertyType.IsEnum)
                            argumentAttributeText += $"{attributeProperty.Name} = {attributeProperty.Name}.{attrValue} ,";
                        else
                            argumentAttributeText += $"{attributeProperty.Name} = {attrValue} ,";
                    }

                    argumentAttributeText = argumentAttributeText.TrimEnd(SyntaxFactory.Token(SyntaxKind.CommaToken).Text[0]);
                    argumentAttributeText += SyntaxFactory.Token(SyntaxKind.CloseParenToken).Text;
                }

                AttributeSyntax attributeSyntax = null;

                if (!hasAttributeProperties)
                {
                    attributeSyntax = SyntaxFactory.Attribute(SyntaxFactory.ParseName(attributeInfo.Type.FullName));
                }
                else
                {
                    attributeSyntax = SyntaxFactory.Attribute(
                        SyntaxFactory.ParseName(attributeInfo.Type.FullName),
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

