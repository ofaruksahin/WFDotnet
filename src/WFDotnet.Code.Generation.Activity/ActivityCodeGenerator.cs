using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WFDotnet.Code.Activity.Common.Interfaces;
using WFDotnet.Code.Activity.Common.Models;
using WFDotnet.Code.Common.Constants;
using WFDotnet.Code.Generation.Common;
using WFDotnet.Code.Generation.Common.Models;

namespace WFDotnet.Code.Generation.Activity
{
    public class ActivityCodeGenerator : WFCodeGenerator<WorkFlow>
    {
        private const string ModuleName = "Activities";
        private const string DefaultMethodReturnType = "Task<object>";
        private const string DefaultMethodName = "Execute";
        private const string DefaultIdentifierName = "var";

        public override WFCodeGenerationResult Generate(WorkFlow instance)
        {
            try
            {
                ClassDeclarationSyntax modelClass = SyntaxFactory.ClassDeclaration(instance.Name);
                modelClass = modelClass.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                var defaultUsings = GetUsings();

                var defaultMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(DefaultMethodReturnType), DefaultMethodName)
                    .WithModifiers(SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                        SyntaxFactory.Token(SyntaxKind.AsyncKeyword)));

                var blocks = new List<StatementSyntax>();

                if (instance.Activities is not null)
                {
                    foreach (var activity in instance.Activities)
                    {
                        var activityType = activity.Activity.GetType();

                        var activityNamespace = activityType.Namespace;

                        if (!defaultUsings.Any(f => f == activityNamespace))
                            defaultUsings.Add(activityNamespace);

                        var interfaces = activityType.GetInterfaces();

                        if (interfaces.Any(f => f.Name == typeof(IStartActivity).Name))
                        {
                            var startActivitySyntax = CodeGenerationForStartActivity(activity.Activity as IStartActivity);
                            blocks.AddRange(startActivitySyntax);
                        }
                    }
                }

                defaultMethod = defaultMethod.WithBody(SyntaxFactory.Block(blocks));

                modelClass = modelClass.AddMembers(defaultMethod);

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

        private List<StatementSyntax> CodeGenerationForStartActivity(IStartActivity startActivity)
        {
            var blocks = new List<StatementSyntax>();

            var block = SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName(DefaultIdentifierName)
                        )
                    .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.VariableDeclarator(
                                            SyntaxFactory.Identifier(startActivity.Name)
                                        )
                                    .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                    SyntaxFactory.ObjectCreationExpression(
                                                            SyntaxFactory.IdentifierName(startActivity.GetType().Name)
                                                        ).WithArgumentList(SyntaxFactory.ArgumentList())
                                                )
                                        )
                                )
                        )
                );

            blocks.Add(block);

            var argumentsExpressionStatements = new List<ExpressionStatementSyntax>();

            foreach (var item in startActivity.Arguments)
            {
                var expressionStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName(startActivity.Name),
                            SyntaxFactory.IdentifierName(nameof(startActivity.Arguments))),
                        SyntaxFactory.IdentifierName(nameof(startActivity.Arguments.Add)))
                )
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]{
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(item.Key))),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(item.Value)))
                            }
                        )
                    )
                ));

                argumentsExpressionStatements.Add(expressionStatement);
            }

            blocks.AddRange(argumentsExpressionStatements);

            var onExecuteStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AwaitExpression(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(startActivity.Name),
                                    SyntaxFactory.IdentifierName(nameof(startActivity.OnExecute))
                                )
                            )
                    )
                );

            blocks.Add(onExecuteStatement);

            return blocks;
        }
    }
}
